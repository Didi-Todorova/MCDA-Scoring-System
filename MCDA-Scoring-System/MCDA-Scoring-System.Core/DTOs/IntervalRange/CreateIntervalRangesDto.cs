namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.IntervalRange
{
    public record CreateIntervalRangesDto(
        int CriterionNumericalRuleId,
        List<IntervalRangeInputDto> Ranges
    );
}
