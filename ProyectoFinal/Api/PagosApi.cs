using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProyectoFinal.Models;

namespace ProyectoFinal.Api
{
    public class PagosApi
    {
        
        private static readonly string URL_SITIOS = "https://starbb.000webhostapp.com/apimovil/";
        private static HttpClient client = new HttpClient();

        public static async Task<List<Pagos>> GetPagosUsuario(string identidad)
        {
            List<Pagos> pagos = new List<Pagos>();

            try
            {
                var uri = new Uri(URL_SITIOS + "listapagousuario.php?identidad='"+identidad+"'");
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    pagos = JsonConvert.DeserializeObject<List<Pagos>>(content);
                    return pagos;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return pagos;
        }

        public static async Task<bool> SetDeudasUsuario(string identidad)
        {
            try
            {
                var uri = new Uri(URL_SITIOS + "creardeudasservicios.php?identidad=" + identidad);
                var response = await client.GetAsync(uri);
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

        public async static Task<bool> UpdatePago(Pagos pago)
        {
            try
            {
                Uri requestUri = new Uri(URL_SITIOS + "actualizarpago.php");
                var jsonObject = JsonConvert.SerializeObject(pago);
                var content = new StringContent(jsonObject, Encoding.UTF8, "application/json");
                //var response = await client.PutAsync(requestUri, content);
                var response = await client.PostAsync(requestUri, content);
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

    }
}
