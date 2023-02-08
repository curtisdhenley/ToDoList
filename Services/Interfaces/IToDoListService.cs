using ToDoList.Models;

namespace ToDoList.Services.Interfaces
{
    public interface IToDoListService
    {
        public Task AddToDoItemToAccessoryAsync(int accessoryId, int toDoItemId);

        public Task AddToDoItemToAccessoriesAsync(IEnumerable<int> accessoryId, int toDoItemId);

        public Task<IEnumerable<Accessory>> GetAppUserAccessoriesAsync(string appUserId);

        public Task<bool> IsToDoItemInAccessory(int accessoryId, int toDoItemId);

        public Task RemoveAllAccessoriesAsync(int toDoItemId);
    }
}
