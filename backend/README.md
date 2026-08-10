//create a new .Net project
dotnet new webapi --name PrimeKare.Api

//create a new xUnit test
dotnet new xunit --name PrimeKare.Api.Tests
dotnet add PrimeKare.Api.Tests/PrimeKare.Api.Tests.csproj reference PrimeKare.Api/PrimeKare.Api.csproj
dotnet add PrimeKare.Api.Tests/PrimeKare.Api.Tests.csproj package Microsoft.AspNetCore.Mvc.Testing --version 10.0.10

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
dotnet user-secrets set ...

//install Swagger UI Package
dotnet add package Swashbuckle.AspNetCore

//clean the project
dotnet clean
rmdir /s /q bin
rmdir /s /q obj
dotnet restore