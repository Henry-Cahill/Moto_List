FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src
COPY src/Moto_List.Shared/Moto_List.Shared.csproj Moto_List.Shared/
COPY src/Moto_List.Web/Moto_List.Web.csproj Moto_List.Web/
RUN dotnet restore Moto_List.Web/Moto_List.Web.csproj
COPY src/Moto_List.Shared/ Moto_List.Shared/
COPY src/Moto_List.Web/ Moto_List.Web/
RUN dotnet publish Moto_List.Web/Moto_List.Web.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Moto_List.Web.dll"]
