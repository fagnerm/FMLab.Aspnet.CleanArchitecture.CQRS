## Project Overview

An ASP.NET Core API built as a Clean Architecture reference implementation.\
The goal is to showcase layer separation, testability, and domain-centric design.

## Pattern Concept

Clean Architecture enforces separation of concerns by dividing the application into layers, each following a single dependency rule:

> [!NOTE]
> *Nothing in an inner circle can know anything at all about something in an outer circle.* [Robert C. Martin]

Details such as UI, database provider, external services, or cloud infrastructure must never influence business logic.

## Architecture

![Clean Architecture](https://blog.cleancoder.com/uncle-bob/images/2012-08-13-the-clean-architecture/CleanArchitecture.jpg)

**Dependency direction:**
> Domain <- Application <- Infrastructure <- Api

### Layers

- **Domain**:
  - Enterprise rules: entities, value objects, domain exceptions
- **Application**:
  - Application flow: use cases, interfaces, validators, DTOs
- **Infrastructure**:
  - Interface adapters: database, external services, notifications
- **Api**:
  - Thin delivery layer: routes requests to use cases, maps responses

### Key Patterns

- **Result Pattern**: Common return type for use case outcomes, replacing exception-based control flow
- **Repository + Gateway**: Separates write operations (repository) from read queries (gateway)
- **Unit of Work**: Commits all changes atomically; rolled back automatically on failure
- **Use Cases**: Single-responsibility handlers for each action a user can perform
- **Value Objects**: Immutable types that encapsulate and validate their own business rules

## API Endpoints

- `POST   /users` - Register a new user
- `GET    /users` - List users
- `GET    /users/{id}` - Get a user by ID
- `PUT    /users/{id}` - Full update. Unset fields are cleared
- `PATCH  /users/{id}` - Partial update. Only provided fields are changed
- `DELETE /users/{id}` - Remove a user
- `POST   /users/{id}/deactivate` - Deactivate a user
  
## License

This project is licensed under the [MIT License](LICENSE).
