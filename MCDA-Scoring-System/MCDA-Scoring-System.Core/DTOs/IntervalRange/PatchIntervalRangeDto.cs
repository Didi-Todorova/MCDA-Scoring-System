namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.IntervalRange
{
    public record PatchIntervalRangeDto
    (
        int? CriterionNumericalRuleId,
        decimal? MinValue,
        decimal? MaxValue
    );
}
