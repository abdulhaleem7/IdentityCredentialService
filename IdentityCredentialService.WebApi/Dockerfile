FROM mcr.microsoft.com/dotnet/aspnet:8.0-nanoserver-1809 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0-nanoserver-1809 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["IdentityCredentialService.WebApi/IdentityCredentialService.WebApi.csproj", "IdentityCredentialService.WebApi/"]
COPY ["IdentityCredentialService.BusinessLogic/IdentityCredentialService.BusinessLogic.csproj", "IdentityCredentialService.BusinessLogic/"]
COPY ["IdentityCredentialService.Domain/IdentityCredentialService.Domain.csproj", "IdentityCredentialService.Domain/"]
COPY ["IdentityCredentialService.Infrastructure/IdentityCredentialService.Infrastructure.csproj", "IdentityCredentialService.Infrastructure/"]
RUN dotnet restore "./IdentityCredentialService.WebApi/IdentityCredentialService.WebApi.csproj"
COPY . .
WORKDIR "/src/IdentityCredentialService.WebApi"
RUN dotnet build "./IdentityCredentialService.WebApi.csproj" -c %BUILD_CONFIGURATION% -o /app/build


FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./IdentityCredentialService.WebApi.csproj" -c %BUILD_CONFIGURATION% -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityCredentialService.WebApi.dll"]