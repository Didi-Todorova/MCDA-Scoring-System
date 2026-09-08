using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Configuration
{
    public class CriterionConfiguration:IEntityTypeConfiguration<Criterion>
    {
        public void Configure(EntityTypeBuilder<Criterion> builder)
        {
            builder.HasData(
                new Criterion
                {
                    Id = 2,
                    DecisionId = 1,
                    Name = "RAM",
                    CriterionType = CriterionType.Numerical,
                    Weight = 2.1m
                },

                new Criterion
                {
                    Id = 4,
                    DecisionId = 1,
                    Name = "Brand",
                    CriterionType = CriterionType.Categorical,
                    Weight = 1.0m
                },

                new Criterion
                {
                    Id = 5,
                    DecisionId = 1,
                    Name = "Performance",
                    CriterionType = CriterionType.Categorical,
                    Weight = 1.5m
                },

                new Criterion
                {
                    Id = 1,
                    DecisionId = 1,
                    Name = "Price",
                    CriterionType = CriterionType.Numerical,
                    Weight = 3.4m,                     
                },

                new Criterion
                {
                    Id = 3,
                    DecisionId = 1,
                    Name = "Storage",
                    CriterionType = CriterionType.Numerical,
                    Weight = 2.5m
                }
            );
        }
    }
}
