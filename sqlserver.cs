using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace xcomplus
{
    public class sqlserver
    {
        private string connectionString = "Data Source=localhost;Initial Catalog=sql_server_tempandsoon;Integrated Security=True";

        // 构造函数可用于读取数据库
        public sqlserver(DataTable dt)
        {
            string sql = "SELECT * FROM tempandsoon";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(sql, connection);
                adapter.Fill(dt);
            }
        }

        // ✅ 插入数据库的方法
        public void InsertToDatabase(string rawData)
        {
            try
            {
                // 调试输出原始数据
                Console.WriteLine($"原始数据: {rawData}");

                // 修改1: 使用英文冒号作为分隔符
                int colonIndex = rawData.IndexOf(':'); // 英文冒号
                if (colonIndex == -1)
                {
                    // 尝试中文冒号作为后备
                    colonIndex = rawData.IndexOf('：'); // 中文冒号
                    if (colonIndex == -1) return;
                }

                // 修改2: 直接使用当前时间，不依赖字符串中的时间
                DateTime receiveTime = DateTime.Now;

                // 提取数据部分
                string dataPart = rawData.Substring(colonIndex + 1).Trim();

                // 修改3: 更健壮的数据解析
                string temp = "";
                string humidity = "";
                int? light = null; // 使用可空类型

                string[] parts = dataPart.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string part in parts)
                {
                    if (part.StartsWith("temp=", StringComparison.OrdinalIgnoreCase))
                        temp = part.Substring(5).TrimEnd('C'); // 移除可能的单位

                    else if (part.StartsWith("humidity=", StringComparison.OrdinalIgnoreCase))
                        humidity = part.Substring(9).TrimEnd('%'); // 移除百分比符号

                    else if (part.StartsWith("local_light=", StringComparison.OrdinalIgnoreCase))
                    {
                        string lightStr = part.Substring(12);
                        if (int.TryParse(lightStr, out int lightValue))
                            light = lightValue;
                    }
                }

                // 检查必要数据是否存在
                if (string.IsNullOrEmpty(temp) || string.IsNullOrEmpty(humidity) || !light.HasValue)
                {
                    Console.WriteLine("数据不完整，跳过插入");
                    return;
                }

                // 修改4: 使用参数化查询防止SQL注入
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"INSERT INTO tempandsoon 
                          (date, temp, humidity, local_light) 
                          VALUES (@Date, @Temp, @Humidity, @Light)";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@Date", SqlDbType.DateTime).Value = receiveTime;
                        cmd.Parameters.Add("@Temp", SqlDbType.Decimal).Value = decimal.Parse(temp);
                        cmd.Parameters.Add("@Humidity", SqlDbType.Decimal).Value = decimal.Parse(humidity);
                        cmd.Parameters.Add("@Light", SqlDbType.Int).Value = light.Value;

                        int rows = cmd.ExecuteNonQuery();
                        Console.WriteLine($"插入成功，影响行数: {rows}");
                    }
                }
            }
            catch (Exception ex)
            {
               // UI线程显示错误
                MessageBox.Show("数据插入失败，请查看错误日志"+ex.Message);
            }
        }
    }
}
