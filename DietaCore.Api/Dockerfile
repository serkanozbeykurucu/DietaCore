# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

##

COPY ../DietaCore.sln ./
COPY ../DietaCore.Api/DietaCore.Api.csproj ./DietaCore.Api/
COPY ../DietaCore.Business/DietaCore.Business.csproj ./DietaCore.Business/
COPY ../DietaCore.DataAccess/DietaCore.DataAccess.csproj ./DietaCore.DataAccess/
COPY ../DietaCore.Dto/DietaCore.Dto.csproj ./DietaCore.Dto/
COPY ../DietaCore.Entities/DietaCore.Entities.csproj ./DietaCore.Entities/
COPY ../DietaCore.Shared/DietaCore.Shared.csproj ./DietaCore.Shared/

##

RUN dotnet restore "./DietaCore.Api/DietaCore.Api.csproj"

COPY . .

WORKDIR "/src/DietaCore.Api"
RUN dotnet build "./DietaCore.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DietaCore.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DietaCore.Api.dll"]