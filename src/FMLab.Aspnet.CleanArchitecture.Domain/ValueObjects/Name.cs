// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.CleanArchitecture.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace FMLab.Aspnet.CleanArchitecture.Domain.ValueObjects;

public record Name : IComparable<Name>
{
    public string Value { get; init; }

    public Name(string name)
    {
        name.ThrowIfNullOrEmpty("Must inform a name");
        if (!IsValid(name))
        {
            DomainGuard.Throw("Must inform a valid name");
        }

        Value = name;
    }

    private bool IsValid(string name)
    {
        var pattern = @"^[\p{L}\p{Zs}]+$";
        return Regex.IsMatch(name, pattern);

    }
    public int CompareTo(Name? other)
    {
        return string.Compare(Value, other?.Value, StringComparison.Ordinal);
    }

    public override string ToString() => Value.ToString();
}
