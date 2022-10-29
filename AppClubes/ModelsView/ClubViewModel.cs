using AppClubes.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppClubes.ModelsView
{
    public class ClubViewModel
    {
        public List<Club> ListaClubes { get; set; }
        public SelectList ListaCategorias { get; set; }
        public string buscarNombre { get; set; }
        public string buscarPais { get; set; }
        public paginador paginador { get; set; }
    }
}
