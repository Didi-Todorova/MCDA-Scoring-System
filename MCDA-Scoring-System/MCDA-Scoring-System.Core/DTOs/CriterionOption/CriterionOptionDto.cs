namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.CriterionOption
{
    public record CriterionOptionDto
    (
        int Id,
        int CriterionId,
        string Value,
        int Rank
    );
}
