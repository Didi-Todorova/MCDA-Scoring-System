namespace MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities
{
    public class NumericRange
    {
        public int Id { get; set; }
        public int NumericalCriterionRuleId { get; set; }
        public NumericalCriterionRule NumericalCriterionRule { get; set; } = null!;
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
    }
}
