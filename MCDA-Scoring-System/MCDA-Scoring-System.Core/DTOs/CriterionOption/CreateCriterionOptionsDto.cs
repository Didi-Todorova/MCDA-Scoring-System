namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.CriterionOption
{
    public record CreateCriterionOptionsDto(
    int CriterionId,
    List<string> Options
    );
}
