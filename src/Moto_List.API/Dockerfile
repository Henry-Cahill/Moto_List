FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src
COPY src/Moto_List.Shared/Moto_List.Shared.csproj Moto_List.Shared/
COPY src/Moto_List.API/Moto_List.API.csproj Moto_List.API/
RUN dotnet restore Moto_List.API/Moto_List.API.csproj
COPY src/Moto_List.Shared/ Moto_List.Shared/
COPY src/Moto_List.API/ Moto_List.API/
RUN dotnet publish Moto_List.API/Moto_List.API.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Moto_List.API.dll"]
