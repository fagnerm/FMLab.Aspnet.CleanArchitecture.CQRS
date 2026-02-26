// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.CleanArchitecture.Domain.Exceptions;

namespace FMLab.Aspnet.CleanArchitecture.Tests.Domain.Exceptions;

public class DomainGuardTests
{
    [Fact]
    public void Throw_AlwaysThrowsDomainException()
    {
        var ex = Assert.Throws<DomainException>(() => DomainGuard.Throw("something went wrong"));

        Assert.Equal("something went wrong", ex.Message);
    }

    [Fact]
    public void Throw_WithInnerException_AttachesInnerException()
    {
        var inner = new InvalidOperationException("inner");

        var ex = Assert.Throws<DomainException>(() => DomainGuard.Throw("outer", inner));

        Assert.Equal("outer", ex.Message);
        Assert.Same(inner, ex.InnerException);
    }


    [Fact]
    public void ThrowIfNull_OnNull_ThrowsDomainException()
    {
        object? obj = null;

        Assert.Throws<DomainException>(() => obj!.ThrowIfNull());
    }

    [Fact]
    public void ThrowIfNull_OnNonNull_DoesNotThrow()
    {
        var obj = new object();

        var ex = Record.Exception(() => obj.ThrowIfNull());

        Assert.Null(ex);
    }

    [Fact]
    public void ThrowIfNullOrEmpty_OnNull_ThrowsDomainExceptionWithMessage()
    {
        string? value = null;

        var ex = Assert.Throws<DomainException>(() => value!.ThrowIfNullOrEmpty("field is required"));

        Assert.Equal("field is required", ex.Message);
    }

    [Fact]
    public void ThrowIfNullOrEmpty_OnEmpty_ThrowsDomainExceptionWithMessage()
    {
        var ex = Assert.Throws<DomainException>(() => "".ThrowIfNullOrEmpty("field is required"));

        Assert.Equal("field is required", ex.Message);
    }

    [Fact]
    public void ThrowIfNullOrEmpty_OnValidString_DoesNotThrow()
    {
        var ex = Record.Exception(() => "valid".ThrowIfNullOrEmpty("field is required"));

        Assert.Null(ex);
    }
}
