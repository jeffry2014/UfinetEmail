// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName=LogInsight
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
using Microsoft.ApplicationInsights.DataContracts;

namespace UfinetEmail
{
    public static class LogInsight
    {
        [FunctionName("LogInsight")]
        public static void Run([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log)
        {
            try
            {
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.InstrumentationKey = Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY");

                String requestname = eventGridEvent.EventType.ToString();

                //captura de datos
                DateTime time = Convert.ToDateTime(eventGridEvent.EventTime);
                var sw = Stopwatch.StartNew();
                telemetry.Context.Operation.Id = eventGridEvent.Id.ToString();
                string idLog = eventGridEvent.Id.ToString();
                string dateString = time.ToString("yyyyMMdd hhmm");

                string _customMessage = $"{idLog} {dateString}";
                //Send custom message
                telemetry.TrackTrace("UFINET LOG: " + _customMessage, SeverityLevel.Error);

                //send result to App Insights
                telemetry.TrackRequest(requestname, time, sw.Elapsed, "200", true);

                log.LogInformation("LOG INSIGTH PARA UBICAR!");

            }
            catch (Exception)
            {

                throw;
            }
           
        }
    }
}
