using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.Decision;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Enums;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Services
{
    public class DecisionService : IDecisionService
    {
        private readonly IRepository _repo;

        public DecisionService(IRepository repo)
        {
            _repo = repo;
        }

        public async Task<DecisionDto> CreateDecisionAsync(
            CreateDecisionDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            ValidateName(dto.Name);
            ValidateWeightingMethod(dto.WeightingMethod);

            var normalizedName = dto.Name.Trim();

            var duplicateExists = await _repo
                .AllReadonly<Decision>()
                .AnyAsync(d =>
                    d.Name.ToLower() == normalizedName.ToLower());

            if (duplicateExists)
            {
                throw new ArgumentException(
                    "A decision with this name already exists.");
            }

            var decision = new Decision
            {
                Name = normalizedName,
                WeightingMethod = dto.WeightingMethod,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(decision);
            await _repo.SaveChangesAsync();

            return new DecisionDto(
                decision.Id,
                decision.Name,
                decision.WeightingMethod
            );
        }

        public async Task<bool> DeleteDecisionAsync(int id)
        {
            var decision = await _repo.GetByIdAsync<Decision>(id);

            if (decision == null)
                return false;

            var alternativeValues = await _repo
                .All<AlternativeValue>()
                .Where(av =>
                    av.Alternative.DecisionId == id)
                .ToListAsync();

            foreach (var alternativeValue in alternativeValues)
            {
                _repo.Delete(alternativeValue);
            }

            _repo.Delete(decision);

            await _repo.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<DecisionDto>>
            GetAllDecisionsAsync()
        {
            return await _repo.AllReadonly<Decision>()
                .Select(d => new DecisionDto(
                    d.Id,
                    d.Name,
                    d.WeightingMethod
                ))
                .ToListAsync();
        }

        public async Task<DecisionDto?>
            GetDecisionByIdAsync(int id)
        {
            return await _repo.AllReadonly<Decision>()
                .Where(d => d.Id == id)
                .Select(d => new DecisionDto(
                    d.Id,
                    d.Name,
                    d.WeightingMethod
                ))
                .FirstOrDefaultAsync();
        }

        public async Task PartialUpdateDecisionAsync(
    int id,
    PatchDecisionDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var decision = await _repo.GetByIdAsync<Decision>(id);

            if (decision == null)
                throw new KeyNotFoundException(
                    $"Decision with ID {id} not found.");

            if (dto.Name != null)
            {
                ValidateName(dto.Name);

                var normalizedName = dto.Name.Trim();

                var duplicateExists = await _repo
                    .AllReadonly<Decision>()
                    .AnyAsync(d =>
                        d.Id != id &&
                        d.Name.ToLower() == normalizedName.ToLower());

                if (duplicateExists)
                {
                    throw new ArgumentException(
                        "A decision with this name already exists.");
                }

                decision.Name = normalizedName;
            }

            if (dto.WeightingMethod.HasValue)
            {
                ValidateWeightingMethod(dto.WeightingMethod.Value);

                if (dto.WeightingMethod.Value != decision.WeightingMethod)
                {
                    var criteria = await _repo
                        .All<Criterion>()
                        .Where(c => c.DecisionId == id)
                        .ToListAsync();

                    foreach (var criterion in criteria)
                    {
                        criterion.Weight = null;
                    }

                    decision.WeightingMethod = dto.WeightingMethod.Value;
                }
            }

            await _repo.SaveChangesAsync();
        }

        public async Task UpdateDecisionAsync(
    int id,
    UpdateDecisionDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var decision = await _repo.GetByIdAsync<Decision>(id);

            if (decision == null)
                throw new KeyNotFoundException(
                    $"Decision with ID {id} not found.");

            ValidateName(dto.Name);
            ValidateWeightingMethod(dto.WeightingMethod);

            var normalizedName = dto.Name.Trim();

            var duplicateExists = await _repo
                .AllReadonly<Decision>()
                .AnyAsync(d =>
                    d.Id != id &&
                    d.Name.ToLower() == normalizedName.ToLower());

            if (duplicateExists)
            {
                throw new ArgumentException(
                    "A decision with this name already exists.");
            }

            if (dto.WeightingMethod != decision.WeightingMethod)
            {
                var criteria = await _repo
                    .All<Criterion>()
                    .Where(c => c.DecisionId == id)
                    .ToListAsync();

                foreach (var criterion in criteria)
                {
                    criterion.Weight = null;
                }
            }

            decision.Name = normalizedName;
            decision.WeightingMethod = dto.WeightingMethod;

            await _repo.SaveChangesAsync();
        }

        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(
                    "Decision name is required.");
            }

            if (name.Trim().Length > 200)
            {
                throw new ArgumentException(
                    "Decision name cannot exceed 200 characters.");
            }
        }

        private static void ValidateWeightingMethod(
            WeightingMethod weightingMethod)
        {
            if (weightingMethod != WeightingMethod.PercentageAllocation &&
                weightingMethod != WeightingMethod.DirectRanking)
            {
                throw new ArgumentException(
                    "The selected weighting method is not currently supported.");
            }
        }

    }

}
