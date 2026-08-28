using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.CriterionOption;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts
{
    public interface ICriterionOptionService
    {
        public Task<CriterionOptionDto> CreateCriterionOptionAsync(CreateCriterionOptionDto createCriterionOptionDto);
        public Task<CriterionOptionDto?> GetCriterionOptionByIdAsync(int id);
        public Task<IEnumerable<CriterionOptionDto>> GetAllCriterionOptionsAsync();
        public Task UpdateCriterionOptionAsync(int id, UpdateCriterionOptionDto updateCriterionOptionDto);
        public Task DeleteCriterionOptionAsync(int id);
    }
}
