namespace task_management_system.Models
{
    public class Notification
    {
        public int Id { get; init; }
        public string Title { get; set; }
        public string Body { get; set; }
        public User? User { get; set; }
        public string? UserId { get; set; }
        public DateTime CreatedAt { get; init; }

        public Notification() { }

        public Notification(string title, string body, User user)
        {
            Title = title;
            Body = body;
            User = user;
            UserId = user.Id;
            CreatedAt = DateTime.Now;
        }
    }                                     
}
