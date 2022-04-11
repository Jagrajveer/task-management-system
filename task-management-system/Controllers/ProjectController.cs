using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using task_management_system.Data;
using task_management_system.Models;

namespace task_management_system.Controllers;

[Authorize(Roles = "ProjectManager")]
public class ProjectController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public ProjectController(ApplicationDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET
    [Route("/ProjectManager")]
    public IActionResult Index()
    {
        var projects = _context.Projects.Include(p => p.ProjectManager);
        return View(projects);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return BadRequest();
        }

        var project = await _context
            .Projects
            .Include(p => p.ProjectManager)
            .Include(p => p.Tasks)
            .ThenInclude(t => t.Developer)
            .Include(p => p.ProjectDevelopers)
            .ThenInclude(pd => pd.Developer)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null)
        {
            return NotFound();
        }

        return View(project);
    }

    // GET /Project/Create
    public async Task<IActionResult> Create()
    {
        var users = _userManager.Users;
        List<User> projectManager = new();
        foreach (var user in users)
        {
            if (await _userManager.IsInRoleAsync(user, "ProjectManager"))
            {
                projectManager.Add(user);
            }
        }
        ViewBag.ProjectManagers = new SelectList(projectManager, "Id", "UserName");
        return View();
    }
    
    // POST /Project/Create
    [HttpPost]
    public async Task<IActionResult> Create([Bind("Name,Body,ProjectManagerId,Deadline,Budget")] Project project)
    {
        var projectManager = _userManager.Users.First(u => u.Id == project.ProjectManagerId);

        Project newProject = new(project.Name, project.Body, project.Deadline, project.Budget, projectManager);

        try
        {
            await _context.AddAsync(newProject);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    // GET /Project/Edit/{id}
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return BadRequest();
        }

        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);

        if (project == null)
        {
            return NotFound();
        }

        return View(project);
    }
    // POST /Project/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int? id,
        string name,
        string body,
        DateTime deadline,
        decimal budget,
        decimal moneySpent,
        string projectManagerId
    )
    {
        if (id == null)
        {
            return BadRequest();
        }
        var updatedProject = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);

        //var projectManager = await _userManager.FindByIdAsync(projectManagerId); 

        if (updatedProject == null)
        {
            return NotFound();
        }

        updatedProject.Name = name;
        updatedProject.Body = body;
        updatedProject.Deadline = deadline;
        updatedProject.Budget = budget;
        updatedProject.MoneySpent = moneySpent;
        // updatedProject.ProjectManager = projectManager;
        // updatedProject.ProjectManagerId = projectManager.Id;

        try
        {
            _context.Projects.Update(updatedProject);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Project");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<IActionResult> AddDeveloperToProject()
    {
        ViewBag.Projects = new SelectList(_context.Projects, "Id", "Name");

        List<User> developers = new();
        foreach (var user in _userManager.Users)
        {
            if (await _userManager.IsInRoleAsync(user, "Developer"))
            {
                developers.Add(user);
            }
        }
        ViewBag.Developers = new SelectList(developers, "Id", "UserName");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddDeveloperToProject(int? projectId, string developerId)
    {
        if (projectId == null && developerId == "")
        {
            return BadRequest();
        }

        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

        var developer = await _userManager.FindByIdAsync(developerId);

        if (project == null && developer == null)
        {
            return NotFound();
        }

        ProjectDeveloper pd = new(project, developer);

        try
        {
            _context.Add(pd);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }


    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return BadRequest();
        }

        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);

        if (project == null)
        {
            return NotFound();
        }

        return View(project);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        if (id == null)
        {
            return BadRequest();
        }

        var project = await _context
            .Projects
            .Include(p => p.ProjectManager)
            //.Include(p => p.ProjectDevelopers)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null)
        {
            return NotFound();
        }

        try
        {
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public IActionResult ProjectExceedingBudget()
    {
        var projects = _context.Projects.Where(p => p.MoneySpent > p.Budget).ToList();

        return View(projects);
    }

    public IActionResult NotFinishedPassedDeadline()
    {
        var tasks = _context.Tasks.Where(t => !t.IsCompleted && t.Deadline < t.CreatedAt).ToList();

        return View(tasks);
    }
}
