using LeaveManagement.Domain.Common;
using LeaveManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using LeaveManagement.Application.Interfaces.Services;

namespace LeaveManagement.Persistence.Contexts
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IAuthenticatedUserService authenticatedUser) : DbContext(options)
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Employee configuration
            builder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.EmployeeId);
                entity.Property(e => e.EmployeeNumber).IsRequired().HasMaxLength(10);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CellphoneNumber).HasMaxLength(20);

                entity.HasOne(e => e.Manager)
                    .WithMany(e => e.Subordinates)
                    .HasForeignKey(e => e.ManagerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.EmployeeNumber).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // LeaveRequest configuration
            builder.Entity<LeaveRequest>(entity =>
            {
                entity.HasKey(lr => lr.LeaveRequestId);
                entity.Property(lr => lr.Comments).HasMaxLength(500);
                entity.Property(lr => lr.ApprovalComments).HasMaxLength(500);

                entity.HasOne(lr => lr.Employee)
                    .WithMany(e => e.LeaveRequests)
                    .HasForeignKey(lr => lr.EmployeeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(lr => lr.ApprovedBy)
                    .WithMany(e => e.ApprovedLeaveRequests)
                    .HasForeignKey(lr => lr.ApprovedById)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<AuditableBaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.LastRecordUpdateDate = DateTime.Now;
                        entry.Entity.LastRecordUpdateUserid = authenticatedUser.UserId;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastRecordUpdateDate = DateTime.Now;
                        entry.Entity.LastRecordUpdateUserid = authenticatedUser.UserId;
                        break;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
