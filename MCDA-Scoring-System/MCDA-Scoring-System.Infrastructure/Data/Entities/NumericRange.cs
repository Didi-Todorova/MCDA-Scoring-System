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
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
    }
}
