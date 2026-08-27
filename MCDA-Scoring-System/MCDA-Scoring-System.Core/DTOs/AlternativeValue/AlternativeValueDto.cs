namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.AlternativeValue
{
    public record AlternativeValueDto
    (
        int Id,
        int AlternativeId,
        int CriterionId,
        decimal? NumericValue,
        int? CriterionOptionId
    );
}
