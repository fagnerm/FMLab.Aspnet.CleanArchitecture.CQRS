// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho
// Licensed under the MIT License. See LICENSE file in the project root for details.

using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using FMLab.Aspnet.CleanArchitecture.Api.Configurations;
using FMLab.Aspnet.CleanArchitecture.Application.Handlers.CreateUser;
using FMLab.Aspnet.CleanArchitecture.Domain.Entities;
using FMLab.Aspnet.CleanArchitecture.Infrastructure.Persistence.Repositories;
using MediatR;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace FMLab.Aspnet.CleanArchitecture.Tests._Architecture;

public class CleanArchitectureTests
{
    private static readonly System.Reflection.Assembly DomainAssembly = typeof(User).Assembly;

    private static readonly System.Reflection.Assembly ApplicationAssembly = typeof(CreateUserHandler).Assembly;

    private static readonly System.Reflection.Assembly InfrastructureAssembly = typeof(UserRepository).Assembly;

    private static readonly System.Reflection.Assembly ApiAssembly = typeof(AppEndpoints).Assembly;

    private static readonly Architecture Architecture = new ArchLoader()
        .LoadAssemblies(
            DomainAssembly,
            ApplicationAssembly,
            InfrastructureAssembly,
            ApiAssembly)
        .Build();

    private static readonly IObjectProvider<IType> DomainLayer = Types().That().ResideInAssembly(DomainAssembly);

    private static readonly IObjectProvider<IType> ApplicationLayer = Types().That().ResideInAssembly(ApplicationAssembly);

    private static readonly IObjectProvider<IType> InfrastructureLayer = Types().That().ResideInAssembly(InfrastructureAssembly);

    private static readonly IObjectProvider<IType> ApiLayer = Types().That().ResideInAssembly(ApiAssembly);

    [Fact]
    public void Domain_ShouldNotDependOn_Application()
    {
        Types().That().ResideInAssembly(DomainAssembly)
            .Should().NotDependOnAny(ApplicationLayer)
            .Because("Domain must not have any knowledge of Application layer")
            .Check(Architecture);
    }

    [Fact]
    public void Domain_ShouldNotDependOn_Infrastructure()
    {
        Types().That().ResideInAssembly(DomainAssembly)
            .Should().NotDependOnAny(InfrastructureLayer)
            .Because("Domain must not have any knowledge of Infrastructure layer")
            .Check(Architecture);
    }

    [Fact]
    public void Domain_ShouldNotDependOn_Api()
    {
        Types().That().ResideInAssembly(DomainAssembly)
            .Should().NotDependOnAny(ApiLayer)
            .Because("Domain must not have any knowledge of Api layer")
            .Check(Architecture);
    }

    [Fact]
    public void Application_ShouldNotDependOn_Infrastructure()
    {
        Types().That().ResideInAssembly(ApplicationAssembly)
            .Should().NotDependOnAny(InfrastructureLayer)
            .Because("Application defines abstractions and must not depend on Infrastucture layer")
            .Check(Architecture);
    }

    [Fact]
    public void Application_ShouldNotDependOn_Api()
    {
        Types().That().ResideInAssembly(ApplicationAssembly)
            .Should().NotDependOnAny(ApiLayer)
            .Because("Application must not depend on API layer")
            .Check(Architecture);
    }

    [Fact]
    public void Infrastructure_ShouldNotDependOn_Api()
    {
        Types().That().ResideInAssembly(InfrastructureAssembly)
            .Should().NotDependOnAny(ApiLayer)
            .Because("Infrastructure implements persistence and services and must not reference API")
            .Check(Architecture);
    }

    [Fact]
    public void HandlerClasses_ShouldReside_InApplicationLayer()
    {
        Classes().That().ImplementInterface(typeof(IRequestHandler<,>))
            .And().HaveNameMatching(".*Handler")
            .Should().ResideInAssembly(ApplicationAssembly)
            .Because("Handler logic belongs to the Application layer")
            .Check(Architecture);
    }

    [Fact]
    public void RepositoryImplementations_ShouldReside_InInfrastructureLayer()
    {
        Classes().That().HaveNameMatching(".*Repository")
            .And().AreNotAbstract()
            .Should().ResideInAssembly(InfrastructureAssembly)
            .Because("Concrete repositories belong to the Infrastructure layer")
            .Check(Architecture);
    }

    [Fact]
    public void GatewayImplementations_ShouldReside_InInfrastructureLayer()
    {
        Classes().That().HaveNameMatching(".*Gateway")
            .And().AreNotAbstract()
            .Should().ResideInAssembly(InfrastructureAssembly)
            .Because("Concrete gateway belong to the Infrastructure layer")
            .Check(Architecture);
    }

    [Fact]
    public void DomainEntities_ShouldReside_InDomainLayer()
    {
        Classes().That().ResideInNamespace(
                "FMLab.Aspnet.CleanArchitecture.Domain.Entities")
            .Should().ResideInAssembly(DomainAssembly)
            .Because("Domain entities must be defined in the Domain layer")
            .Check(Architecture);
    }

    [Fact]
    public void InputDTOs_ShouldReside_InApplicationLayer()
    {
        Classes().That().HaveNameMatching(".*Command")
            .Should().ResideInAssembly(ApplicationAssembly)
            .Because("Commands must be defined on Application layer")
            .Check(Architecture);
    }

    [Fact]
    public void OutputDTOs_ShouldReside_InApplicationLayer()
    {
        Classes().That().HaveNameMatching(".*OutputDTO")
            .Should().ResideInAssembly(ApplicationAssembly)
            .Because("Output DTOs must be defined on Application layer")
            .Check(Architecture);
    }
}
