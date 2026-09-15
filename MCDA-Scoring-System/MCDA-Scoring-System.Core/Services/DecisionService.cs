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

            var decision = new Decision
            {
                Name = dto.Name.Trim(),
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
                decision.Name = dto.Name.Trim();
            }

            if (dto.WeightingMethod.HasValue)
            {
                ValidateWeightingMethod(dto.WeightingMethod.Value);

                decision.WeightingMethod =
                    (WeightingMethod)dto.WeightingMethod.Value;
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

            decision.Name = dto.Name.Trim();
            decision.WeightingMethod = dto.WeightingMethod;

            await _repo.SaveChangesAsync();
        }

        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(
                    "Decision name is required.");
        }

        private static void ValidateWeightingMethod(
            WeightingMethod weightingMethod)
        {
            if (!Enum.IsDefined(
                    typeof(WeightingMethod),
                    weightingMethod))
            {
                throw new ArgumentException(
                    "Invalid weighting method.");
            }
        }    
    }
}
