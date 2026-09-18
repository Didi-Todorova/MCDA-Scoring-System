using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.Alternative;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.Services.RatingRuleService;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Services
{
    public class ScoringService : IScoringService
    {
        private readonly IRepository _repo;
        private readonly IRatingService _ratingService;

        public ScoringService(
            IRepository repo,
            IRatingService ratingService)
        {
            _repo = repo;
            _ratingService = ratingService;
        }

        public async Task<List<ScoredAlternativeDto>>
            CalculateScoreAsync(int decisionId)
        {
            var decision = await _repo
                .AllReadonly<Decision>()
                .Where(d => d.Id == decisionId)
                .Include(d => d.Criteria)
                .Include(d => d.Alternatives)
                    .ThenInclude(a => a.AlternativeValues)
                .FirstOrDefaultAsync();

            if (decision == null)
            {
                throw new KeyNotFoundException(
                    $"Decision with ID {decisionId} not found.");
            }

            ValidateDecision(decision);

            var scoredAlternatives =
                new List<ScoredAlternativeDto>();

            foreach (var alternative in decision.Alternatives)
            {
                ValidateAlternative(
                    alternative,
                    decision.Criteria);

                decimal score = 0m;

                foreach (var criterion in decision.Criteria)
                {
                    var rating = await _ratingService
                        .CalculateRatingAsync(
                            alternative.Id,
                            criterion.Id);

                    score +=
                        rating *
                        criterion.Weight!.Value;
                }

                scoredAlternatives.Add(
                    new ScoredAlternativeDto(
                        alternative.Id,
                        alternative.DecisionId,
                        alternative.Name,
                        Math.Round(score, 2)
                    )
                );
            }

            return scoredAlternatives
                .OrderByDescending(a => a.Score)
                .ToList();
        }

        private static void ValidateDecision(
            Decision decision)
        {
            if (decision.Criteria == null ||
                decision.Criteria.Count == 0)
            {
                throw new InvalidOperationException(
                    "The decision must have at least one criterion before scores can be calculated.");
            }

            if (decision.Alternatives == null ||
                decision.Alternatives.Count == 0)
            {
                throw new InvalidOperationException(
                    "The decision must have at least one alternative before scores can be calculated.");
            }

            foreach (var criterion in decision.Criteria)
            {
                if (!criterion.Weight.HasValue)
                {
                    throw new InvalidOperationException(
                        $"Criterion '{criterion.Name}' has no weight.");
                }

                if (criterion.Weight.Value < 0m ||
                    criterion.Weight.Value > 1m)
                {
                    throw new InvalidOperationException(
                        $"Criterion '{criterion.Name}' has an invalid weight.");
                }
            }

            var totalWeight = decision.Criteria
                .Sum(c => c.Weight!.Value);

            if (Math.Abs(totalWeight - 1m) > 0.000001m)
            {
                throw new InvalidOperationException(
                    $"Criterion weights must sum to 1. Current total: {totalWeight}.");
            }
        }

        private static void ValidateAlternative(
            Alternative alternative,
            ICollection<Criterion> criteria)
        {
            var criterionIds = criteria
                .Select(c => c.Id)
                .ToHashSet();

            var values = alternative.AlternativeValues
                .ToList();

            var duplicateCriterionIds = values
                .GroupBy(av => av.CriterionId)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateCriterionIds.Count > 0)
            {
                throw new InvalidOperationException(
                    $"Alternative '{alternative.Name}' contains duplicate values for one or more criteria.");
            }

            if (values.Count != criteria.Count)
            {
                throw new InvalidOperationException(
                    $"Alternative '{alternative.Name}' must have exactly one value for every criterion.");
            }

            var missingCriterionIds = criterionIds
                .Except(values.Select(av => av.CriterionId))
                .ToList();

            if (missingCriterionIds.Count > 0)
            {
                throw new InvalidOperationException(
                    $"Alternative '{alternative.Name}' is missing one or more criterion values.");
            }

            var invalidCriterionIds = values
                .Select(av => av.CriterionId)
                .Except(criterionIds)
                .ToList();

            if (invalidCriterionIds.Count > 0)
            {
                throw new InvalidOperationException(
                    $"Alternative '{alternative.Name}' contains a value for a criterion that does not belong to this decision.");
            }
        }
    }
}