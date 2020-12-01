FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["OpenFTTH.Schematic.Service/OpenFTTH.Schematic.Service.csproj", "OpenFTTH.Schematic.Service/"]
RUN dotnet restore "OpenFTTH.Schematic.Service/OpenFTTH.Schematic.Service.csproj"
COPY . .
WORKDIR "/src/OpenFTTH.Schematic.Service"
RUN dotnet build "OpenFTTH.Schematic.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OpenFTTH.Schematic.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OpenFTTH.Schematic.Service.dll"]
