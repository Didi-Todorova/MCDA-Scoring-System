namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.NumericRange
{
    public record IntervalRangeDto
    (
        int Id,
        int CriterionNumericalRuleId,
        decimal MinValue,
        decimal MaxValue
    );
}
