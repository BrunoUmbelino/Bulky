using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bulky.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
<<<<<<< HEAD
        [MaxLength(200)]
=======
        [StringLength(200)]
>>>>>>> e3132a7 (.)
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "tinyint")]
        public byte DisplayOrder { get; set; }
    }
}
