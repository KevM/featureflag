using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace featureflags
{
    public class FeatureFlagContext : DbContext
    {
        public FeatureFlagContext(DbContextOptions<FeatureFlagContext> options) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public DbSet<FeatureFlag> FeatureFlags { get; set; }
        public DbSet<Rule> Rules { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<RuleSchedule> RuleSchedules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RuleSchedule>().HasKey(rs => new { rs.RuleId, rs.ScheduleId });
        }
    }

    public class FeatureFlag
    {
        public Guid FeatureFlagId { get; set; }
        [Required]
        public string Name { get; set; }
        public bool IsEnabled { get; set; } = false;
        public bool ForceEnabled { get; set; } = false;
    }

    public enum RuleOperator
    {
        None,
        EqualTo,
        NotEqualTo,
    }

    public class Rule
    {
        public Guid RuleId { get; set; }
        public DateTime Activated { get; set; }
        public DateTime? Deactivated { get; set; }
        public string Key { get; set; }
        public string Operator { get; set; }
        public string MatchExpression { get; set; }

        public FeatureFlag FeatureFlag { get; set; }
        public IList<RuleSchedule> RuleSchedules { get; set; }
    }

    public class Schedule
    {
        public Guid ScheduleId { get; set; }
        public string Name { get; set; }
        public DateTime Activated { get; set; }
        public DateTime? Deactivated { get; set; }
        public double TrafficPercentage { get; set; }
        
        public IList<RuleSchedule> RuleSchedules { get; set; }
    }

    public class RuleSchedule
    {
        public Guid RuleId { get; set; }
        public Guid ScheduleId { get; set; }
        public int MatchCount { get; set; } = 0;
        public int MatchTrafficCount { get; set; } = 0;
    }
}
