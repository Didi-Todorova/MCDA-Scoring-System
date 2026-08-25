using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Enums;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities
{
    public class NumericalCriterionRule
    {
        public int Id { get; set; }
        public int CriterionId { get; set; }
        public Criterion Criterion { get; set; } = null!;
        public NumericType NumericType { get; set; }
        public decimal TargetValue { get; set; }
        public Direction Direction { get; set; }
        public ICollection<NumericRange> NumericRanges { get; set; } = new List<NumericRange>();
    }
}
