using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProyectoFinal.Models;

namespace ProyectoFinal.Api
{
    public class TransferenciaApi
    {
        private static readonly string URL_SITIOS = "https://starbb.000webhostapp.com/apimovil/";
        private static HttpClient client = new HttpClient();

        public async static Task<bool> CreateTransferencia(Transferencia transferencia)
        {
            try
            {
                Uri requestUri = new Uri(URL_SITIOS + "creartransferencia.php");
                var jsonObject = JsonConvert.SerializeObject(transferencia);
                var content = new StringContent(jsonObject, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(requestUri, content);

                Console.WriteLine(response.ToString());

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;
        }

        public static async Task<List<Transferencia>> GetTransferencias()
        {
            List<Transferencia> transferencias = new List<Transferencia>();

            try
            {
                var uri = new Uri(URL_SITIOS + "listatransferencia.php");
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    transferencias = JsonConvert.DeserializeObject<List<Transferencia>>(content);
                    return transferencias;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return transferencias;
        }


    }
}
