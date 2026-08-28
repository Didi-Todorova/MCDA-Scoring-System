using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.Decision;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts
{
    public interface IDecisionService
    {
        public Task<DecisionDto> CreateDecisionAsync(CreateDecisionDto createDecisionDto);
        public Task<DecisionDto?> GetDecisionByIdAsync(int id);
        public Task<IEnumerable<DecisionDto>> GetAllDecisionsAsync();
        public Task UpdateDecisionAsync(int id, UpdateDecisionDto updateDecisionDto);
        public Task DeleteDecisionAsync(int id);
    }
}
