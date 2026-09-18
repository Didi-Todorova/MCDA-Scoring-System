using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.Criterion;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Enums;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Services
{
    public class CriterionService : ICriterionService
    {
        private readonly IRepository _repo;

        public CriterionService(IRepository repo)
        {
            _repo = repo;
        }

        public async Task<CriterionDto> CreateCriterionAsync(
            CreateCriterionDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            ValidateName(dto.Name);
            ValidateCriterionType(dto.CriterionType);
            ValidateWeight(dto.Weight);

            var normalizedName = dto.Name.Trim();

            var duplicateExists = await _repo
                .AllReadonly<Criterion>()
                .AnyAsync(c =>
                    c.DecisionId == dto.DecisionId &&
                    c.Name.ToLower() == normalizedName.ToLower());

            if (duplicateExists)
            {
                throw new ArgumentException(
                    "A criterion with this name already exists in this decision.");
            }

            var decision = await _repo.GetByIdAsync<Decision>(dto.DecisionId);

            if (decision == null)
                throw new KeyNotFoundException(
                    $"Decision with ID {dto.DecisionId} not found.");

            var criterion = new Criterion
            {
                DecisionId = dto.DecisionId,
                Name = normalizedName,
                CriterionType = dto.CriterionType,
                Unit = dto.Unit?.Trim(),
                Weight = dto.Weight
            };

            await _repo.AddAsync(criterion);
            await _repo.SaveChangesAsync();

            return new CriterionDto(
                criterion.Id,
                criterion.DecisionId,
                criterion.Name,
                criterion.CriterionType,
                criterion.Unit,
                criterion.Weight
            );
        }

        public async Task<bool> DeleteCriterionAsync(int id)
        {
            var criterion = await _repo.GetByIdAsync<Criterion>(id);

            if (criterion == null)
                return false;

            var alternativeValues = await _repo
                .All<AlternativeValue>()
                .Where(av => av.CriterionId == id)
                .ToListAsync();

            foreach (var alternativeValue in alternativeValues)
            {
                _repo.Delete(alternativeValue);
            }

            _repo.Delete(criterion);

            await _repo.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<CriterionDto>>
            GetAllCriteriaAsync()
        {
            return await _repo.AllReadonly<Criterion>()
                .Select(c => new CriterionDto(
                    c.Id,
                    c.DecisionId,
                    c.Name,
                    c.CriterionType,
                    c.Unit,
                    c.Weight
                ))
                .ToListAsync();
        }

        public async Task<CriterionDto?>
            GetCriterionByIdAsync(int id)
        {
            return await _repo.AllReadonly<Criterion>()
                .Where(c => c.Id == id)
                .Select(c => new CriterionDto(
                    c.Id,
                    c.DecisionId,
                    c.Name,
                    c.CriterionType,
                    c.Unit,
                    c.Weight
                ))
                .FirstOrDefaultAsync();
        }

        public async Task PatchCriterionAsync(
            int id,
            PatchCriterionDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var criterion = await _repo.GetByIdAsync<Criterion>(id);

            if (criterion == null)
                throw new KeyNotFoundException(
                    $"Criterion with ID {id} not found.");

            var decisionId =
                dto.DecisionId ?? criterion.DecisionId;

            var name =
                dto.Name ?? criterion.Name;

            var criterionType =
                dto.CriterionType ?? criterion.CriterionType;

            var unit =
                dto.Unit ?? criterion.Unit;

            var weight =
                dto.Weight ?? criterion.Weight;

            ValidateName(name);
            ValidateCriterionType(criterionType);
            ValidateWeight(weight);

            if (decisionId != criterion.DecisionId)
            {
                var decision =
                    await _repo.GetByIdAsync<Decision>(decisionId);

                if (decision == null)
                    throw new KeyNotFoundException(
                        $"Decision with ID {decisionId} not found.");

                var hasAlternativeValues = await _repo
                    .AllReadonly<AlternativeValue>()
                    .AnyAsync(av => av.CriterionId == id);

                var hasOptions = await _repo
                    .AllReadonly<CriterionOption>()
                    .AnyAsync(co => co.CriterionId == id);

                var hasNumericalRule = await _repo
                    .AllReadonly<CriterionNumericalRule>()
                    .AnyAsync(nr => nr.CriterionId == id);

                if (hasAlternativeValues ||
                    hasOptions ||
                    hasNumericalRule)
                {
                    throw new ArgumentException(
                        "A configured criterion cannot be moved to another decision.");
                }

                criterion.DecisionId = decisionId;
            }

            if (criterionType != criterion.CriterionType)
            {
                var hasOptions = await _repo
                    .AllReadonly<CriterionOption>()
                    .AnyAsync(co => co.CriterionId == criterion.Id);

                var hasNumericalRule = await _repo
                    .AllReadonly<CriterionNumericalRule>()
                    .AnyAsync(nr => nr.CriterionId == criterion.Id);

                if (hasOptions || hasNumericalRule)
                {
                    throw new ArgumentException(
                        "Criterion type cannot be changed while the criterion has existing configuration.");
                }

                criterion.CriterionType = criterionType;
            }

            criterion.Name = name.Trim();
            criterion.Unit = string.IsNullOrWhiteSpace(unit)
                ? null
                : unit.Trim();
            criterion.Weight = weight;

            await _repo.SaveChangesAsync();
        }

        public async Task UpdateCriterionAsync(
    int id,
    UpdateCriterionDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var criterion = await _repo.GetByIdAsync<Criterion>(id);

            if (criterion == null)
                throw new KeyNotFoundException(
                    $"Criterion with ID {id} not found.");

            ValidateName(dto.Name);
            ValidateCriterionType(dto.CriterionType);
            ValidateWeight(dto.Weight);

            var decision = await _repo.GetByIdAsync<Decision>(
                dto.DecisionId);

            if (decision == null)
                throw new KeyNotFoundException(
                    $"Decision with ID {dto.DecisionId} not found.");

            var normalizedName = dto.Name.Trim();

            var duplicateExists = await _repo
                .AllReadonly<Criterion>()
                .AnyAsync(c =>
                    c.Id != id &&
                    c.DecisionId == dto.DecisionId &&
                    c.Name.ToLower() == normalizedName.ToLower());

            if (duplicateExists)
            {
                throw new ArgumentException(
                    "A criterion with this name already exists in this decision.");
            }

            if (dto.CriterionType != criterion.CriterionType)
            {
                var hasOptions = await _repo
                    .AllReadonly<CriterionOption>()
                    .AnyAsync(co => co.CriterionId == criterion.Id);

                var hasNumericalRule = await _repo
                    .AllReadonly<CriterionNumericalRule>()
                    .AnyAsync(nr => nr.CriterionId == criterion.Id);

                if (hasOptions || hasNumericalRule)
                {
                    throw new ArgumentException(
                        "Criterion type cannot be changed while the criterion has existing configuration.");
                }
            }

            if (criterion.DecisionId != dto.DecisionId)
            {
                var hasAlternativeValues = await _repo
                    .AllReadonly<AlternativeValue>()
                    .AnyAsync(av => av.CriterionId == id);

                var hasOptions = await _repo
                    .AllReadonly<CriterionOption>()
                    .AnyAsync(co => co.CriterionId == id);

                var hasNumericalRule = await _repo
                    .AllReadonly<CriterionNumericalRule>()
                    .AnyAsync(nr => nr.CriterionId == id);

                if (hasAlternativeValues ||
                    hasOptions ||
                    hasNumericalRule)
                {
                    throw new ArgumentException(
                        "A configured criterion cannot be moved to another decision.");
                }
            }

            criterion.DecisionId = dto.DecisionId;
            criterion.Name = normalizedName;
            criterion.CriterionType = dto.CriterionType;
            criterion.Unit = string.IsNullOrWhiteSpace(dto.Unit)
                ? null
                : dto.Unit.Trim();
            criterion.Weight = dto.Weight;

            await _repo.SaveChangesAsync();
        }

        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(
                    "Criterion name is required.");
            }

            if (name.Trim().Length > 200)
            {
                throw new ArgumentException(
                    "Criterion name cannot exceed 200 characters.");
            }
        }

        private static void ValidateCriterionType(
    CriterionType criterionType)
        {
            if (!Enum.IsDefined(criterionType))
            {
                throw new ArgumentException(
                    "Invalid criterion type.");
            }
        }

        private static void ValidateWeight(decimal? weight)
        {
            if (weight.HasValue &&
                (weight.Value < 0 || weight.Value > 1))
            {
                throw new ArgumentException(
                    "Criterion weight must be between 0 and 1.");
            }
        }
    }
}
