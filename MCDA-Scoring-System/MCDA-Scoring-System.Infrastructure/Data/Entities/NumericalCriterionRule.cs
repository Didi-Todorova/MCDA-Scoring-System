using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities
{
    public class NumericalCriterionRule
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CriterionId { get; set; }
        public Criterion Criterion { get; set; } = null!;

        [Required]
        public NumericType NumericType { get; set; }
        public decimal? TargetValue { get; set; } //for TargetValue
        public Direction? Direction { get; set; } // for scope
        public ICollection<NumericRange> NumericRanges { get; set; } = new List<NumericRange>();
    }
}
