FROM mcr.microsoft.com/dotnet/aspnet:8.0-nanoserver-ltsc2022 AS base
WORKDIR /app
EXPOSE 5028

ENV ASPNETCORE_URLS=http://+:5028

FROM mcr.microsoft.com/dotnet/sdk:8.0-nanoserver-ltsc2022 AS build
ARG configuration=Release
WORKDIR /src
COPY ["MoneyMap.Api/MoneyMap.Api.csproj", "MoneyMap.Api/"]
RUN dotnet restore "MoneyMap.Api\MoneyMap.Api.csproj"
COPY . .
WORKDIR "/src/MoneyMap.Api"
RUN dotnet build "MoneyMap.Api.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "MoneyMap.Api.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MoneyMap.Api.dll"]
