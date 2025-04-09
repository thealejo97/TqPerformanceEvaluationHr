# TQ Performance Evaluation HR

Solución tecnológica para el proceso de evaluación de desempeño de Recursos Humanos, basada en modelos evaluativos (90°, 180°, 360°).

---

## Arquitectura del Proyecto

Este proyecto está desarrollado con **.NET 8**, utilizando **Clean Architecture**, **Razor Pages** como frontend y **PostgreSQL** como base de datos. La estructura sigue buenas prácticas y separación por capas siguiendo principios de Clean Architecture:
```
src/
├── TqPerformanceEvaluationHr.Web           → Capa de presentación (Razor Pages)
├── TqPerformanceEvaluationHr.Application   → Lógica de negocio
├── TqPerformanceEvaluationHr.Domain        → Entidades y reglas del dominio
├── TqPerformanceEvaluationHr.Infrastructure→ Persistencia, EF Core, PostgreSQL
```

Cada capa cumple con su rol específico:

- **Domain**: contiene las entidades puras y relaciones, sin dependencias externas.
- **Application**: orquesta la lógica, incluye DTOs, validaciones y servicios de aplicación.
- **Infrastructure**: gestiona la persistencia con Entity Framework Core y la conexión a PostgreSQL.
- **Web**: interfaz de usuario basada en Razor Pages, donde el usuario interactúa con el sistema.


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
git clone https://github.com/thealejo97/TqPerformanceEvaluationHr.git
docker compose up --build
```
## Como ejecutar migraciones

Cree un servicio en docker que hace las migraciones en la db con docker, para ejecutarlo, despues de ejecutar el up se debe ejecutar
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


### Funcionalidades

La aplicación cuenta con CRUD completo para las siguientes entidades:

* Evaluation Models

* Questionnaires

* Evaluations

* Positions

* Employees

Cada sección tiene su propia interfaz en Razor Pages para crear, editar, eliminar y listar los registros.

Adicionalmente cuenta con una sección dedicada a reportes, donde se muestra la información agregada por cargo, permitiendo visualizar:

Esto permite medir el desempeño promedio por cargo y tomar decisiones basadas en datos.

## Autor

Alejandro Montaño Quintero
Ingeniero de Soluciones - Prueba Técnica TQ