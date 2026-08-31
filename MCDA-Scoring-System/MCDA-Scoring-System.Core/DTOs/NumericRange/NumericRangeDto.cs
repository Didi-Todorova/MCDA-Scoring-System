namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.NumericRange
{
    public record NumericRangeDto
    (
        int Id,
        int NumericalCriterionRuleId,
        decimal MinValue,
        decimal MaxValue
    );
}
