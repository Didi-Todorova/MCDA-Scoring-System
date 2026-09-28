using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.CriterionOption;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Enums;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Services
{
    public class CriterionOptionService : ICriterionOptionService
    {
        private readonly IRepository _repo;

        public CriterionOptionService(IRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<CriterionOptionDto>>
    CreateCriterionOptionAsync(CreateCriterionOptionsDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var criterion = await _repo.GetByIdAsync<Criterion>(dto.CriterionId);

            if (criterion == null)
                throw new KeyNotFoundException(
                    $"Criterion with ID {dto.CriterionId} not found.");

            ValidateCriterionType(criterion);

            if (dto.Options == null || dto.Options.Count < 2)
                throw new ArgumentException(
                    "At least two criterion options are required.");

            var normalizedOptions = dto.Options
                .Select(option => option.Trim())
                .ToList();

            foreach (var option in normalizedOptions)
            {
                ValidateOptionValue(option);
            }

            if (normalizedOptions
                .GroupBy(
                    value => value,
                    StringComparer.OrdinalIgnoreCase)
                .Any(group => group.Count() > 1))
            {
                throw new ArgumentException(
                    "Criterion options must be unique.");
            }

            var existingOptions = await _repo
                .AllReadonly<CriterionOption>()
                .AnyAsync(co =>
                    co.CriterionId == dto.CriterionId);

            if (existingOptions)
                throw new ArgumentException(
                    "This criterion already has options.");

            var options = normalizedOptions
                .Select((value, index) => new CriterionOption
                {
                    CriterionId = dto.CriterionId,
                    Value = value,
                    Rank = index + 1
                })
                .ToList();

            await _repo.AddRangeAsync(options);
            await _repo.SaveChangesAsync();

            return options
                .OrderBy(o => o.Rank)
                .Select(o => new CriterionOptionDto(
                    o.Id,
                    o.CriterionId,
                    o.Value,
                    o.Rank
                ));
        }

        public async Task<bool> DeleteCriterionOptionAsync(int id)
        {
            var option = await _repo.GetByIdAsync<CriterionOption>(id);

            if (option == null)
                return false;

            var isUsed = await _repo
                .AllReadonly<AlternativeValue>()
                .AnyAsync(av =>
                    av.CriterionOptionId == option.Id);

            if (isUsed)
            {
                throw new ArgumentException(
                    "This criterion option is currently used by an alternative and cannot be deleted.");
            }

            var remainingOptions = await _repo.All<CriterionOption>()
                .Where(co =>
                    co.CriterionId == option.CriterionId &&
                    co.Id != id)
                .OrderBy(co => co.Rank)
                .ToListAsync();

            _repo.Delete(option);

            for (int i = 0; i < remainingOptions.Count; i++)
            {
                remainingOptions[i].Rank = i + 1;
            }

            await _repo.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<CriterionOptionDto>>
            GetAllCriterionOptionsAsync()
        {
            return await _repo.AllReadonly<CriterionOption>()
                .OrderBy(co => co.CriterionId)
                .ThenBy(co => co.Rank)
                .Select(co => new CriterionOptionDto(
                    co.Id,
                    co.CriterionId,
                    co.Value,
                    co.Rank
                ))
                .ToListAsync();
        }

        public async Task<CriterionOptionDto?>
            GetCriterionOptionByIdAsync(int id)
        {
            return await _repo.AllReadonly<CriterionOption>()
                .Where(co => co.Id == id)
                .Select(co => new CriterionOptionDto(
                    co.Id,
                    co.CriterionId,
                    co.Value,
                    co.Rank
                ))
                .FirstOrDefaultAsync();
        }

        public async Task PatchCriterionOptionAsync(
    int id,
    PatchCriterionOptionDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var criterionOption =
                await _repo.GetByIdAsync<CriterionOption>(id);

            if (criterionOption == null)
                throw new KeyNotFoundException(
                    $"Criterion Option with ID {id} not found.");

            if (dto.CriterionId.HasValue &&
                dto.CriterionId.Value != criterionOption.CriterionId)
            {
                throw new ArgumentException(
                    "A criterion option cannot be moved to another criterion.");
            }

            var value = dto.Value ?? criterionOption.Value;

            var criterion =
                await _repo.GetByIdAsync<Criterion>(
                    criterionOption.CriterionId);

            if (criterion == null)
                throw new KeyNotFoundException(
                    $"Criterion with ID {criterionOption.CriterionId} not found.");

            ValidateCriterionType(criterion);
            ValidateOptionValue(value);

            var valueChanged = !string.Equals(
                criterionOption.Value.Trim(),
                value.Trim(),
                StringComparison.OrdinalIgnoreCase);

            if (valueChanged)
            {
                var isUsed = await _repo
                    .AllReadonly<AlternativeValue>()
                    .AnyAsync(av =>
                        av.CriterionOptionId == criterionOption.Id);

                if (isUsed)
                {
                    throw new ArgumentException(
                        "This criterion option is already used by an alternative and cannot be renamed.");
                }

                var duplicateExists = await _repo
                    .AllReadonly<CriterionOption>()
                    .AnyAsync(co =>
                        co.Id != id &&
                        co.CriterionId == criterionOption.CriterionId &&
                        co.Value.ToLower() == value.Trim().ToLower());

                if (duplicateExists)
                {
                    throw new ArgumentException(
                        "A criterion option with this value already exists.");
                }
            }

            criterionOption.Value = value.Trim();

            await _repo.SaveChangesAsync();
        }

        public async Task UpdateCriterionOptionAsync(
            int criterionId,
            UpdateCriterionOptionsDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var criterion = await _repo.GetByIdAsync<Criterion>(criterionId);

            if (criterion == null)
            {
                throw new KeyNotFoundException(
                    $"Criterion with ID {criterionId} not found.");
            }

            ValidateCriterionType(criterion);

            if (dto.Options == null || dto.Options.Count < 2)
            {
                throw new ArgumentException(
                    "At least two criterion options are required.");
            }

            ValidateUpdateOptions(dto.Options);

            var existingOptions = await _repo.All<CriterionOption>()
                .Where(co => co.CriterionId == criterionId)
                .ToListAsync();

            var existingById = existingOptions
                .ToDictionary(o => o.Id);

            var submittedExistingIds = dto.Options
                .Where(o => o.Id > 0)
                .Select(o => o.Id)
                .ToHashSet();

            var optionsToDelete = existingOptions
                .Where(o => !submittedExistingIds.Contains(o.Id))
                .ToList();

            foreach (var option in optionsToDelete)
            {
                var isUsed = await _repo
                    .AllReadonly<AlternativeValue>()
                    .AnyAsync(av =>
                        av.CriterionOptionId == option.Id);

                if (isUsed)
                {
                    throw new ArgumentException(
                        $"Criterion option '{option.Value}' is currently used by an alternative and cannot be removed.");
                }
            }
     
            var newOptions = new List<CriterionOption>();

            for (int i = 0; i < dto.Options.Count; i++)
            {
                var input = dto.Options[i];
                var value = input.Value.Trim();
                var rank = i + 1;

                if (input.Id > 0)
                {
                    if (!existingById.TryGetValue(input.Id, out var option))
                    {
                        throw new ArgumentException(
                            $"Criterion option with ID {input.Id} does not belong to criterion {criterionId}.");
                    }

                    var valueChanged = !string.Equals(
                        option.Value.Trim(),
                        value,
                        StringComparison.OrdinalIgnoreCase);

                    if (valueChanged)
                    {
                        var isUsed = await _repo
                            .AllReadonly<AlternativeValue>()
                            .AnyAsync(av =>
                                av.CriterionOptionId == option.Id);

                        if (isUsed)
                        {
                            throw new ArgumentException(
                                $"Criterion option '{option.Value}' is already used by an alternative and cannot be renamed.");
                        }
                    }

                    option.Value = value;
                    option.Rank = rank;
                }
                else
                {
                    var newOption = new CriterionOption
                    {
                        CriterionId = criterionId,
                        Value = value,
                        Rank = rank
                    };

                    newOptions.Add(newOption);
                }
            }

           
            foreach (var option in optionsToDelete)
            {
                _repo.Delete(option);
            }

          
            if (newOptions.Count > 0)
            {
                await _repo.AddRangeAsync(newOptions);
            }

            await _repo.SaveChangesAsync();
        }

        private static void ValidateCriterionType(Criterion criterion)
        {
            if (criterion.CriterionType != CriterionType.Categorical)
            {
                throw new ArgumentException(
                    "Criterion options can only be created for a categorical criterion.");
            }
        }

        private static void ValidateOptionValues(
            IEnumerable<string> values)
        {
            foreach (var value in values)
            {
                ValidateOptionValue(value);
            }

            if (values
                .GroupBy(
                    value => value.Trim(),
                    StringComparer.OrdinalIgnoreCase)
                .Any(group => group.Count() > 1))
            {
                throw new ArgumentException(
                    "Criterion options must be unique.");
            }
        }

        private static void ValidateUpdateOptions(
            IEnumerable<CriterionOptionOrderDto> options)
        {
            foreach (var option in options)
            {
                ValidateOptionValue(option.Value);
            }

            if (options
                .GroupBy(
                    option => option.Value.Trim(),
                    StringComparer.OrdinalIgnoreCase)
                .Any(group => group.Count() > 1))
            {
                throw new ArgumentException(
                    "Criterion option values must be unique.");
            }
        }

        private static void ValidateOptionValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(
                    "Criterion option value is required.");
            }

            if (value.Trim().Length > 200)
            {
                throw new ArgumentException(
                    "Criterion option value cannot exceed 200 characters.");
            }


        }
    }
}
