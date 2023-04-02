using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProyectoFinal.Models;

namespace ProyectoFinal.Api
{
    public class PrecioDolar
    {
        private static readonly string URL_SITIOS = "https://starbb.000webhostapp.com/apimovil/";
        private static HttpClient client = new HttpClient();

        public static async Task<Dolar> GetPrecioDolar(string fecha)
        {
            List<Dolar> precio = new List<Dolar>();

            try
            {
                var uri = new Uri(URL_SITIOS + "listaprecio.php?fecha='" + fecha+"'");
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    precio = JsonConvert.DeserializeObject<List<Dolar>>(content);
                    
                    if (precio.Count > 0)
                    {
                        return precio[0];
                    }
                }
                throw new Exception("No se pudo obtener el precio del dolar");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // Lanzar una excepción personalizada o devolver null según el caso
                throw new Exception("Ocurrió un error al obtener el precio del dolar", ex);
                //return null;
            }

            //return precio[0];
        }

        public static async Task<List<Dolar>> GetListaPrecioDolar()
        {
            List<Dolar> precios = new List<Dolar>();

            try
            {
                var uri = new Uri(URL_SITIOS + "listaprecio.php");
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    precios = JsonConvert.DeserializeObject<List<Dolar>>(content);
                    return precios;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return precios;
        }
    }
}
