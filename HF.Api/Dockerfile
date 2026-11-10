
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY HF.Api/HF.Api.csproj HF.Api/

RUN dotnet restore HF.Api/HF.Api.csproj

COPY HF.Api/ HF.Api/

RUN dotnet publish HF.Api/HF.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .

USER $APP_UID

ENTRYPOINT ["dotnet", "HF.Api.dll"]
