using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Configuration
{
    public class DecisionConfiguration:IEntityTypeConfiguration<Decision>
    {
        public void Configure(EntityTypeBuilder<Decision> builder)
        {
            builder.HasData(
                new Decision
                {
                    Id = 1,
                    Name = "Choose a Laptop",
                    WeightingMethod = WeightingMethod.DirectRanking,
                    CreatedAt = new DateTime(2026, 9, 8, 0, 0, 0, DateTimeKind.Utc)
                },
                new Decision
                {
                    Id = 2,
                    Name = "Choose a University",
                    WeightingMethod = WeightingMethod.PercentageAllocation,
                    CreatedAt = new DateTime(2026, 9, 8, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
