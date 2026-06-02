# Sistema de Estadísticas de Estudiantes — .NET 10

Sistema que genera estadísticas de estudiantes sobre los cursos que toman y han tomado en una plataforma de aprendizaje. Implementado en tres arquitecturas independientes: **Monolítica**, **Servicios** y **MicroServicios**.

---


## Índice

1. [Bases de datos](#bases-de-datos)
2. [Arquitectura Monolítica](#arquitectura-monolítica)
3. [Arquitectura de Servicios](#arquitectura-de-servicios)
4. [Arquitectura de MicroServicios](#arquitectura-de-microservicios)
5. [Guía de comandos](#guía-de-comandos)

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
cd "Carpeta general, donde estan las 3 arquitecturas"

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
cd /Monolitica

dotnet run
# Disponible en: http://localhost:5001
```

#### Servicios (3 terminales separadas)

```bash
# Terminal 1
cd /Servicios/Cuestionarios
dotnet run
# http://localhost:5020

# Terminal 2
cd /Servicios/Clases
dotnet run
# http://localhost:5021

# Terminal 3
cd /Servicios/Estadisticas
dotnet run
# http://localhost:5022
```

#### MicroServicios (4 terminales separadas)

```bash
# Terminal 1
cd /MicroServicios/Notas
dotnet run
# http://localhost:5010

# Terminal 2
cd /MicroServicios/CursosAcabados
dotnet run
# http://localhost:5011

# Terminal 3
cd /MicroServicios/ClasesMasTomadas
dotnet run
# http://localhost:5012

# Terminal 4
cd /MicroServicios/MejoresEstudiantes
dotnet run
# http://localhost:5013
```

---

### Opción B — Correr con Docker (recomendado)

> Las bases de datos (`content_db` y `user_db`) se levantan automáticamente junto con los servicios. Los contenedores de la aplicación esperan a que las bases estén saludables antes de arrancar (`depends_on` con `condition: service_healthy`).

#### Monolítica

```bash
cd /Monolitica

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
cd /Servicios

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
cd /MicroServicios

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
