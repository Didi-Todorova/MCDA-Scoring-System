using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.NumericalCriterionRule;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts
{
    public interface INumericalCriterionRuleService
    {
        public Task<NumericalCriterionRuleDto> CreateNumericalCriterionRuleAsync(CreateNumericalCriterionRuleDto createNumericalCriterionRuleDto);
        public Task<NumericalCriterionRuleDto?> GetNumericalCriterionRuleByIdAsync(int id);
        public Task<IEnumerable<NumericalCriterionRuleDto>> GetAllNumericalCriterionRulesAsync();
        public Task UpdateNumericalCriterionRuleAsync(int id, UpdateNumericalCriterionRuleDto updateNumericalCriterionRuleDto);
        public Task DeleteNumericalCriterionRuleAsync(int id);
    }
}
