# .NET Core SDK
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Sets the working directory
WORKDIR /app

ARG ENVIRONMENT=Development

#COPY *.sln .
COPY ["FintechTestTask.WebAPI/FintechTestTask.WebAPI.csproj", "FintechTestTask.WebAPI/"]
COPY ["FintechTestTask.Infrastructure/FintechTestTask.Infrastructure.csproj", "FintechTestTask.Infrastructure/"]
COPY ["FintechTestTask.Domain/FintechTestTask.Domain.csproj", "FintechTestTask.Domain/"]
COPY ["FintechTestTask.Application/FintechTestTask.Application.csproj", "FintechTestTask.Application/"]

# .NET Core Restore1
RUN dotnet restore FintechTestTask.WebAPI/FintechTestTask.WebAPI.csproj

# Copy All Files
COPY . .
EXPOSE 8080

# .NET Core Build and Publish
RUN dotnet publish FintechTestTask.WebAPI/FintechTestTask.WebAPI.csproj -c Development -o /publish

# ASP.NET Core Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /publish ./

ENV ASPNETCORE_ENVIRONMENT $ENVIRONMENT

ENTRYPOINT ["dotnet", "FintechTestTask.WebAPI.dll"]