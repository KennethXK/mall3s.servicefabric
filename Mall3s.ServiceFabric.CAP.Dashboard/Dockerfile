#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 31001

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Mall3s.ServiceFabric.CAP.Dashboard/Mall3s.ServiceFabric.CAP.Dashboard.csproj", "Mall3s.ServiceFabric.CAP.Dashboard/"]
COPY ["Mall3s.Logging.Serilog/Mall3s.Logging.Serilog.csproj", "Mall3s.Logging.Serilog/"]
COPY ["ServiceFabric/Mall3s.ServiceFabric/Mall3s.ServiceFabric.csproj", "ServiceFabric/Mall3s.ServiceFabric/"]
COPY ["Mall3s.Nacos/Mall3s.Nacos.csproj", "Mall3s.Nacos/"]
COPY ["Mall3s/Mall3s.csproj", "Mall3s/"]
COPY ["Mall3s.Common/Mall3s.Common.csproj", "Mall3s.Common/"]
COPY ["Mall3s.Mapster/Mall3s.Mapster.csproj", "Mall3s.Mapster/"]
COPY ["Mall3s.Data.SqlSugar/Mall3s.Data.SqlSugar.csproj", "Mall3s.Data.SqlSugar/"]
COPY ["Mall3s.Authentication.JwtBearer/Mall3s.Authentication.JwtBearer.csproj", "Mall3s.Authentication.JwtBearer/"]
COPY ["Mall3s.ServiceFabric.Cap/Mall3s.ServiceFabric.Cap.csproj", "Mall3s.ServiceFabric.Cap/"]
COPY ["ServiceFabric/Mall3s.ServiceFabric.Core/Mall3s.ServiceFabric.Core.csproj", "ServiceFabric/Mall3s.ServiceFabric.Core/"]
RUN dotnet restore "Mall3s.ServiceFabric.CAP.Dashboard/Mall3s.ServiceFabric.CAP.Dashboard.csproj"
COPY . .
WORKDIR "/src/Mall3s.ServiceFabric.CAP.Dashboard"
RUN dotnet build "Mall3s.ServiceFabric.CAP.Dashboard.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Mall3s.ServiceFabric.CAP.Dashboard.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mall3s.ServiceFabric.CAP.Dashboard.dll"]