using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using task_management_system.Data;
using task_management_system.Models;

namespace task_management_system.Controllers;

[Authorize]
public class NotificationController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public NotificationController(ApplicationDbContext context, UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // GET /Notifications
    [Authorize(Roles = "Developer,ProjectManager")]
    [Route("/Notifications")]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var notifications = _context.Notifications.Where(n => n.UserId == user.Id);
        return View(notifications);
    }

    // GET /Notifications/Details/{id}
    [Authorize(Roles = "Developer,ProjectManager")]
    [Route("/Notifications/Details/{id}")]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return BadRequest();
        }
        var notification = await _context.Notifications.FindAsync(id);

        if (notification == null)
        {
            return NotFound();
        }

        notification.IsOpened = true;
        try
        {
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        return View(notification);
    }
}
