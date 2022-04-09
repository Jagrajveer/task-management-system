using Microsoft.AspNetCore.Identity;

namespace task_management_system.Models
{
    public class User : IdentityUser
    {
        public decimal Salary { get; set; }
        public PaymentDurationType? PaymentDuration { get; set; }
        public DateTime CreatedAt { get; init; }

        public virtual ICollection<ProjectDeveloper> ProjectDevelopers { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }

        public virtual ICollection<Note> Notes { get; set; }

        public enum PaymentDurationType {
            Hourly = 1,
            Daily = 2,
            Weekly = 3,
            BiWeekly = 4,
            Monthly = 5,
            Contract = 6
        };
        
        public User(){}
        
        public User(decimal salary, PaymentDurationType paymentDuration) {
            Salary = salary;
            PaymentDuration = paymentDuration;
            CreatedAt = DateTime.Now;
            Notes = new HashSet<Note>();
            ProjectDevelopers = new HashSet<ProjectDeveloper>();
            Tasks = new HashSet<Task>();
            Notifications = new HashSet<Notification>();
        }
    }
}
