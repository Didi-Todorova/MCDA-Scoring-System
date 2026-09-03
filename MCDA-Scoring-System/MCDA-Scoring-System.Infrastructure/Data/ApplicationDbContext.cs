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
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Alternative>()
                .HasMany(a => a.AlternativeValues)
                .WithOne(av => av.Alternative)
                .HasForeignKey(av => av.AlternativeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Criterion>()
                .HasMany(c => c.AlternativeValues)
                .WithOne(av => av.Criterion)
                .HasForeignKey(av => av.CriterionId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Criterion>()
                .HasMany(c => c.CriterionOptions)
                .WithOne(co => co.Criterion)
                .HasForeignKey(co => co.CriterionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Criterion>()
                .HasMany(c => c.NumericalCriterionRules)
                .WithOne(ncr => ncr.Criterion)
                .HasForeignKey(ncr => ncr.CriterionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CriterionOption>()
                .HasMany(co => co.AlternativeValues)
                .WithOne(av => av.CriterionOption)
                .HasForeignKey(av => av.CriterionOptionId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Decision>()
                .HasMany(d => d.Criteria)
                .WithOne(c => c.Decision)
                .HasForeignKey(c => c.DecisionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Decision>()
                .HasMany(d => d.Alternatives)
                .WithOne(a => a.Decision)
                .HasForeignKey(a => a.DecisionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NumericalCriterionRule>()
                .HasMany(ncr => ncr.NumericRanges)
                .WithOne(nr => nr.NumericalCriterionRule)
                .HasForeignKey(nr => nr.NumericalCriterionRuleId)
                .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<User>()
            //    .HasMany(u => u.Decisions)
            //    .WithOne(d => d.User)
            //    .HasForeignKey(d => d.UserId)
            //    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AlternativeValue>()
                .HasIndex(av => new { av.AlternativeId, av.CriterionId })
                .IsUnique();

            modelBuilder.Entity<CriterionOption>()
                .HasIndex(co => new { co.CriterionId, co.Value })
                .IsUnique();

            modelBuilder.Entity<AlternativeValue>()
            .Property(av => av.NumericValue)
            .HasPrecision(18, 2);

            modelBuilder.Entity<Criterion>()
                .Property(c => c.Weight)
                .HasPrecision(18, 2);

            modelBuilder.Entity<NumericRange>()
                .Property(nr => nr.MinValue)
                .HasPrecision(18, 2);

            modelBuilder.Entity<NumericRange>()
                .Property(nr => nr.MaxValue)
                .HasPrecision(18, 2);

            modelBuilder.Entity<NumericalCriterionRule>()
                .Property(ncr => ncr.TargetValue)
                .HasPrecision(18, 2);
        }
    }
}