using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.NumericRange;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Services
{
    public class NumericRangeService : INumericRangeService
    {
        private readonly IRepository _repo;

        public NumericRangeService(IRepository repo)
        {
            this._repo = repo;
        }

        public async Task<NumericRangeDto> CreateNumericRangeAsync(CreateNumericRangeDto dto)
        {
            var numericRange = new NumericRange
            {
                NumericalCriterionRuleId = dto.NumericalCriterionRuleId,
                MinValue = dto.MinValue,
                MaxValue = dto.MaxValue,
            };

            await _repo.AddAsync<NumericRange>(numericRange);
            await _repo.SaveChangesAsync();

            return new NumericRangeDto(
                numericRange.Id,
                numericRange.NumericalCriterionRuleId,
                numericRange.MinValue,
                numericRange.MaxValue
            );
        }

        public async Task<bool> DeleteNumericRangeAsync(int id)
        {
            var numericRange = await _repo.GetByIdAsync<NumericRange>(id);

            if (numericRange == null)
                return false;

            await _repo.DeleteAsync<NumericRange>(numericRange);
            await _repo.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<NumericRangeDto>> GetAllNumericRangesAsync()
        {
            return await _repo.AllReadonly<NumericRange>()
                .Select(nr => new NumericRangeDto(
                    nr.Id,
                    nr.NumericalCriterionRuleId,
                    nr.MinValue,
                    nr.MaxValue
                ))
                .ToListAsync();
        }

        public async Task<NumericRangeDto?> GetNumericRangeByIdAsync(int id)
        {
            return await _repo.AllReadonly<NumericRange>()
                .Where(nr => nr.Id == id)
                .Select(nr => new NumericRangeDto(
                    nr.Id,
                    nr.NumericalCriterionRuleId,
                    nr.MinValue,
                    nr.MaxValue
                ))
                .FirstOrDefaultAsync();
        }

        public async Task PatchNumericRangeAsync(int id, PatchNumericRangeDto dto)
        {
            var numericRange = await _repo.GetByIdAsync<NumericRange>(id) ?? throw new ArgumentException($"Numeric Range with ID {id} not found.");

            if (dto.NumericalCriterionRuleId.HasValue)
                numericRange.NumericalCriterionRuleId = dto.NumericalCriterionRuleId.Value;
            if(dto.MinValue.HasValue)
                numericRange.MinValue = dto.MinValue.Value;
            if (dto.MaxValue.HasValue)
                numericRange.MaxValue = dto.MaxValue.Value;

            await _repo.SaveChangesAsync();

        }

        public async Task UpdateNumericRangeAsync(int id, UpdateNumericRangeDto dto)
        {
            var numericRange = await _repo.GetByIdAsync<NumericRange>(id) ?? throw new ArgumentException($"Numeric Range with ID {id} not found.");

            numericRange.NumericalCriterionRuleId = dto.NumericalCriterionRuleId;
            numericRange.MinValue = dto.MinValue;
            numericRange.MaxValue = dto.MaxValue;

            await _repo.SaveChangesAsync();
        }
    }
}
