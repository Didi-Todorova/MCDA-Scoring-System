using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.NumericRange;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts
{
    public interface INumericRangeService
    {
        public Task<IntervalRangeDto> CreateNumericRangeAsync(CreateIntervalRangeDto dto);
        public Task<IntervalRangeDto?> GetNumericRangeByIdAsync(int id);
        public Task<IEnumerable<IntervalRangeDto>> GetAllNumericRangesAsync();
        public Task UpdateNumericRangeAsync(int id, UpdateIntervalRangeDto dto);
        public Task PatchNumericRangeAsync(int id, PatchIntervalRangeDto dto);
        public Task<bool> DeleteNumericRangeAsync(int id);
    }
}
