namespace task_management_system.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Body { get; set; }
        public virtual Note Note { get; set; }
        public int? NoteId { get; set; }
        public virtual User User { get; set; }
        public string? UserId { get; set; }
        public DateTime CreatedAt { get; set; }

        public Comment() { }

        public Comment(string body, Note note, User user)
        {
            Body = body;
            Note = note;
            NoteId = note.Id;
            User = user;
            UserId = user.Id;
            CreatedAt = DateTime.Now;
        }
    }
}
