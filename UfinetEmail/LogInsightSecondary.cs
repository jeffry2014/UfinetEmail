// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName=LogInsightSecondary
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace UfinetEmail
{
    public static class LogInsightSecondary
    {
        static HttpClient client = new HttpClient();

        [FunctionName("LogInsightSecondary")]
        public static void Run([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log)
        {
            var url = "http://localhost:7071/api/LogInsightWarning";

            try
            {
                var response = client.GetAsync(url);
            }
            catch (Exception)
            {

                throw;
            }
           
        }
    }
}
