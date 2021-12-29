using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TodoApi.Models;
using TodoApiDTO.Repository;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;
        private readonly ILogger<TodoItemRepository> _logger;
        private TodoItemRepository _todoItemRepository;

        public TodoItemsController(TodoContext context, ILogger<TodoItemRepository> logger)
        {
            _context = context;
            _logger = logger;
            _todoItemRepository = new TodoItemRepository(_context, _logger);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItems()
        {
            return await _todoItemRepository.ReadAll().ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemDTO>> GetTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return TodoItemRepository.ItemToDTO(todoItem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodoItem(long id, TodoItemDTO todoItemDTO)
        {
            var result = await _todoItemRepository.UpdateAsync(id, todoItemDTO);
            if (result)
                return NoContent();
            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<TodoItemDTO>> CreateTodoItem(TodoItemDTO todoItemDTO)
        {
            var todoItem = await _todoItemRepository.CreateAsync(todoItemDTO);

            return CreatedAtAction(
                nameof(GetTodoItem),
                new { id = todoItem.Id },
                TodoItemRepository.ItemToDTO(todoItem));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            var result = await _todoItemRepository.DeleteAsync(id);
            if (result)
                return NoContent();
            return NotFound();
        }
    }
}
