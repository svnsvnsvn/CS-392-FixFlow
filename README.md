This project uses a PostgresDB, a MongoDB (CE) and ASP.Net web application behind an NGINX reverse proxy to provide a webhosted service to track trouble tickets for an apartment complex.


Designed to be deployed using Docker, to install take the following steps

1. Create a FixFlow folder and download this repository to that folder.  It should have "nginx" and "fixflow.web" folders as well as the docker-compose.yaml

2. In the nginx folder  you should have this nginx.conf file:

```nginx.conf
events {}

http {
	server {
	listen 80;
	server_name _;
		location / {
			proxy_pass http://fixflow-web:8080/;
			proxy_set_header Host $host;
			proxy_set_header X-Real-IP $remote_addr;
		}
	}
}
```

3. In the FixFlow.web folder you should have this Dockerfile to build from source:

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


3. Return to main FixFlow folder and verify your docker-compose.yaml.  Recommended is below:

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
      - public_face
      - backend

  fixflow-db-postgres:
    image: postgres:16
    container_name: fixflow-postgres
    restart: always
    environment:
      POSTGRES_DB: fixflow
      POSTGRES_USER: fixflow
      POSTGRES_PASSWORD: securePassword
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U fixflow -d fixflow"]
      interval: 5s
      timeout: 5s
      retries: 12
    volumes:
      - fixflow-postgres-db-data:/var/lib/postgresql/data
    networks:
      - backend

  fixflow-db-mongo:
    image: mongo:7
    container_name: fixflow-mongo
    restart: always
    environment:
      MONGO_INITDB_ROOT_USERNAME: fixflow
      MONGO_INITDB_ROOT_PASSWORD: securePassword
      MONGO_INITDB_DATABASE: fixflow_notes
    healthcheck:
      test: ["CMD", "mongosh", "--username", "fixflow", "--password", "securePassword", "--authenticationDatabase", "admin", "--eval", "db.adminCommand('ping')"]
      interval: 5s
      timeout: 5s
      retries: 10
    volumes:
      - fixflow-mongo-db-data:/data/db
    networks:
      - backend

  fixflow-web:
    build: ./fixflow.web
    container_name: fixflow-web
    restart: always
    environment:
      ConnectionStrings__Postgres: "Host=fixflow-db-postgres;Database=fixflow;Username=fixflow;Password=securePassword"
      ConnectionStrings__Mongo: "mongodb://fixflow:securePassword@fixflow-db-mongo:27017/fixflow_notes?authSource=admin"
    depends_on:
      fixflow-db-postgres:
        condition: service_healthy
      fixflow-db-mongo:
        condition: service_healthy
    networks:
      - backend

networks:
  backend:
    driver: bridge
  public_face:
    driver: bridge

volumes:
  fixflow-postgres-db-data:
  fixflow-mongo-db-data:
```
You can specify an IP address, but ensure you use the nginx-proxy container as the destination.

4. Run the following commands from the PSTS folder:
````Bash
docker compose build --no-cache
docker compose up -d
````
5. Navigate to ASSIGNED-IP:80 for access
