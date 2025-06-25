using BerberApp_Backend.Domain.Abstractions;
using BerberApp_Backend.Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BerberApp_Backend.Infrastructure.Configurations;
internal sealed class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.HasIndex(p => p.UserName).IsUnique();
        builder.HasIndex(p => p.PhoneNumber).IsUnique();

        builder.Property(p => p.FirstName).HasColumnType("varchar(50)");
        builder.Property(p => p.LastName).HasColumnType("varchar(50)");
        builder.Property(p => p.UserName).HasColumnType("varchar(15)");
        builder.Property(p => p.PhoneNumber).HasColumnType("varchar(15)");
    }
}


internal sealed class IdentityUserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<Guid>> builder)
    {
        builder.HasKey(ur => new { ur.UserId, ur.RoleId });

        builder.HasOne<AppUser>()
              .WithMany()
              .HasForeignKey(ur => ur.UserId)
              .IsRequired();

        builder.HasOne<IdentityRole<Guid>>()
              .WithMany()
              .HasForeignKey(ur => ur.RoleId)
              .IsRequired(); ;
    }
}
