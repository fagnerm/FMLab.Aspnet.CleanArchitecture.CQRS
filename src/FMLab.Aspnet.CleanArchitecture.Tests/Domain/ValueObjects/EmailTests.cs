// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.CleanArchitecture.Domain.Exceptions;
using FMLab.Aspnet.CleanArchitecture.Domain.ValueObjects;

namespace FMLab.Aspnet.CleanArchitecture.Tests.Domain.ValueObjects;

public class EmailTests
{

    [Theory]
    [InlineData("user@example.com")]
    [InlineData("user.name@domain.org")]
    [InlineData("user+tag@sub.domain.com")]
    public void Constructor_WithValidEmail_SetsValue(string validEmail)
    {
        var email = new Email(validEmail);

        Assert.Equal(validEmail, email.Value);
    }

    [Fact]
    public void Constructor_WithEmptyString_ThrowsDomainException()
    {
        var ex = Assert.Throws<DomainException>(() => new Email(""));

        Assert.Equal("Invalid email format", ex.Message);
    }


    [Theory]
    [InlineData("notanemail")]
    [InlineData("user@")]
    [InlineData("plainaddress")]
    [InlineData("User <user@example.com>")]
    public void Constructor_WithInvalidEmail_ThrowsDomainException(string invalidEmail)
    {
        var ex = Assert.Throws<DomainException>(() => new Email(invalidEmail));

        Assert.Equal("Invalid email format", ex.Message);
    }

    [Fact]
    public void EqualityOperator_SameValue_ReturnsTrue()
    {
        var a = new Email("user@example.com");
        var b = new Email("user@example.com");

        Assert.True(a == b);
    }

    [Fact]
    public void EqualityOperator_DifferentValues_ReturnsFalse()
    {
        var a = new Email("user@example.com");
        var b = new Email("other@example.com");

        Assert.True(a != b);
    }

    [Fact]
    public void Equals_SameValue_ReturnsTrue()
    {
        var a = new Email("user@example.com");
        var b = new Email("user@example.com");

        Assert.True(a.Equals(b));
    }

    [Fact]
    public void Equals_DifferentValue_ReturnsFalse()
    {
        var a = new Email("user@example.com");
        var b = new Email("other@example.com");

        Assert.False(a.Equals(b));
    }

    [Fact]
    public void Equals_Null_ReturnsFalse()
    {
        var a = new Email("user@example.com");

        Assert.False(a.Equals(null));
    }

    [Fact]
    public void GetHashCode_SameValue_ReturnsSameHash()
    {
        var a = new Email("user@example.com");
        var b = new Email("user@example.com");

        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void ToString_ReturnsValue()
    {
        var email = new Email("user@example.com");

        Assert.Equal("user@example.com", email.ToString());
    }
}
