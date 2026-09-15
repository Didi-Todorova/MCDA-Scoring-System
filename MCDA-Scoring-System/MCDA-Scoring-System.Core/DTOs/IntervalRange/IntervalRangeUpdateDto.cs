namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.IntervalRange
{
    public record IntervalRangeUpdateDto(
        int Id,
        decimal MinValue,
        decimal MaxValue
    );
}
