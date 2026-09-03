using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.Alternative;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Services
{
    public class AlternativeService : IAlternativeService
    {

        private readonly IRepository _repo;

        public AlternativeService(IRepository repo)
        {
            this._repo = repo;
        }
        public async Task<AlternativeDto> CreateAlternativeAsync(CreateAlternativeDto dto)
        {
            var alternative = new Alternative
            {
                Name = dto.Name,
                DecisionId = dto.DecisionId
            };

            await _repo.AddAsync(alternative);
            await _repo.SaveChangesAsync();

            return new AlternativeDto(
                alternative.Id,
                alternative.Name,
                alternative.DecisionId
                );
        }

        public async Task<bool> DeleteAlternativeAsync(int id)
        {
            var alternative = await _repo.GetByIdAsync<Alternative>(id);

            if (alternative == null)
                return false;

            await _repo.DeleteAsync<Alternative>(id);
            await _repo.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<AlternativeDto>> GetAllAlternativesAsync()
        {
            return await _repo
                .AllReadonly<Alternative>()
                .Select(a => new AlternativeDto(
                    a.Id,
                    a.Name,
                    a.DecisionId
                ))
                .ToListAsync();
        }

        public async Task<AlternativeDto?> GetAlternativeByIdAsync(int id)
        {
            return await _repo
                .AllReadonly<Alternative>()
                .Where(a => a.Id == id)
                .Select(a => new AlternativeDto(
                    a.Id,
                    a.Name,
                    a.DecisionId
                ))
                .FirstOrDefaultAsync();
        }
        public async Task PatchAlternativeAsync(int id, PatchAlternativeDto dto)
        {
            var alternative = await _repo.GetByIdAsync<Alternative>(id)
                ?? throw new ArgumentException($"Alternative with ID {id} not found.");

            if (dto.Name != null) alternative.Name = dto.Name;
            if (dto.DecisionId.HasValue) alternative.DecisionId = dto.DecisionId.Value;

            await _repo.SaveChangesAsync();
        }

        public async Task UpdateAlternativeAsync(int id, UpdateAlternativeDto dto)
        {
            var alternative = await _repo.GetByIdAsync<Alternative>(id) ?? throw new ArgumentException($"Alternative with ID {id} not found");

            alternative.Name = dto.Name;
            alternative.DecisionId = dto.DecisionId;

            await _repo.SaveChangesAsync();
        }

    }
}
