using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Gaming.Data;
using Gaming.Models;
using Gaming.Tools;
using Azure.ResourceManager.Resources;

namespace Gaming.Controllers
{
    public class VirtualMsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VirtualMsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: VirtualMs
        public async Task<IActionResult> Index()
        {
            return _context.VirtualMs != null ?
                        View(await _context.VirtualMs.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.VirtualMs'  is null.");
        }

        // GET: VirtualMs/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.VirtualMs == null)
            {
                return NotFound();
            }

            var virtualM = await _context.VirtualMs
                .FirstOrDefaultAsync(m => m.Name == id);
            if (virtualM == null)
            {
                return NotFound();
            }

            return View(virtualM);
        }

        // GET: VirtualMs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: VirtualMs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Login,Password")] VirtualM virtualM)
        {
            ModelState.Remove(nameof(virtualM.Name));
            ModelState.Remove(nameof(virtualM.IP));
            ModelState.Remove(nameof(virtualM.IsStarted));

            if (ModelState.IsValid)
            {
                AzureTools azureTools = new(GetUserName());

                ResourceGroupResource resourceGroup = await azureTools.GetResourceGroupAsync();

                azureTools.CreateVirtualMachine(resourceGroup, virtualM.Login, virtualM.Password);


                // Données temporaire en attendant la méthode de création d'une VM
                virtualM.Name = GetUserName();
                virtualM.IP = "192.168.1.1";
                virtualM.IsStarted = true;

                _context.Add(virtualM);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(virtualM);
        }

        // GET: VirtualMs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.VirtualMs == null)
            {
                return NotFound();
            }

            var virtualM = await _context.VirtualMs.FindAsync(id);
            if (virtualM == null)
            {
                return NotFound();
            }
            return View(virtualM);
        }

        // POST: VirtualMs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Name,Login,Password,IP,IsStarted")] VirtualM virtualM)
        {
            if (id != virtualM.Name)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(virtualM);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VirtualMExists(virtualM.Name))
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
            return View(virtualM);
        }

        // GET: VirtualMs/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.VirtualMs == null)
            {
                return NotFound();
            }

            var virtualM = await _context.VirtualMs
                .FirstOrDefaultAsync(m => m.Name == id);
            if (virtualM == null)
            {
                return NotFound();
            }

            return View(virtualM);
        }

        // POST: VirtualMs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.VirtualMs == null)
            {
                return Problem("Entity set 'ApplicationDbContext.VirtualMs'  is null.");
            }
            var virtualM = await _context.VirtualMs.FindAsync(id);
            if (virtualM != null)
            {
                _context.VirtualMs.Remove(virtualM);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VirtualMExists(string id)
        {
            return (_context.VirtualMs?.Any(e => e.Name == id)).GetValueOrDefault();
        }

        private string GetUserName()
        {
            string user = string.Empty;
            if (HttpContext.User.Identity != null &&
                HttpContext.User.Identity.Name != null)
            {
                user = HttpContext.User.Identity.Name;
                user = user.Split("@")[0].Replace(".", "");
            }
            return user;
        }
    }
}
