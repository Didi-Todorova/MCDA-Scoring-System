using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.Alternative;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts
{
    public interface IScoringService
    {
        public Task<List<ScoredAlternativeDto>> CalculateScoreAsync(int decisionId);
    }
}
