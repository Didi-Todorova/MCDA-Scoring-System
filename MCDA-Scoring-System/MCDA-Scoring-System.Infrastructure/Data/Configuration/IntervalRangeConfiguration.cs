using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Configuration
{
    public class IntervalRangeConfiguration : IEntityTypeConfiguration<IntervalRange>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<IntervalRange> builder)
        {
            builder.HasData(
                new IntervalRange
                {
                    Id = 1,
                    CriterionNumericalRuleId = 3,
                    MinValue = 570m,
                    MaxValue = 1000.0m
                },
                new IntervalRange
                {
                    Id = 2,
                    CriterionNumericalRuleId = 3,
                    MinValue = 240m,
                    MaxValue = 560m
                },
                new IntervalRange
                {
                    Id = 3,
                    CriterionNumericalRuleId = 3,
                    MinValue = 124m,
                    MaxValue = 239m
                }
            );
        }
    }
}
