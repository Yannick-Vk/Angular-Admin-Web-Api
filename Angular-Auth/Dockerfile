# Use the official .NET Core SDK as a parent image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files to leverage Docker cache
COPY "Angular-Auth.sln" .
COPY "Angular-Auth/Angular-Auth.csproj" "Angular-Auth/"

# Restore dependencies
RUN dotnet restore "Angular-Auth.sln"

# Copy everything else and build
COPY . .
WORKDIR "/src/Angular-Auth"
RUN dotnet publish "Angular-Auth.csproj" -c Release -o /app/publish

# Build the runtime image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
EXPOSE 443

# Install .NET Core SDK and ASP.NET Core Runtime


# Generate a self-signed certificate for development
RUN dotnet dev-certs https -t
ENV ASPNETCORE_URLS="https://+;http://+"
ENV ASPNETCORE_HTTPS_PORT=443

ENTRYPOINT ["dotnet", "Angular-Auth.dll"]
