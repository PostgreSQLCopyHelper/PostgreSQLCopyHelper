# PostgreSQLCopyHelper #

[![stable](https://img.shields.io/nuget/v/PostgreSQLCopyHelper.svg?label=stable)](https://www.nuget.org/packages/PostgreSQLCopyHelper/)

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

```csharp
private void WriteToDatabase(PostgreSQLCopyHelper<TestEntity> copyHelper, IEnumerable<TestEntity> entities)
{
    using (var connection = new NpgsqlConnection("Server=127.0.0.1;Port=5432;Database=sampledb;User Id=philipp;Password=test_pwd;"))
    {
        connection.Open();

        copyHelper.SaveAll(connection, entities);
    }
}
```

## Case-Sensitive Identifiers ##

By default the library does not apply quotes to identifiers, such as Table Names and Column Names. If you want PostgreSQL-conform quoting for identifiers, 
then use the ``UsePostgresQuoting`` method like this:

```csharp
var copyHelper = new PostgreSQLCopyHelper<MixedCaseEntity>("sample", "MixedCaseEntity")
                     .UsePostgresQuoting()
                     .MapInteger("Property_One", x => x.Property_One)
                     .MapText("Property_Two", x => x.Property_Two);
```
