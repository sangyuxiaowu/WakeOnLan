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

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
RUN sed -i 's/deb.debian.org/mirrors.tuna.tsinghua.edu.cn/g' /etc/apt/sources.list.d/debian.sources \
    && apt-get update \
    && apt-get install -y --no-install-recommends \
    iputils-ping \
    && rm -rf /var/lib/apt/lists/* \
    && chmod u+s /bin/ping
WORKDIR /app
USER app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["./WakeOnLan"]