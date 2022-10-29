using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AppClubes.Models
{
    public class Categoria
    {
        public int id { get; set; }

        [Display(Name = "Descripción")]
        public string descripcion { get; set; }
    }
}
