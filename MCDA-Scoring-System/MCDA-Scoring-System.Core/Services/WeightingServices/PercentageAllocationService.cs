using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.Criterion.Weights;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Enums;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Services.WeightingServices
{
    public class PercentageAllocationService : IPercentageAllocationService
    {
        private readonly IRepository _repo;

        public PercentageAllocationService(IRepository repo)
        {
            _repo = repo;
        }

        public async Task SetWeightsAsync(
            int decisionId,
            List<CriterionPercentageAllocationDto> weights)
        {
            var decision = await _repo
                .All<Decision>()
                .Include(d => d.Criteria)
                .FirstOrDefaultAsync(d => d.Id == decisionId);

            if (decision == null)
            {
                throw new KeyNotFoundException(
                    $"Decision with ID {decisionId} not found.");
            }

            if (decision.WeightingMethod !=
                WeightingMethod.PercentageAllocation)
            {
                throw new ArgumentException(
                    "This decision does not use Percentage Allocation.");
            }

            if (weights == null || weights.Count == 0)
            {
                throw new ArgumentException(
                    "No criterion weights provided.");
            }

            if (weights.Any(w =>
                w.Weight < 0 ||
                w.Weight > 100))
            {
                throw new ArgumentException(
                    "Each weight must be between 0% and 100%.");
            }

            if (weights.Count != decision.Criteria.Count)
            {
                throw new ArgumentException(
                    "A weight must be provided for every criterion.");
            }

            if (weights
                .Select(w => w.CriterionId)
                .Distinct()
                .Count() != weights.Count)
            {
                throw new ArgumentException(
                    "Duplicate criterion IDs were provided.");
            }

            var criterionIds = decision.Criteria
                .Select(c => c.Id)
                .ToHashSet();

            if (weights.Any(w =>
                !criterionIds.Contains(w.CriterionId)))
            {
                throw new ArgumentException(
                    "One or more criteria do not belong to this decision.");
            }

            var totalWeight = weights.Sum(w => w.Weight);

            if (totalWeight != 100m)
            {
                throw new ArgumentException(
                    $"Weights must sum to 100%. " +
                    $"Current total: {totalWeight}%.");
            }

            var weightsByCriterion =
                weights.ToDictionary(w => w.CriterionId);

            foreach (var criterion in decision.Criteria)
            {
                var input =
                    weightsByCriterion[criterion.Id];

                criterion.Weight =
                    input.Weight / 100m;
            }

            await _repo.SaveChangesAsync();
        }
    }
}