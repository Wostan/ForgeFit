FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/ForgeFit.Api/ForgeFit.Api.csproj", "src/ForgeFit.Api/"]
COPY ["src/ForgeFit.Application/ForgeFit.Application.csproj", "src/ForgeFit.Application/"]
COPY ["src/ForgeFit.Domain/ForgeFit.Domain.csproj", "src/ForgeFit.Domain/"]
COPY ["src/ForgeFit.Infrastructure/ForgeFit.Infrastructure.csproj", "src/ForgeFit.Infrastructure/"]
RUN dotnet restore "src/ForgeFit.Api/ForgeFit.Api.csproj"
COPY . .
WORKDIR "/src/src/ForgeFit.Api"
RUN dotnet build "./ForgeFit.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ForgeFit.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ForgeFit.Api.dll"]
