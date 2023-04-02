using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Labo09.Data;
using Labo09.Models;
using Labo09.Models.ViewModels;
using System.Reflection.Metadata;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;

namespace Labo09.Controllers
{
    public class ArtistesController : Controller
    {
        private readonly Lab09_EmployesContext _context;

        public ArtistesController(Lab09_EmployesContext context)
        {
            _context = context;
        }

        // GET: Artistes
        public async Task<IActionResult> Index()
        {
            var lab09_EmployesContext = _context.Artistes.Include(a => a.Employe);
            return View(await lab09_EmployesContext.ToListAsync());
        }
        // GET: Artistes
        public async Task<IActionResult> IndexV2()
        {
            IEnumerable<VwListeArtiste> artistes = await _context.VwListeArtistes.ToListAsync();
            return View(artistes);
        }

        // GET: Artistes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Artistes == null)
            {
                return NotFound();
            }

            var artiste = await _context.Artistes
                .Include(a => a.Employe)
                .FirstOrDefaultAsync(m => m.ArtisteId == id);
            if (artiste == null)
            {
                return NotFound();
            }

            return View(artiste);
        }

        // GET: Artistes/Create
        public IActionResult Create()
        {
            //ViewData["EmployeId"] = new SelectList(_context.Employes, "EmployeId", "EmployeId");
            return View();
        }

        // POST: Artistes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Artiste,Employe")] ArtisteEmployeViewModel artisteEmploye)
        {
            //if (ModelState.IsValid)
            //{
            //    _context.Add(artiste);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}
            //ViewData["EmployeId"] = new SelectList(_context.Employes, "EmployeId", "EmployeId", artiste.EmployeId);
            //return View(artiste);
            //string that hold la req
            string query = "EXEC Employes.USP_AjouterArtiste @Prenom, @Nom, @NoTel, @Courriel, @Specialite";
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter(parameterName : "@Prenom" ,value : artisteEmploye.Employe.Prenom),
                new SqlParameter(parameterName :  "@Nom" , value :  artisteEmploye.Employe.Nom),
                new SqlParameter(parameterName : "@NoTel" ,  value : artisteEmploye.Employe.NoTel),
                new SqlParameter(parameterName :  "@Courriel" ,value :   artisteEmploye.Employe.Courriel),
                new SqlParameter(parameterName :  "@Specialite" ,value :   artisteEmploye.Employe.Nom),
            };
            await _context.Database.ExecuteSqlRawAsync(query , parameters.ToArray());
            //
            await _context.SaveChangesAsync();
            return View(artisteEmploye);
        }

        // GET: Artistes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Artistes == null)
            {
                return NotFound();
            }

            var artiste = await _context.Artistes.FindAsync(id);
            var employe = await _context.Employes.FindAsync(artiste.EmployeId);

            if (artiste == null || employe == null)
            {
                return NotFound();
            }
            ArtisteEmployeViewModel vm = new ArtisteEmployeViewModel
            {
                Artiste = artiste,
                Employe = employe
            };
            //ViewData["EmployeId"] = new SelectList(_context.Employes, "EmployeId", "EmployeId", artiste.EmployeId);
            return View(vm);
        }

        // POST: Artistes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ArtisteEmployeViewModel artisteEmploye)
        {

            artisteEmploye.Artiste.Employe = artisteEmploye.Employe;
            //obj bool qui check des truc
            bool isValid = Validator.TryValidateObject(artisteEmploye,
                                new ValidationContext(artisteEmploye),
                                new List<ValidationResult>(), true);
            if (id != artisteEmploye.Artiste.ArtisteId)
            {
                return NotFound();
            }

            if (isValid)
            {
                try
                {
                    _context.Update(artisteEmploye.Artiste);
                    _context.Update(artisteEmploye.Employe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArtisteExists(artisteEmploye.Artiste.ArtisteId))
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
            //ViewData["EmployeId"] = new SelectList(_context.Employes, "EmployeId", "EmployeId", artiste.EmployeId);
            return View(artisteEmploye);
        }

        // GET: Artistes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Artistes == null)
            {
                return NotFound();
            }

            var artiste = await _context.Artistes
                .Include(a => a.Employe)
                .FirstOrDefaultAsync(m => m.ArtisteId == id);
            if (artiste == null)
            {
                return NotFound();
            }

            return View(artiste);
        }

        // POST: Artistes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Artistes == null)
            {
                return Problem("Entity set 'Lab09_EmployesContext.Artistes'  is null.");
            }
            var artiste = await _context.Artistes.FindAsync(id);
            if (artiste != null)
            {
                _context.Artistes.Remove(artiste);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArtisteExists(int id)
        {
          return (_context.Artistes?.Any(e => e.ArtisteId == id)).GetValueOrDefault();
        }
    }
}
