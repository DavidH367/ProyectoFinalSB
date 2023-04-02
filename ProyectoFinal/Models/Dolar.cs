using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoFinal.Models
{
    public class Dolar
    {
        [PrimaryKey, JsonProperty("fecha")]
        public string Fecha { get; set; }
        [JsonProperty("compra")]
        public double Compra { get; set; }
        [JsonProperty("venta")]
        public double Venta { get; set; }
        [JsonProperty("precio")]
        public double Precio { get; set; }
    }
}
