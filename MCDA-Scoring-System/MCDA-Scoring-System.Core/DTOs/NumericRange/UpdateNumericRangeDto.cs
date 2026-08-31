namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.NumericRange
{
    public record UpdateNumericRangeDto
    (
        int NumericalCriterionRuleId,
        decimal MinValue,
        decimal MaxValue
    );
}
