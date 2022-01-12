#nullable disable

using AdminHubNC6.Data;
using AdminHubNC6.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AdminHubNC6.Controllers
{
    public class TodoEventController : Controller
    {
        private readonly TodoEventDbContext _context;

        public TodoEventController(TodoEventDbContext context)
        {
            _context = context;
        }

        // GET: TodoEvent
        public async Task<IActionResult> Index()
        {
            return View(await _context.TodoEvent.ToListAsync());
        }

        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetEvents()
        {
            Debug.WriteLine("GetEvents");
            var result = _context.TodoEvent;
            if (!result.Any())
            {
                return NotFound();
            }

            return Json(await _context.TodoEvent.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveEvent([Bind("id,title,start,end,description,color,allDay")] TodoEventModel todoEventModel)
        {
            var status = false;
            Debug.WriteLine("SaveEvent");
            Debug.WriteLine("[id]:" + todoEventModel.id);
            Debug.WriteLine("[titls]:" + todoEventModel.title);
            Debug.WriteLine("[start]:" + todoEventModel.start);
            Debug.WriteLine("[end]:" + todoEventModel.end);
            Debug.WriteLine("[description]:" + todoEventModel.description);
            Debug.WriteLine("[color]:" + todoEventModel.color);
            Debug.WriteLine("[allDay]:" + todoEventModel.allDay);

            if (ModelState.IsValid)
            {
                try
                {
                    if (todoEventModel.id > 0)
                    {
                        Debug.WriteLine("[Update Event]");
                        _context.Update(todoEventModel);
                    }
                    else
                    {
                        Debug.WriteLine("[Add Event]");
                        _context.Add(todoEventModel);
                    }

                    await _context.SaveChangesAsync();
                    status = true;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TodoEventModelExists(todoEventModel.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            Debug.WriteLine("[Before return]");
            return new JsonResult(new { status });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEvent(int eventID)
        {
            var status = false;
            var todoEventModel = await _context.TodoEvent.FindAsync(eventID);
            if (todoEventModel != null)
            {
                _context.TodoEvent.Remove(todoEventModel);
                await _context.SaveChangesAsync();
                status = true;
            }

            return new JsonResult(new { status });
        }       

        private bool TodoEventModelExists(int id)
        {
            return _context.TodoEvent.Any(e => e.id == id);
        }
    }
}