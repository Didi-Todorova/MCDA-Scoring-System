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

            var scoredAlternatives =
                new List<ScoredAlternativeDto>();

            foreach (var alternative in decision.Alternatives)
            {
                decimal score = 0m;

                foreach (var criterion in decision.Criteria)
                {
                    if (criterion.Weight == null)
                    {
                        throw new ArgumentException(
                            $"Criterion '{criterion.Name}' has no weight.");
                    }

                    var alternativeValue = alternative.AlternativeValues
                        .FirstOrDefault(
                            av => av.CriterionId == criterion.Id);

                    if (alternativeValue == null)
                    {
                        throw new ArgumentException(
                            $"Alternative '{alternative.Name}' has no value " +
                            $"for criterion '{criterion.Name}'.");
                    }

                    var rating = await _ratingService
                        .CalculateRatingAsync(
                            alternative.Id,
                            criterion.Id);

                    score += rating * criterion.Weight.Value;
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
    }
}