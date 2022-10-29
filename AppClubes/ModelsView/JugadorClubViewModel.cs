using AppClubes.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppClubes.ModelsView
{
    public class JugadorClubViewModel
    {
        public List<JugadorClub> ListaJugadorClubes { get; set; }
        public SelectList ListaClub { get; set; }
        public SelectList ListaJugador { get; set; }
        public paginador paginador { get; set; }

    }
}
