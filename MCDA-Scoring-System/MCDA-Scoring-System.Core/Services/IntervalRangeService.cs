using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.IntervalRange;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Services
{
    public class IntervalRangeService : IIntervalRangeService
    {
        private readonly IRepository _repo;

        public IntervalRangeService(IRepository repo)
        {
            this._repo = repo;
        }

        public async Task<IntervalRangeDto> CreateIntervalRangeAsync(CreateIntervalRangeDto dto)
        {
            var intervalRange = new IntervalRange
            {
                CriterionNumericalRuleId = dto.CriterionNumericalRuleId,
                MinValue = dto.MinValue,
                MaxValue = dto.MaxValue,
            };

            await _repo.AddAsync<IntervalRange>(intervalRange);
            await _repo.SaveChangesAsync();

            return new IntervalRangeDto(
                intervalRange.Id,
                intervalRange.CriterionNumericalRuleId,
                intervalRange.MinValue,
                intervalRange.MaxValue
            );
        }

        public async Task<bool> DeleteIntervalRangeAsync(int id)
        {
            var intervalRange = await _repo.GetByIdAsync<IntervalRange>(id);

            if (intervalRange == null)
                return false;

            await _repo.DeleteAsync<IntervalRange>(intervalRange);
            await _repo.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<IntervalRangeDto>> GetAllIntervalRangesAsync()
        {
            return await _repo.AllReadonly<IntervalRange>()
                .Select(nr => new IntervalRangeDto(
                    nr.Id,
                    nr.CriterionNumericalRuleId,
                    nr.MinValue,
                    nr.MaxValue
                ))
                .ToListAsync();
        }

        public async Task<IntervalRangeDto?> GetIntervalRangeByIdAsync(int id)
        {
            return await _repo.AllReadonly<IntervalRange>()
                .Where(nr => nr.Id == id)
                .Select(nr => new IntervalRangeDto(
                    nr.Id,
                    nr.CriterionNumericalRuleId,
                    nr.MinValue,
                    nr.MaxValue
                ))
                .FirstOrDefaultAsync();
        }

        public async Task PatchIntervalRangeAsync(int id, PatchIntervalRangeDto dto)
        {
            var intervalRange = await _repo.GetByIdAsync<IntervalRange>(id) ?? throw new ArgumentException($"Interval Range with ID {id} not found.");

            if (dto.CriterionNumericalRuleId.HasValue)
                intervalRange.CriterionNumericalRuleId = dto.CriterionNumericalRuleId.Value;
            if(dto.MinValue.HasValue)
                intervalRange.MinValue = dto.MinValue.Value;
            if (dto.MaxValue.HasValue)
                intervalRange.MaxValue = dto.MaxValue.Value;

            await _repo.SaveChangesAsync();

        }

        public async Task UpdateIntervalRangeAsync(int id, UpdateIntervalRangeDto dto)
        {
            var intervalRange = await _repo.GetByIdAsync<IntervalRange>(id) ?? throw new ArgumentException($"Interval Range with ID {id} not found.");

            intervalRange.CriterionNumericalRuleId = dto.CriterionNumericalRuleId;
            intervalRange.MinValue = dto.MinValue;
            intervalRange.MaxValue = dto.MaxValue;

            await _repo.SaveChangesAsync();
        }
    }
}
