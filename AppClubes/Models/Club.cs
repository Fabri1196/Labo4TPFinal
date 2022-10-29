using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AppClubes.Models
{
    public class Club
    {
        public int id { get; set; }

        [Display(Name = "Nombre")]
        public string nombre { get; set; }

        [Display(Name = "Resumen")]
        public string resumen { get; set; }

        [Display(Name = "Fecha de Fundación")]
        public DateTime fechaFund { get; set; }

        [Display(Name = "Imagen del Escudo")]
        public string imagenEscudo { get; set; }

        [Display(Name = "País")]
        public string pais { get; set; }

        public int Categoriaid { get; set; }
        [Display(Name = "Categoría")]
        public Categoria categoria { get; set; }
    }
}
