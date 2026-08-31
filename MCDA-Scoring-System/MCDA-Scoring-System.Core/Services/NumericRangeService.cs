using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.NumericRange;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Services
{
    public class NumericRangeService : INumericRangeService
    {
        private readonly IRepository repo;

        public NumericRangeService(IRepository _repo)
        {
            repo = _repo;
        }

        public async Task<NumericRangeDto> CreateNumericRangeAsync(CreateNumericRangeDto createNumericRangeDto)
        {
            var numericRange = new NumericRange
            {
                NumericalCriterionRuleId = createNumericRangeDto.NumericalCriterionRuleId,
                MinValue = createNumericRangeDto.MinValue,
                MaxValue = createNumericRangeDto.MaxValue,
            };

            await repo.AddAsync<NumericRange>(numericRange);
            await repo.SaveChangesAsync();

            return new NumericRangeDto(
                numericRange.Id,
                numericRange.NumericalCriterionRuleId,
                numericRange.MinValue,
                numericRange.MaxValue
            );
        }

        public async Task DeleteNumericRangeAsync(int id)
        {
            var numericRange = await repo.GetByIdAsync<NumericRange>(id) ?? throw new ArgumentException($"NumericRange with ID {id} not found.");

            await repo.DeleteAsync<NumericRange>(numericRange);
            await repo.SaveChangesAsync();
        }

        public async Task<IEnumerable<NumericRangeDto>> GetAllNumericRangesAsync()
        {
            return await repo.AllReadonly<NumericRange>()
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
            return await repo.AllReadonly<NumericRange>()
                .Where(nr => nr.Id == id)
                .Select(nr => new NumericRangeDto(
                    nr.Id,
                    nr.NumericalCriterionRuleId,
                    nr.MinValue,
                    nr.MaxValue
                ))
                .FirstOrDefaultAsync();
        }

        public async Task UpdateNumericRangeAsync(int id, UpdateNumericRangeDto updateNumericRangeDto)
        {
            var numericRange = await repo.GetByIdAsync<NumericRange>(id) ?? throw new ArgumentException($"NumericRange with ID {id} not found.");

            numericRange.NumericalCriterionRuleId = updateNumericRangeDto.NumericalCriterionRuleId;
            numericRange.MinValue = updateNumericRangeDto.MinValue;
            numericRange.MaxValue = updateNumericRangeDto.MaxValue;

            await repo.SaveChangesAsync();
        }
    }
}
