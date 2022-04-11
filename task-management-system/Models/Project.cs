namespace task_management_system.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Body { get; set; }

        public DateTime Deadline { get; set; }

        public decimal Budget { get; set; }

        public decimal MoneySpent { get; set; }  
        public decimal CompletionPercentage { get; set; }
        public DateTime CreatedAt { get; init; }

        public virtual User? ProjectManager { get; set; }
        public string? ProjectManagerId { get; set; }

        public virtual ICollection<Task> Tasks { get; set; }

        public virtual ICollection<ProjectDeveloper> ProjectDevelopers { get; set; }

        public Project()
        {
            Tasks = new HashSet<Task>();
            ProjectDevelopers = new HashSet<ProjectDeveloper>();
        }

        public Project(string name, string body ,DateTime deadline, decimal budget, User projectManager)
        {
            Name = name;
            Body = body;
            MoneySpent = 0;
            Deadline = deadline;
            Budget = budget;
            CreatedAt = DateTime.Now;
     
            ProjectManager = projectManager;
            ProjectManagerId = projectManager.Id;
            Tasks = new HashSet<Task>();
            ProjectDevelopers = new HashSet<ProjectDeveloper>();
        }
    }
}
