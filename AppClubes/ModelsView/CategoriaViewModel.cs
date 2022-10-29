using AppClubes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppClubes.ModelsView
{
    public class CategoriaViewModel
    {
        public List<Categoria> ListaCategorias { get; set; }
        public paginador paginador { get; set; }
    }
}
