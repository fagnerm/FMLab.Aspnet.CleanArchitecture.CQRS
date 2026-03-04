// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.CleanArchitecture.Domain.Entities;
using FMLab.Aspnet.CleanArchitecture.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FMLab.Aspnet.CleanArchitecture.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(n => n.Name)
               .HasConversion<NameToStringConverter>()
               .HasColumnType("VARCHAR(255)")
               .IsRequired();
        builder.Property(e => e.Email)
               .HasConversion<EmailToStringConverter>()
               .HasColumnType("VARCHAR(255)");
        builder.Property(s => s.Status)
               .HasColumnType("VARCHAR(50)")
               .IsRequired();
        builder.HasIndex(u => u.Name).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();
    }
}
