<p align="center">
  <img src="https://github.com/guidospadavecchia/FluentSQL/blob/main/design/fsql-logo.png">
</p>

## Description
**FluentSQL** is a **.NET Standard** library wrapping [Dapper](https://github.com/StackExchange/Dapper) micro-ORM, and adding a fluent syntax for query building, similar to [Dapper.SqlBuilder](https://www.nuget.org/packages/Dapper.SqlBuilder/) (but different), all in one small package.  
The **FSQL** syntax is as similar as SQL syntax as you can get, while using a Fluent API which resembles LINQ.

## Packages
Package | Version | Downloads |
--- | --- | --- |
[FluentSQL.Core](https://www.nuget.org/packages/FluentSQL.Core/) | [![Version](https://img.shields.io/nuget/v/FluentSQL.Core?style=flat-square)](https://www.nuget.org/packages/FluentSQL.Core/) | [![Downloads](https://img.shields.io/nuget/dt/FluentSQL.Core?color=lightgreen&style=flat-square)](https://www.nuget.org/packages/FluentSQL.Core/) |

## Usage

### Connection
To create a new query, start by using `FluentSqlBuilder.Connect()`, which returns a `IFluentSqlQuery` object. From there, you can build different queries and execute them against the database.

#### Create with Connection String
This method will create, open and close an `IDbConnection` for each query execution.
```csharp
IFluentSqlQuery query = FluentSqlBuilder.Connect(CONNECTION_STRING);
```

#### Create with transaction (to execute multiple queries in the same scope)
This method will use the `IDbConnection` linked to the `IDbTransaction`, and will remain open after each query execution.
```csharp
using (IDbConnection connection = new SqlConnection(CONNECTION_STRING))
{
    connection.Open();

    using (IDbTransaction transaction = connection.BeginTransaction())
    {
        using (IFluentSqlQuery query = FluentSqlBuilder.Connect(transaction))
        {
            //Query execution...

            transaction.Commit();
        }
    }
}
```

_Note: It is recommended to always wrap the `IFluentSqlQuery` instantiation inside a `using` statement regardless of the overload used._
***

### Select

#### Simple Select
```csharp
IEnumerable<dynamic> result = query.Select("C.Name", "C.Surname").From("Customers", "C").ToDynamic();
```
```csharp
IEnumerable<dynamic> result = query.SelectAll().Distinct().Top(100).From("Customers").Where("ID > 5").ToDynamic();
```

#### Full example using all available chaining methods for Select
```csharp
var result = query.Select("C.ID", "C.Name", "C.Surname", "COUNT(I.ID)")
                  .Distinct()
                  .Top(10)
                  .From("Customers", "C")
                  .Join("Invoices", "I", JoinTypes.Inner)
                  .On("C.ID = I.CustomerId")
                  .GroupBy("C.ID", "C.Name", "C.Surname")
                  .Having("COUNT(I.ID) > 10")
                  .OrderBy("C.Surname", "C.Name")
                  .Descending()
                  .ToDynamic();
 ```
 
#### Return methods
The `Select()` method, when executed, can return a collection of dynamic objects (`IEnumerable<dynamic>`) whose properties match the columns specified, or map to a specified DTO (`IEnumerable<T>`):

```csharp
IEnumerable<dynamic> result = query.Select("C.ID", "C.Name", "C.Surname").From("Customers", "C").ToDynamic();
dynamic firstCustomer = result.ElementAt(0);

int id = firstCustomer.ID;
string name = firstCustomer.Name;
string surname = firstCustomer.Surname;
```

```csharp
public class CustomerDTO
{
    public int ID;
    public string Name;
    public string Surname;
    public DateTime CreationDate;
}

IEnumerable<CustomerDTO> result = query.SelectAll().From("Customers").Where("ID IN (1, 2, 3)").ToMappedObject<CustomerDTO>();
CustomerDTO firstCustomer = lista.ElementAt(0);

int id = firstCustomer.ID;
string name = firstCustomer.Name;
string surname = firstCustomer.Surname;
DateTime creationDate = firstCustomer.CreationDate;
```

***

## Insert, Update & Delete
This non-query methods can start from `InsertInto()`, `Update()` or `DeleteFrom()` and end with `Execute()`, returning the affected rows.

### Insert
Returns the affected rows.
```csharp
int affectedRows = query.InsertInto("Currencies")
                        .Columns("Description", "Code", "CountryCode", "MaxNoteValue", "MinNoteValue")
                        .Values("Argentine Peso", "AR", "ARG", 1000, 5)
                        .Execute();
```

### Update
Returns the affected rows.
```csharp
int affectedRows = query.Update("Currencies")
                        .Set("Description = 'U.S. Dollar'", "Code = 'US'")
                        .Where("ID = 2")
                        .Execute();
```

```csharp
var assignments = new Dictionary<string, object>();

assignments.Add("Description", "U.S. Dollar");
assignments.Add("Code", "US");

int affectedRows = query.Update("Currencies")
                        .Set(assignments)
                        .Where("ID = 2")
                        .Execute();
```

### Delete
Returns the affected rows.
```csharp
int affectedRows = query.DeleteFrom("Currencies")
                        .Where("ID = 2")
                        .Execute();
```

***

## Stored Procedures

### Non-query executions
Returns the affected rows.
```csharp
int affectedRows = query.StoreProcedure("ProcessDailyInvoices").ExecuteNonQuery();
```

### Non-query executions with input parameters
Returns the affected rows.
```csharp
int affectedRows = query.StoreProcedure("ProcessCustomerInvoices")
                        .WithParameter("CustomerId", 55)
                        .WithParameter("Date", DateTime.Now)
                        .ExecuteNonQuery();
```

```csharp
Dictionary<string, object> parameters = new Dictionary<string, object>();

parameters.Add("CustomerId", 55);
parameters.Add("Date", DateTime.Now);

int affectedRows = query.StoreProcedure("ProcessCustomerInvoices")
                        .WithParameters(parameters)
                        .ExecuteNonQuery();
```

### Query executions
Similar to `Select()` method, *Stored Procedures* can return a collection of dynamic objects (`IEnumerable<dynamic>`) or map to a collection of DTO objects (`IEnumerable<T>`):

```csharp
IEnumerable<dynamic> result = query.StoreProcedure("GetInvoices")
                                   .WithParameter("CustomerId", 55)
                                   .WithParameter("Date", DateTime.Now)
                                   .ExecuteToDynamic();
```

```csharp
public class Invoice
{
    public int Id;
    public int CustomerId;
    public DateTime Timestamp;
}

IEnumerable<Invoice> result = query.StoreProcedure("GetInvoices")
                                   .WithParameter("CustomerId", 55)
                                   .WithParameter("Date", DateTime.Now)
                                   .ExecuteToMappedObject<Invoice>();
```


### Non-query execution with output parameters
Apart from return objects, *Stored Procedures* can return output values. In case you have to retrieve output parameters, end methods after calling `WithOutputParameter()` or `WithOutputParameters()` will return a `StoreProcedureWithOutputResult` object, which has two properties: `T ReturnValue` and `Dictionary<string, object> OutputParameters`:

```csharp
public sealed class StoredProcedureWithOutputResult<T>
{
    public T ReturnValue { get; private set; }
    public Dictionary<string, object> OutputParameters { get; private set; }
}
```

_Note that calls to `WithOutputParameter()` or `WithOutputParameters()` must be done after all input parameters have been set._

**Non-query execution**
```csharp
StoreProcedureWithOutputResult<int> result = query.StoreProcedure("FormatStringDate")
                                                  .WithParameter("Date", DateTime.Now)
                                                  .WithParameter("Prefix", "Valid through ")
                                                  .WithOutputParameter(new OutputParameter("DateTimeSTR", DbType.String, 500))
                                                  .ExecuteNonQuery();
//Return value                                             
int affectedRows = result.ReturnValue;
//Output parameters
Dictionary<string, object> outputParameters = result.OutputParameters;

object value = outputParameters.FirstOrDefault()?.Value;
string formattedResult = Convert.ToString(value);
```

**Query execution**
```csharp
StoreProcedureWithOutputResult<IEnumerable<dynamic>> result = query.StoreProcedure("ProcessInvoicesAndReturnCustomers")
                                                                   .WithParameter("Date", DateTime.Now)
                                                                   .WithOutputParameter(new OutputParameter("InvoicesProcessed", DbType.Int32))
                                                                   .ExecuteToDynamic();
//Return value
IEnumerable<dynamic> returnCollection = result.ReturnValue;
//Output parameters
Dictionary<string, object> outputParameters = result.OutputParameters;

// Get dynamic object & values
dynamic firstCustomer = returnCollection.ElementAt(0);
int id = firstCustomer.ID;
string name = firstCustomer.Name;
string surname = firstCustomer.Surname;                                         

// Get output parameter value
object value = outputParameters.FirstOrDefault()?.Value;
int invoicesProcessed = Convert.ToInt32(value);
```

```csharp
public class Customer
{
    public int ID;
    public string Name;
    public string Surname;
}

StoreProcedureWithOutputResult<IEnumerable<Customer>> result = query.StoreProcedure("ProcessInvoicesAndReturnCustomers")
                                                                    .WithParameter("Date", DateTime.Now)
                                                                    .WithOutputParameter(new OutputParameter("InvoicesProcessed", DbType.Int32))
                                                                    .ExecuteToMappedObject<Customer>();
//Return value
IEnumerable<dynamic> returnCollection = result.ReturnValue;
//Output parameters
Dictionary<string, object> outputParameters = result.OutputParameters;

// Get mapped object & values
Customer firstCustomer = returnCollection.ElementAt(0);
int id = firstCustomer.ID;
string name = firstCustomer.Name;
string surname = firstCustomer.Surname;                                         

// Get output parameter value
object value = outputParameters.FirstOrDefault()?.Value;
int invoicesProcessed = Convert.ToInt32(value);
```

***

## Custom queries
There are specific scenarios you can't use **FluentSql** for what you intend to do, because it is very complex or the library simply doesn't offer a fluent way of doing it. In such cases, it's possible to execute a custom query the same way you would do it with `ADO.NET`.

### Custom queries with return values
```csharp
IEnumerable<dynamic> result = query.ExecuteCustomQuery("SELECT * FROM Currencies WHERE Code = 'AR'");
```

```csharp
public class Customer
{
    public int ID;
    public string Name;
    public string Surname;
}

IEnumerable<Customer> result = query.ExecuteCustomQuery<Customer>("SELECT * FROM Customers WHERE ID BETWEEN 1 AND 100");
```

### Custom queries with no return values
```csharp
int affectedRows = query.ExecuteCustomNonQuery("EXEC dbo.ProcessInvoices");
```

### Custom queries with parameters
To pass parameter to a custom query, put the parameter name prefixed with `@`. Then, pass an object as a second argument containing a property for each parameter inside the query. The name of the parameter must match the name of the property:

**Non-query**
```csharp
int affectedRows = query.ExecuteCustomNonQuery("UPDATE Customers SET Name = 'John', Surname = 'Doe' WHERE ID = @CustomerId", new { CustomerId = 1 });
```

**Query**
```csharp
IEnumerable<dynamic> result = query.ExecuteCustomQuery("SELECT * FROM Customers WHERE Name = @Name AND Surname = @Surname", new { Name = "John", Surname = "Doe" });
```

```csharp
public class Customer
{
    public int ID;
    public string Name;
    public string Surname;
}

IEnumerable<Customer> result = query.ExecuteCustomQuery<Customer>("SELECT * FROM Customers WHERE Name = @Name AND Surname = @Surname", new { Name = "John", Surname = "Doe" });
```

***

## Async methods

All available end methods can be executed asynchronously. If there's an `Execute()`, there is also an awaitable `ExecuteAsync()`.

## Contributions

Contributions are welcome! Submit a [pull request](https://github.com/guidospadavecchia/FluentSQL/pulls).

## License
This proyect is distributed under the [MIT License](https://github.com/guidospadavecchia/FluentSQL/blob/main/LICENSE).

***
<p align="center">
  <img src="http://ForTheBadge.com/images/badges/built-with-love.svg">
</p>
