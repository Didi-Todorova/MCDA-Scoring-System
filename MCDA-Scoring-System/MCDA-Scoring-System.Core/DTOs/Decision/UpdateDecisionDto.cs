using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Enums;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.Decision
{
    public record UpdateDecisionDto
    (
        string Name,        
        WeightingMethod WeightingMethod
    );
}
