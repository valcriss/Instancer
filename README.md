# Instancer

Instancer is a platform that allows you to remotely deploy Docker stacks dynamically. The monorepo is composed of three .NET 8 projects:

- **Instancer.Server**: ASP.NET Core API that orchestrates stacks and persistence.
- **Instancer.Cli**: command line client used to interact with the server.
- **Instancer.Daemon**: local service responsible for TCP proxies to the remote services.

## Prerequisites

- [Docker](https://www.docker.com/) to run the database and build the server image.
- [.NET 8 SDK](https://dotnet.microsoft.com/) to build and run the projects.

## Running the components in development

1. Start the PostgreSQL database required by the server:
   ```bash
   docker compose -f Instancer.Server/docker-compose.dev.yml up -d
   ```
2. Launch the API:
   ```bash
   dotnet run --project Instancer.Server
   ```
3. Start the local daemon (for port forwarding):
   ```bash
   dotnet run --project Instancer.Daemon
   ```
4. Use the CLI client to deploy or manage stacks:
   ```bash
   dotnet run --project Instancer.Cli -- <command>
   ```
   The available commands are `up`, `down`, `status` and `proxy`.

## Building the server Docker image

A Docker image can be created using `Instancer.Server/Dockerfile`:

```bash
docker build -t instancer-server -f Instancer.Server/Dockerfile .
```

The resulting image contains the ASP.NET Core API ready to be deployed on your container platform.
