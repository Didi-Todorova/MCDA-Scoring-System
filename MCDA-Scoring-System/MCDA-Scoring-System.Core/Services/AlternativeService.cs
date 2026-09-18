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

            var normalizedName = dto.Name.Trim();

            var duplicateExists = await _repo
                .AllReadonly<Alternative>()
                .AnyAsync(a =>
                    a.DecisionId == dto.DecisionId &&
                    a.Name.ToLower() == normalizedName.ToLower());

            if (duplicateExists)
            {
                throw new ArgumentException(
                    "An alternative with this name already exists in this decision.");
            }

            var alternative = new Alternative
            {
                Name = normalizedName,
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

                var normalizedName = dto.Name.Trim();

                var duplicateExists = await _repo
                    .AllReadonly<Alternative>()
                    .AnyAsync(a =>
                        a.Id != id &&
                        a.DecisionId == alternative.DecisionId &&
                        a.Name.ToLower() == normalizedName.ToLower());

                if (duplicateExists)
                {
                    throw new ArgumentException(
                        "An alternative with this name already exists in this decision.");
                }

                alternative.Name = normalizedName;
            }

            if (dto.DecisionId.HasValue &&
                dto.DecisionId.Value != alternative.DecisionId)
            {
                var decision = await _repo.GetByIdAsync<Decision>(
                    dto.DecisionId.Value);

                if (decision == null)
                    throw new KeyNotFoundException(
                        $"Decision with ID {dto.DecisionId.Value} not found.");

                var hasValues = await _repo
                    .AllReadonly<AlternativeValue>()
                    .AnyAsync(av => av.AlternativeId == id);

                if (hasValues)
                {
                    throw new ArgumentException(
                        "An alternative with existing values cannot be moved to another decision.");
                }

                var duplicateExists = await _repo
                    .AllReadonly<Alternative>()
                    .AnyAsync(a =>
                        a.Id != id &&
                        a.DecisionId == dto.DecisionId.Value &&
                        a.Name.ToLower() == alternative.Name.ToLower());

                if (duplicateExists)
                {
                    throw new ArgumentException(
                        "An alternative with this name already exists in the target decision.");
                }

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

            var normalizedName = dto.Name.Trim();

            var duplicateExists = await _repo
                .AllReadonly<Alternative>()
                .AnyAsync(a =>
                    a.Id != id &&
                    a.DecisionId == dto.DecisionId &&
                    a.Name.ToLower() == normalizedName.ToLower());

            if (duplicateExists)
            {
                throw new ArgumentException(
                    "An alternative with this name already exists in this decision.");
            }

            if (dto.DecisionId != alternative.DecisionId)
            {
                var hasValues = await _repo
                    .AllReadonly<AlternativeValue>()
                    .AnyAsync(av => av.AlternativeId == id);

                if (hasValues)
                {
                    throw new ArgumentException(
                        "An alternative with existing values cannot be moved to another decision.");
                }
            }

            alternative.Name = normalizedName;
            alternative.DecisionId = dto.DecisionId;

            await _repo.SaveChangesAsync();
        }

        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(
                    "Alternative name is required.");
            }

            if (name.Trim().Length > 200)
            {
                throw new ArgumentException(
                    "Alternative name cannot exceed 200 characters.");
            }
        }
    }
}