using Domain.ProTrack.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.ProTrack.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Project>()
                .HasOne(p=> p.AssignedManager)
                .WithMany()
                .HasForeignKey(p => p.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ProjectUser>()
                .HasOne(pu=>pu.Project)
                .WithMany()
                .HasForeignKey(pu=>pu.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProjectUser>()
                .HasOne(pu=>pu.AssignedUser)
                .WithMany()
                .HasForeignKey(pu=>pu.AssignedUserId)
                .OnDelete(DeleteBehavior.SetNull);
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        //Adding tables to the database
        public DbSet<AppUser> Users {  get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectUser> ProjectUsers { get; set; }
    }
}
