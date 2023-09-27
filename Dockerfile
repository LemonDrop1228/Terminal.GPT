# Use the official .NET 6 SDK image as the base image
FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build

# Set the working directory inside the container
WORKDIR /app

# Copy the .csproj and .sln files and restore any dependencies
COPY *.csproj .
COPY *.sln .
RUN dotnet restore

# Copy the application files and build the application
COPY . .
RUN dotnet publish -c Release -o out

# Create the final runtime image
FROM mcr.microsoft.com/dotnet/runtime:6.0-alpine AS runtime

# Set the working directory inside the container
WORKDIR /app

# Copy the published application from the build image
COPY --from=build /app/out .

# Set the entry point for your application
ENTRYPOINT ["dotnet", "TerminalGPT.dll"]
