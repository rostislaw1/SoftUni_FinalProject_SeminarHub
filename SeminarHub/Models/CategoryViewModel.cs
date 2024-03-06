using static SeminarHub.Data.DataConstants;
using System.ComponentModel.DataAnnotations;

namespace SeminarHub.Models
{
    public class CategoryViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(
            ErrorMessage = RequireErrorMessage)]
        [StringLength(
            CategoryNameMaximumLength,
            MinimumLength = CategoryNameMinimumLength,
            ErrorMessage = StringLengthErrorMessage)]
        public string Name { get; set; } = string.Empty;
    }
}
