using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoFinal.Models
{
    public class Servicio
    {
        //Tipo de servicio (Id)
        //1 agua
        //2 eeh
        //>3 promocion

        [PrimaryKey, JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("nombre")]
        public string Nombre { get; set; }
        [JsonProperty("descripcion")]
        public string Descripcion { get; set; }
        [JsonProperty("imagen")]
        public byte[] Imagen { get; set; }
        [JsonProperty("tipo")]
        public int Tipo { get; set; }
        [JsonProperty("precio")]
        public double Precio { get; set; }
    }
}
