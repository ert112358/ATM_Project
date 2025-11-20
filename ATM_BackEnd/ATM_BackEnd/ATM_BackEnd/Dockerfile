FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5001
EXPOSE 5002

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ATM_BackEnd/ATM_BackEnd.csproj", "ATM_BackEnd/"]
RUN dotnet restore "ATM_BackEnd/ATM_BackEnd.csproj"
COPY . .
WORKDIR "/src/ATM_BackEnd"
RUN dotnet build "./ATM_BackEnd.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ATM_BackEnd.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ATM_BackEnd.dll"]
