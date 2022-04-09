using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using task_management_system.Data;
using task_management_system.Models;

namespace task_management_system.Controllers; 
[Authorize(Roles="ProjectManager")]
public class UserController : Controller {
    private readonly ApplicationDbContext _context;

    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    
    public UserController(ApplicationDbContext context, UserManager<User> userManager,  RoleManager<IdentityRole> roleManager){
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }
    
    // GET
    public IActionResult Index() {
        var users = _userManager.Users;
        return View(users);
    }
    
    // GET /User/AddRole
    public IActionResult AddToRole() {
        ViewBag.users = new SelectList( _userManager.Users.ToList(), "Id", "UserName");
        ViewBag.roles = new SelectList( _roleManager.Roles.ToList(), "Id", "Name");
        
        return View();
    }
    
    // POST /User/AddRole{userId}{roleId}
    [HttpPost]
    [AutoValidateAntiforgeryToken]
    public async Task<IActionResult> AddRole(string userId, string roleId) {
        Console.WriteLine(userId);
        Console.WriteLine(roleId);
        
        var user = await _userManager.FindByIdAsync(userId);
        var role = await _roleManager.FindByIdAsync(roleId);

        Console.WriteLine(user.UserName);
        Console.WriteLine(role.Name);
        
        try {
            await _userManager.AddToRoleAsync(user, role.Name);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "User");
        }
        catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }
    }
    
    // GET /User/GetRolesOfAUser
    public IActionResult GetRolesOfAUser()  {
        ViewBag.roles = null!;
        var users = _context.Users.Where(u => u.UserName != "Admin").ToList();
        ViewBag.users = new SelectList(users, "Id", "UserName");
        return View();
    }
    
    // POST /User/GetRolesOfAUser{userId}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetRolesOfAUser(string userId) {
        User user = await _userManager.FindByIdAsync(userId);
        
        ViewBag.roles = await _userManager.GetRolesAsync(user);
        
        return View(user);
    }
    
    // GET /User/CheckForUserRole
    public IActionResult CheckForUserRole() {
        ViewBag.IsInRole = null!;
        ViewBag.users = new SelectList( _userManager.Users.ToList(), "Id", "UserName");
        ViewBag.roles = new SelectList( _roleManager.Roles.ToList(), "Id", "Name");
        
        return View();
    }
    
    // POST /User/CheckForUserRole/{userId}{roleUd}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckForUserRole(string userId, string roleId) {
        var user = await _userManager.FindByIdAsync(userId);
        var role = await _roleManager.FindByIdAsync(roleId);
 
        if (await _userManager.IsInRoleAsync(user, role.Name)) {
            ViewBag.IsInRole = true;
        }
        else {
            ViewBag.IsInRole = false;
        }

        ViewBag.Role = role;
        
        return View(user);
    }

    public class InputModel {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        
        public decimal Salary { get; set; }
        public User.PaymentDurationType PaymentDuration { get; set; }
    }
    
    // GET /User/Create
    public IActionResult Create() {
        //ViewBag.PaymentDurationType = new SelectList(Enum.GetValues(typeof(User.PaymentDurationType)).Cast<User.PaymentDurationType>());
        return View();
    }
    
    // POST /User/Create
    [HttpPost]
    public async Task<IActionResult> Create([Bind("Email,Salary,PaymentDuration,Password,ConfirmPassword")] InputModel user) {
        User newUser = new() {
            UserName = user.Email,
            Email = user.Email,
            NormalizedUserName = user.Email.ToUpper(),
            NormalizedEmail = user.Email.ToUpper(),
            EmailConfirmed = true,
            Salary = user.Salary,
            PaymentDuration = user.PaymentDuration
        };
        
        var passwordHasher = new PasswordHasher<User>();
        var newUserHashed = passwordHasher.HashPassword(newUser, user.Password);
        newUser.PasswordHash = newUserHashed;

        try {
            await _userManager.CreateAsync(newUser);
            await _userManager.AddToRoleAsync(newUser, "User");
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "User");
        }
        catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }
    }

    // GET /User/Edit/{id}
    public async Task<IActionResult> Edit(string? id) {
        if (id == null) {
            return BadRequest();
        }

        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (user == null) {
            return NotFound();
        }
        
        return View(user);
    }
    
    // POST /User/Edit
    [HttpPost]
    public async Task<IActionResult> Edit([Bind("Id,Salary,PaymentDuration")] User user) {
        if (user.Id == null) {
            return BadRequest();
        }

        var updatedUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == user.Id);

        if (updatedUser == null) {
            return NotFound();
        }
        
        updatedUser.Salary = user.Salary;
        updatedUser.PaymentDuration = user.PaymentDuration;

        try {
            _context.Users.Update(updatedUser);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "User");
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }
    }

    // GET /User/Delete/{id}
    public async Task<IActionResult> Delete(string? id) {
        if (id == null) {
            return BadRequest();
        }
        
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (user == null) {
            return NotFound();
        }
        
        return View(user);
    }

    // POST /User/Delete
    [HttpPost]
    [ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(string? id) {
        if (id == null) {
            return BadRequest();
        }

        var user = await _userManager
            .Users
            .Include(u => u.Notes)
            .Include(u => u.Notifications)
            .Include(u => u.ProjectDevelopers)
            .Include(u => u.Notes)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null) {
            return NotFound();
        }

        try {
            await _userManager.DeleteAsync(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "User");
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }
    }
}
