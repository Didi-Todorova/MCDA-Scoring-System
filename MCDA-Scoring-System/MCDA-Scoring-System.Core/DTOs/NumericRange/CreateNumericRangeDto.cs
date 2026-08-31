namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.NumericRange
{
    public record CreateNumericRangeDto
    (
        int NumericalCriterionRuleId,
        decimal MinValue,
        decimal MaxValue
    );
}
