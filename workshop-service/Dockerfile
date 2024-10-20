FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["YCompanyWorkshopApi.csproj", "./"]
RUN dotnet restore "YCompanyWorkshopApi.csproj"
COPY . .
RUN dotnet build "YCompanyWorkshopApi.csproj" -o /app/build

FROM build AS publish
RUN dotnet publish "YCompanyWorkshopApi.csproj" -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /src/Properties /app/Properties

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5003
ENV DOTNET_SYSTEM_NET_HTTP_USESOCKETSHTTPHANDLER=0

# Expose port 5000
EXPOSE 5003
ENTRYPOINT ["dotnet", "YCompanyWorkshopApi.dll"]