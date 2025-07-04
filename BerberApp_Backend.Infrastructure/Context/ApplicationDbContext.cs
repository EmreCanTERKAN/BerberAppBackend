﻿using BerberApp_Backend.Domain.Abstractions;
using BerberApp_Backend.Domain.Employees;
using BerberApp_Backend.Domain.Users;
using GenericRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BerberApp_Backend.Infrastructure.Context;
internal sealed class ApplicationDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>, IUnitOfWork
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        modelBuilder.Ignore<IdentityUserClaim<Guid>>();
        modelBuilder.Ignore<IdentityRoleClaim<Guid>>();
        modelBuilder.Ignore<IdentityUserToken<Guid>>();
        modelBuilder.Ignore<IdentityUserLogin<Guid>>();
        //modelBuilder.Ignore<IdentityUserRole<Guid>>();

        modelBuilder.Entity<IdentityUserRole<Guid>>(entity =>
        {
            entity.HasKey(ur => new { ur.UserId, ur.RoleId });

            entity.HasOne<AppUser>()
                  .WithMany()
                  .HasForeignKey(ur => ur.UserId)
                  .IsRequired();

            entity.HasOne<IdentityRole<Guid>>()
                  .WithMany()
                  .HasForeignKey(ur => ur.RoleId)
                  .IsRequired();
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<Entity>();

        HttpContextAccessor httpContextAccessor = new();

        string userIdString = httpContextAccessor.HttpContext!.User.Claims.First(p => p.Type == ClaimTypes.NameIdentifier).Value;

        Guid userId = Guid.Parse(userIdString);

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property(p => p.CreateAt).CurrentValue = DateTimeOffset.Now;
                entry.Property(p => p.CreateUserId).CurrentValue = userId;
            }

            if (entry.State == EntityState.Modified)
            {
                if (entry.Property(p => p.IsDeleted).CurrentValue == true)
                {
                    entry.Property(p => p.DeleteAt).CurrentValue = DateTimeOffset.Now;
                    entry.Property(p => p.DeleteUserId).CurrentValue = userId;
                }
                else
                {
                    entry.Property(p => p.UpdateAt).CurrentValue = DateTimeOffset.Now;
                    entry.Property(p => p.UpdateUserId).CurrentValue = userId;
                }
            }

            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified; // Soft delete
                entry.Property(p => p.IsDeleted).CurrentValue = true;
                entry.Property(p => p.DeleteAt).CurrentValue = DateTimeOffset.Now;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
