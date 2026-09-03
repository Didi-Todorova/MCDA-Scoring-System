using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.Criterion;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts
{
    public interface ICriterionService
    {
        public Task<CriterionDto> CreateCriterionAsync(CreateCriterionDto createCriterionDto);
        public Task<CriterionDto?> GetCriterionByIdAsync(int id);
        public Task<IEnumerable<CriterionDto>> GetAllCriteriaAsync();
        public Task UpdateCriterionAsync(int id, UpdateCriterionDto updateCriterionDto);
        public Task PatchCriterionAsync(int id, PatchCriterionDto patchCriterionDto);
        public Task<bool> DeleteCriterionAsync(int id);
    }
}
