# agents.md – Monorepo Instancer

## Présentation

Instancer est une plateforme Dev Environment-as-a-Service permettant aux développeurs de lancer dynamiquement des stacks `docker-compose` sur un serveur distant, sans avoir besoin de Docker local ou de droits administrateur. Le projet est organisé en plusieurs composants distincts, chacun exposant ses propres interfaces et points d’intégration.

---

## 📦 Projets du monorepo

### 1. `Instancer.Server`

**Type** : API Web ASP.NET Core  
**Rôle** : Orchestration centrale des stacks sur Docker/Swarm/Kubernetes

#### Interfaces exposées

- API REST HTTP :
  - `POST /api/stacks` : déployer une stack
  - `GET /api/stacks` : liste des stacks
  - `DELETE /api/stacks/{id}` : arrêt d’une stack
  - `GET /api/templates` : templates disponibles
- Health check : `GET /health`
- (optionnel) `/metrics` pour Prometheus

#### Observabilité

- Tracing HTTP via OpenTelemetry (optionnel)
- Logs applicatifs via Serilog (JSON)
- Métriques Prometheus (ex: nombre de stacks actives, erreurs, latence)
- Health/Liveness pour déploiement en cluster (K8s, Swarm)

---

### 2. `Instancer.CLI`

**Type** : Application console multiplateforme .NET  
**Rôle** : Outil utilisateur pour déployer, arrêter, inspecter les stacks

#### Interfaces exposées

- Commandes :
  - `instancer up` : envoie un `docker-compose.yml`
  - `instancer down` : supprime une stack
  - `instancer proxy` : démarre un tunnel TCP
  - `instancer status` : affiche les redirections actives

#### Observabilité

- Logs locaux (stdout et fichier dans `~/.instancer/logs/cli.log`)
- Possibilité d’ajouter un mode `--verbose` pour tracer les appels HTTP
- Trace des proxys actifs via `~/.instancer/proxy.lock.json`

---

### 3. `Instancer.Daemon` (a.k.a. `instancerd`)

**Type** : Service local (Console App .NET)  
**Rôle** : Proxy TCP entre `localhost` et les containers distants

#### Interfaces exposées

- API HTTP locale (`http://localhost:5151`) :
  - `POST /start-proxy` : lance un proxy TCP
  - `POST /stop-proxy` : arrête un proxy
  - `GET /status` : liste les proxys actifs
- (optionnel) `/metrics` pour monitoring local (nombre de tunnels actifs)

#### Observabilité

- Logs dans `~/.instancer/logs/daemon.log`
- Lockfile `~/.instancer/proxy.lock.json` pour supervision par outil tiers
- Support possible de Prometheus en local (port 5152)

---

## 🔌 Intégration Cortex & outils tiers

| Source         | Type     | Recommandé |
|----------------|----------|------------|
| Instancer.Server | Traces HTTP, logs Serilog, Prometheus | ✅ |
| Instancer.CLI    | Logs + traces HTTP optionnels         | ⚠️ |
| Instancer.Daemon | Proxy state JSON + HTTP status        | ✅ |

## 🧩 Notes

- Le CLI et le Daemon peuvent fonctionner indépendamment du serveur (mode déconnecté à venir)
- Une architecture multi-tenant et multi-utilisateur est prévue via des modules entreprise séparés

---

## 📁 Localisation des fichiers spécifiques

| Projet              | agents.md |
|---------------------|-----------|
| Instancer.Server    | `/Instancer.Server/agents.md` |
| Instancer.CLI       | `/Instancer.Cli/agents.md` |
| Instancer.Daemon    | `/Instancer.Daemon/agents.md` |
| Global (ce fichier) | `/agents.md` |

## Testing Avant chaque commit

- il faut vérifier qu'il n'y ai pas de warning ou d'erreur dans le build + intellisence 
- Lancer les tests unitaires et s'assurer que la couverture de code est a 100% sur chaque critère de test (lignes, branches, fonctions)
- Vérifier que le build passe bien
- Si un Dockerfile est présent a la racine, vérifier que le build de l'image fonctionne correctement