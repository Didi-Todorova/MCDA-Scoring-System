namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.Alternative
{
    public record ScoredAlternativeDto(
        int Id,
        int DecisionId,
        string Name,
        decimal Score
    );
}
