using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoFinal.Models
{
    public class Usuario
    {
        [JsonProperty("id"), PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [JsonProperty("foto")]
        public byte[] Fotografia { get; set; }

        [JsonProperty("nombrec")]
        public string NombreCompleto { get; set; }

        [JsonProperty("fechanac")]
        public string FechaNacimiento { get;set; }

        [JsonProperty("sexo")]
        public string Sexo { get; set; }

        [JsonProperty("direccion")]
        public string Direccion { get; set; }

        [JsonProperty("nidentidad")]
        public string NumeroIdentidad { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("usuario")]
        public string NombreUsuario { get; set; }

        [JsonProperty("clave")]
        public string Contraseña { get; set; }

        [JsonProperty("codigov")]
        public string CodigoVerificacion { get; set; }

        [JsonProperty("clavet")]
        public string ContraseñaTemporal { get; set; }

        [JsonProperty("idcliente")]
        public string IdCliente { get; set; }
    }
}
