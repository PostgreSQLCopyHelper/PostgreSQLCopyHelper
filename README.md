# PostgreSQLCopyHelper #

[![Build Status](https://dev.azure.com/PostgreSqlCopyHelper/PostgreSQLCopyHelper/_apis/build/status/postgresqlcopyhelper.PostgreSQLCopyHelper?branchName=master)](https://dev.azure.com/PostgreSqlCopyHelper/PostgreSQLCopyHelper/_build/latest?definitionId=1&branchName=master)
[![stable](https://img.shields.io/nuget/v/PostgreSQLCopyHelper.svg?label=stable)](https://www.nuget.org/packages/PostgreSQLCopyHelper/)
[![prerelease](https://img.shields.io/nuget/vpre/PostgreSQLCopyHelper.svg?label=prerelease)](https://www.nuget.org/packages/PostgreSQLCopyHelper/)

PostgreSQLCopyHelper is a library for efficient bulk inserts to PostgreSQL databases. It wraps the COPY methods from Npgsql behind a nice Fluent API.

## Installing ##

To install PostgreSQLCopyHelper, run the following command in the Package Manager Console:

```
PM> Install-Package PostgreSQLCopyHelper
```

## Basic Usage ##

Imagine we have the following table we want to copy data to:

```sql
CREATE TABLE sample.unit_test
(
	col_smallint smallint,
	col_integer integer,
	col_money money,
	col_bigint bigint,
	col_timestamp timestamp,
	col_real real,
	col_double double precision,
	col_bytea bytea,
	col_uuid uuid,
	col_numeric numeric,
	col_inet inet,
	col_macaddr macaddr,
	col_date date,
	col_interval interval
);
```

The corresponding domain model in our application could look like this:

```csharp
private class TestEntity
{
	public Int16? SmallInt { get; set; }
	public Int32? Integer { get; set; }
	public Int64? BigInt { get; set; }
	public Decimal? Money { get; set; }
	public DateTime? Timestamp { get; set; }
	public Decimal? Numeric { get; set; }
	public Single? Real { get; set; }
	public Double? DoublePrecision { get; set; }
	public byte[] ByteArray { get; set; }
	public Guid? UUID { get; set; }
	public IPAddress IpAddress { get; set; }
	public PhysicalAddress MacAddress { get; set; }
	public DateTime? Date { get; set; }
	public TimeSpan? TimeSpan { get; set; }
}
```

The PostgreSQLCopyHelper now defines the mapping between domain model and the database table:

```csharp
var copyHelper = new PostgreSQLCopyHelper<TestEntity>("sample", "unit_test")
	.MapSmallInt("col_smallint", x => x.SmallInt)
	.MapInteger("col_integer", x => x.Integer)
	.MapMoney("col_money", x => x.Money)
	.MapBigInt("col_bigint", x => x.BigInt)
	.MapTimeStamp("col_timestamp", x => x.Timestamp)
	.MapReal("col_real", x => x.Real)
	.MapDouble("col_double", x => x.DoublePrecision)
	.MapByteArray("col_bytea", x => x.ByteArray)
	.MapUUID("col_uuid", x => x.UUID)
	.MapInetAddress("col_inet", x => x.IpAddress)
	.MapMacAddress("col_macaddr", x => x.MacAddress)
	.MapDate("col_date", x => x.Date)
	.MapInterval("col_interval", x => x.TimeSpan)
	.MapNumeric("col_numeric", x => x.Numeric);
```

And then we can use it to efficiently store the data:

Synchronously:

```csharp
private ulong WriteToDatabase(PostgreSQLCopyHelper<TestEntity> copyHelper, IEnumerable<TestEntity> entities)
{
    using (var connection = new NpgsqlConnection("Server=127.0.0.1;Port=5432;Database=sampledb;User Id=philipp;Password=test_pwd;"))
    {
        connection.Open();

        // Returns count of rows written 
        return copyHelper.SaveAll(connection, entities);
    }
}
```

Or asynchronously:

```csharp
private async Task<ulong> WriteToDatabaseAsync(PostgreSQLCopyHelper<TestEntity> copyHelper, IEnumerable<TestEntity> entities, CancellationToken cancellationToken = default)
{
    using (var connection = new NpgsqlConnection("Server=127.0.0.1;Port=5432;Database=sampledb;User Id=philipp;Password=test_pwd;"))
    {
        await connection.OpenAsync(cancellationToken);

        // Returns count of rows written 
        return await copyHelper.SaveAllAsync(connection, entities, cancellationToken);
    }
}
```

Or asynchronously with asynchronous enumerables:

```csharp
private async Task<ulong> WriteToDatabaseAsync(PostgreSQLCopyHelper<TestEntity> copyHelper, IAsyncEnumerable<TestEntity> entities, CancellationToken cancellationToken = default)
{
    using (var connection = new NpgsqlConnection("Server=127.0.0.1;Port=5432;Database=sampledb;User Id=philipp;Password=test_pwd;"))
    {
        await connection.OpenAsync(cancellationToken);

        // Returns count of rows written 
        return await copyHelper.SaveAllAsync(connection, entities, cancellationToken);
    }
}
```

## PostgreSQLCopyHelper Custom Type Maps ##

One can always define a custom map function for any property to any `Npgsql` type.

For example:

```csharp
.Map("geo", x => x.geo, NpgsqlDbType.Point)
```

## Mapping Composite Types ##

Imagine you have a composite type called ``person_type`` in a schema of your database:

```sql
create type sample.person_type as
(
    first_name text,
    last_name text,
    birth_date date
);
```

And it is used in a table called ``CompositeTest``:

```sql
create table sample.CompositeTest
(
    col_text text,
    col_person sample.person_type                
)
```

You first need to map the Postgres ``person_type`` to a C\# class:

```csharp
private class PersonType
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public DateTime BirthDate { get; set; }
}
```

A hint: Npgsql always converts the property name to a snake case column name, so ``FirstName`` is mapped 
to ``first_name`` by convention. You can use the ``[PgName]`` attribute to explicitly set the Postgres type 
name.

Next the table is mapped to the following C\# model:

```csharp
private class SampleEntity
{
    public string TextColumn { get; set; }

    public PersonType CompositeTypeColumn { get; set; }
}
```

And now we can bulk write ``SampleEntity`` instances using PostgreSQLCopyHelper like this:

```csharp
connection.TypeMapper.MapComposite<PersonType>("sample.person_type");

// ... alternatively you can set it globally at any place in your application using the NpgsqlConnection.GlobalTypeMapper:
//
// NpgsqlConnection.GlobalTypeMapper.MapComposite<PersonType>("sample.person_type");

var subject = new PostgreSQLCopyHelper<SampleEntity>("sample", "CompositeTest")
         .MapText("col_text", x => x.TextColumn)
         .Map("col_person", x => x.CompositeTypeColumn);

var entities = new List<SampleEntity>();

entities.Add(new SampleEntity
{
    TextColumn = "0",
    CompositeTypeColumn = new PersonType { FirstName = "Fake", LastName = "Fakerton", BirthDate = new DateTime(1987, 1, 11) }
});

entities.Add(new SampleEntity
{
    TextColumn = "1",
    CompositeTypeColumn = new PersonType { FirstName = "Philipp", LastName = "Wagner", BirthDate = new DateTime(1912, 1, 11) }
});

subject.SaveAll(connection, entities);

```

In the listing you see, that we need to tell Npgsql how to map the Postgres type using ``MapComposite<>``. This can 
be done per Connection like this:

```
connection.TypeMapper.MapComposite<PersonType>("sample.person_type");
```

Or you can alternatively set the Mapping globally at any place in your application using the ``NpgsqlConnection.GlobalTypeMapper``:

```
NpgsqlConnection.GlobalTypeMapper.MapComposite<PersonType>("sample.person_type");
```

## PostgreSQLCopyHelper.NodaTime: NodaTime Support ##

The [PostgreSQLCopyHelper.NodaTime](https://www.nuget.org/packages/PostgreSQLCopyHelper.NodaTime/) package extends PostgreSQLCopyHelper for [NodaTime](https://nodatime.org/) types. 

To install PostgreSQLCopyHelper.NodaTime, run the following command in the Package Manager Console:

```
PM> Install-Package PostgreSQLCopyHelper
```

It uses the [Npgsql.NodaTime plugin](https://www.npgsql.org/doc/types/nodatime.html), which needs to be enabled by running:

```csharp
using Npgsql;

// Place this at the beginning of your program to use NodaTime everywhere (recommended)
NpgsqlConnection.GlobalTypeMapper.UseNodaTime();

// Or to temporarily use NodaTime on a single connection only:
conn.TypeMapper.UseNodaTime();
```

For more details see the [Npgsql documentation for NodaTime](https://www.npgsql.org/doc/types/nodatime.html).

## Case-Sensitive Identifiers ##

By default the library does not apply quotes to identifiers, such as Table Names and Column Names. If you want PostgreSQL-conform quoting for identifiers, 
then use the ``UsePostgresQuoting`` method like this:

```csharp
var copyHelper = new PostgreSQLCopyHelper<MixedCaseEntity>("sample", "MixedCaseEntity")
                     .UsePostgresQuoting()
                     .MapInteger("Property_One", x => x.Property_One)
                     .MapText("Property_Two", x => x.Property_Two);
```

## License ##

PostgreSQLCopyHelper is licensed under the MIT License. See [LICENSE](LICENSE) for details.

Copyright (c) Philipp Wagner, Steven Yeh and [Contributors](https://github.com/PostgreSQLCopyHelper/PostgreSQLCopyHelper/graphs/contributors)
