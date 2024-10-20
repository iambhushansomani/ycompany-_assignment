FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["YCompanyClaimsApi.csproj", "./"]
RUN dotnet restore "YCompanyClaimsApi.csproj"
COPY . .
RUN dotnet build "YCompanyClaimsApi.csproj" -o /app/build

FROM build AS publish
RUN dotnet publish "YCompanyClaimsApi.csproj" -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /src/Properties /app/Properties

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5001
ENV DOTNET_SYSTEM_NET_HTTP_USESOCKETSHTTPHANDLER=0

# Expose port 5000
EXPOSE 5001
ENTRYPOINT ["dotnet", "YCompanyClaimsApi.dll"]