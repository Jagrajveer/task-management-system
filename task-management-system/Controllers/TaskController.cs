using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using task_management_system.Data;
using task_management_system.Models;
using static task_management_system.Models.Task;
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
    // GET /Task/Create
    public async Task<IActionResult> Create(int? id)
    {
        if (id == null)
        {
            return BadRequest();
        }

        ViewBag.ProjectId = id;

        var users = _userManager.Users;

        List<User> developers = new();
        foreach (var user in users)
        {
            if (await _userManager.IsInRoleAsync(user, "Developer"))
            {
                developers.Add(user);
            }
        }

        ViewBag.Developers = new SelectList(developers, "Id", "UserName");


        var priorities = from PriorityType p in Enum.GetValues(typeof(PriorityType))
                         select new { ID = (int)p, Name = p.ToString() };
        ViewBag.Priority = new SelectList(priorities, "ID", "Name");

        ViewBag.Projects = new SelectList(_context.Projects.ToList(), "Id", "Name");
        return View();
    }

    [Authorize(Roles = "ProjectManager")]
    // POST /Task/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Title,Content,ProjectId,DeveloperId,Priority,Deadline")] Models.Task task)
    {
        Project? project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == task.ProjectId);
        var developer = await _userManager.FindByIdAsync(task.DeveloperId);

        if (project == null || developer == null)
        {
            return NotFound();
        }

        Models.Task newTask = new(task.Title, task.Content, project,task.Deadline, developer, task.Priority);

        try
        {
            await _context.Tasks.AddAsync(newTask);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    // GET /Task/Edit{id}
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return BadRequest();
        }

        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);

        return View(task);
    }

    [Authorize(Roles = "ProjectManager")]
    // POST /Task/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int? id,
        string title,
        string content,
        PriorityType priority,
        DateTime deadline,
        string developerId
    )
    {
        if (id == null)
        {
            return BadRequest();
        }

        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);

        var developer = await _userManager.FindByIdAsync(developerId);

        if (task == null || developer == null)
        {
            return NotFound();
        }

        task.Title = title;
        task.Content = content;
        task.Priority = priority;
        task.Deadline = deadline;
        task.Developer = developer;
        task.DeveloperId = developer.Id;

        try
        {
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();

            return RedirectToAction("DashBoard", "Home");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    [Authorize(Roles = "ProjectManager")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return BadRequest();
        }

        var task = await _context.Tasks.FirstOrDefaultAsync(p => p.Id == id);

        if (task == null)
        {
            return NotFound();
        }

        return View(task);
    }

    [Authorize(Roles = "ProjectManager")]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        if (id == null)
        {
            return BadRequest();
        }

        var task = await _context
            .Tasks
            .Include(p => p.Notes)
            //.Include(p => p.ProjectDevelopers)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (task == null)
        {
            return NotFound();
        }

        try
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }
}
