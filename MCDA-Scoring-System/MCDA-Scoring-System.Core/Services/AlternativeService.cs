using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.Alternative;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Services
{
    public class AlternativeService : IAlternativeService
    {
        private readonly IRepository _repo;

        public AlternativeService(IRepository repo)
        {
            _repo = repo;
        }

        public async Task<AlternativeDto> CreateAlternativeAsync(
            CreateAlternativeDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            ValidateName(dto.Name);

            var decision = await _repo.GetByIdAsync<Decision>(dto.DecisionId);

            if (decision == null)
                throw new KeyNotFoundException(
                    $"Decision with ID {dto.DecisionId} not found.");

            var alternative = new Alternative
            {
                Name = dto.Name.Trim(),
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

            _repo.Delete(alternative);
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

        public async Task PatchAlternativeAsync(
            int id,
            PatchAlternativeDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var alternative = await _repo.GetByIdAsync<Alternative>(id);

            if (alternative == null)
                throw new KeyNotFoundException(
                    $"Alternative with ID {id} not found.");

            if (dto.Name != null)
            {
                ValidateName(dto.Name);
                alternative.Name = dto.Name.Trim();
            }

            if (dto.DecisionId.HasValue)
            {
                var decision = await _repo.GetByIdAsync<Decision>(
                    dto.DecisionId.Value);

                if (decision == null)
                    throw new KeyNotFoundException(
                        $"Decision with ID {dto.DecisionId.Value} not found.");

                alternative.DecisionId = dto.DecisionId.Value;
            }

            await _repo.SaveChangesAsync();
        }

        public async Task UpdateAlternativeAsync(
            int id,
            UpdateAlternativeDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var alternative = await _repo.GetByIdAsync<Alternative>(id);

            if (alternative == null)
                throw new KeyNotFoundException(
                    $"Alternative with ID {id} not found.");

            ValidateName(dto.Name);

            var decision = await _repo.GetByIdAsync<Decision>(dto.DecisionId);

            if (decision == null)
                throw new KeyNotFoundException(
                    $"Decision with ID {dto.DecisionId} not found.");

            alternative.Name = dto.Name.Trim();
            alternative.DecisionId = dto.DecisionId;

            await _repo.SaveChangesAsync();
        }

        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(
                    "Alternative name is required.");
        }
    }
}
