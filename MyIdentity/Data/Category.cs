using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyIdentity.Data
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Category Name is not empty")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "{0} has length from {2} to {1}")]
        public string? Name { get; set; }

        [DataType(DataType.Text)]
        public string? Description { get; set; }

        public int? ParentId { get; set; }

        [ForeignKey("ParentId")]
        public Category? Parent { get; set; }

        public ICollection<Category>? Children { get; set;}
    }
}
