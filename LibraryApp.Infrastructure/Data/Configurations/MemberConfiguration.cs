using LibraryApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LibraryApp.Domain.Constants;

namespace LibraryApp.Infrastructure.Data.Configurations;

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.HasKey(m => m.Id);
        
        builder.HasQueryFilter(m => !m.IsDeleted);
        
        builder.Property(m => m.FullName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(m => m.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasIndex(m => m.Email)
            .IsUnique();

        builder.HasData(
            new Member 
            { 
                Id = 1, 
                FullName = "Admin User", 
                Email = "admin@library.com",
                // Pre-computed BCrypt hash of "admin123" — static value required by EF Core seed data
                PasswordHash = "$2a$11$x5RZfGMl5sCqaP6IoJNM4eK1SitGbDJfYGNaIXZfzJGKfE3cXHfNi",
                Role = Roles.Admin,
                CreatedAtUtc = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
