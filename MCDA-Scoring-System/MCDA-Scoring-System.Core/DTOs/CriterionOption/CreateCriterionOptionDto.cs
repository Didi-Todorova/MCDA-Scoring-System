namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.CriterionOption
{
    public record CreateCriterionOptionDto
    (
        int CriterionId,
        string Value,
        int Rank
    );
}
