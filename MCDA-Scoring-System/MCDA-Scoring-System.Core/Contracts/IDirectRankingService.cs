using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.Criterion.Weights;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts
{
    public interface IDirectRankingService
    {
        Task SetWeightsAsync(
        int decisionId,
        List<CriterionDirectRankingDto> rankings);
    }
}
