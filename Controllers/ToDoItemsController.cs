using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Mono.TextTemplating;
using ToDoList.Data;
using ToDoList.Models;
using ToDoList.Services.Interfaces;

namespace ToDoList.Controllers
{
    [Authorize]
    public class ToDoItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IToDoListService _toDoListService;

        public ToDoItemsController(ApplicationDbContext context, UserManager<AppUser> userManager, IToDoListService toDoListService)
        {
            _context = context;
            _userManager = userManager;
            _toDoListService = toDoListService;
        }

        // GET: ToDoItems
        public async Task<IActionResult> Index(int? accessoryId)
        {
            string userId = _userManager.GetUserId(User)!;

            // Get the ToDoItems from the appuser
            List<ToDoItem> toDoItem = new List<ToDoItem>();
                
            // Get the Accessory from the appuser based on whether they have chosen an accessory to "filter" by
            List<Accessory> accessories = await _context.Accessory.Where(a => a.AppUserId == userId).ToListAsync();

            if (accessoryId == null)
            {
                toDoItem = await _context.ToDoItem
                                         .Where(t => t.AppUserId == userId)
                                         .Include(t => t.Accessories)
                                         .ToListAsync();
            }
            else
            {
                toDoItem = (await _context.Accessory
                                         .Include(t => t.ToDoItems)
                                         .FirstOrDefaultAsync(t => t.Id == accessoryId && t.AppUserId == userId))!
                                         .ToDoItems.ToList();
            }

            toDoItem = await _context.ToDoItem.Where(t => t.AppUserId == userId && t.Completed == false).ToListAsync();

            ViewData["AccessoryId"] = new SelectList(accessories, "Id", "Name", accessoryId);

            return View(toDoItem);
        }

        // GET: completedItems
        public async Task<IActionResult> CompletedItems(int? accessoryId)
        {
            string userId = _userManager.GetUserId(User)!;

            // Get the ToDoItems from the appuser
            List<ToDoItem> toDoItem = new List<ToDoItem>();

            // Get the Accessory from the appuser based on whether they have chosen an accessory to "filter" by
            List<Accessory> accessories = await _context.Accessory.Where(a => a.AppUserId == userId).ToListAsync();

            if (accessoryId == null)
            {
                toDoItem = await _context.ToDoItem
                                         .Where(t => t.AppUserId == userId)
                                         .Include(t => t.Accessories)
                                         .ToListAsync();
            }
            else
            {
                toDoItem = (await _context.Accessory
                                         .Include(t => t.ToDoItems)
                                         .FirstOrDefaultAsync(t => t.Id == accessoryId && t.AppUserId == userId))!
                                         .ToDoItems.ToList();
            }

            toDoItem = await _context.ToDoItem.Where(t => t.AppUserId == userId && t.Completed == true).ToListAsync();

            ViewData["AccessoryId"] = new SelectList(accessories, "Id", "Name", accessoryId);

            return View(toDoItem);
        }

        // GET: ToDoItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ToDoItem == null)
            {
                return NotFound();
            }

            var toDoItem = await _context.ToDoItem
                .Include(t => t.AppUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (toDoItem == null)
            {
                return NotFound();
            }

            return View(toDoItem);
        }

        // GET: ToDoItems/Create
        public async Task<IActionResult> Create()
        {
            // Query and present list of user specified Categories
            string userId = _userManager.GetUserId(User)!;

            IEnumerable<Accessory> accessoriesList = await _context.Accessory
                .Where(a => a.AppUserId == userId)
                .ToListAsync();

            ViewData["AccessoryList"] = new MultiSelectList(accessoriesList, "Id", "Name");

            return View();
        }

        // POST: ToDoItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,AppUserId,Created,DueDate,Completed")] ToDoItem toDoItem, IEnumerable<int> selected)
        {
            ModelState.Remove("AppUserId");

            if (ModelState.IsValid)
            {
                toDoItem.AppUserId = _userManager.GetUserId(User);

                toDoItem.Created = DateTime.UtcNow;

                toDoItem.DueDate = DateTime.UtcNow;

                if (toDoItem.DueDate != null)
                {
                    toDoItem.DueDate = DateTime.SpecifyKind(toDoItem.DueDate.Value, DateTimeKind.Utc);
                }

                _context.Add(toDoItem);
                await _context.SaveChangesAsync();

                //TODO: Add Service call
                await _toDoListService.AddToDoItemToAccessoriesAsync(selected, toDoItem.Id);

                return RedirectToAction(nameof(Index));
            }

            return View(toDoItem);
        }

        // GET: ToDoItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ToDoItem == null)
            {
                return NotFound();
            }

            var toDoItem = await _context.ToDoItem
                                         .Include(t => t.Accessories)
                                         .FirstOrDefaultAsync(t => t.Id == id);

            if (toDoItem == null)
            {
                return NotFound();
            }

            // Query and present list of user specified Accessories
            string userId = _userManager.GetUserId(User)!;

            IEnumerable<Accessory> accessoriesList = await _context.Accessory
                                                                   .Where(t => t.AppUserId == userId)
                                                                   .ToListAsync();

            IEnumerable<int> currentAccessories = toDoItem!.Accessories.Select(t => t.Id);

            ViewData["AccessoryList"] = new MultiSelectList(accessoriesList, "Id", "Name", currentAccessories);

            return View(toDoItem);
        }

        // POST: ToDoItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,AppUserId,Created,DueDate,Completed")] ToDoItem toDoItem, IEnumerable<int> selected)
        {
            if (id != toDoItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Reformat Created Date
                    toDoItem.Created = DateTime.SpecifyKind(toDoItem.Created, DateTimeKind.Utc);

                    // Reformat Birth Date
                    if (toDoItem.DueDate != null)
                    {
                        toDoItem.DueDate = DateTime.SpecifyKind(toDoItem.DueDate.Value, DateTimeKind.Utc);
                    }

                    _context.Update(toDoItem);
                    await _context.SaveChangesAsync();

                    if (selected != null)
                    {
                        // 1. Remove Contact's categories
                        await _toDoListService.RemoveAllAccessoriesAsync(toDoItem.Id);
                        // 2. Add selected Categories to the contact
                        await _toDoListService.AddToDoItemToAccessoriesAsync(selected, toDoItem.Id);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ToDoItemExists(toDoItem.Id))
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
            ViewData["AppUserId"] = new SelectList(_context.Set<AppUser>(), "Id", "Id", toDoItem.AppUserId);
            return View(toDoItem);
        }

        // GET: ToDoItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ToDoItem == null)
            {
                return NotFound();
            }

            var toDoItem = await _context.ToDoItem
                .Include(t => t.AppUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (toDoItem == null)
            {
                return NotFound();
            }

            return View(toDoItem);
        }

        // POST: ToDoItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ToDoItem == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ToDoItem'  is null.");
            }
            var toDoItem = await _context.ToDoItem.FindAsync(id);
            if (toDoItem != null)
            {
                _context.ToDoItem.Remove(toDoItem);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ToDoItemExists(int id)
        {
          return (_context.ToDoItem?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
