# agents.md – Instancer.Daemon

## Description

`Instancer.Daemon` est un processus léger tournant localement sur la machine utilisateur. Il agit comme un proxy TCP vers les ports exposés à distance sur les containers pilotés par `Instancer.Server`.

## Interfaces exposées

- **API HTTP locale (par défaut `http://localhost:5151`)**
  - `POST /start-proxy` – démarre un tunnel TCP (ex: `localhost:3306`)
  - `POST /stop-proxy` – coupe un tunnel
  - `GET /status` – retourne la liste des proxys actifs

## Intégration Cortex

- **Événements de proxy TCP** (start/stop) loggés dans `~/.instancer/logs/daemon.log`
- **Traces internes** (temps de démarrage de tunnel, nombre de connexions, erreurs)
- Option d’exposition de `/metrics` en local pour Prometheus (ex: nombre de proxys actifs)

## Observabilité recommandée

- Logging structuré (JSON) pour ingestion par un agent local (Filebeat, Fluentbit)
- Metrics Prometheus optionnelles sur port `localhost:5152`
- Fichier `proxy.lock.json` servant de cache pour visualiser l’état des tunnels

## Testing Avant chaque commit

- il faut vérifier qu'il n'y ai pas de warning ou d'erreur dans le build + intellisence 
- Lancer les tests unitaires et s'assurer que la couverture de code est a 100% sur chaque critère de test (lignes, branches, fonctions)
- Vérifier que le build passe bien