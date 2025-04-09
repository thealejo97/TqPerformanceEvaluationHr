# TQ Performance Evaluation HR

Solución tecnológica para el proceso de evaluación de desempeño de Recursos Humanos, basada en modelos evaluativos (90°, 180°, 360°).

---

## Arquitectura del Proyecto

Este proyecto está desarrollado con **.NET 8**, utilizando **Clean Architecture**, **Razor Pages** como frontend y **PostgreSQL** como base de datos. La estructura sigue buenas prácticas y separación por capas:
```
src/
├── TqPerformanceEvaluationHr.Web           → Capa de presentación (Razor Pages)
├── TqPerformanceEvaluationHr.Application   → Lógica de negocio (casos de uso, DTOs)
├── TqPerformanceEvaluationHr.Domain        → Entidades y reglas del dominio
├── TqPerformanceEvaluationHr.Infrastructure→ Persistencia, EF Core, PostgreSQL
```

## Tecnologías y herramientas

- ASP.NET Core 8 (WebApp con Razor Pages)
- Entity Framework Core
- PostgreSQL
- Docker + Docker Compose
- Clean Architecture

---

## Cómo ejecutar la solución con Docker

### Clona el repositorio y ejecutar la solución:

```
git clone [<repo>](https://github.com/thealejo97/TqPerformanceEvaluationHr.git)
docker compose up --build
```
## Como ejecutar migraciones

Cree un servicio que hace las migraciones en la db con docker, para ejecutarlo, despues de ejecutar el up se debe ejecutar
```
 docker compose up --build migrator
```

Despues de esto la aplicacion deberia estar disponible en:
http://localhost:5000

### Configuración de conexión a base de datos
La cadena de conexión se encuentra en appsettings.Development.json dentro del proyecto Web:

```
"ConnectionStrings": {
  "DefaultConnection": "Host=db;Database=tq_performance;Username=postgres;Password=tq123"
}
```


## Autor

Alejandro Montaño Quintero
Ingeniero de Soluciones - Prueba Técnica TQ