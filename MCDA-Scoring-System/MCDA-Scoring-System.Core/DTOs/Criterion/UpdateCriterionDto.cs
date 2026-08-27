using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Enums;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.Criterion
{
    public record UpdateCriterionDto
    (
        int DecisionId,
        string Name,
        CriterionType CriterionType,
        decimal? Weight
    );
}
