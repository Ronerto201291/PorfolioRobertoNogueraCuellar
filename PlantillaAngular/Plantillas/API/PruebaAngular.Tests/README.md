# ?? PruebaAngular.Tests

Proyecto de tests unitarios para el Portfolio Full-Stack.

## ?? Estructura

```
PruebaAngular.Tests/
??? Fixtures/                    # Utilidades compartidas
?   ??? TestDatabaseFixture.cs   # Helper para crear DB en memoria
??? Handlers/                    # Tests de Handlers (CQRS)
?   ??? Projects/                # Tests de proyectos
?   ?   ??? CreateProjectCommandHandlerTests.cs
?   ?   ??? UpdateProjectCommandHandlerTests.cs
?   ?   ??? DeleteProjectCommandHandlerTests.cs
?   ??? Tasks/                   # Tests de tareas
?       ??? CreateTaskCommandHandlerTests.cs
?       ??? UpdateTaskCommandHandlerTests.cs
?       ??? DeleteTaskCommandHandlerTests.cs
??? README.md
```

## ?? Ejecutar Tests

```bash
# Todos los tests
dotnet test

# Con detalles
dotnet test --verbosity normal

# Solo un archivo de tests
dotnet test --filter "FullyQualifiedName~CreateProjectCommandHandlerTests"

# Con cobertura de código
dotnet test --collect:"XPlat Code Coverage"
```

## ?? Tests Incluidos

### Projects (8 tests)
- ? Crear proyecto con datos válidos
- ? Crear proyecto sin descripción
- ? Crear proyecto sin nombre
- ? Crear proyecto con nombre muy largo
- ? Actualizar proyecto existente
- ? Actualizar proyecto inexistente
- ? Eliminar proyecto existente
- ? Eliminar proyecto inexistente

### Tasks (12 tests)
- ? Crear tarea con datos válidos
- ? Crear tarea en proyecto inexistente
- ? Crear tarea con estados válidos (Pending, InProgress, Completed)
- ? Crear tarea con estado inválido
- ? Crear tarea con prioridades válidas (Low, Medium, High)
- ? Crear tarea con prioridad inválida
- ? Actualizar tarea existente
- ? Actualizar tarea inexistente
- ? Eliminar tarea existente
- ? Eliminar tarea inexistente

## ??? Tecnologías

| Librería | Uso |
|----------|-----|
| **xUnit** | Framework de testing |
| **FluentAssertions** | Assertions legibles |
| **Moq** | Mocking de dependencias |
| **EF Core InMemory** | Base de datos para tests |

## ?? Convenciones

- **Nombre del test**: `Handle_Escenario_ResultadoEsperado`
- **Estructura AAA**: Arrange, Act, Assert
- **Un assert principal** por test
- **Tests independientes**: Cada test crea su propia DB
