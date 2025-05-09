﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tesis.Models
{
    public class MUsuarios
    {
        public string Id_User { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public string Imagen { get; set; }
        public string Correo { get; set; }
        public string Contraseña { get; set; }
        public string TipoPerfil { get; set; }
        [JsonIgnore]
        public string NombreCompleto
        {
            get
            {
                return $"{Nombre} {Apellido}";
            }
        }

    }
}
