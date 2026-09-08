namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.IntervalRange
{
    public record CreateIntervalRangeDto
    (
        int CriterionNumericalRuleId,
        decimal MinValue,
        decimal MaxValue
    );
}
