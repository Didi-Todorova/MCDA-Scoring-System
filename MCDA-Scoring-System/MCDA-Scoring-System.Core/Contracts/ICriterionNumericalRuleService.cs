using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.CriterionNumericalRule;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts
{
    public interface ICriterionNumericalRuleService
    {
        public Task<CriterionNumericalRuleDto> CreateCriterionNumericalRuleAsync(CreateCriterionNumericalRuleDto dto);
        public Task<CriterionNumericalRuleDto?> GetCriterionNumericalRuleByIdAsync(int id);
        public Task<IEnumerable<CriterionNumericalRuleDto>> GetAllCriterionNumericalRulesAsync();
        public Task UpdateCriterionNumericalRuleAsync(int id, UpdateCriterionNumericalRuleDto dto);
        public Task PatchCriterionNumericalRuleAsync(int id, PatchCriterionNumericalDto dto);
        public Task<bool> DeleteCriterionNumericalRuleAsync(int id);
    }
}
