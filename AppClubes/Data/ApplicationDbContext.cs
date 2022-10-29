using AppClubes.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppClubes.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Categoria> categoria { get; set; }
        public DbSet<Club> club { get; set; }
        public DbSet<Jugador> jugador { get; set; }
        public DbSet<AppClubes.Models.JugadorClub> JugadorClub { get; set; }
    }
}
