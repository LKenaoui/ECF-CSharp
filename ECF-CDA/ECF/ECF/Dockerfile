# Étape 1 : build du projet
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copie des fichiers projet
COPY ECF.csproj ./
COPY TestECF/TestECF.csproj ./TestECF/
RUN dotnet restore ECF.csproj

# Copie du reste
COPY . .

# Build et publish
RUN dotnet publish ECF.csproj -c Release -o /app/publish

# Étape 2 : image finale avec ASP.NET Core
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Port exposé
EXPOSE 80

# Démarrage
ENTRYPOINT ["dotnet", "ECF.dll"]
