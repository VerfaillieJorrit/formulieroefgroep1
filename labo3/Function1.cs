using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Collections.Generic;
using labo3.models;

namespace labo3
{
    public static class Function1
    {

        [FunctionName("GetDagenVandeweek")]
        public static async Task<IActionResult> GetDagenVandeweek(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "dagenvandeweek")] HttpRequest req,
            ILogger log)
        {
            try
            {
                List<String> dagen = new List<String>();
                string connectionstring = Environment.GetEnvironmentVariable("sqlServer");
                //SqlConnection sqlConnection = new SqlConnection(connectionstring);

                using (SqlConnection sqlconnection = new SqlConnection(connectionstring))
                {
                    await sqlconnection.OpenAsync();
                    using (SqlCommand sqlCommand = new SqlCommand())
                    {
                        sqlCommand.Connection = sqlconnection;
                        sqlCommand.CommandText = "select Dagvandeweek from labo3";
                        SqlDataReader reader = await sqlCommand.ExecuteReaderAsync();
                        while (await reader.ReadAsync())
                        {
                            string dagvandeweek = reader["Dagvandeweek"].ToString();
                            dagen.Add(dagvandeweek);

                        }

                    }
                }

                return new OkObjectResult(dagen);
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }


        [FunctionName("Getbezoeken")]
        public static async Task<IActionResult> Getbezoeken(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "bezoeken/{dagenvandeweek}")] HttpRequest req, string dagvandeweek,
            ILogger log)
        {
            try
            {
                string connectionstring = Environment.GetEnvironmentVariable("sqlServer");
                List<Bezoek> bezoeken = new List<Bezoek>();
                using (SqlConnection sqlconnection = new SqlConnection(connectionstring))
                {
                    using (SqlCommand sqlCommand = new SqlCommand())
                    {
                        sqlCommand.Connection = sqlconnection;
                        sqlCommand.CommandText = "select * from labo3 where Dagvandeweek = @dagvandeweek";
                        sqlCommand.Parameters.AddWithValue("@dagvandeweek", dagvandeweek);
                        var sqlDataReader = await sqlCommand.ExecuteReaderAsync();
                        while (await sqlDataReader.ReadAsync())
                        {
                            int tijdstip = int.Parse(sqlDataReader["tijdstip"].ToString());
                            int bezoekers = int.Parse(sqlDataReader["Aantalbezoekers"].ToString());
                            string dagVandeweek = sqlDataReader["Dagvandeweek"].ToString();

                            Bezoek bezoek = new Bezoek()
                            {
                                Tijdstip = tijdstip,
                                Aantalbezoekers = bezoekers,
                                Dagvandeweek = dagVandeweek

                            };
                            bezoeken.Add(bezoek);
                        };
                    };                
                }
                return new OkObjectResult(bezoeken);
            }

            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }
    }
}
