# Unit of Work

The Unit of Work is a design pattern used to maintain a list of operations (CRUD) on a database and commit them as a single transaction. 
It helps manage changes across multiple repositories to ensure data consistency and reduce unnecessary database calls.

#### Why Use the Unit of Work Pattern?
- ✅ Ensures atomic transactions → Multiple operations succeed or fail together.
- ✅ Minimizes database calls → Reduces redundant operations.
- ✅ Manages multiple repositories → Keeps business logic clean.
- ✅ Improves testability → Easy to mock and unit test.

#### Registering the UnitOfWork with Dependency Injection:
```csharp
builder.Services.AddUnitOfWork(options =>
{
    string connectionString = "your-database-connection";
    options.UseMySql<MyDbContext>(connectionString);
});
