using Newtonsoft.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using static SeminarHub.Data.DataConstants;

namespace SeminarHub.Models
{
    public class SeminarFormViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(
            ErrorMessage = RequireErrorMessage)]
        [StringLength(
            SeminarTopicMaximumLength,
            MinimumLength = SeminarTopicMinimumLength,
            ErrorMessage = StringLengthErrorMessage)]
        public string Topic { get; set; } = string.Empty;

        [Required(
            ErrorMessage = RequireErrorMessage)]
        [StringLength(
            SeminarLecturerMaximumLength,
            MinimumLength = SeminarLecturerMinimumLength,
            ErrorMessage = StringLengthErrorMessage)]
        public string Lecturer { get; set; } = string.Empty;

        [Required(
            ErrorMessage = RequireErrorMessage)]
        [StringLength(
            SeminarDetailsMaximumLength,
            MinimumLength = SeminarDetailsMinimumLength,
            ErrorMessage = StringLengthErrorMessage)]
        public string Details { get; set; } = string.Empty;

        [Required(
            ErrorMessage = RequireErrorMessage)]
        public string DateAndTime { get; set; } = string.Empty;

        [Range(
            SeminarDurationMinimumValue,
            SeminarDurationMaximumValue,
            ErrorMessage = SeminarDurationErrorMessage)]
        public int Duration { get; set; }

        [Required(
            ErrorMessage = RequireErrorMessage)]
        public int CategoryId { get; set; }

        [Required(
            ErrorMessage = RequireErrorMessage)]
        public IEnumerable<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>(); 
    }
}