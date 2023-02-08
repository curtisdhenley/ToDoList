using System.ComponentModel.DataAnnotations;

namespace ToDoList.Models
{
    public class Accessory
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Accessory Name")]
        public string? Name { get; set; }

        [Required]
        public string? AppUserId { get; set; }

        // Navigation Properties
        public virtual ICollection<ToDoItem> ToDoItems { get; set; } = new HashSet<ToDoItem>();
    }
}
