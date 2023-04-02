using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProyectoFinal.Models;

namespace ProyectoFinal.Api
{
    public class UsuarioApi
    {

        private static readonly string URL_SITIOS = "https://starbb.000webhostapp.com/apimovil/";
        private static HttpClient client = new HttpClient();

        public static async Task<List<Usuario>> GetAllUsuarios()
        {
            List<Usuario> ListaUsuarios = new List<Usuario>();
            try
            {
                var uri = new Uri(URL_SITIOS + "listausuario.php");
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    ListaUsuarios = JsonConvert.DeserializeObject<List<Usuario>>(content);
                    return ListaUsuarios;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return ListaUsuarios;
        }

        /*public static async Task<Usuario> GetUsuario(Usuario usuario)
        {
            List<Usuario> ListaUsuarios = new List<Usuario>();
            try
            {
                var uri = new Uri(URL_SITIOS + "listausuario.php?nidentidad=" + usuario.NumeroIdentidad);
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    ListaUsuarios = JsonConvert.DeserializeObject<List<Usuario>>(content);
                    return ListaUsuarios[0];
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return ListaUsuarios[0];
        }*/
        public static async Task<Usuario> GetUsuarioI(Usuario usuario)
        {
            List<Usuario> ListaUsuariosI = new List<Usuario>();
            try
            {
                var uri = new Uri(URL_SITIOS + "listausuarioindividual.php?nidentidad=" + usuario.NumeroIdentidad);
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    ListaUsuariosI = JsonConvert.DeserializeObject<List<Usuario>>(content);
                    if (ListaUsuariosI.Count > 0)
                    {
                        return ListaUsuariosI[0];
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }

        public async static Task<bool> DeleteUsuario(string id)
        {
            try
            {
                var uri = new Uri(URL_SITIOS + "eliminarsitio.php?id=" + id);
                var result = await client.GetAsync(uri);
                if (result.IsSuccessStatusCode)
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


        public async static Task<bool> CreateUsuario(Usuario usuario)
        {
            try
            {
                Uri requestUri = new Uri(URL_SITIOS + "crearusuario.php");
                var jsonObject = JsonConvert.SerializeObject(usuario);
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


        public async static Task<bool> UpdateUsuario(Usuario usuario)
        {
            try
            {
                Uri requestUri = new Uri(URL_SITIOS + "actualizarusuario.php");
                var jsonObject = JsonConvert.SerializeObject(usuario);
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



        public static async Task<string> GetFechaServidor()
        {
            try
            {
                var uri = new Uri(URL_SITIOS + "obtenerfecha.php");
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    return response.Content.ReadAsStringAsync().Result;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }

        internal static Task<List<Usuario>> GetUsuarioI()
        {
            throw new NotImplementedException();
        }
    }
}
