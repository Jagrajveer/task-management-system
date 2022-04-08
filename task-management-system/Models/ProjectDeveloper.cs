using System.ComponentModel.DataAnnotations.Schema;

namespace task_management_system.Models
{
    public class ProjectDeveloper
    {
        public int Id { get; set; }

        public virtual Project? Project { get; set; }
        public int? ProjectId { get; set; }

        public virtual User? Developer { get; set; }
        public string? DeveloperId { get; set; }
        public DateTime CreatedAt { get; set; }

        public ProjectDeveloper() { }

        public ProjectDeveloper(Project project, User developer)
        {
            Project = project;
            ProjectId = project.Id;
            Developer = developer;
            DeveloperId = developer.Id;
        }
    }
}
