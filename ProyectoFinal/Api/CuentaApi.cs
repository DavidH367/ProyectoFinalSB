using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProyectoFinal.Models;

namespace ProyectoFinal.Api
{
    public class CuentaApi
    {
        //private static readonly string URL_SITIOS = "https://pm2examen2.000webhostapp.com/apiproyecto/";
        private static readonly string URL_SITIOS = "https://starbb.000webhostapp.com/apimovil/";
        private static HttpClient client = new HttpClient();

        public static async Task<List<Cuenta>> GetAllCuentas()
        {
            List<Cuenta> ListaCuentas = new List<Cuenta>();
            try
            {
                var uri = new Uri(URL_SITIOS + "listacuenta.php");
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    ListaCuentas = JsonConvert.DeserializeObject<List<Cuenta>>(content);
                    return ListaCuentas;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return ListaCuentas;
        }

        public async static Task<bool> CreateCuenta(Cuenta cuenta)
        {
            try
            {
                Uri requestUri = new Uri(URL_SITIOS + "crearcuenta.php");
                var jsonObject = JsonConvert.SerializeObject(cuenta);
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

        public async static Task<bool> UpdateCuenta(Cuenta cuenta)
        {
            try
            {
                Uri requestUri = new Uri(URL_SITIOS + "actualizarcuenta.php");
                var jsonObject = JsonConvert.SerializeObject(cuenta);
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
    }
}
