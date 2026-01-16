# Unit of Work

The Unit of Work is a design pattern used to maintain a list of operations (CRUD) on a database and commit them as a single transaction. 
It helps manage changes across multiple repositories to ensure data consistency and reduce unnecessary database calls.

✅ Unit of Work manages multiple repositories and ensures all database operations are performed as one atomic transaction.

✅ Repositories handle CRUD operations and reduce duplication in the service layer.

✅ Combining UnitOfWork + Repository Pattern improves scalability, maintainability, and testability in EF Core applications.


#### Registering the UnitOfWork with Dependency Injection:
```csharp
builder.Services.AddUnitOfWork(options =>
{
    string connectionString = "your-database-connection-string";
    options.UseMySql<MyDbContext>(connectionString);
});
