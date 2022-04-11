using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using task_management_system.Data;
using task_management_system.Models;
using Task = System.Threading.Tasks.Task;

namespace task_management_system.Controllers;
[Authorize]
public class TaskController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public TaskController(ApplicationDbContext context, UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // GET
    [Authorize(Roles = "Developer")]
    [Route("/Developer")]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var tasks = _context
            .Tasks
            .Include(t => t.Project)
            .Where(t => t.DeveloperId == user.Id);

        return View(tasks);
    }

    // GET /Tasks/{id}/Update
    [Authorize(Roles = "Developer")]
    [Route("/Developer/Tasks/{id}/Update")]
    public async Task<IActionResult> UpdateCompletion(int? id)
    {
        if (id == null)
        {
            return BadRequest();
        }

        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);

        return View(task);
    }

    // POST /Tasks/{id}/Update
    [Authorize(Roles = "Developer")]
    [HttpPost]
    [Route("/Developer/Tasks/{id}/Update")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateCompletion(
        int? id,
        decimal completionPercentage,
        bool isCompleted = false
    )
    {
        if (id == null)
        {
            return BadRequest();
        }

        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);


        if (task == null)
        {
            return NotFound();
        }

        task.CompletionPercentage = completionPercentage;
        if (task.CompletionPercentage == 100)
        {
            task.IsCompleted = isCompleted;
        }

        try
        {
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Task");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    // GET /Developer/Task/{id}/LeaveNote
    [Authorize(Roles = "Developer")]
    [Route("/Developer/Task/{id}/LeaveNote")]
    public IActionResult LeaveNote(int? id)
    {
        if (id == null)
        {
            return BadRequest();
        }

        ViewBag.TaskId = id;
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Developer")]
    [Route("/Developer/Task/{id}/LeaveNote")]
    public async Task<IActionResult> LeaveNote([Bind("Body,TaskId")] Note note)
    {
        var task = await _context.Tasks.FindAsync(note.TaskId);
        var user = await _userManager.GetUserAsync(User);
        if (task == null)
        {
            return NotFound();
        }

        Note newNote = new(note.Body, user, task);

        try
        {
            _context.Notes.Add(newNote);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [Authorize(Roles = "Developer")]
    public async Task<IActionResult> CommentOnNote(int? id)
    {
        if (id == null)
        {
            return BadRequest();
        }

        var note = await _context.Notes.FirstOrDefaultAsync(n => n.Id == id);

        if (note == null)
        {
            return NotFound();
        }

        return View();
    }

    // GET /Task/Details/{id}
    [Authorize(Roles = "ProjectManager,Developer")]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return BadRequest();
        }

        var task = await _context
            .Tasks
            .Include(t => t.Project)
            .Include(t => t.Developer)
            .Include(t => t.Notes)
            .ThenInclude(n => n.Comments)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task == null)
        {
            return NotFound();
        }

        return View(task);
    }
}
