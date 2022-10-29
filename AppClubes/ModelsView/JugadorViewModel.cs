using AppClubes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppClubes.ModelsView
{
    public class JugadorViewModel
    {
        public List<Jugador> ListaJugadores { get; set; }
        public string buscarApellido { get; set; }
        public string buscarNombre { get; set; }
        public string buscarBiografia { get; set; }
        public paginador paginador { get; set; }
    }

    public class paginador
    {
        public int cantReg { get; set; }
        public int regXpag { get; set; }
        public int pagActual { get; set; }
        public int totalPag => (int)Math.Ceiling((decimal)cantReg / regXpag);
        public Dictionary<string, string> ValoresQueryString { get; set; } = new Dictionary<string, string>();
        public object ValuesQueryString { get; internal set; }
    }
}
