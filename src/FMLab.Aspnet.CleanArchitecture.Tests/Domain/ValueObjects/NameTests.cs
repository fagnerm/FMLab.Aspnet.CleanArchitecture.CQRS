// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.CleanArchitecture.Domain.Exceptions;
using FMLab.Aspnet.CleanArchitecture.Domain.ValueObjects;

namespace FMLab.Aspnet.CleanArchitecture.Tests.Domain.ValueObjects;

public class NameTests
{
    [Theory]
    [InlineData("John")]
    [InlineData("Mary Jane")]
    [InlineData("François")]
    [InlineData("Ñoño")]
    public void Constructor_WithValidName_SetsValue(string validName)
    {
        var name = new Name(validName);

        Assert.Equal(validName, name.Value);
    }

    [Fact]
    public void Constructor_WithEmptyString_ThrowsDomainException()
    {
        var ex = Assert.Throws<DomainException>(() => new Name(""));

        Assert.Equal("Must inform a name", ex.Message);
    }

    [Theory]
    [InlineData("John123")]
    [InlineData("John@Doe")]
    [InlineData("John-Doe")]
    [InlineData("John_Doe")]
    [InlineData("1234")]
    public void Constructor_WithInvalidCharacters_ThrowsDomainException(string invalidName)
    {
        var ex = Assert.Throws<DomainException>(() => new Name(invalidName));

        Assert.Equal("Must inform a valid name", ex.Message);
    }

    [Fact]
    public void EqualityOperator_SameValue_ReturnsTrue()
    {
        var a = new Name("John");
        var b = new Name("John");

        Assert.True(a == b);
    }

    [Fact]
    public void EqualityOperator_DifferentValues_ReturnsFalse()
    {
        var a = new Name("John");
        var b = new Name("Jane");

        Assert.True(a != b);
    }

    [Fact]
    public void Equals_SameValue_ReturnsTrue()
    {
        var a = new Name("John");
        var b = new Name("John");

        Assert.True(a.Equals(b));
    }

    [Fact]
    public void Equals_DifferentValue_ReturnsFalse()
    {
        var a = new Name("John");
        var b = new Name("Jane");

        Assert.False(a.Equals(b));
    }

    [Fact]
    public void Equals_Null_ReturnsFalse()
    {
        var a = new Name("John");

        Assert.False(a.Equals(null));
    }

    [Fact]
    public void CompareTo_SameValue_ReturnsZero()
    {
        var a = new Name("John");
        var b = new Name("John");

        Assert.Equal(0, a.CompareTo(b));
    }

    [Fact]
    public void CompareTo_Null_ReturnsPositive()
    {
        var a = new Name("John");

        Assert.True(a.CompareTo(null) > 0);
    }

    [Fact]
    public void GetHashCode_SameValue_ReturnsSameHash()
    {
        var a = new Name("John");
        var b = new Name("John");

        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void ToString_ReturnsValue()
    {
        var name = new Name("John");

        Assert.Equal("John", name.ToString());
    }
}
