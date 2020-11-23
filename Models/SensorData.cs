using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using MySqlConnector;

namespace WebAppJson.Models
{
    public class SensorData
    {
        public class SensorOutput
        {
            [JsonPropertyName("DevEUI")]
            public string devid { get; set; }
            [JsonPropertyName("DevLocTime")]
            public DateTime time { get; set; }

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
                ConnectionString = connectionString;
            }

            static MySqlConnection GetConnection()
            {
                return new MySqlConnection(ConnectionString);
            }

            public static SensorOutput GetOneSensor(string DevId)
            {
                SensorOutput sensorData = new SensorOutput();
                //
                MySqlConnection conn = GetConnection();
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select * from sensors.sensors where devid = '"+DevId+"'", conn);

                using (var reader = cmd.ExecuteReader())
                {
                        while (reader.Read())
                        {
                            sensorData.devid = Convert.ToString(reader["devid"]);
                            sensorData.time = Convert.ToDateTime(reader["time"]);
                            sensorData.lat = (float)reader["lat"];
                            sensorData.lon = (float)reader["lon"];
                            sensorData.alt = (float)reader["alt"];
                            sensorData.channel = Convert.ToString(reader["channel"]);
                            sensorData.rssi = (float)reader["rssi"];
                            sensorData.snr = (float)reader["snr"];
                            sensorData.freq = (float)reader["alt"];
                            sensorData.port = (int)reader["port"];
                            sensorData.payload_hex = Convert.ToString(reader["payload_hex"]);
                        }
                    
                }
                return sensorData;
            }

            public static async Task InsertSensorHistory(SensorOutput sensorInput)
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("INSERT INTO `sensors.history` (`devid`, `time`, `lat`, `lon`, `alt`, `rssi`, `snr`, `freq`, `port`, `payload_hex`)" + 
                        "VALUES ("+sensorInput.devid+", "+sensorInput.time+", "+sensorInput.lat+", "+sensorInput.lon+", "+sensorInput.alt+", "+sensorInput.channel+
                        ", "+sensorInput.rssi+", "+sensorInput.snr+", "+sensorInput.freq+", "+sensorInput.freq+", "+sensorInput.port+", "+sensorInput.payload_hex+");", conn);
                    
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            public static async Task InsertNewSensorId(SensorOutput sensorInput)
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("INSERT INTO `sensors.sensors` (`devid`, `tennantid`, `name`, `Last_time`, `lat`, `lon`, `alt`, `rssi`, `snr`, `freq`, `port`, `payload_hex`)" +
                        "VALUES (" + sensorInput.devid + ", '1', " + sensorInput.devid + ", " + sensorInput.time + ", " + sensorInput.lat + ", " + sensorInput.lon + ", " +
                        sensorInput.alt + ", " + sensorInput.channel + ", " + sensorInput.rssi + ", " + sensorInput.snr + ", " + sensorInput.freq + ", " + sensorInput.freq + ", " + sensorInput.port + ", " + sensorInput.payload_hex + ");", conn);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
            public static async Task UpdateSensorTime(SensorOutput sensorInput)
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("UPDATE `sensors.sensors` SET last_time='"+sensorInput.time+"' WHERE devid='"+sensorInput.devid+"';", conn);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
            public static async Task UpdateSensorNameTennant(string devid, string name, int tennantid)
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("UPDATE `sensors.sensors` SET name='" + name + "', tennantid='"+tennantid+ "' WHERE devid='" + devid + "';", conn);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
