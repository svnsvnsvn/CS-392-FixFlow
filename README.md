This project uses a PostgresDB, a MongoDB (CE) and ASP.Net web application behind an NGINX reverse proxy to provide a webhosted service to track trouble tickets for an apartment complex.
The option to use adminer for DB access is included in the docker-compose.yaml, but is NOT required for proper operation.

Designed to be deployed using Docker, to install take the following steps

1. Create a FixFlow folder and download this repository to that folder.  It should have "nginx" and "fixflow.web" folders as well as the docker-compose.yaml

2. In the FixFlow.web folder you should have this Dockerfile to build from source:

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish ./fixflow.web.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "fixflow.web.dll"]
```


3. Switch back to main FixFlow folder and verify your docker-compose.yaml.  Recommended is below:

```YAML
services:
  nginx-proxy:
    image: nginx:latest
    container_name: fixflow-reverse-proxy
    restart: always
    volumes:
      - ./nginx/nginx.conf:/etc/nginx/nginx.conf:ro
    ports:
      - "80:80"
    networks:
      public_face:
      backend:

  db:
    image: postgres:16
    container_name: fixflow-db
    restart: always
    environment:
      POSTGRES_DB: fixflow
      POSTGRES_USER: fixflow
      POSTGRES_PASSWORD: fixflow_local_dev
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U fixflow -d fixflow"]
      interval: 5s
      timeout: 5s
      retries: 12
    volumes:
      - fixflow-db-data:/var/lib/postgresql/data
    networks:
      - backend

  web:
    build: ./fixflow.web
    container_name: fixflow-web
    restart: always
    environment:
      ConnectionStrings__Postgres: "Host=db;Port=5432;Database=fixflow;Username=fixflow;Password=fixflow_local_dev"
    depends_on:
      db:
        condition: service_healthy
    networks:
      - backend

  adminer:
    image: adminer
    container_name: fixflow-adminer
    restart: always
    ports:
        - "8080:8080"
    networks:
      - backend

networks:
  backend:
    driver: bridge
  public_face:
    driver: bridge

volumes:
  fixflow-db-data:
```
You can specify an IP address, but ensure you use the nginx-proxy container as the destination.

4. Run the following commands from the PSTS folder:
````Bash
docker compose build
docker compose up -d
````
5. Navigate to ASSIGNED-IP:80 for access
