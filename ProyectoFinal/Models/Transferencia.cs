using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoFinal.Models
{
    public class Transferencia
    {
        [PrimaryKey, JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("accion")]
        public string Accion { get; set; } // La accion es relativa al usuario (en si es la cuenta) que envia si debito o credito
        [JsonProperty("moneda")]
        public string Moneda { get; set; }
        [JsonProperty("valor")]
        public double Valor { get; set; }
        [JsonProperty("envia")]
        public string Envia { get; set; }
        [JsonProperty("recibe")]
        public string Recibe { get; set; }
        [JsonProperty("fecha")]
        public string Fecha { get; set; }
        [JsonProperty("comentario")]
        public string Comentario { get; set; }
    }
}
