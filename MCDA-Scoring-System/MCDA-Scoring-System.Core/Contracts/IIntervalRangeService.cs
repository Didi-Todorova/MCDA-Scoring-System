using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.IntervalRange;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts
{
    public interface IIntervalRangeService
    {
        public Task<IntervalRangeDto> CreateIntervalRangeAsync(CreateIntervalRangeDto dto);
        public Task<IntervalRangeDto?> GetIntervalRangeByIdAsync(int id);
        public Task<IEnumerable<IntervalRangeDto>> GetAllIntervalRangesAsync();
        public Task UpdateIntervalRangeAsync(int id, UpdateIntervalRangeDto dto);
        public Task PatchIntervalRangeAsync(int id, PatchIntervalRangeDto dto);
        public Task<bool> DeleteIntervalRangeAsync(int id);
    }
}
