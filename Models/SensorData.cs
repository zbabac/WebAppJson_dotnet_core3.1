using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using MySqlConnector;
using System.Globalization;

namespace WebAppJson.Models
{
    public class SensorData
    {
        public class DevEUIUplink
        {
            // define type of vars that are parsed from json and inserted in DB
            [JsonPropertyName("DevEUI")]
            public string devid { get; set; }
            [JsonPropertyName("DevLocTime")]
            public string time { get; set; }
            public string Time { get; set; }

            [JsonPropertyName("Channel")]
            public string channel { get; set; }
            [JsonPropertyName("DevLAT")]
            public string lat { get; set; }
            public string latLrrLAT { get; set; }
            [JsonPropertyName("DevLON")]
            public string lon { get; set; }
            public string latLrrLON { get; set; }
            [JsonPropertyName("DevALT")]
            public string alt { get; set; }
            [JsonPropertyName("LrrRSSI")]
            public string rssi { get; set; }
            [JsonPropertyName("LrrSNR")]
            public string snr { get; set; }
            [JsonPropertyName("Frequency")]
            public string freq { get; set; }
            [JsonPropertyName("FPort")]
            public string port { get; set; }
            public string payload_hex { get; set; }
        }

        public class SensorOutput
        {
            public DevEUIUplink DevEUI_uplink { get; set; }
        }
        public class JsonString
        {
            public string Jstring { get; set; }
        }
        public class MysqlDataContext
        {
            static string ConnectionString { get; set; }

            public MysqlDataContext(string connectionString)
            {
                // this is odd, for some reason gets empty strinf from Startup.cs so I add workaround here, will try to fix this
                ConnectionString = "server=localhost;user=sensor_admin;password=yourpassword;database=sensors";
            }

            static MySqlConnection GetConnection()
            {
                return new MySqlConnection(ConnectionString);
            }

            public static DevEUIUplink GetOneSensor(string DevId)
            {
                // Get sensor data from DB is it exists (DevId)
                try
                {
                    DevEUIUplink deveuiuplink = new DevEUIUplink();
                    //
                    MySqlConnection conn = GetConnection();
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("select * from sensors where devid = '" + DevId + "'", conn);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            deveuiuplink.devid = Convert.ToString(reader["devid"]);
                        }

                    }
                    return deveuiuplink;
                }
                catch (Exception e)
                {
                    string t = e.Message;
                    return null;
                }
            }

            public static async Task InsertSensorHistory(SensorOutput sensorInput)
            {
                // Insert newly received data into DB, table history
                try
                {
                    using (MySqlConnection conn = GetConnection())
                    {
                        string tlat, tlon, ttime;
                        if (sensorInput.DevEUI_uplink.lat is null)
                            tlat = sensorInput.DevEUI_uplink.latLrrLAT;
                        else tlat = sensorInput.DevEUI_uplink.lat;
                        if (sensorInput.DevEUI_uplink.lon is null)
                            tlon = sensorInput.DevEUI_uplink.latLrrLON;
                        else tlon = sensorInput.DevEUI_uplink.lon;
                        if (sensorInput.DevEUI_uplink.time is null)
                            ttime = sensorInput.DevEUI_uplink.Time;
                        else ttime = sensorInput.DevEUI_uplink.time;
                        // It is neccessary to convert the format to datetime accepted by mysql
                        string format = "yyyy-mm-dd hh:mm:ss";
                        string time = Convert.ToDateTime(ttime).ToString(format);
                        conn.Open();
                        MySqlCommand cmd = new MySqlCommand("INSERT INTO `history` (`devid`, `time`, `lat`, `lon`, `alt`, `channel`, `rssi`, `snr`, `freq`, `port`, `payload_hex`)" +
                            "VALUES ('" + sensorInput.DevEUI_uplink.devid + "', '" + ttime + "', '" + tlat + "', '" + tlon + "', '" + sensorInput.DevEUI_uplink.alt + "', '" + sensorInput.DevEUI_uplink.channel +
                            "', '" + sensorInput.DevEUI_uplink.rssi + "', '" + sensorInput.DevEUI_uplink.snr + "', '" + sensorInput.DevEUI_uplink.freq + "', '" + sensorInput.DevEUI_uplink.port + "', '" + sensorInput.DevEUI_uplink.payload_hex + "');", conn);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception e)
                {
                    string t = e.Message;
                }
            }
            public static async Task InsertNewSensorId(SensorOutput sensorInput)
            {
                // If the sensor in new in DB, then add it to the table sensors
                try
                {
                    using (MySqlConnection conn = GetConnection())
                    {
                        string tlat, tlon, ttime;
                        if (sensorInput.DevEUI_uplink.lat is null)
                            tlat = sensorInput.DevEUI_uplink.latLrrLAT;
                        else tlat = sensorInput.DevEUI_uplink.lat;
                        if (sensorInput.DevEUI_uplink.lon is null)
                            tlon = sensorInput.DevEUI_uplink.latLrrLON;
                        else tlon = sensorInput.DevEUI_uplink.lon;
                        if (sensorInput.DevEUI_uplink.time is null)
                            ttime = sensorInput.DevEUI_uplink.Time;
                        else ttime = sensorInput.DevEUI_uplink.time;
                        string format = "yyyy-mm-dd hh:mm:ss";
                        string time = Convert.ToDateTime(ttime).ToString(format);
                        conn.Open();
                        MySqlCommand cmd = new MySqlCommand("INSERT INTO `sensors` (`devid`, `tennantid`, `name`, `Last_time`, `lat`, `lon`, `alt`, `channel`)" +
                            "VALUES ('" + sensorInput.DevEUI_uplink.devid + "', '1', '" + sensorInput.DevEUI_uplink.devid + "', '" + ttime + "', '" + tlat + "', '" + tlon + "', '" +
                            sensorInput.DevEUI_uplink.alt + "', '" + sensorInput.DevEUI_uplink.channel + "');", conn);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception e)
                {
                    string t = e.Message;
                }
            }
            public static async Task UpdateSensorTime(SensorOutput sensorInput)
            {
                // If the sensor is already known, just update its last_time and geo data
                try
                {
                    using (MySqlConnection conn = GetConnection())
                    {
                        string tlat, tlon, ttime;
                        if (sensorInput.DevEUI_uplink.lat is null)
                            tlat = sensorInput.DevEUI_uplink.latLrrLAT;
                        else tlat = sensorInput.DevEUI_uplink.lat;
                        if (sensorInput.DevEUI_uplink.lon is null)
                            tlon = sensorInput.DevEUI_uplink.latLrrLON;
                        else tlon = sensorInput.DevEUI_uplink.lon;
                        if (sensorInput.DevEUI_uplink.time is null)
                            ttime = sensorInput.DevEUI_uplink.Time;
                        else ttime = sensorInput.DevEUI_uplink.time;
                        string format = "yyyy-mm-dd hh:mm:ss";
                        string time = Convert.ToDateTime(ttime).ToString(format);
                        conn.Open();
                        MySqlCommand cmd = new MySqlCommand("UPDATE `sensors` SET last_time='" + ttime + "', lat='" + tlat + "', lon='"+tlon+ "', alt='"+ sensorInput.DevEUI_uplink.alt + "' WHERE devid='" + sensorInput.DevEUI_uplink.devid + "';", conn);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception e)
                {
                    string t = e.Message;
                }
            }
            public static async Task UpdateSensorNameTennant(string devid, string name, int tennantid)
            {
                // This is to be implemented for the admin interface: 
                // to update tennant data for the new sensor
                try
                {
                    using (MySqlConnection conn = GetConnection())
                    {
                        conn.Open();
                        MySqlCommand cmd = new MySqlCommand("UPDATE `sensors` SET name='" + name + "', tennantid='" + tennantid + "' WHERE devid='" + devid + "';", conn);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception e)
                {
                    string t = e.Message;
                }
            }
        }
    }
}
