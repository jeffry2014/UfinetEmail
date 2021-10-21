// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName=GuardarCorreos
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace UfinetEmail
{
    public static class GuardarCorreos
    {
        [FunctionName("GuardarCorreos")]
        public static void Run([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log)
        {
            dynamic dataBody = JsonConvert.DeserializeObject(eventGridEvent.Data.ToString());
            string email = dataBody.email;
            string mensajeMostrar = string.Empty;

            try
            {
                var str = Environment.GetEnvironmentVariable("ConnectionStrings:UFINET_DB");

                if (!validarExiste(email, str))
                {
                    //Se procede a ingresar el correo enviado
                    using (SqlConnection conn = new SqlConnection(str))
                    {
                        conn.Open();
                        var text = "INSERT INTO EnvioEmail (DataEmail) " +
                                $"VALUES ('{email}');";

                        using (SqlCommand cmd = new SqlCommand(text, conn))
                        {
                            // Execute the command and log the # rows affected.
                            var rows = cmd.BeginExecuteNonQuery();
                            log.LogInformation($"{rows} rows were updated");
                            mensajeMostrar = $"Se ha ingresado el correo: {email}. De forma satisfactoria.";
                        }
                    }
                }
                else
                {
                    mensajeMostrar = "El correo que desea ingresar ya existe.";
                }

            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
            }
          
        }

        private static bool validarExiste(string email, string stringConexion)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(stringConexion))
                {
                    connection.Open();
                    var query = $"Select * from EnvioEmail Where DataEmail = '{email}'";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(dt);
                }
            }
            catch (Exception e)
            {
                throw;
            }

            if (dt.Rows.Count > 0)
                return true;
            else
                return false;
        }
    }
}
