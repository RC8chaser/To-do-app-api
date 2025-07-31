using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using System.Collections.Concurrent;
using System.Linq;
using System.Collections.Generic;

namespace TodoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        private static readonly ConcurrentDictionary<int, TodoItem> _todos = new();
        private static int _nextId = 1;

        [HttpGet]
        public ActionResult<IEnumerable<TodoItem>> GetAll()
        {
            return Ok(_todos.Values.OrderBy(t => t.Id));
        }

        [HttpPost]
        public ActionResult<TodoItem> Create([FromBody] TodoItem item)
        {
            item.Id = _nextId++;
            item.CreatedAt = DateTime.UtcNow;
            item.UpdatedAt = null;

            if (string.IsNullOrEmpty(item.Priority))
                item.Priority = "Medium";
            if (string.IsNullOrEmpty(item.Status))
                item.Status = "Not Done";

            _todos.TryAdd(item.Id, item);
            return CreatedAtAction(nameof(GetAll), item);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] TodoItem updatedItem)
        {
            if (!_todos.TryGetValue(id, out var existing))
                return NotFound();

            existing.Title = updatedItem.Title;
            existing.IsComplete = updatedItem.IsComplete;
            existing.Priority = updatedItem.Priority;
            existing.Status = updatedItem.Status;
            existing.UpdatedAt = DateTime.UtcNow;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (!_todos.TryRemove(id, out _))
                return NotFound();

            return NoContent();
        }
    }
}
