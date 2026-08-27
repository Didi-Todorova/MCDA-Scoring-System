namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.AlternativeValue
{
    public record UpdateAlternativeValueDto
      (
        int AlternativeId,
        int CriterionId,
        decimal? NumericValue,
        int? CriterionOptionId
    );
}
