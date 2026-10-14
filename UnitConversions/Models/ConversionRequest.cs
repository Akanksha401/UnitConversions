using System.ComponentModel.DataAnnotations;
namespace UnitConversions.Models
{
    public class ConversionRequest
    {
        [Required]
        public string Category { get; set; } = string.Empty;

        [Required]
        public string FromUnit { get; set; } = string.Empty; 

        [Required]
        public string ToUnit { get; set; } = string.Empty; 

        [Required]
        public double Value { get; set; }
    }
}
