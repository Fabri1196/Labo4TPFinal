using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AppClubes.Data;
using AppClubes.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using AppClubes.ModelsView;
using System.Text;

namespace AppClubes.Controllers
{
    public class ClubsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment env;


        public ClubsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            this.env = env;
        }

        public FileResult Exportar()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("ID; Nombre; Resumen; FechaDeFundación; País; CategoríaId; Categoría;\r\n");
            foreach (Club club in _context.club.Include(a => a.categoria).ToList())
            {
                sb.Append(club.id + ";");
                sb.Append(club.nombre + ";");
                sb.Append(club.resumen + ";");
                sb.Append(club.fechaFund + ";");
                sb.Append(club.pais + ";");
                sb.Append(club.Categoriaid + ";");
                sb.Append(club.categoria.descripcion + ";");
                //Append new line character.
                sb.Append("\r\n");
            }

            return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "listado.csv");
        }

        public IActionResult Importar()
        {
            var archivos = HttpContext.Request.Form.Files;
            if (archivos != null && archivos.Count > 0)
            {
                var archivoImpo = archivos[0];
                var pathDestino = Path.Combine(env.WebRootPath, "importaciones");
                if (archivoImpo.Length > 0)
                {
                    var archivoDestino = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(archivoImpo.FileName);
                    string rutaCompleta = Path.Combine(pathDestino, archivoDestino);
                    using (var filestream = new FileStream(rutaCompleta, FileMode.Create))
                    {
                        archivoImpo.CopyTo(filestream);
                    };

                    using (var file = new FileStream(rutaCompleta, FileMode.Open))
                    {
                        List<string> renglones = new List<string>();
                        List<Club> ClubArch = new List<Club>();

                        StreamReader fileContent = new StreamReader(file); // System.Text.Encoding.Default
                        do
                        {
                            renglones.Add(fileContent.ReadLine());
                        }
                        while (!fileContent.EndOfStream);

                        foreach (string renglon in renglones)
                        {
                            int salida;
                            string[] datos = renglon.Split(';');
                            int categoria = int.TryParse(datos[datos.Length - 1], out salida) ? salida : 0;
                            if (categoria > 0 && _context.categoria.Where(c => c.id == categoria).FirstOrDefault() != null)
                            {
                                Club clubtemporal = new Club()
                                {
                                    Categoriaid = categoria,
                                    nombre = datos[0],
                                    resumen = datos[1],
                                    fechaFund = DateTime.TryParse(datos[2], out DateTime fecha) ? fecha : DateTime.MinValue,
                                    pais = datos[3],
                                };
                                ClubArch.Add(clubtemporal);
                            }
                        }
                        if (ClubArch.Count > 0)
                        {
                            _context.club.AddRange(ClubArch);
                            _context.SaveChanges();
                        }

                        ViewBag.cantReng = ClubArch.Count + " de " + renglones.Count;
                    }
                }
            }

            return View();
        }

        // GET: Clubs
        public async Task<IActionResult> Index(string buscarNombre, string buscarPais, int? categoriaId, int pagina = 1)
        {
            //var applicationDbContext = _context.customer.Include(c => c.healthcareSystem);
            //return View(await applicationDbContext.ToListAsync());
            paginador paginador = new paginador()
            {
                pagActual = pagina,
                regXpag = 3
            };

            var consulta = _context.club.Include(a => a.categoria).Select(a => a);
            if (!string.IsNullOrEmpty(buscarNombre))
            {
                consulta = consulta.Where(e => e.nombre.Contains(buscarNombre));
                //paginador.ValoresQueryString.Add("busqNombre", busqNombre);
            }
            if (!string.IsNullOrEmpty(buscarPais))
            {
                //(busqInsc != null && busqInsc > 0)
                consulta = consulta.Where(e => e.pais.Contains(buscarPais));
                //.ToString().Contains(busqInsc.ToString()));
                //paginador.ValoresQueryString.Add("busqInsc", busqInsc.ToString());
            }
            if (categoriaId.HasValue)
            {
                consulta = consulta.Where(e => e.Categoriaid == categoriaId);
                //paginador.ValoresQueryString.Add("carreraId", carreraId.ToString());
            }

            paginador.cantReg = consulta.Count();

            //paginador.totalPag = (int)Math.Ceiling((decimal)paginador.cantReg / paginador.regXpag);
            var datosAmostrar = consulta
                .Skip((paginador.pagActual - 1) * paginador.regXpag)
                .Take(paginador.regXpag);

            foreach (var item in Request.Query)
                paginador.ValoresQueryString.Add(item.Key, item.Value);

            ClubViewModel information = new ClubViewModel()
            {
                ListaClubes = datosAmostrar.ToList(),
                ListaCategorias = new SelectList(_context.categoria, "id", "descripcion", categoriaId),
                buscarNombre = buscarNombre,
                buscarPais = buscarPais,
                paginador = paginador
            };

            return View(information);
        }

        // GET: Clubs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var club = await _context.club
                .Include(c => c.categoria)
                .FirstOrDefaultAsync(m => m.id == id);
            if (club == null)
            {
                return NotFound();
            }

            return View(club);
        }

        // GET: Clubs/Create
        public IActionResult Create()
        {
            ViewData["Categoriaid"] = new SelectList(_context.categoria, "id", "descripcion");
            return View();
        }

        // POST: Clubs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,nombre,resumen,fechaFund,imagenEscudo,pais,Categoriaid")] Club club)
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
                            club.imagenEscudo = archivoDestino;
                        };
                    }
                }
                _context.Add(club);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Categoriaid"] = new SelectList(_context.categoria, "id", "descripcion", club.Categoriaid);
            return View(club);
        }

        // GET: Clubs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var club = await _context.club.FindAsync(id);
            if (club == null)
            {
                return NotFound();
            }
            ViewData["Categoriaid"] = new SelectList(_context.categoria, "id", "descripcion", club.Categoriaid);
            return View(club);
        }

        // POST: Clubs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,nombre,resumen,fechaFund,imagenEscudo,pais,Categoriaid")] Club club)
        {
            if (id != club.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var files = HttpContext.Request.Form.Files;
                    if (files != null && files.Count > 0)
                    {
                        var filePhoto = files[0];
                        var pathDestiny = Path.Combine(env.WebRootPath, "fotos");
                        if (filePhoto.Length > 0)
                        {
                            var fileDestiny = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(filePhoto.FileName);

                            if (!string.IsNullOrEmpty(club.imagenEscudo))
                            {
                                string photoBefore = Path.Combine(pathDestiny, club.imagenEscudo);
                                if (System.IO.File.Exists(photoBefore))
                                    System.IO.File.Delete(photoBefore);
                            }

                            using (var filestream = new FileStream(Path.Combine(pathDestiny, fileDestiny), FileMode.Create))
                            {
                                filePhoto.CopyTo(filestream);
                                club.imagenEscudo = fileDestiny;
                            };
                        }
                    }
                    _context.Update(club);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClubExists(club.id))
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
            ViewData["Categoriaid"] = new SelectList(_context.categoria, "id", "descripcion", club.Categoriaid);
            return View(club);
        }

        // GET: Clubs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var club = await _context.club
                .Include(c => c.categoria)
                .FirstOrDefaultAsync(m => m.id == id);
            if (club == null)
            {
                return NotFound();
            }

            return View(club);
        }

        // POST: Clubs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var club = await _context.club.FindAsync(id);
            _context.club.Remove(club);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClubExists(int id)
        {
            return _context.club.Any(e => e.id == id);
        }
    }
}