//create a new .Net project
dotnet new webapi --name PrimeKare.Api

//create a new xUnit test
dotnet new xunit --name PrimeKare.Api.Tests
//reference the project
dotnet add PrimeKare.Api.Tests/PrimeKare.Api.Tests.csproj reference dev/PrimeKare.Api/PrimeKare.Api.csproj
dotnet add PrimeKare.Api.Tests/PrimeKare.Api.Tests.csproj package Microsoft.AspNetCore.Mvc.Testing --version 10.0.10
dotnet add package Microsoft.EntityFrameworkCore.InMemory

//install Entity Framework Core, PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Design

dotnet tool install --global dotnet-ef
//create database migration
dotnet ef migrations add InitialCreate
//apply the migration
dotnet ef database update

//install dotnetenv
dotnet add package DotNetEnv
//save the secrets locally with user-secrets
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "..."

//install Swagger UI Package
dotnet add package Swashbuckle.AspNetCore

//clean the project
dotnet clean
rmdir /s /q bin
rmdir /s /q obj
dotnet restore

//install JWT
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer

//install Firebase Admin SDK
dotnet add package FirebaseAdmin

//install Azure
dotnet add package Azure.Storage.Blobs
dotnet add package Azure.Identity

//install FluentValidation
dotnet add package FluentValidation
dotnet add package FluentValidation.DependencyInjectionExtensions

dotnet add package Microsoft.AspNetCore.Authentication.Google

dotnet add package Resend
dotnet add package Azure.ResourceManager.Communication