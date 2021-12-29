using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TodoApi.Controllers;
using TodoApi.Models;

namespace TodoApiDTO.Repository
{
    public class TodoItemRepository
    {
        private readonly TodoContext _context;
        private readonly ILogger<TodoItemRepository> _logger;

        public TodoItemRepository(TodoContext context, ILogger<TodoItemRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IQueryable<TodoItemDTO> ReadAll()
        {
            return _context.TodoItems
                .Select(x => ItemToDTO(x));
        }

        public async Task<bool> UpdateAsync(long id, TodoItemDTO todoItemDTO)
        {
            if (id != todoItemDTO.Id)
            {
                _logger.LogError($"ERROR: Entity ID({id}) not equal todoItemDTO ID ({todoItemDTO.Id})");
                return false;
            }

            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return false;
            }

            todoItem.Name = todoItemDTO.Name;
            todoItem.IsComplete = todoItemDTO.IsComplete;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e) when (!TodoItemExists(id))
            {
                _logger.LogError($"ERROR: {e.Message},\n {e.StackTrace}");
                return false;
            }

            return true;
        }

        public async Task<TodoItem> CreateAsync(TodoItemDTO todoItemDTO)
        {
            var todoItem = new TodoItem
            {
                IsComplete = todoItemDTO.IsComplete,
                Name = todoItemDTO.Name
            };

            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();
            return todoItem;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return false;
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return true;
        }

        private bool TodoItemExists(long id) =>
             _context.TodoItems.Any(e => e.Id == id);

        public static TodoItemDTO ItemToDTO(TodoItem todoItem) =>
            new TodoItemDTO
            {
                Id = todoItem.Id,
                Name = todoItem.Name,
                IsComplete = todoItem.IsComplete
            };
    }
}
