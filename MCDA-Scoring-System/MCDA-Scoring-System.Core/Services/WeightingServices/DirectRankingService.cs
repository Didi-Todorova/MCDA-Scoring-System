using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.Criterion.Weights;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Enums;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Services.WeightingServices
{
    public class DirectRankingService : IDirectRankingService
    {
        private readonly IRepository _repo;

        public DirectRankingService(IRepository repo)
        {
            _repo = repo;
        }

        public async Task SetWeightsAsync(
            int decisionId,
            List<CriterionDirectRankingDto> rankings)
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

            if (decision.WeightingMethod != WeightingMethod.DirectRanking)
            {
                throw new ArgumentException(
                    "This decision does not use Direct Ranking.");
            }

            if (rankings == null || rankings.Count == 0)
            {
                throw new ArgumentException(
                    "No criterion rankings provided.");
            }

            if (rankings.Count != decision.Criteria.Count)
            {
                throw new ArgumentException(
                    "A rank must be provided for every criterion.");
            }

            if (rankings
                .Select(r => r.CriterionId)
                .Distinct()
                .Count() != rankings.Count)
            {
                throw new ArgumentException(
                    "A criterion cannot be ranked more than once.");
            }

            var criterionIds = decision.Criteria
                .Select(c => c.Id)
                .ToHashSet();

            if (rankings.Any(r =>
                !criterionIds.Contains(r.CriterionId)))
            {
                throw new ArgumentException(
                    "One or more criteria do not belong to this decision.");
            }

            if (rankings
                .Select(r => r.Rank)
                .Distinct()
                .Count() != rankings.Count)
            {
                throw new ArgumentException(
                    "Each rank can be assigned to only one criterion.");
            }

            var numberOfCriteria = decision.Criteria.Count;

            if (rankings.Any(r =>
                r.Rank < 1 ||
                r.Rank > numberOfCriteria))
            {
                throw new ArgumentException(
                    $"Ranks must be between 1 and {numberOfCriteria}.");
            }

            var totalScore =
                numberOfCriteria *
                (numberOfCriteria + 1) /
                2m;

            var rankingsByCriterion =
                rankings.ToDictionary(r => r.CriterionId);

            foreach (var criterion in decision.Criteria)
            {
                var ranking =
                    rankingsByCriterion[criterion.Id];

                var score =
                    numberOfCriteria -
                    ranking.Rank +
                    1;

                criterion.Weight =
                    score / totalScore;
            }

            await _repo.SaveChangesAsync();
        }
    }
}