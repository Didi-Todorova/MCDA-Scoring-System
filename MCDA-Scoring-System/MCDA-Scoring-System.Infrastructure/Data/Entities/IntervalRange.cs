using System.ComponentModel.DataAnnotations;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities
{
    public class IntervalRange
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CriterionNumericalRuleId { get; set; }
        public CriterionNumericalRule CriterionNumericalRule { get; set; } = null!;
        [Required]
        public decimal MinValue { get; set; }
        [Required]
        public decimal MaxValue { get; set; }

        [Required]
        public int Rank { get; set; }
    }
}
