using School_VerticalSlice.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace School_VerticalSlice.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<StudentAnswers> StudentAnswers { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasOne(d => d.Teacher)
                      .WithOne(u => u.User)
                      .HasForeignKey<Teacher>(d => d.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(p => p.Student)
                      .WithOne(u => u.User)
                      .HasForeignKey<Student>(p => p.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasOne(c => c.Class)
                .WithMany(s=>s.Students)
                .HasForeignKey(c => c.ClassId)
                .OnDelete(DeleteBehavior.SetNull);
            });
            


            modelBuilder.Entity<Attendance>(entity =>
            {
                entity.HasOne(a => a.Student)
                      .WithMany(u => u.Attendances)
                      .HasForeignKey(b => b.StudentId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Session>(entity =>
            {
                entity.HasOne(s => s.Schedule)
                      .WithMany(t => t.Sessions)
                      .HasForeignKey(s => s.ScheduleId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(s => new { s.Day, s.SessionNumber, s.ScheduleId })
                      .IsUnique();
                entity.HasIndex(s => new { s.Day, s.SessionNumber, s.TeacherId })
                      .IsUnique();
            });

            modelBuilder.Entity<Quiz>()
                        .HasOne(q => q.Teacher)
                        .WithMany(t => t.Quizzes)
                        .HasForeignKey(q => q.TeacherId)
                        .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Question>(entity =>
            {
                entity.HasOne(q => q.Quiz)
                      .WithMany(qu => qu.Question)
                      .HasForeignKey(q => q.QuizId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<StudentAnswers>(entity =>
            {
                entity.HasOne(sa => sa.Student)
                      .WithMany(s => s.StudentAnswers)
                      .HasForeignKey(sa => sa.StudentId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(sa => sa.Question)
                      .WithMany(q => q.StudentAnswers)
                      .HasForeignKey(sa => sa.QuestionId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

        }
    }
}
