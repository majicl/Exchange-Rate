FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /api

# Copy files and restoring packages
COPY ./ ./
RUN dotnet restore ./Src/ExchangeRate.API

# testing
RUN dotnet build ./Src/ExchangeRate.API
RUN dotnet test ./Test/ExchangeRate.Test

# publishing
RUN dotnet publish ./Src/ExchangeRate.API -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
COPY --from=build /api/out .
ENTRYPOINT ["dotnet", "ExchangeRate.API.dll"]
