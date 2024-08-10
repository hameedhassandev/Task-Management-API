
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TaskManagement.Core.Entities;
namespace TaskManagement.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<TaskEntity> Tasks { get; set; }
        public DbSet<TaskAssignment> TaskAssignments { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //M2M between User and Role
            modelBuilder.Entity<UserRole>()
                .HasKey(e => new { e.UserId, e.RoleId });
            modelBuilder.Entity<UserRole>().HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(u => u.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
              .WithMany(u => u.UserRoles)
              .HasForeignKey(u => u.RoleId);

            //M2M between User and Task
            modelBuilder.Entity<TaskAssignment>()
                 .HasKey(ta => new { ta.TaskId, ta.UserId });

            modelBuilder.Entity<TaskAssignment>()
                  .HasOne(ta => ta.Task)
                  .WithMany(t => t.TaskAssignments)
                  .HasForeignKey(ta => ta.TaskId);

            modelBuilder.Entity<TaskAssignment>()
                .HasOne(ta => ta.User)
                .WithMany(u => u.TaskAssignments)
                .HasForeignKey(ta => ta.UserId)
                .OnDelete(DeleteBehavior.Restrict);


            //M2M between User and Team
            modelBuilder.Entity<TeamMember>()
           .HasKey(tm => new { tm.TeamId, tm.UserId });
            modelBuilder.Entity<TeamMember>()
                .HasOne(tm => tm.Team)
                .WithMany(t => t.TeamMembers)
                .HasForeignKey(tm => tm.TeamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TeamMember>()
                .HasOne(tm => tm.User)
                .WithMany(u => u.TeamMemberships)
                .HasForeignKey(tm => tm.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            //O2M between Project and Task
            modelBuilder.Entity<TaskEntity>()
             .HasOne(te => te.Project)
             .WithMany(p => p.Tasks)
             .HasForeignKey(te => te.ProjectId);

            //O2M between  task and attachment
            modelBuilder.Entity<AttachmentEntity>()
            .HasOne(ae => ae.Task)
            .WithMany(te => te.Attachments)
            .HasForeignKey(ae => ae.TaskId);
        }
    }
}
