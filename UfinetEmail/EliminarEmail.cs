// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName=EliminarEmail
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace UfinetEmail
{
    public static class EliminarEmail
    {
        [FunctionName("EliminarEmail")]
        public static void Run([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log)
        {
            dynamic dataBody = JsonConvert.DeserializeObject(eventGridEvent.Data.ToString());
            string email = dataBody.email;

            try
            {
                var str = Environment.GetEnvironmentVariable("ConnectionStrings:UFINET_DB");
                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();
                    var query = $"DELETE FROM EnvioEmail WHERE DataEmail = '{email}'";
                    SqlCommand command = new SqlCommand(query, conn);
                    command.ExecuteNonQuery();

                }
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
            }
        }
    }
}
