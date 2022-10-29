using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AppClubes.Models
{
    public class JugadorClub
    {
        public int id { get; set; }

        public int Clubid { get; set; }
        [Display(Name = "Club")]
        public Club club { get; set; }

        public int Jugadorid { get; set; }
        [Display(Name = "Jugador")]
        public Jugador jugador { get; set; }
    }
}
