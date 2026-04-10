# Bước 1: Khởi tạo môi trường chạy (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Bước 2: Sử dụng SDK để build ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy file .csproj vào trước và restore các package (Giúp tận dụng cache của Docker)
COPY ["ShortenUrlWeb.csproj", "./"]
RUN dotnet restore "./ShortenUrlWeb.csproj"

# Copy toàn bộ mã nguồn còn lại và build
COPY . .
WORKDIR "/src/."
RUN dotnet build "ShortenUrlWeb.csproj" -c Release -o /app/build

# Bước 3: Publish ứng dụng ra các file DLL
FROM build AS publish
RUN dotnet publish "ShortenUrlWeb.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Bước 4: Copy file đã publish vào môi trường chạy ở Bước 1
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Định nghĩa lệnh sẽ chạy khi Container khởi động
ENTRYPOINT ["dotnet", "ShortenUrlWeb.dll"]