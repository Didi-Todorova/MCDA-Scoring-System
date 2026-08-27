using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Enums;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.NumericalCriterionRule
{
    public record CreateNumericalCriterionRule
    (
        int CriterionId,
        NumericType NumericType,
        decimal? TargetValue,
        Direction? Direction
    );
}
