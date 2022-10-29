using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AppClubes.Data;
using AppClubes.Models;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using System.IO;
using AppClubes.ModelsView;

namespace AppClubes.Controllers
{
    public class JugadorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment env;

        public JugadorsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            this.env = env;
        }

        public FileResult Export()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("ID; Apellido; Nombre; Biografía; \r\n");
            foreach (Jugador customer in _context.jugador.ToList())
            {
                sb.Append(customer.id + ";");
                sb.Append(customer.apellido + ";");
                sb.Append(customer.nombre + ";");
                sb.Append(customer.biografía + ";");
                //Append new line character.
                sb.Append("\r\n");
            }

            return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "listadoJugadores.csv");
        }

        public IActionResult Import()
        {
            var files = HttpContext.Request.Form.Files;
            if (files != null && files.Count > 0)
            {
                var fileImports = files[0];
                var pathDestiny = Path.Combine(env.WebRootPath, "imports");
                if (fileImports.Length > 0)
                {
                    var fileDestiny = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(fileImports.FileName);
                    string route = Path.Combine(pathDestiny, fileDestiny);
                    using (var filestream = new FileStream(route, FileMode.Create))
                    {
                        fileImports.CopyTo(filestream);
                    };

                    using (var file = new FileStream(route, FileMode.Open))
                    {
                        List<string> rows = new List<string>();
                        List<Jugador> JugadorArch = new List<Jugador>();

                        StreamReader fileContent = new StreamReader(file); // System.Text.Encoding.Default
                        do
                        {
                            rows.Add(fileContent.ReadLine());
                        }
                        while (!fileContent.EndOfStream);

                        foreach (string row in rows)
                        {
                            //int output;
                            string[] information = row.Split(';');
                            //int healthcareSystem = int.TryParse(information[information.Length - 1], out output) ? output : 0;
                            //if (healthcareSystem > 0 && _context.healthcareSystem.Where(c => c.id == healthcareSystem).FirstOrDefault() != null)
                            //{
                            Jugador temporalJugador = new Jugador()
                            {
                                apellido = information[0],
                                nombre = information[1],
                                biografía = information[2],
                            };
                            JugadorArch.Add(temporalJugador);
                            //}
                        }
                        if (JugadorArch.Count > 0)
                        {
                            _context.jugador.AddRange(JugadorArch);
                            _context.SaveChanges();
                        }

                        ViewBag.amountrows = JugadorArch.Count + " de " + rows.Count;
                    }
                }
            }

            return View();
        }

        // GET: Jugadors
        public async Task<IActionResult> Index(string buscarApellido, string buscarNombre, string buscarBiografia, int pagina = 1)
        {
            //ViewData["carreraList"] = new SelectList(_context.carreras, "id", "nombre");
            paginador paginador = new paginador()
            {
                pagActual = pagina,
                regXpag = 5
            };

            var consulta = _context.jugador.Select(a => a);
            if (!string.IsNullOrEmpty(buscarApellido))
            {
                consulta = consulta.Where(e => e.nombre.Contains(buscarApellido));
                //paginador.ValoresQueryString.Add("busqNombre", busqNombre);
            }
            if (!string.IsNullOrEmpty(buscarNombre))
            {
                consulta = consulta.Where(e => e.nombre.Contains(buscarNombre));
                //paginador.ValoresQueryString.Add("busqNombre", busqNombre);
            }
            if (!string.IsNullOrEmpty(buscarBiografia))
            {
                consulta = consulta.Where(e => e.nombre.Contains(buscarBiografia));
                //paginador.ValoresQueryString.Add("busqNombre", busqNombre);
            }

            paginador.cantReg = consulta.Count();

            //paginador.totalPag = (int)Math.Ceiling((decimal)paginador.cantReg / paginador.regXpag);
            var datosAmostrar = consulta
                .Skip((paginador.pagActual - 1) * paginador.regXpag)
                .Take(paginador.regXpag);

            foreach (var item in Request.Query)
                paginador.ValoresQueryString.Add(item.Key, item.Value);

            JugadorViewModel Datos = new JugadorViewModel()
            {
                ListaJugadores = datosAmostrar.ToList(),
                buscarApellido = buscarApellido,
                buscarNombre = buscarNombre,
                buscarBiografia = buscarBiografia,
                paginador = paginador
            };

            return View(Datos);
        }

        // GET: Jugadors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jugador = await _context.jugador
                .FirstOrDefaultAsync(m => m.id == id);
            if (jugador == null)
            {
                return NotFound();
            }

            return View(jugador);
        }

        // GET: Jugadors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Jugadors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,apellido,nombre,biografía,foto")] Jugador jugador)
        {
            if (ModelState.IsValid)
            {
                var archivos = HttpContext.Request.Form.Files;
                if (archivos != null && archivos.Count > 0)
                {
                    var archivoFoto = archivos[0];
                    var pathDestino = Path.Combine(env.WebRootPath, "fotos");
                    if (archivoFoto.Length > 0)
                    {
                        var archivoDestino = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(archivoFoto.FileName);

                        using (var filestream = new FileStream(Path.Combine(pathDestino, archivoDestino), FileMode.Create))
                        {
                            archivoFoto.CopyTo(filestream);
                            jugador.foto = archivoDestino;
                        };

                    }
                }
                _context.Add(jugador);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(jugador);
        }

        // GET: Jugadors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jugador = await _context.jugador.FindAsync(id);
            if (jugador == null)
            {
                return NotFound();
            }
            return View(jugador);
        }

        // POST: Jugadors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,apellido,nombre,biografía,foto")] Jugador jugador)
        {
            if (id != jugador.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var archivos = HttpContext.Request.Form.Files;
                    if (archivos != null && archivos.Count > 0)
                    {
                        var archivoFoto = archivos[0];
                        var pathDestino = Path.Combine(env.WebRootPath, "fotos");
                        if (archivoFoto.Length > 0)
                        {
                            var archivoDestino = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(archivoFoto.FileName);

                            if (!string.IsNullOrEmpty(jugador.foto))
                            {
                                string fotoAnterior = Path.Combine(pathDestino, jugador.foto);
                                if (System.IO.File.Exists(fotoAnterior))
                                    System.IO.File.Delete(fotoAnterior);
                            }

                            using (var filestream = new FileStream(Path.Combine(pathDestino, archivoDestino), FileMode.Create))
                            {
                                archivoFoto.CopyTo(filestream);
                                jugador.foto = archivoDestino;
                            };
                        }
                    }
                    _context.Update(jugador);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JugadorExists(jugador.id))
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
            return View(jugador);
        }

        // GET: Jugadors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jugador = await _context.jugador
                .FirstOrDefaultAsync(m => m.id == id);
            if (jugador == null)
            {
                return NotFound();
            }

            return View(jugador);
        }

        // POST: Jugadors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jugador = await _context.jugador.FindAsync(id);
            _context.jugador.Remove(jugador);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JugadorExists(int id)
        {
            return _context.jugador.Any(e => e.id == id);
        }
    }
}