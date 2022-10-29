using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AppClubes.Data;
using AppClubes.Models;
using AppClubes.ModelsView;

namespace AppClubes.Controllers
{
    public class JugadorClubsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JugadorClubsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: JugadorClubs
        public async Task<IActionResult> Index(int? clubId, int? jugadorId, int pagina = 1)
        {
            //var applicationDbContext = _context.customer.Include(c => c.healthcareSystem);
            //return View(await applicationDbContext.ToListAsync());
            paginador paginador = new paginador()
            {
                pagActual = pagina,
                regXpag = 3
            };

            var consulta = _context.JugadorClub.Include(a => a.club).Include(a => a.jugador).Select(a => a);
            if (clubId.HasValue)
            {
                consulta = consulta.Where(e => e.Clubid == clubId);
                //paginador.ValoresQueryString.Add("carreraId", carreraId.ToString());
            }
            if (jugadorId.HasValue)
            {
                consulta = consulta.Where(e => e.Jugadorid == jugadorId);
                //paginador.ValoresQueryString.Add("carreraId", carreraId.ToString());
            }

            paginador.cantReg = consulta.Count();

            //paginador.totalPag = (int)Math.Ceiling((decimal)paginador.cantReg / paginador.regXpag);
            var datosAmostrar = consulta
                .Skip((paginador.pagActual - 1) * paginador.regXpag)
                .Take(paginador.regXpag);

            foreach (var item in Request.Query)
                paginador.ValoresQueryString.Add(item.Key, item.Value);

            JugadorClubViewModel information = new JugadorClubViewModel()
            {
                ListaJugadorClubes = datosAmostrar.ToList(),
                ListaClub = new SelectList(_context.club, "id", "nombre", clubId),
                ListaJugador = new SelectList(_context.jugador, "id", "apellido", jugadorId),
                paginador = paginador
            };

            return View(information);
        }

        // GET: JugadorClubs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jugadorClub = await _context.JugadorClub
                .Include(j => j.club)
                .Include(j => j.jugador)
                .FirstOrDefaultAsync(m => m.id == id);
            if (jugadorClub == null)
            {
                return NotFound();
            }

            return View(jugadorClub);
        }

        // GET: JugadorClubs/Create
        public IActionResult Create()
        {
            ViewData["Clubid"] = new SelectList(_context.club, "id", "nombre");
            ViewData["Jugadorid"] = new SelectList(_context.jugador, "id", "apellido");
            return View();
        }

        // POST: JugadorClubs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,Clubid,Jugadorid")] JugadorClub jugadorClub)
        {
            if (ModelState.IsValid)
            {
                _context.Add(jugadorClub);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Clubid"] = new SelectList(_context.club, "id", "nombre", jugadorClub.Clubid);
            ViewData["Jugadorid"] = new SelectList(_context.jugador, "id", "apellido", jugadorClub.Jugadorid);
            return View(jugadorClub);
        }

        // GET: JugadorClubs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jugadorClub = await _context.JugadorClub.FindAsync(id);
            if (jugadorClub == null)
            {
                return NotFound();
            }
            ViewData["Clubid"] = new SelectList(_context.club, "id", "nombre", jugadorClub.Clubid);
            ViewData["Jugadorid"] = new SelectList(_context.jugador, "id", "apellido", jugadorClub.Jugadorid);
            return View(jugadorClub);
        }

        // POST: JugadorClubs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,Clubid,Jugadorid")] JugadorClub jugadorClub)
        {
            if (id != jugadorClub.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jugadorClub);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JugadorClubExists(jugadorClub.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Clubid"] = new SelectList(_context.club, "id", "nombre", jugadorClub.Clubid);
            ViewData["Jugadorid"] = new SelectList(_context.jugador, "id", "apellido", jugadorClub.Jugadorid);
            return View(jugadorClub);
        }

        // GET: JugadorClubs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jugadorClub = await _context.JugadorClub
                .Include(j => j.club)
                .Include(j => j.jugador)
                .FirstOrDefaultAsync(m => m.id == id);
            if (jugadorClub == null)
            {
                return NotFound();
            }

            return View(jugadorClub);
        }

        // POST: JugadorClubs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jugadorClub = await _context.JugadorClub.FindAsync(id);
            _context.JugadorClub.Remove(jugadorClub);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JugadorClubExists(int id)
        {
            return _context.JugadorClub.Any(e => e.id == id);
        }
    }
}