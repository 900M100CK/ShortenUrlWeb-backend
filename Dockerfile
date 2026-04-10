# Bước 1: Khởi tạo môi trường chạy (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080                                         # ← Sửa 80 thành 8080

# Bước 2: Sử dụng SDK để build ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["ShortenUrlWeb.csproj", "./"]
RUN dotnet restore "./ShortenUrlWeb.csproj"

COPY . .
WORKDIR "/src/."
RUN dotnet build "ShortenUrlWeb.csproj" -c Release -o /app/build

# Bước 3: Publish
FROM build AS publish
RUN dotnet publish "ShortenUrlWeb.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Bước 4: Copy vào runtime
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:8080                   # ← Thêm dòng này

ENTRYPOINT ["dotnet", "ShortenUrlWeb.dll"]