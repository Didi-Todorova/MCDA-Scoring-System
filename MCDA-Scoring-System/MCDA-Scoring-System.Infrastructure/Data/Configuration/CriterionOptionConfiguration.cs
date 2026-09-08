using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Configuration
{
    public class CriterionOptionConfiguration : IEntityTypeConfiguration<CriterionOption>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<CriterionOption> builder)
        {
            builder.HasData(
                new CriterionOption
                {
                    Id = 1,
                    CriterionId = 2,
                    Value = "Lenovo",
                    Rank = 1
                },
                new CriterionOption
                {
                    Id = 2,
                    CriterionId = 2,
                    Value = "HP",
                    Rank = 2
                },
                new CriterionOption
                {
                    Id = 3,
                    CriterionId = 2,
                    Value = "Mac",
                    Rank = 3
                },
                new CriterionOption
                {
                    Id = 4,
                    CriterionId = 3,
                    Value = "Excellent",
                    Rank = 1
                },
                 new CriterionOption
                 {
                     Id = 5,
                     CriterionId = 3,
                     Value = "Good",
                     Rank = 2
                 },
                  new CriterionOption
                  {
                      Id = 6,
                      CriterionId = 3,
                      Value = "Poor",
                      Rank = 3
                  }
            );
        }    
    }
}
