using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoFinal.Models
{
    public class Cuenta
    {
        [PrimaryKey, JsonProperty("codigocuenta")]
        public string CodigoCuenta { get; set; }
        [JsonProperty("codigousuario")]
        public string CodigoUsuario { get; set; }
        [JsonProperty("moneda")]
        public string Moneda { get; set; }
        [JsonProperty("saldo")]
        public double Saldo { get; set; }
        [JsonProperty("tipo")]
        public string Tipo { get; set; }
    }
}
