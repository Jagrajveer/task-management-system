using Microsoft.AspNetCore.Identity;

namespace task_management_system.Models
{
    public class User : IdentityUser
    {

        public DateTime CreatedAt { get; init; }

        public virtual ICollection<ProjectDeveloper> ProjectDevelopers { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }

        public virtual ICollection<Note> Notes { get; set; }


        
        public User()
        {
            CreatedAt = DateTime.Now;
            Notes = new HashSet<Note>();
            ProjectDevelopers = new HashSet<ProjectDeveloper>();
            Tasks = new HashSet<Task>();
            Notifications = new HashSet<Notification>();
        }
    }

}
