using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POE_part_2.Models;

namespace POE_part_2.Controllers
{
    public class ArtWorkController : Controller
    {
        private readonly KhumaloCraftContext _context;

        public ArtWorkController(KhumaloCraftContext context)
        {
            _context = context;
        }

        // GET: ArtWork
        public async Task<IActionResult> Index()
        {
            var khumaloCraftContext = _context.ArtWork.Include(a => a.Artist);
            return View(await khumaloCraftContext.ToListAsync());
        }

        // GET: ArtWork/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artWork = await _context.ArtWork
                .Include(a => a.Artist)
                .FirstOrDefaultAsync(m => m.ArtWorkId == id);
            if (artWork == null)
            {
                return NotFound();
            }

            return View(artWork);
        }
        [Authorize(Roles = "Admin,Artist")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        //[Authorize(Roles = "Admin,Artist")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductName,Price,Picture,Availability,Quantity,MaxQuantity")] ArtWork artWork/*, IFormFile imageFile*/)
        {
            if (ModelState.IsValid)
            {
                artWork.ArtWorkId = Guid.NewGuid().ToString();
                artWork.UserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                //if (imageFile != null && imageFile.Length > 0)
                //{
                //    using (var memoryStream = new MemoryStream())
                //    {
                //        await imageFile.CopyToAsync(memoryStream);
                //        artWork.Picture = memoryStream.ToArray();
                //    }

                //    artWork.Quantity = artWork.MaxQuantity;
                //    artWork.QuatityThreshold = (int)(artWork.MaxQuantity * 0.2);
                //}

                _context.Add(artWork);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(artWork);
        }


        [Authorize(Roles = "Admin,Artist")]
        // GET: ArtWork/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artWork = await _context.ArtWork.FindAsync(id);
            if (artWork == null)
            {
                return NotFound();
            }

            return View(artWork);
        }

        [Authorize(Roles = "Admin,Artist")]
        // POST: ArtWork/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ArtWorkId,UserId,ProductName,Price,Picture,Availability,Quantity,QuatityThreshold,MaxQuantity")] ArtWork artWork)
        {
            if (id != artWork.ArtWorkId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(artWork);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArtWorkExists(artWork.ArtWorkId))
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

            return View(artWork);
        }

        [Authorize(Roles = "Admin,Artist")]
        // GET: ArtWork/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artWork = await _context.ArtWork
                .Include(a => a.Artist)
                .FirstOrDefaultAsync(m => m.ArtWorkId == id);
            if (artWork == null)
            {
                return NotFound();
            }

            return View(artWork);
        }

        [Authorize(Roles = "Admin,Artist")]
        // POST: ArtWork/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var artWork = await _context.ArtWork.FindAsync(id);
            if (artWork != null)
            {
                _context.ArtWork.Remove(artWork);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArtWorkExists(string id)
        {
            return _context.ArtWork.Any(e => e.ArtWorkId == id);
        }
    }
}
