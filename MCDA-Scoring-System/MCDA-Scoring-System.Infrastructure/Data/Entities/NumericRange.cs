using System.ComponentModel.DataAnnotations;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities
{
    public class NumericRange
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int NumericalCriterionRuleId { get; set; }
        public NumericalCriterionRule NumericalCriterionRule { get; set; } = null!;
        [Required]
        public decimal MinValue { get; set; }
        [Required]
        public decimal MaxValue { get; set; }
    }
}
