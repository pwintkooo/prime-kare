dotnet new webapi --name PrimeKare.Api
dotnet new xunit --name PrimeKare.Api.Tests
dotnet add PrimeKare.Api.Tests/PrimeKare.Api.Tests.csproj reference PrimeKare.Api/PrimeKare.Api.csproj
dotnet add PrimeKare.Api.Tests/PrimeKare.Api.Tests.csproj package Microsoft.AspNetCore.Mvc.Testing --version 10.0.10