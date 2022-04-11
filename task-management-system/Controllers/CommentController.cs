using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using task_management_system.Data;
using task_management_system.Models;

namespace task_management_system.Controllers;

public class CommentController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public CommentController(ApplicationDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Create(int? id)
    {
        if (id == null)
        {
            return BadRequest();
        }

        var note = await _context.Notes.FirstOrDefaultAsync(n => n.Id == id);
        ViewBag.NoteId = note.Id;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create([Bind("Body,NoteId")] Comment comment)
    {
        var note = await _context.Notes.FirstOrDefaultAsync(n => n.Id == comment.NoteId);

        if (note == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);

        Comment newComment = new(comment.Body, note, user);

        try
        {
            _context.Comments.Add(newComment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

}