using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Alternative> Alternatives { get; set; }
        public DbSet<AlternativeValue> AlternativeValues { get; set; }
        public DbSet<Criterion> Criteria { get; set; }
        public DbSet<CriterionOption> CriterionOptions { get; set; }
        public DbSet<Decision> Decisions { get; set; }
        public DbSet<NumericalCriterionRule> NumericalCriterionRules { get; set; }
        public DbSet<NumericRange> NumericRanges { get; set; }
        public DbSet<User> Users { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Alternative>()
                .
            
            base.OnModelCreating(modelBuilder);        
        }
    }
}