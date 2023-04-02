using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProyectoFinal.Models;

namespace ProyectoFinal.Api
{
    public class ServicioApi
    {
        private static readonly string URL_SITIOS = "https://starbb.000webhostapp.com/apimovil/";
        private static HttpClient client = new HttpClient();

        public static async Task<List<Servicio>> GetAllServicios()
        {
            List<Servicio> ListaServicios = new List<Servicio>();
            try
            {
                var uri = new Uri(URL_SITIOS + "listaservicio.php");
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    ListaServicios = JsonConvert.DeserializeObject<List<Servicio>>(content);
                    return ListaServicios;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return ListaServicios;
        }

        public static async Task<Servicio> GetSingleServicio(int id)
        {
            List<Servicio> Servicio = new List<Servicio>();
            try
            {
                var uri = new Uri(URL_SITIOS + "listasingleservicio.php?id="+id);
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    Servicio = JsonConvert.DeserializeObject<List<Servicio>>(content);
                    return Servicio[0];
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return Servicio[0];
        }

    }
}
