using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoFinal.Models
{
    public class Pagos
    {
        [PrimaryKey, AutoIncrement, JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("identidad")]
        public string Identidad { get; set; }
        [JsonProperty("mes")]
        public int Mes { get; set; }
        [JsonProperty("valor")]
        public double Valor { get; set; }
        [JsonProperty("pago")]
        public string Pago { get; set; }
        [JsonProperty("servicio")]
        public int Servicio { get; set; }
    }
}
