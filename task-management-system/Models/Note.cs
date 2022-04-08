namespace task_management_system.Models
{
    public class Note
    {
        public int Id { get; set; }
        public string Body { get; set; }
        public virtual User? User { get; set; }
        public string? UserId { get; set; }
        public virtual Task? Task { get; set; }
        public int? TaskId { get; set; }
        public DateTime CreatedAt { get; set; }

        public Note() { }

        public Note(string body, User user, Task task)
        {
            Body = body;
            User = user;
            UserId = user.Id;
            Task = task;
            TaskId = task.Id;
            CreatedAt = DateTime.Now;
        }
    }
}
