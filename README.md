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

## 10. Docker

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
