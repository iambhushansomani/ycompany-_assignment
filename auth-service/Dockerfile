FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["YCompanyAuthApi.csproj", "./"]
RUN dotnet restore "YCompanyAuthApi.csproj"
COPY . .
RUN dotnet build "YCompanyAuthApi.csproj" -o /app/build

FROM build AS publish
RUN dotnet publish "YCompanyAuthApi.csproj" -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /src/Properties /app/Properties

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5000
ENV DOTNET_SYSTEM_NET_HTTP_USESOCKETSHTTPHANDLER=0

# Expose port 5000
EXPOSE 5000
ENTRYPOINT ["dotnet", "YCompanyAuthApi.dll"]