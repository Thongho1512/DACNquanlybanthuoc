# ============================
#  Stage 1: Base runtime image
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# ============================
#  Stage 2: Build the project
# ============================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only the .csproj file first (for caching NuGet restore)
COPY ["quanlybanthuoc/quanlybanthuoc.csproj", "quanlybanthuoc/"]

# Restore dependencies (this step is cached if project files don't change)
RUN dotnet restore "quanlybanthuoc/quanlybanthuoc.csproj"

# Copy the rest of the project files into the container
COPY . .

# Set the working directory to the project folder
WORKDIR "/src/quanlybanthuoc"

# Build the project in Release mode
RUN dotnet build "quanlybanthuoc.csproj" -c Release -o /app/build

# ============================
#  Stage 3: Publish the app
# ============================
FROM build AS publish
RUN dotnet publish "quanlybanthuoc.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ============================
#  Stage 4: Final runtime image
# ============================
FROM base AS final
WORKDIR /app

# Copy published output from the previous stage
COPY --from=publish /app/publish .

# Set the container's entry point to run your .NET app
ENTRYPOINT ["dotnet", "quanlybanthuoc.dll"]
