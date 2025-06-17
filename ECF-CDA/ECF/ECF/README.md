# Refuge Animalier

Application de gestion d'un refuge animalier développée en ASP.NET Core.

## Prérequis

- Docker Desktop
- Git

## Installation et Lancement

1. Cloner le projet :
```bash
git clone [URL_DU_REPO]
cd ECF
```

2. Lancer l'application avec Docker :
```bash
docker-compose up --build
```

3. Accéder à l'application :
- Interface web : http://localhost:8080
- Base de données SQL Server : localhost,1433
  - Utilisateur : sa
  - Mot de passe : Your_password123

## Fonctionnalités

- Gestion des animaux
- Gestion des races
- Recherche d'animaux
- Tests unitaires

## Tests

Pour exécuter les tests :
```bash
docker-compose run tests
``` 