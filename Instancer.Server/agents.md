# agents.md – Instancer.Server

## Description

`Instancer.Server` est une API REST développée avec ASP.NET Core qui orchestre l’instanciation de stacks Docker/Swarm/Kubernetes à partir de templates `docker-compose.yml`. Il gère les utilisateurs, les templates, le suivi des stacks, l’allocation dynamique de ports, et la persistance des données.

## Interfaces exposées

- **API HTTP REST**
  - `POST /api/stacks` – déploie une stack à partir d’un template
  - `GET /api/stacks` – liste les stacks actives
  - `DELETE /api/stacks/{id}` – arrête et supprime une stack
  - `GET /api/templates` – liste les templates disponibles
  - `GET /api/status` – état général du serveur

## Intégration Cortex

- **Traçabilité API** : chaque appel REST peut être observé via OpenTelemetry (ou autre middleware HTTP).
- **Journalisation** : tous les événements systèmes sont loggés via Serilog (format JSON).
- **Health check** : exposé via `/health` (HTTP 200 si service actif).
- **Exports Prometheus (optionnel)** via middleware custom (`/metrics`).

## Observabilité recommandée

- Activation de OpenTelemetry pour traces des requêtes HTTP
- Serilog sink JSON + Fluentd ou Loki
- Endpoint `/health` utilisé pour le liveness probe dans K8s

## Testing Avant chaque commit

- il faut vérifier qu'il n'y ai pas de warning ou d'erreur dans le build + intellisence 
- Lancer les tests unitaires et s'assurer que la couverture de code est a 100% sur chaque critère de test (lignes, branches, fonctions)
- Vérifier que le build passe bien
- Si un Dockerfile est présent a la racine, vérifier que le build de l'image fonctionne correctement