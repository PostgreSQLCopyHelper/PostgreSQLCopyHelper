# PostgreSQLCopyHelper #

Hiding the COPY methods from Npgsql behind a nice Fluent API.

## Basic Usage ##

Imagine we have the following table we want to copy data to:

```sql
CREATE TABLE sample.local_weather
(
  local_weather_id serial NOT NULL,
  wban text,
  sky_condition text,
  date date  
)
```

The corresponding domain model in our application could look like this:

```csharp
public class LocalWeatherData
{
    public string WBAN { get; set; }

    public DateTime Date { get; set; }

    public string SkyCondition { get; set; }
}
```

The PostgreSQLCopyHelper now defines the mapping between domain model and the database table:

```csharp
var copyHelper = new PostgreSQLCopyHelper<LocalWeatherData>()
    .WithTableName("sample", "local_weather")
    .AddColumn("wban", x => x.WBAN)
    .AddColumn("sky_condition", x => x.SkyCondition)
    .AddColumn("date", x => x.Date);
``

And then we can use it to efficiently store the data:

```csharp
private void WriteToDatabase(PostgreSQLCopyHelper<LocalWeatherData> copyHelper, IEnumerable<LocalWeatherData> entities)
{
    using (var connection = new NpgsqlConnection("Server=127.0.0.1;Port=5432;Database=sampledb;User Id=philipp;Password=test_pwd;"))
    {
        connection.Open();

        copyHelper.SaveAll(connection, entities);
    }
}
```

