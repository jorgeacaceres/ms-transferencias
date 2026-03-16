# ms-transferencias

Microservicio .NET 8 para gestión de transferencias con API REST, base de datos PostgreSQL y mensajería Kafka.

## 1. Tipo de proyecto

- .NET 8 (Target framework `net8.0`)
- API Minimal con `WebApplication`.
- Arquitectura basada en capas:
  - `Endpoints`: definición de rutas y resultados HTTP.
  - `Application`: lógica de negocio, comandos, queries, validaciones, mapeo, comportamientos.
  - `Infrastructure`: persistencia, entidades, repositorios, mensajería e integración.

## 2. Tecnologías principales y dependencias

- C# 12 / .NET 8
- Entity Framework Core con Npgsql (PostgreSQL)
- DotNetCore.CAP (integración con Kafka y persistencia EF)
- MediatR (CQRS estilo Command/Query)
- FluentValidation
- Serilog (Console + Seq)
- Mapeador `Riok.Mapperly` para mapeo de DTO <-> Entidad
- OneOf (resultados tipo `OneOf<T1, T2>`)

## 3. Conexiones y configuración

### 3.1 base de datos

`appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=postgres;Username=postgres;Password=postgres;Include Error Detail=true;"
}
```

- `ApplicationDbContext` usa Npgsql y la tabla `transfers`.

### 3.2 Kafka + CAP

`appsettings.json`:

```json
"Configuration": {
  "KafkaBootstrapServers": "localhost:9092",
  "TopicRiskEvaluationRequest": "risk-evaluation-request",
  "TopicRiskEvaluationResponse": "risk-evaluation-response"
}
```

- En `Infrastructure.DependencyInjection`: `services.AddCap(x => { x.UseEntityFramework<ApplicationDbContext>(); x.UseKafka("localhost:9092"); });`
- Publica en tópico `risk-evaluation-request` cuando se crea transfer.
- Escucha `risk-evaluation-response` para actualizar estado.

### 3.3 Logging

- Serilog configurado con consola y Seq.

## 4. Arquitectura / flujo

1. `POST /api/payments` crea transferencia:
   - Comando `CreateTransferCommand`
   - Repositorio `ITransferRepository.AddTransferAsync`
   - Publicar evento de riesgo (`PublishMessageCommand` -> `MessagePublisher` (CAP -> Kafka)).
2. Kafka procesa riesgo externo y envía `risk-evaluation-response`.
3. `RiskResultListener` recibe evento y lanza `UpdateTransferCommand` para actualizar estado.
4. `GET /api/payments/{externalOperationId}` consulta el estado:
   - Query `GetTransferByIdQuery`
   - Retorna 200 con info o 204 si no existe.

## 5. Endpoints

- `GET /api/payments/{externalOperationId}`
  - Respuesta 200: `GetTransferByIdResponse`
  - Respuesta 204: no encontrado
  - 400/500: `ErrorResponse`

- `POST /api/payments`
  - Body: `CreateTransferCommand` (según DTOs en carpeta `Application/Commands/CreateTransfer`)
  - Respuesta 201: `CreateTransferResponse`
  - 400/500: `ErrorResponse`

> Nota: el proyecto expone Swagger UI automáticamente (`/swagger`).

## 6. Entidad principal

Tabla `transfers` (clase `Infrastructure.Entities.Transfer`):

- `ExternalOperationId` (GUID, PK)
- `CustomerId` (string)
- `ServiceProviderId` (string)
- `PaymentMethodId` (int)
- `Amount` (decimal)
- `Status` (string, default `evaluating`)
- `CreatedAt` (DateTime)
- `UpdatedAt` (DateTime?)

## 7. Requisitos de instalación

- .NET 8 SDK
- PostgreSQL
- Kafka y Zookeeper según versión
- Seq

## 8. Ejecución local

1. Clonar repo.
2. Ajustar `appsettings.json` conexiones (Postgres/Kafka/Seq).
3. Ejecutar:
   - `dotnet run`
4. Visitar Swagger:
   - `https://localhost:5001/swagger`

## 9. Validaciones y control de errores

- `CreateTransferValidator` revisa campos obligatorios (título, montos, etc.).
- `ValidationBehavior` aplica validación MediatR antes de ejecutar handlers.
- `GlobalExceptionHandler` maneja excepciones no controladas.

## 10. Base de datos

```
CREATE TABLE IF NOT EXISTS transfers (
    external_operation_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    customer_id VARCHAR(60) NOT NULL,
    service_provider_id VARCHAR(60) NOT NULL,
    payment_method_id NUMERIC(4) NOT NULL,
    amount NUMERIC(18, 2) NOT NULL DEFAULT 0.00,
    status VARCHAR(20) NOT NULL DEFAULT 'Pending',
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ
);
CREATE INDEX idx_transfers_customer_id ON transfers(customer_id);
COMMENT ON TABLE transfers IS 'Tabla referente a transferencias';
```

## 11. Docker

Para despliegue en docker mediante docker-compose.yaml

```
  ms-transferencias:
    build:
      context: ./ms-transferencias
      dockerfile: Dockerfile
    image: ms-transferencias-api:latest
    container_name: ms-transferencias
    ports:
      - "5000:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=postgres;Username=postgres;Password=postgres;Include Error Detail=true;
      - Configuration__KafkaBootstrapServers=kafka:29092
      - Serilog__WriteTo__1__Args__serverUrl=http://seq:80
    depends_on:
      - postgres
      - kafka
      - seq
```

## 11. Notas

- Se crearon dos proyectos “ms-transferencias” y “ms-validacion-riesgo” ambos estan en .NET Core 8.
- Cada proyecto tiene su propio readme con toda la documentación correspondiente.
- Se adjunta el archivo script_base_de_datos para la creación de la base y tabla.
- Los links de los repositorios son los sigioentes:
  - ms-transferencias: https://github.com/jorgeacaceres/ms-transferencias
  - ms-validacion-riesgo: https://github.com/jorgeacaceres/ms-validacion-riesgo
- Se actualizo el docker compose con los siguientes servicios:
  - Postgres -> se mantiene el original.
  - Zookeeper -> se mantiene el original.
  - Kafka -> se mantiene el original.
  - kafka-ui -> nuevo servicio, se utilizó para monitorear los tópicos y mensajes.
  - Seq -> nuevo servicio, se utilizó para poder monitorear los logs de las aplicaciones desplegadas.
  - ms-transferencias -> nuevo servicio, corresponde a la gestión de las transferencias, cuenta con dos - endpoints, para crear la solicitud y el otro para poder consultar dicha solicitud.
  - ms-validacion-riesgo -> nuevo servicio, corresponde al worker/listener que evaluda el riesgo para poder aceptar o rechazar la transferencia.
- Los nuevos servicios agregados al docker compose, cuentan con sus respectivos parámetros.
- El ms-transferencias cuenta con documentación https://localhost:5000/swagger
- Ejecutar el comando docker-compose up -d en general o por separado mediante docker-compose up -d --build ms-validacion-riesgo para hacerlo uno por uno según necesidad.

### Docker general

```
version: "3.7"
services:
  postgres:
    image: postgres:14
    container_name: postgres-1
    ports:
      - "5432:5432"
    environment:
      - POSTGRES_USER=postgres
      - POSTGRES_PASSWORD=postgres

  zookeeper:
    image: confluentinc/cp-zookeeper:5.5.3
    container_name: zookeeper-1
    environment:
      ZOOKEEPER_CLIENT_PORT: 2181

  kafka:
    image: confluentinc/cp-enterprise-kafka:5.5.3
    container_name: kafka-1
    depends_on: [zookeeper]
    environment:
      KAFKA_ZOOKEEPER_CONNECT: "zookeeper:2181"
      KAFKA_ADVERTISED_LISTENERS: PLAINTEXT://kafka:29092,PLAINTEXT_HOST://localhost:9092
      KAFKA_LISTENER_SECURITY_PROTOCOL_MAP: PLAINTEXT:PLAINTEXT,PLAINTEXT_HOST:PLAINTEXT
      KAFKA_BROKER_ID: 1
      KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR: 1
    ports:
      - 9092:9092

  kafka-ui:
    image: provectuslabs/kafka-ui:latest
    container_name: kafka_ui
    ports:
      - "8080:8080"
    environment:
      KAFKA_CLUSTERS_0_NAME: local-cluster
      KAFKA_CLUSTERS_0_BOOTSTRAPSERVERS: kafka:29092
    depends_on:
      - kafka

  seq:
    image: datalust/seq:latest
    container_name: seq_logs
    ports:
      - "8081:80"
      - "5341:5341"
    environment:
      - ACCEPT_EULA=Y
      - SEQ_FIRSTRUN_ADMINPASSWORD=a.123456
  ms-transferencias:
    build:
      context: ./ms-transferencias
      dockerfile: Dockerfile
    image: ms-transferencias-api:latest
    container_name: ms-transferencias
    ports:
      - "5000:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=postgres;Username=postgres;Password=postgres;Include Error Detail=true;
      - Configuration__KafkaBootstrapServers=kafka:29092
      - Serilog__WriteTo__1__Args__serverUrl=http://seq:80
    depends_on:
      - postgres
      - kafka
      - seq
  ms-validacion-riesgo:
    build:
      context: ./ms-validacion-riesgo
      dockerfile: Dockerfile
    image: ms-validacion-riesgo-api:latest
    container_name: ms-validacion-riesgo
    ports:
      - "5001:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=postgres;Username=postgres;Password=postgres;Include Error Detail=true
      - Configuration__KafkaBootstrapServers=kafka:29092
      - Configuration__TopicRiskEvaluationResponse=risk-evaluation-response
      - Configuration__BaseAmount=2000
      - Configuration__AccumulatedAmount=5000
      - Serilog__WriteTo__1__Args__serverUrl=http://seq:80
    depends_on:
      - postgres
      - kafka
      - seq
```
