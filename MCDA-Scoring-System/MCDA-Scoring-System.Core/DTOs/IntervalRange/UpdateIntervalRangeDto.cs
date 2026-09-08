namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.IntervalRange
{
    public record UpdateIntervalRangeDto
    (
        int CriterionNumericalRuleId,
        decimal MinValue,
        decimal MaxValue
    );
}
