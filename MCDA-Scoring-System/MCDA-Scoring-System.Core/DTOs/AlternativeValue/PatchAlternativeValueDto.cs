namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.AlternativeValue
{
    public record PatchAlternativeValueDto
    (
        int? AlternativeId,
        int? CriterionId,
        decimal? NumericValue,
        int? CriterionOptionId
    );
}
