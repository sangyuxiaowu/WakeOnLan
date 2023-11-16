FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WakeOnLan/WakeOnLan.csproj", "WakeOnLan/"]
RUN dotnet restore "WakeOnLan/WakeOnLan.csproj"
COPY . .
WORKDIR "/src/WakeOnLan"
RUN dotnet build "WakeOnLan.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "WakeOnLan.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=true

FROM base AS final
COPY --from=publish /app/publish .
ENTRYPOINT ["./WakeOnLan"]