using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AppClubes.Models
{
    public class Jugador
    {
        [Display(Name = "ID")]
        public int id { get; set; }

        [Display(Name = "Apellido")]
        public string apellido { get; set; }

        [Display(Name = "Nombre")]
        public string nombre { get; set; }

        [Display(Name = "Biografía")]
        public string biografía { get; set; }

        [Display(Name = "Foto")]
        public string foto { get; set; }
    }
}
