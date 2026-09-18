using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.AlternativeValue;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Enums;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Services
{
    public class AlternativeValueService : IAlternativeValueService
    {
        private readonly IRepository _repo;

        public AlternativeValueService(IRepository repo)
        {
            _repo = repo;
        }

        public async Task<AlternativeValueDto> CreateAlternativeValueAsync(
            CreateAlternativeValueDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var alternative = await _repo.GetByIdAsync<Alternative>(
                dto.AlternativeId);

            if (alternative == null)
                throw new KeyNotFoundException(
                    $"Alternative with ID {dto.AlternativeId} not found.");

            var criterion = await _repo.GetByIdAsync<Criterion>(
                dto.CriterionId);

            if (criterion == null)
                throw new KeyNotFoundException(
                    $"Criterion with ID {dto.CriterionId} not found.");

            ValidateDecisionRelationship(
                alternative,
                criterion);

            await ValidateValueAsync(
                criterion,
                dto.NumericValue,
                dto.CriterionOptionId);

            var existingValue = await _repo.AllReadonly<AlternativeValue>()
                .AnyAsync(av =>
                    av.AlternativeId == dto.AlternativeId &&
                    av.CriterionId == dto.CriterionId);

            if (existingValue)
                throw new ArgumentException(
                    "An alternative value already exists for this alternative and criterion.");

            var alternativeValue = new AlternativeValue
            {
                AlternativeId = dto.AlternativeId,
                CriterionId = dto.CriterionId,
                NumericValue = dto.NumericValue,
                CriterionOptionId = dto.CriterionOptionId
            };

            await _repo.AddAsync(alternativeValue);
            await _repo.SaveChangesAsync();

            return new AlternativeValueDto(
                alternativeValue.Id,
                alternativeValue.AlternativeId,
                alternativeValue.CriterionId,
                alternativeValue.NumericValue,
                alternativeValue.CriterionOptionId
            );
        }

        public async Task<bool> DeleteAlternativeValueAsync(int id)
        {
            var alternativeValue =
                await _repo.GetByIdAsync<AlternativeValue>(id);

            if (alternativeValue == null)
                return false;

            _repo.Delete(alternativeValue);
            await _repo.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<AlternativeValueDto>>
            GetAllAlternativeValuesAsync()
        {
            return await _repo.AllReadonly<AlternativeValue>()
                .Select(av => new AlternativeValueDto(
                    av.Id,
                    av.AlternativeId,
                    av.CriterionId,
                    av.NumericValue,
                    av.CriterionOptionId
                ))
                .ToListAsync();
        }

        public async Task<AlternativeValueDto?>
            GetAlternativeValueByIdAsync(int id)
        {
            return await _repo.AllReadonly<AlternativeValue>()
                .Where(av => av.Id == id)
                .Select(av => new AlternativeValueDto(
                    av.Id,
                    av.AlternativeId,
                    av.CriterionId,
                    av.NumericValue,
                    av.CriterionOptionId
                ))
                .FirstOrDefaultAsync();
        }

        public async Task PatchAlternativeValueAsync(
            int id,
            PatchAlternativeValueDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var alternativeValue =
                await _repo.GetByIdAsync<AlternativeValue>(id);

            if (alternativeValue == null)
                throw new KeyNotFoundException(
                    $"AlternativeValue with ID {id} not found.");

            var alternativeId = dto.AlternativeId
                ?? alternativeValue.AlternativeId;

            var criterionId = dto.CriterionId
                ?? alternativeValue.CriterionId;

            var numericValue = dto.NumericValue
                ?? alternativeValue.NumericValue;

            var criterionOptionId = dto.CriterionOptionId
                ?? alternativeValue.CriterionOptionId;

            var alternative = await _repo.GetByIdAsync<Alternative>(
                alternativeId);

            if (alternative == null)
                throw new KeyNotFoundException(
                    $"Alternative with ID {alternativeId} not found.");

            var criterion = await _repo.GetByIdAsync<Criterion>(
                criterionId);

            if (criterion == null)
                throw new KeyNotFoundException(
                    $"Criterion with ID {criterionId} not found.");

            ValidateDecisionRelationship(
                alternative,
                criterion);

            await ValidateValueAsync(
                criterion,
                numericValue,
                criterionOptionId);

            var duplicateExists = await _repo.AllReadonly<AlternativeValue>()
                .AnyAsync(av =>
                    av.Id != id &&
                    av.AlternativeId == alternativeId &&
                    av.CriterionId == criterionId);

            if (duplicateExists)
                throw new ArgumentException(
                    "An alternative value already exists for this alternative and criterion.");

            alternativeValue.AlternativeId = alternativeId;
            alternativeValue.CriterionId = criterionId;
            alternativeValue.NumericValue = numericValue;
            alternativeValue.CriterionOptionId = criterionOptionId;

            await _repo.SaveChangesAsync();
        }

        public async Task UpdateAlternativeValueAsync(
            int id,
            UpdateAlternativeValueDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var alternativeValue =
                await _repo.GetByIdAsync<AlternativeValue>(id);

            if (alternativeValue == null)
                throw new KeyNotFoundException(
                    $"AlternativeValue with ID {id} not found.");

            var alternative = await _repo.GetByIdAsync<Alternative>(
                dto.AlternativeId);

            if (alternative == null)
                throw new KeyNotFoundException(
                    $"Alternative with ID {dto.AlternativeId} not found.");

            var criterion = await _repo.GetByIdAsync<Criterion>(
                dto.CriterionId);

            if (criterion == null)
                throw new KeyNotFoundException(
                    $"Criterion with ID {dto.CriterionId} not found.");

            ValidateDecisionRelationship(
                alternative,
                criterion);

            await ValidateValueAsync(
                criterion,
                dto.NumericValue,
                dto.CriterionOptionId);

            var duplicateExists = await _repo.AllReadonly<AlternativeValue>()
                .AnyAsync(av =>
                    av.Id != id &&
                    av.AlternativeId == dto.AlternativeId &&
                    av.CriterionId == dto.CriterionId);

            if (duplicateExists)
                throw new ArgumentException(
                    "An alternative value already exists for this alternative and criterion.");

            alternativeValue.AlternativeId = dto.AlternativeId;
            alternativeValue.CriterionId = dto.CriterionId;
            alternativeValue.NumericValue = dto.NumericValue;
            alternativeValue.CriterionOptionId = dto.CriterionOptionId;

            await _repo.SaveChangesAsync();
        }

        private static void ValidateDecisionRelationship(
            Alternative alternative,
            Criterion criterion)
        {
            if (alternative.DecisionId != criterion.DecisionId)
            {
                throw new ArgumentException(
                    "The alternative and criterion must belong to the same decision.");
            }
        }

        private async Task ValidateValueAsync(
    Criterion criterion,
    decimal? numericValue,
    int? criterionOptionId)
        {
            if (criterion.CriterionType == CriterionType.Numerical)
            {
                if (!numericValue.HasValue)
                {
                    throw new ArgumentException(
                        "Numeric value is required for a numerical criterion.");
                }

                if (criterionOptionId.HasValue)
                {
                    throw new ArgumentException(
                        "A criterion option cannot be provided for a numerical criterion.");
                }

                var rule = await _repo
                    .AllReadonly<CriterionNumericalRule>()
                    .FirstOrDefaultAsync(r =>
                        r.CriterionId == criterion.Id);

                if (rule == null)
                {
                    throw new ArgumentException(
                        "The numerical criterion is not configured.");
                }

                if (numericValue.Value < rule.MinValue ||
                    numericValue.Value > rule.MaxValue)
                {
                    throw new ArgumentException(
                        $"Numeric value must be between {rule.MinValue} and {rule.MaxValue}.");
                }

                if (rule.NumericType == NumericType.Interval)
                {
                    var intervalExists = await _repo
                        .AllReadonly<IntervalRange>()
                        .AnyAsync(r =>
                            r.CriterionNumericalRuleId == rule.Id &&
                            numericValue.Value >= r.MinValue &&
                            numericValue.Value <= r.MaxValue);

                    if (!intervalExists)
                    {
                        throw new ArgumentException(
                            "Numeric value does not belong to any configured interval range.");
                    }
                }

                return;
            }

            if (criterion.CriterionType == CriterionType.Categorical)
            {
                if (!criterionOptionId.HasValue)
                {
                    throw new ArgumentException(
                        "Criterion option is required for a categorical criterion.");
                }

                if (numericValue.HasValue)
                {
                    throw new ArgumentException(
                        "Numeric value cannot be provided for a categorical criterion.");
                }

                var option = await _repo.GetByIdAsync<CriterionOption>(
                    criterionOptionId.Value);

                if (option == null)
                {
                    throw new KeyNotFoundException(
                        $"Criterion option with ID {criterionOptionId.Value} not found.");
                }

                if (option.CriterionId != criterion.Id)
                {
                    throw new ArgumentException(
                        "The selected criterion option does not belong to this criterion.");
                }

                return;
            }

            throw new ArgumentException(
                "Invalid criterion type.");
        }
    }
}

