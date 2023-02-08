using System.ComponentModel.DataAnnotations;

namespace ToDoList.Models
{
    public class ToDoItem
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Task Name")]
        public string? Name { get; set; }

        [Required]
        public string? AppUserId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Created { get; set; }

        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }

        [Required]
        public bool Completed { get; set; }

        // Navigation Properties
        public virtual AppUser? AppUser { get; set; }
        public virtual ICollection<Accessory> Accessories { get; set; } = new HashSet<Accessory>();
    }
}
