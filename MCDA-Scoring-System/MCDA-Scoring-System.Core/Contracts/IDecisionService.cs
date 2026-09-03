using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.Decision;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts
{
    public interface IDecisionService
    {
        public Task<DecisionDto> CreateDecisionAsync(CreateDecisionDto dto);
        public Task<DecisionDto?> GetDecisionByIdAsync(int id);
        public Task<IEnumerable<DecisionDto>> GetAllDecisionsAsync();
        public Task UpdateDecisionAsync(int id, UpdateDecisionDto dto);
        public Task PartialUpdateDecisionAsync(int id, PatchDecisionDto dto);
        public Task<bool> DeleteDecisionAsync(int id);
    }
}
