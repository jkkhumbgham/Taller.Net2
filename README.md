# Sistema de Estadísticas de Estudiantes — .NET 10

Sistema que genera estadísticas de estudiantes sobre los cursos que toman y han tomado en una plataforma de aprendizaje. Implementado en tres arquitecturas independientes: **Monolítica**, **Servicios** y **MicroServicios**.

---

## Índice

1. [Prerequisitos](#prerequisitos)
2. [Bases de datos](#bases-de-datos)
3. [Arquitectura Monolítica](#arquitectura-monolítica)
4. [Arquitectura de Servicios](#arquitectura-de-servicios)
5. [Arquitectura de MicroServicios](#arquitectura-de-microservicios)
6. [Guía de comandos](#guía-de-comandos)

---

## Prerequisitos

| Herramienta | Versión | Verificar |
|---|---|---|
| .NET SDK | 10.0 | `dotnet --version` |
| Docker | 24+ | `docker --version` |
| Docker Compose | v2+ | `docker compose version` |
| PostgreSQL (vía Docker) | 16 | Incluido en este proyecto |

Las bases de datos se levantan automáticamente junto con las aplicaciones al usar Docker Compose. No se requiere ningún proyecto externo.

---

## Bases de datos

El sistema incluye sus propias bases de datos PostgreSQL definidas en `db/`. Los scripts de inicialización crean el esquema y cargan datos de prueba automáticamente al levantar los contenedores por primera vez.

| Base de datos | Puerto host | Usuario | Contraseña | Contenido | Scripts |
|---|---|---|---|---|---|
| `content_db` | `5432` | `content_user` | `content_pass` | Cursos, módulos, lecciones, cuestionarios | `db/content/` |
| `user_db` | `5433` | `user_user` | `user_pass` | Usuarios, inscripciones, progresos, intentos | `db/user/` |

### Tablas relevantes

**user_db**
```
users            → id, name, email, created_at
enrollments      → id, user_id, course_id, enrolled_at, progress (0–100)
lesson_progress  → id, user_id, lesson_id, status, progress_percent, time_spent, completed_at
quiz_attempts    → id, user_id, quiz_id, score, max_score, attempt_number, time_spent, attempted_at
question_attempts→ id, quiz_attempt_id, question_id, is_correct, time_spent
```

**content_db**
```
courses   → id, title, description, level, language, is_published
modules   → id, course_id, title, position
lessons   → id, module_id, title, duration
contents  → id, lesson_id, type, position, purpose
quizzes   → id, content_id, title
questions → id, quiz_id, question_text, type
```

---

## Arquitectura Monolítica

Un único proceso, una única aplicación. Las capas **Datos** y **Lógica** se separan por carpetas y namespaces dentro del mismo proyecto.

### Diagrama

```
┌─────────────────────────────────────────────────────────┐
│                    MONOLÍTICA  :5001                    │
│                                                         │
│  ┌──────────────────────────────────────────────────┐   │
│  │  Controladores/                                  │   │
│  │    EstadisticasController                        │   │
│  └───────────────────────┬──────────────────────────┘   │
│                          │ IServicioEstadisticas         │
│  ┌───────────────────────▼──────────────────────────┐   │
│  │  Logica/                                         │   │
│  │    ServicioEstadisticas                          │   │
│  └──────┬────────────────┬──────────────────────────┘   │
│         │                │ Interfaces repositorios       │
│  ┌──────▼────────────────▼──────────────────────────┐   │
│  │  Datos/                                          │   │
│  │    RepositorioInscripciones                      │   │
│  │    RepositorioProgresoLecciones                  │   │
│  │    RepositorioIntentosCuestionarios              │   │
│  └──────┬────────────────┬──────────────────────────┘   │
│         │                │  EF Core                      │
│  ┌──────▼────┐    ┌───────▼────┐                        │
│  │ UserDb    │    │ ContentDb  │                        │
│  │ :5433     │    │ :5432      │                        │
│  └───────────┘    └────────────┘                        │
└─────────────────────────────────────────────────────────┘
```

### Estructura de archivos

```
Monolitica/
├── Datos/                              ← Capa de Datos
│   ├── Contexto/
│   │   ├── UserDbContext.cs            — EF Core context para user_db
│   │   └── ContentDbContext.cs         — EF Core context para content_db
│   ├── Modelos/                        — Entidades EF Core (mapean tablas)
│   │   ├── Usuario.cs
│   │   ├── Inscripcion.cs
│   │   ├── ProgresoLeccion.cs
│   │   ├── IntentoCuestionario.cs
│   │   ├── IntentoRespuesta.cs
│   │   ├── Curso.cs
│   │   ├── Modulo.cs
│   │   ├── Leccion.cs
│   │   └── Cuestionario.cs
│   └── Repositorios/
│       ├── IRepositorioInscripciones.cs
│       ├── RepositorioInscripciones.cs
│       ├── IRepositorioProgresoLecciones.cs
│       ├── RepositorioProgresoLecciones.cs
│       ├── IRepositorioIntentosCuestionarios.cs
│       └── RepositorioIntentosCuestionarios.cs
├── Logica/                             ← Capa de Lógica
│   ├── DTOs/                           — Objetos de respuesta de la API
│   │   ├── ResumenEstudianteDto.cs
│   │   ├── EstadisticasCursoDto.cs
│   │   ├── EstadisticasDetalleCursoDto.cs
│   │   ├── EstadisticasCuestionarioDto.cs
│   │   ├── EstadisticasLeccionDto.cs
│   │   ├── NotaEstudianteDto.cs
│   │   ├── CursoAcabadoDto.cs
│   │   ├── ClaseMasTomadaDto.cs
│   │   └── MejorEstudianteDto.cs
│   └── Servicios/
│       ├── IServicioEstadisticas.cs
│       └── ServicioEstadisticas.cs
├── Controladores/
│   └── EstadisticasController.cs
├── Program.cs
├── appsettings.json
├── Dockerfile
└── docker-compose.yml
```

### Responsabilidades de cada clase

#### Capa de Datos

| Clase | Responsabilidad |
|---|---|
| `UserDbContext` | Contexto EF Core para `user_db`. Registra los DbSets de `Usuario`, `Inscripcion`, `ProgresoLeccion`, `IntentoCuestionario`, `IntentoRespuesta`. Aplica `UseSnakeCaseNamingConvention()`. |
| `ContentDbContext` | Contexto EF Core para `content_db`. Registra `Curso`, `Modulo`, `Leccion`, `Cuestionario`. Aplica `UseSnakeCaseNamingConvention()`. |
| `Usuario` | Entidad que mapea la tabla `users`. Campos: `Id`, `Name`, `Email`, `Password`, `CreatedAt`. Tiene navegación a inscripciones, progresos e intentos. |
| `Inscripcion` | Entidad que mapea `enrollments`. Campos: `Id`, `UserId`, `CourseId`, `EnrolledAt`, `Progress` (0–100). |
| `ProgresoLeccion` | Entidad que mapea `lesson_progress`. Campos: `Id`, `UserId`, `LessonId`, `Status` (NOT_STARTED/IN_PROGRESS/COMPLETED), `ProgressPercent`, `TimeSpent`, `CompletedAt`. |
| `IntentoCuestionario` | Entidad que mapea `quiz_attempts`. Campos: `Id`, `UserId`, `QuizId`, `Score`, `MaxScore`, `AttemptNumber`, `TimeSpent`, `AttemptedAt`. |
| `IntentoRespuesta` | Entidad que mapea `question_attempts`. Campos: `Id`, `QuizAttemptId`, `QuestionId`, `IsCorrect`, `TimeSpent`. |
| `Curso` | Entidad que mapea `courses`. Campos: `Id`, `Title`, `Level`, `Language`, `IsPublished`. Navegación a `Modulos`. |
| `Modulo` | Entidad que mapea `modules`. Campos: `Id`, `CourseId`, `Title`, `Position`. Navegación a `Lecciones`. |
| `Leccion` | Entidad que mapea `lessons`. Campos: `Id`, `ModuleId`, `Title`, `Duration`. |
| `Cuestionario` | Entidad que mapea `quizzes`. Campos: `Id`, `ContentId`, `Title`. |
| `IRepositorioInscripciones` | Contrato para acceso a datos de inscripciones y usuarios. Define: `ObtenerUsuarioPorId`, `ObtenerInscripcionesPorUsuario`, `ObtenerInscripcionPorUsuarioYCurso`, `ObtenerTodasLasInscripciones`. |
| `RepositorioInscripciones` | Implementación con EF Core. Inyecta `UserDbContext` y `ContentDbContext`. |
| `IRepositorioProgresoLecciones` | Contrato para progreso de lecciones y datos de contenido. Define: `ObtenerProgresosPorUsuario`, `ObtenerLeccionesPorCurso`, `ObtenerCursoPorId`, `ObtenerCursosPorIds`. |
| `RepositorioProgresoLecciones` | Implementación con EF Core. Inyecta ambos contextos. |
| `IRepositorioIntentosCuestionarios` | Contrato para intentos de cuestionarios. Define: `ObtenerIntentosPorUsuario`, `ObtenerCuestionariosPorIds`, `ObtenerTodosLosIntentos`, `ObtenerTodosLosUsuarios`. |
| `RepositorioIntentosCuestionarios` | Implementación con EF Core. Inyecta `UserDbContext` y `ContentDbContext`. |

#### Capa de Lógica

| Clase | Responsabilidad |
|---|---|
| `IServicioEstadisticas` | Contrato que define los 9 métodos de estadísticas. No depende de EF Core ni de contextos. |
| `ServicioEstadisticas` | Implementación de la lógica de negocio. Inyecta las 3 interfaces de repositorio. Agrega, calcula y transforma datos en DTOs. |
| `ResumenEstudianteDto` | Respuesta con: `IdEstudiante`, `NombreEstudiante`, `EmailEstudiante`, `TotalCursosInscritos`, `CursosActivos`, `CursosCompletados`, `TotalTiempoInvertido`, `TotalLeccionesCompletadas`, `PromedioCalificacionCuestionarios`. |
| `EstadisticasCursoDto` | Respuesta por curso: `IdCurso`, `TituloCurso`, `PorcentajeProgreso`, `LeccionesCompletadas`, `TotalLecciones`, `TiempoInvertido`, `FechaInscripcion`, `EstadoCurso`. |
| `NotaEstudianteDto` | Respuesta por intento: `IdIntento`, `IdCuestionario`, `TituloCuestionario`, `Calificacion`, `CalificacionMaxima`, `PorcentajeCalificacion`, `NumeroIntento`, `FechaIntento`. |
| `CursoAcabadoDto` | Respuesta de cursos completados: `IdCurso`, `TituloCurso`, `FechaInscripcion`, `FechaCompletado`, `TotalLecciones`, `DuracionTotalSegundos`. |
| `ClaseMasTomadaDto` | Respuesta de ranking de cursos: `IdCurso`, `TituloCurso`, `TotalInscritos`, `PorcentajeCompletados`. |
| `MejorEstudianteDto` | Respuesta de ranking de estudiantes: `IdEstudiante`, `NombreEstudiante`, `PromedioCalificacion`, `TotalIntentos`, `CursosCompletados`. |

#### Controlador

| Clase | Responsabilidad |
|---|---|
| `EstadisticasController` | Expone los 9 endpoints HTTP GET bajo `/api/stats`. Valida existencia del estudiante y retorna `404` si no existe. Delega toda lógica a `IServicioEstadisticas`. |

### Endpoints

```
GET /api/stats/students/{userId}                    → Resumen general del estudiante
GET /api/stats/students/{userId}/courses            → Estadísticas por curso
GET /api/stats/students/{userId}/courses/{courseId} → Detalle de un curso específico
GET /api/stats/students/{userId}/quizzes            → Rendimiento en cuestionarios
GET /api/stats/students/{userId}/lessons            → Progreso por lección
GET /api/stats/notas/estudiante/{userId}            → Lista de notas con porcentaje
GET /api/stats/cursos-acabados/estudiante/{userId}  → Cursos completados (progress = 100%)
GET /api/stats/clases-mas-tomadas                   → Cursos ordenados por inscritos
GET /api/stats/mejores-estudiantes                  → Ranking por promedio de calificaciones
```

---

## Arquitectura de Servicios

Tres servicios independientes y desplegables por separado. Cada servicio es una aplicación Web API completa con sus propias capas **Datos** y **Lógica** internas. No comparten código.

### Diagrama

```
┌─────────────────────────────────────────────────────────────────────────┐
│                        ARQUITECTURA DE SERVICIOS                        │
│                                                                         │
│  ┌──────────────────┐  ┌──────────────────┐  ┌──────────────────────┐  │
│  │  CUESTIONARIOS   │  │     CLASES       │  │    ESTADÍSTICAS      │  │
│  │     :5020        │  │     :5021        │  │       :5022          │  │
│  │                  │  │                  │  │                      │  │
│  │  Controladores/  │  │  Controladores/  │  │  Controladores/      │  │
│  │  └ Cuestionarios │  │  └ Clases        │  │  └ Estadisticas      │  │
│  │                  │  │                  │  │                      │  │
│  │  Logica/         │  │  Logica/         │  │  Logica/             │  │
│  │  └ ServicioCuest.│  │  └ ServicioClases│  │  └ ServicioEstadist. │  │
│  │                  │  │                  │  │                      │  │
│  │  Datos/          │  │  Datos/          │  │  Datos/              │  │
│  │  └ RepoCuest.    │  │  └ RepoClases    │  │  └ RepoEstadist.     │  │
│  └────────┬─────────┘  └────────┬─────────┘  └────────────┬─────────┘  │
│           │                     │                          │             │
│     ┌─────▼────┐  ┌─────────────▼──────────┐  ┌──────────▼──────────┐  │
│     │ user_db  │  │ user_db + content_db    │  │ user_db + content_db│  │
│     │ :5433    │  │ :5433 / :5432           │  │ :5433 / :5432       │  │
│     └──────────┘  └────────────────────────┘  └─────────────────────┘  │
└─────────────────────────────────────────────────────────────────────────┘
```

### Estructura de archivos

```
Servicios/
├── Servicios.sln
│
├── Cuestionarios/                      ← Servicio 1
│   ├── Datos/                          ← Capa de Datos (interna)
│   │   ├── Contexto/
│   │   │   ├── UserDbContext.cs
│   │   │   └── ContentDbContext.cs
│   │   ├── Modelos/
│   │   │   ├── Usuario.cs
│   │   │   ├── IntentoCuestionario.cs
│   │   │   ├── IntentoRespuesta.cs
│   │   │   └── Cuestionario.cs
│   │   └── Repositorios/
│   │       ├── IRepositorioCuestionarios.cs
│   │       └── RepositorioCuestionarios.cs
│   ├── Logica/                         ← Capa de Lógica (interna)
│   │   ├── DTOs/
│   │   │   ├── NotaEstudianteDto.cs
│   │   │   └── MejorEstudianteDto.cs
│   │   └── Servicios/
│   │       ├── IServicioCuestionarios.cs
│   │       └── ServicioCuestionarios.cs
│   ├── Controladores/
│   │   └── CuestionariosController.cs
│   ├── Program.cs
│   ├── appsettings.json
│   ├── Cuestionarios.csproj
│   └── Dockerfile
│
├── Clases/                             ← Servicio 2
│   ├── Datos/
│   │   ├── Contexto/
│   │   │   ├── UserDbContext.cs
│   │   │   └── ContentDbContext.cs
│   │   ├── Modelos/
│   │   │   ├── Usuario.cs
│   │   │   ├── Inscripcion.cs
│   │   │   ├── ProgresoLeccion.cs
│   │   │   ├── Curso.cs
│   │   │   ├── Modulo.cs
│   │   │   └── Leccion.cs
│   │   └── Repositorios/
│   │       ├── IRepositorioClases.cs
│   │       └── RepositorioClases.cs
│   ├── Logica/
│   │   ├── DTOs/
│   │   │   ├── CursoAcabadoDto.cs
│   │   │   └── ClaseMasTomadaDto.cs
│   │   └── Servicios/
│   │       ├── IServicioClases.cs
│   │       └── ServicioClases.cs
│   ├── Controladores/
│   │   └── ClasesController.cs
│   ├── Program.cs
│   ├── appsettings.json
│   ├── Clases.csproj
│   └── Dockerfile
│
├── Estadisticas/                       ← Servicio 3
│   ├── Datos/
│   │   ├── Contexto/
│   │   │   ├── UserDbContext.cs
│   │   │   └── ContentDbContext.cs
│   │   ├── Modelos/
│   │   │   ├── Usuario.cs, Inscripcion.cs
│   │   │   ├── ProgresoLeccion.cs, IntentoCuestionario.cs
│   │   │   ├── Curso.cs, Modulo.cs, Leccion.cs, Cuestionario.cs
│   │   └── Repositorios/
│   │       ├── IRepositorioEstadisticas.cs
│   │       └── RepositorioEstadisticas.cs
│   ├── Logica/
│   │   ├── DTOs/
│   │   │   ├── ResumenEstudianteDto.cs
│   │   │   ├── EstadisticasCursoDto.cs
│   │   │   └── EstadisticasLeccionDto.cs
│   │   └── Servicios/
│   │       ├── IServicioEstadisticas.cs
│   │       └── ServicioEstadisticas.cs
│   ├── Controladores/
│   │   └── EstadisticasController.cs
│   ├── Program.cs
│   ├── appsettings.json
│   ├── Estadisticas.csproj
│   └── Dockerfile
│
└── docker-compose.yml
```

### Responsabilidades de cada clase

#### Servicio Cuestionarios (puerto 5020)

| Clase | Responsabilidad |
|---|---|
| `UserDbContext` | Contexto EF Core para `user_db`. Expone `Usuario`, `IntentoCuestionario`, `IntentoRespuesta`. |
| `ContentDbContext` | Contexto EF Core para `content_db`. Expone solo `Cuestionario`. |
| `IRepositorioCuestionarios` | Contrato: `ObtenerUsuarioPorId`, `ObtenerTodosLosUsuarios`, `ObtenerIntentosPorUsuario`, `ObtenerTodosLosIntentos`, `ObtenerCuestionariosPorIds`. |
| `RepositorioCuestionarios` | Implementación con EF Core. Consulta intentos, agrupa por quiz, obtiene metadatos de cuestionarios del `ContentDbContext`. |
| `IServicioCuestionarios` | Contrato de lógica: `ObtenerNotasEstudianteAsync`, `ObtenerPromedioEstudianteAsync`, `ObtenerMejoresEstudiantesAsync`, `ObtenerTopMejoresEstudiantesAsync`. |
| `ServicioCuestionarios` | Calcula porcentajes de calificación (`score/maxScore*100`), agrupa intentos por cuestionario, ordena estudiantes por promedio descendente. |
| `NotaEstudianteDto` | Respuesta: `IdIntento`, `IdCuestionario`, `TituloCuestionario`, `Calificacion`, `CalificacionMaxima`, `PorcentajeCalificacion`, `NumeroIntento`, `FechaIntento`. |
| `MejorEstudianteDto` | Respuesta de ranking: `IdEstudiante`, `NombreEstudiante`, `PromedioCalificacion`, `TotalIntentos`, `CursosCompletados`. |
| `CuestionariosController` | Enruta `GET /api/cuestionarios/...`. Retorna `404` si el estudiante no existe. |

#### Servicio Clases (puerto 5021)

| Clase | Responsabilidad |
|---|---|
| `UserDbContext` | Contexto para `user_db`. Expone `Usuario`, `Inscripcion`, `ProgresoLeccion`. |
| `ContentDbContext` | Contexto para `content_db`. Expone `Curso`, `Modulo`, `Leccion`. |
| `IRepositorioClases` | Contrato: `ObtenerUsuarioPorId`, `ObtenerInscripcionesPorUsuario`, `ObtenerTodasLasInscripciones`, `ObtenerCursoPorId`, `ObtenerCursosPorIds`, `ObtenerLeccionesPorCurso`, `ObtenerProgresosPorUsuarioYLecciones`. |
| `RepositorioClases` | Implementación con EF Core. Filtra inscripciones con `progress >= 100` para cursos acabados; agrupa inscripciones por `courseId` para ranking. |
| `IServicioClases` | Contrato de lógica: `ObtenerCursosAcabadosEstudianteAsync`, `ObtenerMasTomadas`, `ObtenerTopMasTomadas`. |
| `ServicioClases` | Filtra cursos completados (progress = 100), calcula `PorcentajeCompletados` de cada curso, ordena por `TotalInscritos` descendente. |
| `CursoAcabadoDto` | Respuesta: `IdCurso`, `TituloCurso`, `FechaInscripcion`, `FechaCompletado`, `TotalLecciones`, `DuracionTotalSegundos`. |
| `ClaseMasTomadaDto` | Respuesta de ranking: `IdCurso`, `TituloCurso`, `TotalInscritos`, `PorcentajeCompletados`. |
| `ClasesController` | Enruta `GET /api/clases/...`. |

#### Servicio Estadísticas (puerto 5022)

| Clase | Responsabilidad |
|---|---|
| `UserDbContext` | Contexto para `user_db`. Expone todas las tablas de actividad del usuario. |
| `ContentDbContext` | Contexto para `content_db`. Expone toda la jerarquía de contenido. |
| `IRepositorioEstadisticas` | Contrato unificado: acceso a usuarios, inscripciones, progresos, intentos, cursos y lecciones. |
| `RepositorioEstadisticas` | Implementación que consulta ambas bases de datos. |
| `IServicioEstadisticas` | Contrato de lógica: `ObtenerResumenEstudianteAsync`, `ObtenerEstadisticasCursosAsync`, `ObtenerEstadisticasLeccionesAsync`. |
| `ServicioEstadisticas` | Agrega datos de ambas bases (actividad del usuario + metadatos de contenido) para construir el resumen completo. |
| `ResumenEstudianteDto` | Respuesta completa del estudiante con métricas consolidadas. |
| `EstadisticasCursoDto` | Respuesta por curso con progreso, tiempo y lecciones. |
| `EstadisticasLeccionDto` | Respuesta por lección con estado y tiempo. |
| `EstadisticasController` | Enruta `GET /api/estadisticas/...`. |

### Endpoints

| Servicio | Método | Ruta | Descripción |
|---|---|---|---|
| Cuestionarios | GET | `/api/cuestionarios/notas/estudiante/{userId}` | Todas las notas de un estudiante |
| Cuestionarios | GET | `/api/cuestionarios/notas/promedio/estudiante/{userId}` | Promedio de calificaciones |
| Cuestionarios | GET | `/api/cuestionarios/mejores-estudiantes` | Ranking completo por promedio |
| Cuestionarios | GET | `/api/cuestionarios/mejores-estudiantes/top/{n}` | Top N estudiantes |
| Clases | GET | `/api/clases/cursos-acabados/estudiante/{userId}` | Cursos completados del estudiante |
| Clases | GET | `/api/clases/mas-tomadas` | Todos los cursos ordenados por inscritos |
| Clases | GET | `/api/clases/mas-tomadas/top/{n}` | Top N cursos más populares |
| Estadísticas | GET | `/api/estadisticas/estudiante/{userId}` | Resumen general del estudiante |
| Estadísticas | GET | `/api/estadisticas/estudiante/{userId}/cursos` | Estadísticas por curso |
| Estadísticas | GET | `/api/estadisticas/estudiante/{userId}/lecciones` | Progreso por lección |

---

## Arquitectura de MicroServicios

Cuatro microservicios completamente independientes, uno por estadística. Cada uno es un proceso aislado con su propia capa de Datos y Lógica. No comparten código ni bases de datos de forma exclusiva.

### Diagrama

```
┌─────────────────────────────────────────────────────────────────────────┐
│                      ARQUITECTURA DE MICROSERVICIOS                     │
│                                                                         │
│  ┌─────────────┐  ┌──────────────────┐  ┌──────────────┐  ┌─────────┐  │
│  │    NOTAS    │  │  CURSOS ACABADOS  │  │  CLASES MÁS  │  │MEJORES  │  │
│  │    :5010    │  │      :5011       │  │   TOMADAS    │  │ESTUD.   │  │
│  │             │  │                  │  │    :5012     │  │  :5013  │  │
│  │ Datos/      │  │ Datos/           │  │              │  │         │  │
│  │ └ Repo      │  │ └ Repo           │  │ Datos/       │  │ Datos/  │  │
│  │ Logica/     │  │ Logica/          │  │ └ Repo       │  │ └ Repo  │  │
│  │ └ Servicio  │  │ └ Servicio       │  │ Logica/      │  │ Logica/ │  │
│  │ Controlad.  │  │ Controlad.       │  │ └ Servicio   │  │ └ Serv. │  │
│  └──────┬──────┘  └────────┬─────────┘  └──────┬───────┘  └────┬────┘  │
│         │                  │                    │               │        │
│    user_db            user_db +           user_db +         user_db     │
│    content_db         content_db          content_db        content_db  │
└─────────────────────────────────────────────────────────────────────────┘
```

### Estructura de archivos

```
MicroServicios/
├── MicroServicios.sln
│
├── Notas/                              ← Microservicio: Calificaciones
│   ├── Datos/
│   │   ├── Contexto/
│   │   │   ├── UserDbContext.cs
│   │   │   └── ContentDbContext.cs
│   │   ├── Modelos/
│   │   │   ├── IntentoCuestionario.cs
│   │   │   ├── Usuario.cs
│   │   │   └── Cuestionario.cs
│   │   └── Repositorios/
│   │       ├── IRepositorioNotas.cs
│   │       └── RepositorioNotas.cs
│   ├── Logica/
│   │   ├── DTOs/
│   │   │   └── NotaEstudianteDto.cs
│   │   └── Servicios/
│   │       ├── IServicioNotas.cs
│   │       └── ServicioNotas.cs
│   ├── Controladores/
│   │   └── NotasController.cs
│   ├── Program.cs
│   ├── appsettings.json
│   ├── Notas.csproj
│   └── Dockerfile
│
├── CursosAcabados/                     ← Microservicio: Cursos completados
│   ├── Datos/
│   │   ├── Contexto/ (UserDb + ContentDb)
│   │   ├── Modelos/ (Inscripcion, ProgresoLeccion, Curso, Modulo, Leccion, Usuario)
│   │   └── Repositorios/
│   │       ├── IRepositorioCursosAcabados.cs
│   │       └── RepositorioCursosAcabados.cs
│   ├── Logica/
│   │   ├── DTOs/ (CursoAcabadoDto.cs)
│   │   └── Servicios/ (IServicioCursosAcabados, ServicioCursosAcabados)
│   ├── Controladores/ (CursosAcabadosController)
│   ├── Program.cs, appsettings.json
│   ├── CursosAcabados.csproj
│   └── Dockerfile
│
├── ClasesMasTomadas/                   ← Microservicio: Popularidad de cursos
│   ├── Datos/
│   │   ├── Contexto/ (UserDb + ContentDb)
│   │   ├── Modelos/ (Inscripcion, Curso)
│   │   └── Repositorios/
│   │       ├── IRepositorioClasesMasTomadas.cs
│   │       └── RepositorioClasesMasTomadas.cs
│   ├── Logica/
│   │   ├── DTOs/ (ClaseMasTomadaDto.cs)
│   │   └── Servicios/ (IServicioClasesMasTomadas, ServicioClasesMasTomadas)
│   ├── Controladores/ (ClasesMasTomadosController)
│   ├── Program.cs, appsettings.json
│   ├── ClasesMasTomadas.csproj
│   └── Dockerfile
│
├── MejoresEstudiantes/                 ← Microservicio: Ranking de estudiantes
│   ├── Datos/
│   │   ├── Contexto/ (UserDb + ContentDb)
│   │   ├── Modelos/ (IntentoCuestionario, Inscripcion, Usuario)
│   │   └── Repositorios/
│   │       ├── IRepositorioMejoresEstudiantes.cs
│   │       └── RepositorioMejoresEstudiantes.cs
│   ├── Logica/
│   │   ├── DTOs/ (MejorEstudianteDto.cs)
│   │   └── Servicios/ (IServicioMejoresEstudiantes, ServicioMejoresEstudiantes)
│   ├── Controladores/ (MejoresEstudiantesController)
│   ├── Program.cs, appsettings.json
│   ├── MejoresEstudiantes.csproj
│   └── Dockerfile
│
└── docker-compose.yml
```

### Responsabilidades de cada clase

#### Microservicio Notas (puerto 5010)

| Clase | Responsabilidad |
|---|---|
| `UserDbContext` | Expone `Usuario` e `IntentoCuestionario` de `user_db`. |
| `ContentDbContext` | Expone `Cuestionario` de `content_db` para obtener títulos. |
| `IRepositorioNotas` | Contrato: `ObtenerUsuarioPorId`, `ObtenerIntentosPorUsuario`, `ObtenerCuestionariosPorIds`. |
| `RepositorioNotas` | Implementación mínima: solo consulta lo necesario para notas. |
| `IServicioNotas` | Contrato: `ObtenerNotasEstudianteAsync`, `ObtenerPromedioEstudianteAsync`. |
| `ServicioNotas` | Calcula `PorcentajeCalificacion = (score / maxScore) * 100` por intento. Calcula el promedio general. |
| `NotaEstudianteDto` | Respuesta: `IdIntento`, `IdCuestionario`, `TituloCuestionario`, `Calificacion`, `CalificacionMaxima`, `PorcentajeCalificacion`, `NumeroIntento`, `FechaIntento`. |
| `NotasController` | Enruta `GET /api/notas/estudiante/{userId}` y `GET /api/notas/estudiante/{userId}/promedio`. |

#### Microservicio CursosAcabados (puerto 5011)

| Clase | Responsabilidad |
|---|---|
| `UserDbContext` | Expone `Usuario`, `Inscripcion`, `ProgresoLeccion` de `user_db`. |
| `ContentDbContext` | Expone `Curso`, `Modulo`, `Leccion` de `content_db`. |
| `IRepositorioCursosAcabados` | Contrato: `ObtenerUsuarioPorId`, `ObtenerInscripcionesPorUsuario`, `ObtenerCursoPorId`, `ObtenerLeccionesPorCurso`, `ObtenerProgresosPorUsuarioYLecciones`. |
| `RepositorioCursosAcabados` | Filtra inscripciones con `progress >= 100`. |
| `IServicioCursosAcabados` | Contrato: `ObtenerCursosAcabadosEstudianteAsync`, `ObtenerTotalCursosAcabadosAsync`. |
| `ServicioCursosAcabados` | Determina la `FechaCompletado` como el máximo `completed_at` entre las lecciones del curso. Suma la duración total de lecciones. |
| `CursoAcabadoDto` | Respuesta: `IdCurso`, `TituloCurso`, `FechaInscripcion`, `FechaCompletado`, `TotalLecciones`, `DuracionTotalSegundos`. |
| `CursosAcabadosController` | Enruta `GET /api/cursos-acabados/estudiante/{userId}` y `.../total`. |

#### Microservicio ClasesMasTomadas (puerto 5012)

| Clase | Responsabilidad |
|---|---|
| `UserDbContext` | Expone únicamente `Inscripcion` de `user_db`. |
| `ContentDbContext` | Expone `Curso` de `content_db`. |
| `IRepositorioClasesMasTomadas` | Contrato mínimo: `ObtenerTodasLasInscripciones`, `ObtenerCursosPorIds`. |
| `RepositorioClasesMasTomadas` | Lee todas las inscripciones y agrupa por `courseId`. |
| `IServicioClasesMasTomadas` | Contrato: `ObtenerClasesMasTomadas`, `ObtenerTopClasesMasTomadas(n)`. |
| `ServicioClasesMasTomadas` | Agrupa inscripciones por curso, cuenta inscritos, calcula porcentaje de los que completaron (`progress >= 100`), ordena descendente. |
| `ClaseMasTomadaDto` | Respuesta: `IdCurso`, `TituloCurso`, `TotalInscritos`, `PorcentajeCompletados`. |
| `ClasesMasTomadosController` | Enruta `GET /api/clases-mas-tomadas` y `.../top/{n}`. |

#### Microservicio MejoresEstudiantes (puerto 5013)

| Clase | Responsabilidad |
|---|---|
| `UserDbContext` | Expone `Usuario`, `IntentoCuestionario`, `Inscripcion` de `user_db`. |
| `ContentDbContext` | Presente pero no usado directamente en lógica de ranking. |
| `IRepositorioMejoresEstudiantes` | Contrato: `ObtenerTodosLosIntentos`, `ObtenerTodosLosUsuarios`, `ObtenerTodasLasInscripciones`. |
| `RepositorioMejoresEstudiantes` | Carga todos los intentos, usuarios e inscripciones en memoria para el cálculo de ranking. |
| `IServicioMejoresEstudiantes` | Contrato: `ObtenerMejoresEstudiantesAsync`, `ObtenerTopMejoresEstudiantesAsync(n)`. |
| `ServicioMejoresEstudiantes` | Agrupa intentos por usuario, calcula promedio de `(score/maxScore)*100`, cuenta cursos completados, asigna posición (`Posicion`), ordena descendente. |
| `MejorEstudianteDto` | Respuesta: `Posicion`, `IdEstudiante`, `NombreEstudiante`, `PromedioCalificacion`, `TotalIntentos`, `CursosCompletados`. |
| `MejoresEstudiantesController` | Enruta `GET /api/mejores-estudiantes` y `.../top/{n}`. |

### Endpoints

| Microservicio | Método | Ruta | Descripción |
|---|---|---|---|
| Notas | GET | `/api/notas/estudiante/{userId}` | Lista de notas por intento |
| Notas | GET | `/api/notas/estudiante/{userId}/promedio` | Promedio general del estudiante |
| CursosAcabados | GET | `/api/cursos-acabados/estudiante/{userId}` | Cursos completados del estudiante |
| CursosAcabados | GET | `/api/cursos-acabados/estudiante/{userId}/total` | Contador de cursos completados |
| ClasesMasTomadas | GET | `/api/clases-mas-tomadas` | Todos los cursos ordenados por inscritos |
| ClasesMasTomadas | GET | `/api/clases-mas-tomadas/top/{n}` | Top N cursos más populares |
| MejoresEstudiantes | GET | `/api/mejores-estudiantes` | Ranking completo de estudiantes |
| MejoresEstudiantes | GET | `/api/mejores-estudiantes/top/{n}` | Top N mejores estudiantes |

---

## Guía de comandos

### Paso 0 — Prerequisitos

```bash
# Verificar .NET 10
dotnet --version
# Debe mostrar: 10.x.x

# Verificar Docker
docker --version
docker compose version
```

### Paso 1 — Levantar las bases de datos (solo para `dotnet run`)

Al usar Docker Compose (Opción B), las bases de datos se inician automáticamente. Si prefieres correr las aplicaciones en local con `dotnet run`, levanta solo los contenedores de base de datos:

```bash
cd /home/jkkhumbgham/Documentos/Taller.Net2

# Solo las bases de datos
docker compose up -d content_db user_db

# Verificar que están corriendo y saludables
docker ps | grep -E "content_db|user_db"
```

Resultado esperado:
```
content_db   postgres:16   0.0.0.0:5432->5432/tcp   Up (healthy)
user_db      postgres:16   0.0.0.0:5433->5432/tcp   Up (healthy)
```

---

### Opción A — Correr en local (sin Docker)

#### Monolítica

```bash
cd /home/jkkhumbgham/Documentos/Taller.Net2/Monolitica

dotnet run
# Disponible en: http://localhost:5001
```

#### Servicios (3 terminales separadas)

```bash
# Terminal 1
cd /home/jkkhumbgham/Documentos/Taller.Net2/Servicios/Cuestionarios
dotnet run
# http://localhost:5020

# Terminal 2
cd /home/jkkhumbgham/Documentos/Taller.Net2/Servicios/Clases
dotnet run
# http://localhost:5021

# Terminal 3
cd /home/jkkhumbgham/Documentos/Taller.Net2/Servicios/Estadisticas
dotnet run
# http://localhost:5022
```

#### MicroServicios (4 terminales separadas)

```bash
# Terminal 1
cd /home/jkkhumbgham/Documentos/Taller.Net2/MicroServicios/Notas
dotnet run
# http://localhost:5010

# Terminal 2
cd /home/jkkhumbgham/Documentos/Taller.Net2/MicroServicios/CursosAcabados
dotnet run
# http://localhost:5011

# Terminal 3
cd /home/jkkhumbgham/Documentos/Taller.Net2/MicroServicios/ClasesMasTomadas
dotnet run
# http://localhost:5012

# Terminal 4
cd /home/jkkhumbgham/Documentos/Taller.Net2/MicroServicios/MejoresEstudiantes
dotnet run
# http://localhost:5013
```

---

### Opción B — Correr con Docker (recomendado)

> Las bases de datos (`content_db` y `user_db`) se levantan automáticamente junto con los servicios. Los contenedores de la aplicación esperan a que las bases estén saludables antes de arrancar (`depends_on` con `condition: service_healthy`).

#### Monolítica

```bash
cd /home/jkkhumbgham/Documentos/Taller.Net2/Monolitica

# Construir y levantar
docker-compose up --build

# En segundo plano
docker-compose up --build -d

# Ver logs
docker-compose logs -f

# Detener
docker-compose down
```

Contenedor levantado: `estadisticas-monolito` → `http://localhost:5001`

---

#### Servicios

```bash
cd /home/jkkhumbgham/Documentos/Taller.Net2/Servicios

# Levantar los 3 servicios
docker-compose up --build

# En segundo plano
docker-compose up --build -d

# Ver logs de un servicio específico
docker-compose logs -f cuestionarios
docker-compose logs -f clases
docker-compose logs -f estadisticas

# Detener todos
docker-compose down

# Levantar solo un servicio específico
docker-compose up --build cuestionarios
docker-compose up --build clases
docker-compose up --build estadisticas
```

Contenedores levantados:
- `svc-cuestionarios` → `http://localhost:5020`
- `svc-clases` → `http://localhost:5021`
- `svc-estadisticas` → `http://localhost:5022`

---

#### MicroServicios

```bash
cd /home/jkkhumbgham/Documentos/Taller.Net2/MicroServicios

# Levantar los 4 microservicios
docker-compose up --build

# En segundo plano
docker-compose up --build -d

# Ver logs de un microservicio específico
docker-compose logs -f notas
docker-compose logs -f cursos-acabados
docker-compose logs -f clases-mas-tomadas
docker-compose logs -f mejores-estudiantes

# Detener todos
docker-compose down

# Levantar solo un microservicio
docker-compose up --build notas
docker-compose up --build cursos-acabados
docker-compose up --build clases-mas-tomadas
docker-compose up --build mejores-estudiantes
```

Contenedores levantados:
- `micro-notas` → `http://localhost:5010`
- `micro-cursos-acabados` → `http://localhost:5011`
- `micro-clases-mas-tomadas` → `http://localhost:5012`
- `micro-mejores-estudiantes` → `http://localhost:5013`

---

### Paso 3 — Verificar que funciona (ejemplos curl)

```bash
# --- MONOLÍTICA ---
# Resumen del estudiante 1
curl http://localhost:5001/api/stats/students/1

# Cursos del estudiante 1
curl http://localhost:5001/api/stats/students/1/courses

# Notas del estudiante 1
curl http://localhost:5001/api/stats/notas/estudiante/1

# Cursos acabados del estudiante 1
curl http://localhost:5001/api/stats/cursos-acabados/estudiante/1

# Clases más tomadas
curl http://localhost:5001/api/stats/clases-mas-tomadas

# Mejores estudiantes
curl http://localhost:5001/api/stats/mejores-estudiantes


# --- SERVICIOS ---
# Notas del estudiante 1
curl http://localhost:5020/api/cuestionarios/notas/estudiante/1

# Promedio del estudiante 1
curl http://localhost:5020/api/cuestionarios/notas/promedio/estudiante/1

# Top 5 mejores estudiantes
curl http://localhost:5020/api/cuestionarios/mejores-estudiantes/top/5

# Cursos acabados del estudiante 1
curl http://localhost:5021/api/clases/cursos-acabados/estudiante/1

# Top 3 clases más tomadas
curl http://localhost:5021/api/clases/mas-tomadas/top/3

# Resumen del estudiante 1
curl http://localhost:5022/api/estadisticas/estudiante/1

# Estadísticas por curso del estudiante 1
curl http://localhost:5022/api/estadisticas/estudiante/1/cursos


# --- MICROSERVICIOS ---
# Notas del estudiante 1
curl http://localhost:5010/api/notas/estudiante/1

# Promedio del estudiante 1
curl http://localhost:5010/api/notas/estudiante/1/promedio

# Cursos acabados del estudiante 1
curl http://localhost:5011/api/cursos-acabados/estudiante/1

# Total de cursos acabados
curl http://localhost:5011/api/cursos-acabados/estudiante/1/total

# Todas las clases más tomadas
curl http://localhost:5012/api/clases-mas-tomadas

# Top 10 clases más tomadas
curl http://localhost:5012/api/clases-mas-tomadas/top/10

# Ranking completo de estudiantes
curl http://localhost:5013/api/mejores-estudiantes

# Top 5 mejores estudiantes
curl http://localhost:5013/api/mejores-estudiantes/top/5
```

### Paso 4 — Swagger UI (documentación interactiva)

Cada servicio expone Swagger en desarrollo:

| Implementación | URL Swagger |
|---|---|
| Monolítica | `http://localhost:5001/openapi/v1.json` |
| Cuestionarios | `http://localhost:5020/openapi/v1.json` |
| Clases | `http://localhost:5021/openapi/v1.json` |
| Estadísticas | `http://localhost:5022/openapi/v1.json` |
| Notas | `http://localhost:5010/openapi/v1.json` |
| CursosAcabados | `http://localhost:5011/openapi/v1.json` |
| ClasesMasTomadas | `http://localhost:5012/openapi/v1.json` |
| MejoresEstudiantes | `http://localhost:5013/openapi/v1.json` |

### Paso 5 — Compilar sin ejecutar

```bash
cd /home/jkkhumbgham/Documentos/Taller.Net2

# Compilar solo el monolito
dotnet build Monolitica/Monolitica.csproj

# Compilar todos los servicios
dotnet build Servicios/Servicios.slnx

# Compilar todos los microservicios
dotnet build MicroServicios/MicroServicios.slnx
```

---

### Resumen de puertos

| Implementación | Componente | Puerto |
|---|---|---|
| Monolítica | Monolito completo | **5001** |
| Servicios | Cuestionarios | **5020** |
| Servicios | Clases | **5021** |
| Servicios | Estadísticas | **5022** |
| MicroServicios | Notas | **5010** |
| MicroServicios | CursosAcabados | **5011** |
| MicroServicios | ClasesMasTomadas | **5012** |
| MicroServicios | MejoresEstudiantes | **5013** |
| Bases de datos | content_db (PostgreSQL) | **5432** |
| Bases de datos | user_db (PostgreSQL) | **5433** |
