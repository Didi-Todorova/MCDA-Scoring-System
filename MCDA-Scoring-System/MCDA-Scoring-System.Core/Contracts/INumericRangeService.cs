using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.NumericRange;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts
{
    public interface INumericRangeService
    {
        public Task<NumericRangeDto> CreateNumericRangeAsync(CreateNumericRangeDto createNumericRangeDto);
        public Task<NumericRangeDto?> GetNumericRangeByIdAsync(int id);
        public Task<IEnumerable<NumericRangeDto>> GetAllNumericRangesAsync();
        public Task UpdateNumericRangeAsync(int id, UpdateNumericRangeDto updateNumericRangeDto);
        public Task DeleteNumericRangeAsync(int id);
    }
}
