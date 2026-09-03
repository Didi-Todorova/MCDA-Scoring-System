using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.Alternative;
using System.Diagnostics.Contracts;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts
{
    public interface IAlternativeService
    {
        public Task<AlternativeDto> CreateAlternativeAsync(CreateAlternativeDto dto);
        public Task<AlternativeDto?> GetAlternativeByIdAsync(int id);
        public Task<IEnumerable<AlternativeDto>> GetAllAlternativesAsync();
        public Task UpdateAlternativeAsync(int id, UpdateAlternativeDto dto);
        public Task PatchAlternativeAsync(int id, PatchAlternativeDto dto);
        public Task<bool> DeleteAlternativeAsync(int id);
    }
}
