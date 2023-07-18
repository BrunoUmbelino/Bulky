using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BulkyWeb.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "iiiiiiiiii burrão")]
        [MaxLength(30)]
        [DisplayName("Category Name")]
        public string Name { get; set; } = string.Empty;

        [Range(1, 100, ErrorMessage = "tu é leso é?")]
        [DisplayName("Display Order")]
        public int DisplayOrder { get; set; }
    }
}
