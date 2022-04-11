namespace task_management_system.Models
{
    public class Task
    {
        public int Id { get; init; }
        public string Title { get; set; }
        public string Content { get; set; }
        public decimal CompletionPercentage { get; set; }
     
        public virtual Project? Project { get; set; }
        public int? ProjectId { get; set; }

        public virtual User? Developer { get; set; }
        public string? DeveloperId { get; set; }
        public bool IsCompleted { get; set; }
        public PriorityType Priority { get; set; }
        public DateTime Deadline { get; set; }

        public DateTime CreatedAt { get; init; }
        public virtual ICollection<Note> Notes { get; set; }




        public enum PriorityType
        {
            Low = 1,
            High = 2
        };

        public Task() { }
        public Task(string title, string content, Project? project,DateTime deadline, User developer, PriorityType priotity )
        {
            Title = title;
            Content = content;
            CompletionPercentage = 0;
            Project = project;
            Priority = priotity;
            Deadline = deadline;
            ProjectId = project.Id;
            Developer = developer;
            DeveloperId = developer.Id;
            CreatedAt = DateTime.Now;
        }
    }
}
