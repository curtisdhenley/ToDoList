using Microsoft.EntityFrameworkCore;
using ToDoList.Data;
using ToDoList.Models;
using ToDoList.Services.Interfaces;

namespace ToDoList.Services
{
    public class ToDoListService : IToDoListService
    {
        private readonly ApplicationDbContext _context;

        public ToDoListService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddToDoItemToAccessoriesAsync(IEnumerable<int> accessoryIds, int toDoItemId)
        {
            try
            {
                ToDoItem? toDoItem = await _context.ToDoItem
                    .Include(t => t.Accessories)
                    .FirstOrDefaultAsync(t => t.Id == toDoItemId);

                foreach (int accessoryId in accessoryIds)
                {
                    Accessory? accessory = await _context.Accessory.FindAsync(accessoryId);

                    if (toDoItem != null && accessory != null)
                    {
                        toDoItem.Accessories.Add(accessory);
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task AddToDoItemToAccessoryAsync(int accessoryId, int toDoItemId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Accessory>> GetAppUserAccessoriesAsync(string appUserId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsToDoItemInAccessory(int accessoryId, int toDoItemId)
        {
            try
            {
                ToDoItem? toDoItem = await _context.ToDoItem
                    .Include(t => t.Accessories)
                    .FirstOrDefaultAsync(t => t.Id == toDoItemId);

                bool inCategory = toDoItem!.Accessories.Select(c => c.Id).Contains(accessoryId);

                return inCategory;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task RemoveAllAccessoriesAsync(int toDoItemId)
        {
            try
            {
                ToDoItem? toDoItem = await _context.ToDoItem
                    .Include(t => t.Accessories)
                    .FirstOrDefaultAsync(t => t.Id == toDoItemId);

                toDoItem!.Accessories.Clear();
                _context.Update(toDoItem);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
