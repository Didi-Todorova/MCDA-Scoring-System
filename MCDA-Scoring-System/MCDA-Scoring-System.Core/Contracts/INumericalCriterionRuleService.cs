using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.NumericalCriterionRule;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts
{
    public interface INumericalCriterionRuleService
    {
        public Task<NumericalCriterionRuleDto> CreateNumericalCriterionRuleAsync(CreateNumericalCriterionRuleDto dto);
        public Task<NumericalCriterionRuleDto?> GetNumericalCriterionRuleByIdAsync(int id);
        public Task<IEnumerable<NumericalCriterionRuleDto>> GetAllNumericalCriterionRulesAsync();
        public Task UpdateNumericalCriterionRuleAsync(int id, UpdateNumericalCriterionRuleDto dto);
        public Task PatchNumericalCriterionRuleAsync(int id, PatchNumericalCriterionRuleDto dto);
        public Task<bool> DeleteNumericalCriterionRuleAsync(int id);
    }
}
