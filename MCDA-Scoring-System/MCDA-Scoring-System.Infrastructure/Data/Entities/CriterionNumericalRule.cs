using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities
{
    public class CriterionNumericalRule
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CriterionId { get; set; }
        public Criterion Criterion { get; set; } = null!;

        [Required]
        public NumericType NumericType { get; set; }

        //Acceptrance scope for the criterion, if the value is outside of this scope, it will be considered as not accepted
        [Required]
        public decimal MinValue { get; set; } 
        [Required]
        public decimal MaxValue { get; set; }

        public decimal? TargetValue { get; set; } //for TargetValue
        public Direction? Direction { get; set; } // for scope
        public ICollection<IntervalRange>? IntervalRanges { get; set; } = new List<IntervalRange>();
    }
}
