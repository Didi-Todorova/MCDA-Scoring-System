using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.CriterionOption;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts
{
    public interface ICriterionOptionService
    {
        public Task<CriterionOptionDto> CreateCriterionOptionAsync(CreateCriterionOptionDto dto);
        public Task<CriterionOptionDto?> GetCriterionOptionByIdAsync(int id);
        public Task<IEnumerable<CriterionOptionDto>> GetAllCriterionOptionsAsync();
        public Task UpdateCriterionOptionAsync(int id, UpdateCriterionOptionDto dto);
        public Task PatchCriterionOptionAsync(int id, PatchCriterionOptionDto dto);
        public Task<bool> DeleteCriterionOptionAsync(int id);
    }
}
