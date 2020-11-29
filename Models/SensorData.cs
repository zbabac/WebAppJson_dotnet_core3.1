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
        public class SensorOutput
        {
            [JsonPropertyName("DevEUI")]
            public string devid { get; set; }
            [JsonPropertyName("DevLocTime")]
            public string time { get; set; }

            [JsonPropertyName("Channel")]
            public string channel { get; set; }
            [JsonPropertyName("DevLAT")]
            public float lat { get; set; }
            [JsonPropertyName("DevLON")]
            public float lon { get; set; }
            [JsonPropertyName("DevALT")]
            public float alt { get; set; }
            [JsonPropertyName("LrrRSSI")]
            public float rssi { get; set; }
            [JsonPropertyName("LrrSNR")]
            public float snr { get; set; }
            [JsonPropertyName("Frequency")]
            public float freq { get; set; }
            [JsonPropertyName("FPort")]
            public int port { get; set; }
            public string payload_hex { get; set; }
        }
        public class MysqlDataContext
        {
            static string ConnectionString { get; set; }

            public MysqlDataContext(string connectionString)
            {
                ConnectionString = "server=localhost;user=sensor_admin;password=8*aKaziDino;database=sensors";
            }

            static MySqlConnection GetConnection()
            {
                return new MySqlConnection(ConnectionString);
            }

            public static SensorOutput GetOneSensor(string DevId)
            {
                try
                {
                    SensorOutput sensorData = new SensorOutput();
                    //
                    MySqlConnection conn = GetConnection();
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("select * from sensors where devid = '" + DevId + "'", conn);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var isoDateTimeFormat = CultureInfo.InvariantCulture.DateTimeFormat.SortableDateTimePattern;
                            sensorData.devid = Convert.ToString(reader["devid"]);
                            string format = "yyyy-mm-dd hh:mm:ss";
                            string time = Convert.ToDateTime(reader["last_time"]).ToString(format);
                            sensorData.time = time;
                            sensorData.lat = (float)reader["lat"];
                            sensorData.lon = (float)reader["lon"];
                            sensorData.alt = (float)reader["alt"];
                            sensorData.channel = Convert.ToString(reader["channel"]);

                        }

                    }
                    return sensorData;
                }
                catch (Exception e)
                {
                    string t = e.Message;
                    return null;
                }
            }

            public static async Task InsertSensorHistory(SensorOutput sensorInput)
            {
                try
                {
                    using (MySqlConnection conn = GetConnection())
                    {
                        conn.Open();
                        MySqlCommand cmd = new MySqlCommand("INSERT INTO `history` (`devid`, `time`, `lat`, `lon`, `alt`, `channel`, `rssi`, `snr`, `freq`, `port`, `payload_hex`)" +
                            "VALUES ('" + sensorInput.devid + "', '" + sensorInput.time + "', " + sensorInput.lat + ", " + sensorInput.lon + ", " + sensorInput.alt + ", '" + sensorInput.channel +
                            "', " + sensorInput.rssi + ", " + sensorInput.snr + ", " + sensorInput.freq + ", " + sensorInput.port + ", '" + sensorInput.payload_hex + "');", conn);

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
                try
                {
                    using (MySqlConnection conn = GetConnection())
                    {
                        conn.Open();
                        MySqlCommand cmd = new MySqlCommand("INSERT INTO `sensors` (`devid`, `tennantid`, `name`, `Last_time`, `lat`, `lon`, `alt`, `channel`)" +
                            "VALUES ('" + sensorInput.devid + "', '1', '" + sensorInput.devid + "', '" + sensorInput.time + "', " + sensorInput.lat + ", " + sensorInput.lon + ", " +
                            sensorInput.alt + ", '" + sensorInput.channel + "');", conn);

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
                try
                {
                    using (MySqlConnection conn = GetConnection())
                    {
                        conn.Open();
                        MySqlCommand cmd = new MySqlCommand("UPDATE `sensors` SET last_time='" + sensorInput.time + "' WHERE devid='" + sensorInput.devid + "';", conn);

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
