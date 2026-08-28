using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.AlternativeValue;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts
{
    public interface IAlternativeValueService
    {
        public Task<AlternativeValueDto> CreateAlternativeValueAsync(CreateAlternativeValueDto createAlternativeValueDto);
        public Task<AlternativeValueDto?> GetAlternativeValueByIdAsync(int id);
        public Task<IEnumerable<AlternativeValueDto>> GetAllAlternativeValuesAsync();
        public Task UpdateAlternativeValueAsync(int id, UpdateAlternativeValueDto updateAlternativeValueDto);
        public Task DeleteAlternativeValueAsync(int id);
    }
}
