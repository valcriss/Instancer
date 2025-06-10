# Instancer

Instancer est une plateforme permettant de déployer dynamiquement des stacks Docker à distance. Le monorepo se compose de trois projets .NET 8 :

- **Instancer.Server** : API ASP.NET Core gérant l’orchestration des stacks et la persistance.
- **Instancer.Cli** : client en ligne de commande pour interagir avec le serveur.
- **Instancer.Daemon** : service local responsable des proxys TCP vers les services distants.

## Prérequis

- [Docker](https://www.docker.com/) pour exécuter la base de données et construire l’image du serveur.
- [.NET 8 SDK](https://dotnet.microsoft.com/) pour compiler et exécuter les projets.

## Lancer les composants en développement

1. Démarrer la base PostgreSQL nécessaire au serveur :
   ```bash
   docker compose -f Instancer.Server/docker-compose.dev.yml up -d
   ```
2. Lancer l’API :
   ```bash
   dotnet run --project Instancer.Server
   ```
3. Démarrer le daemon local (pour les redirections de ports) :
   ```bash
   dotnet run --project Instancer.Daemon
   ```
4. Utiliser le client CLI pour déployer ou gérer les stacks :
   ```bash
   dotnet run --project Instancer.Cli -- <commande>
   ```
   Les commandes disponibles sont `up`, `down`, `status` et `proxy`.

## Construction de l’image Docker du serveur

Une image Docker peut être générée à partir du fichier `Instancer.Server/Dockerfile` :

```bash
docker build -t instancer-server -f Instancer.Server/Dockerfile .
```

L’image résultante embarque l’API ASP.NET Core prête à être déployée sur votre plateforme de conteneurisation.

