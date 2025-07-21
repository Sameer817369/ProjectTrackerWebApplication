using Domain.ProTrack.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Infrastructure.ProTrack.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Project>()
                .HasOne(p=> p.ProjectManager) //has one manager
                .WithMany(u=>u.ProjectsManaged)// one manager can manage many project (navigation in appuser)
                .HasForeignKey(p => p.ProjectManagerId)// foreign key 
                .OnDelete(DeleteBehavior.Restrict);// cannot delete user who is a manager in an project

            builder.Entity<ProjectUser>()
                .HasOne(pu=>pu.Project) // has one project
                .WithMany(p=>p.ProjectUsers) // one project many members (naigation property in project)
                .HasForeignKey(pu=>pu.ProjectId) // foreign key
                .OnDelete(DeleteBehavior.Cascade);// project deleted all projectuser deleted related to the project

            builder.Entity<ProjectUser>()
                .HasOne(pu=>pu.AssignedUser)// one manager
                .WithMany(u=>u.ProjectUsers)// many members (navigation property in appuser)
                .HasForeignKey(pu=>pu.AssignedUserId) // foreign key
                .OnDelete(DeleteBehavior.SetNull); // when member deleted it is set to null for that column

            builder.Entity<Tasks>()
                .HasOne(t => t.Project) // has one project
                .WithMany(t => t.Tasks) // project has many task (navigation property in project)
                .HasForeignKey(t => t.ProjectId) // foreign key
                .OnDelete(DeleteBehavior.Cascade); // project deleted all related task also deleted

            builder.Entity<Tasks>()
                .HasOne(t => t.TaskManager)
                .WithMany(u => u.TaskManaged)
                .HasForeignKey(t => t.TaskManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ProjectUserTask>()
                .HasOne(t=>t.Task)// one task
                .WithMany(t=>t.ProjectUserTasks)// many members(navigation property in Task)
                .HasForeignKey(t=>t.TaskId) // foreignkey
                .OnDelete(DeleteBehavior.Cascade); // task deleted all projectusertask deleted related to the task

            builder.Entity<ProjectUserTask>()
                .HasOne(put => put.ProjectUser)// one projectuser meaning same user cannot be assigned to the same task
                .WithMany(pu => pu.ProjectUserTasks) // projectuser can be assigned to multiple task (nav projerty in projectuser)
                .HasForeignKey(put => put.ProjectUserId) //foreign key
                .OnDelete(DeleteBehavior.Restrict);// cannot delete projectuser assigned to taks

            builder.Entity<Comment>()
                .HasOne(c=>c.CommentedProjectUserTask)
                .WithMany(p=>p.Comments)
                .HasForeignKey(c=>c.CommentedProjectUserTaskId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        //Adding tables to the database
        public DbSet<AppUser> Users {  get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectUser> ProjectUsers { get; set; }
        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<ProjectUserTask> ProjectUsersTask { get; set; }
        public DbSet<Comment> Comments  { get; set; }
        public DbSet<ProjectHistory> ProjectHistories { get; set; }
        public DbSet<TaskHistory> TaskHistories { get; set; } 
    }
}
