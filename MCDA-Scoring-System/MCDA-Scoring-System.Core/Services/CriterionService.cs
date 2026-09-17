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

            var decision = await _repo.GetByIdAsync<Decision>(dto.DecisionId);

            if (decision == null)
                throw new KeyNotFoundException(
                    $"Decision with ID {dto.DecisionId} not found.");

            var criterion = new Criterion
            {
                DecisionId = dto.DecisionId,
                Name = dto.Name.Trim(),
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

            criterion.DecisionId = dto.DecisionId;
            criterion.Name = dto.Name.Trim();
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
                throw new ArgumentException(
                    "Criterion name is required.");
        }

        private static void ValidateCriterionType(
            CriterionType criterionType)
        {
            if (!Enum.IsDefined(
                    typeof(CriterionType),
                    criterionType))
            {
                throw new ArgumentException(
                    "Invalid criterion type.");
            }
        }

        private static void ValidateWeight(decimal? weight)
        {
            if (!weight.HasValue)
                return;

            if (weight.Value < 0 || weight.Value > 1)
            {
                throw new ArgumentException(
                    "Criterion weight must be between 0 and 1.");
            }
        }
    }
}
