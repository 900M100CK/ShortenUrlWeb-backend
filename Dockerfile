FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["ShortenUrlWeb.csproj", "./"]
RUN dotnet restore "./ShortenUrlWeb.csproj"

COPY . .
WORKDIR "/src/."
RUN dotnet build "ShortenUrlWeb.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ShortenUrlWeb.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "ShortenUrlWeb.dll"]