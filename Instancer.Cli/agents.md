# agents.md – Instancer.CLI

## Description

`Instancer.CLI` est une application console multiplateforme développée en .NET. Elle permet à l’utilisateur de déployer une stack distante depuis son poste local à partir d’un fichier `docker-compose.yml`, en dialoguant avec `Instancer.Server` via HTTP.

## Interfaces exposées

- **Commande CLI :**
  - `instancer up` – envoie un template vers le serveur
  - `instancer down` – arrête la stack associée
  - `instancer status` – affiche les ports actifs
  - `instancer proxy` – démarre le proxy local vers les services distants

## Intégration Cortex

- **Logs locaux** disponibles dans le terminal ou fichier `~/.instancer/logs/cli.log`
- **Traces HTTP** (vers Instancer.Server) activables via une option `--verbose` ou `--telemetry`
- **Fichier `.instancer.lock.json`** pour exposer les redirections actives

## Observabilité recommandée

- Ajout optionnel de tracing via `HttpClientFactory` + OpenTelemetry
- Stockage local des logs utilisables par des agents Filebeat

## Testing Avant chaque commit

- il faut vérifier qu'il n'y ai pas de warning ou d'erreur dans le build + intellisence 
- Lancer les tests unitaires et s'assurer de maximiser au maximum la couverture de code sur chaque critère de test (lignes, branches, fonctions)
- Vérifier que le build passe bien