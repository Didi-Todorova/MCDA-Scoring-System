using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.Alternative;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts
{
    public interface IAlternativeService
    {
        public Task<AlternativeDto> CreateAlternativeAsync(CreateAlternativeDto createAlternativeDto);
        public Task<AlternativeDto?> GetAlternativeByIdAsync(int id);
        public Task<IEnumerable<AlternativeDto>> GetAllAlternativesAsync();
        public Task UpdateAlternativeAsync(int id, UpdateAlternativeDto updateAlternativeDto);
        public Task DeleteAlternativeAsync(int id);
    }
}
