// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName=EnviarEmail
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;
using Newtonsoft.Json;

namespace UfinetEmail
{
    public static class EnviarEmail
    {
        [FunctionName("EnviarEmail")]
        public static async Task RunAsync([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log)
        {

            List<EmailsUsers> emailList = new List<EmailsUsers>();
            dynamic dataBody = JsonConvert.DeserializeObject(eventGridEvent.Data.ToString());
            string message = dataBody.message;

            try
            {
                var str = Environment.GetEnvironmentVariable("ConnectionStrings:UFINET_DB");

                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();
                    var query = @"SELECT DataEmail FROM EnvioEmail";
                    SqlCommand command = new SqlCommand(query, conn);
                    var reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        EmailsUsers email = new EmailsUsers()
                        {
                            email = (string)reader["DataEmail"],
                        };
                        emailList.Add(email);
                    }

                    if (emailList.Count > 0)
                    {
                        foreach (var mail in emailList)
                        {
                            var mailMessage = new MimeMessage();
                            mailMessage.From.Add(new MailboxAddress("UFINET", "jeffry2014@gmail.com"));
                            mailMessage.To.Add(new MailboxAddress("Notification", mail.email.ToString()));
                            mailMessage.Subject = "Ufinet send mail";
                            mailMessage.Body = new TextPart("plain")
                            {
                                Text = message
                            };


                            using (var smtpClient = new SmtpClient())
                            {
                                smtpClient.Connect("smtp.gmail.com", 465, true);
                                smtpClient.Authenticate("jeffry2014@gmail.com", "ClarkKent2014");
                                smtpClient.Send(mailMessage);
                                smtpClient.Disconnect(true);
                            }

                        }

                    }
                }

            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
            }
            /*
            if (emailList.Count > 0)
            {
                return new OkObjectResult(emailList);
            }
            else
            {
                return new NotFoundResult();
            }
            */
        }

        public class EmailsUsers
        {
            public string email { get; set; }
        }

    }
}
