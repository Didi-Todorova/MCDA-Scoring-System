using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.NumericalCriterionRule;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts
{
    public interface INumericalCriterionRuleService
    {
        public Task<CriterionNumericalRuleDto> CreateNumericalCriterionRuleAsync(CreateCriterionNumericalRuleDto dto);
        public Task<CriterionNumericalRuleDto?> GetNumericalCriterionRuleByIdAsync(int id);
        public Task<IEnumerable<CriterionNumericalRuleDto>> GetAllNumericalCriterionRulesAsync();
        public Task UpdateNumericalCriterionRuleAsync(int id, UpdateCriterionNumericalRuleDto dto);
        public Task PatchNumericalCriterionRuleAsync(int id, PatchCriterionNumericalDto dto);
        public Task<bool> DeleteNumericalCriterionRuleAsync(int id);
    }
}
