using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoFinal.Models
{
    public class Cliente
    {
        [PrimaryKey]
        public int IdUsuario { get; set; }
        public int Servicio { get; set; }
        public double Precio { get; set; }
    }
}
