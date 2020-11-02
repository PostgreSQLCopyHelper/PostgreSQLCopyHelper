# CHANGELOG #

## 2.7.0 ##

* [See Changes](https://github.com/PostgreSQLCopyHelper/PostgreSQLCopyHelper/compare/2.6.3...2.7.0)

#### This release: 

- Add NodaTime Support to PostgreSQLCopyHelper

## 2.6.3 ##

* [See Changes](https://github.com/PostgreSQLCopyHelper/PostgreSQLCopyHelper/compare/2.6.2...2.6.3)

#### This release: 

- Adds the Property ``TargetTable`` and associated classes to get the Table and Column mapping

## 2.6.2 ##

* [See Changes](https://github.com/PostgreSQLCopyHelper/PostgreSQLCopyHelper/compare/2.6.1...2.6.2)

### This release:

- Adds a dependency to ``Microsoft.Bcl.AsyncInterfaces`` to enable ``IAsyncEnumerable`` for .NET 4.6.1 and netstandard 2.0

## 2.6.1 ##

* [See Changes](https://github.com/PostgreSQLCopyHelper/PostgreSQLCopyHelper/compare/2.6.0...2.6.1)

#### This release: 

- Executes asynchronous methods without the ``SynchronizationContext`` to prevent blocking from the UI Thread

## 2.6.0 ##

* [See Changes](https://github.com/PostgreSQLCopyHelper/PostgreSQLCopyHelper/compare/2.5.1...2.6.0)

#### This release: 

- Exposes ability to use IAsyncEnumerable
- Better support when cancelling `SaveAllAsync`

## 2.5.1 ##

* [See Changes](https://github.com/PostgreSQLCopyHelper/PostgreSQLCopyHelper/compare/2.5.0...2.5.1)

#### This release: 
- Takes advantage of C#8 await using (IAsyncDisposable)
- Bumps to Npgsql `4.1.1` which removes some unintentional breaking changes in the below library

## 2.5.0 ##

* [See Changes](https://github.com/PostgreSQLCopyHelper/PostgreSQLCopyHelper/compare/2.4.0...2.5.0)

#### This release: 
- Drops support for `net45`, `net451` and `net452`
- Upgrades Npgsql to `4.1`
- SaveAll now returns a count of rows written
- Async Support

## 2.4.2 ##

* [See Changes](https://github.com/bytefish/PostgreSQLCopyHelper/compare/2.3.0...2.4.2)

Added the following methods for improved ``DateTime`` and ``DateTimeOffset`` mappings, see [Issue #9]():

* ``MapTimeStampTz`` (for ``DateTimeOffset``, ``DateTimeOffset?``)
* ``MapTimeTz``

## 2.4.1 ##

* [See Changes](https://github.com/PostgreSQLCopyHelper/PostgreSQLCopyHelper/compare/2.3.0...2.4.1)

* Reference Npgsql `4.0.4`, see Issue #38.

## 2.4.0 ##

* [See Changes](https://github.com/PostgreSQLCopyHelper/PostgreSQLCopyHelper/compare/2.3.0...2.4.0)

This release reverted the behavior of automatically quoting all identifiers. In order to quote case-sensitive identifiers the new method ``UsePostgresQuoting`` can be used like this:

```csharp
var copyHelper = new PostgreSQLCopyHelper<MixedCaseEntity>("sample", "MixedCaseEntity")
                     .UsePostgresQuoting()
                     .MapInteger("Property_One", x => x.Property_One)
                     .MapText("Property_Two", x => x.Property_Two);
```

## 2.3.0 ##

* [See Changes](https://github.com/PostgreSQLCopyHelper/PostgreSQLCopyHelper/compare/2.2.0...2.3.0)

In this release PostgreSQL-compatible quoting for case-sensitive identifiers is added as default. That means: All identifiers, such as schema names, table names and column names containing upper-case (or other special characters) will be quoted automatically.

### Please Note ###

* Release 2.4.0 reverts this behavior and requires it to be enabled explicitly with the UsePostgresQuoting method.

## 2.2.0 ##

* [See Changes](https://github.com/PostgreSQLCopyHelper/PostgreSQLCopyHelper/compare/2.1.0...2.2.0)

Fixes the Null Value Handling with Npgsql `4.0.0`, see PostgreSQLCopyHelper Issue #22.

Thanks to @say25 for fixing the bug, adding Unit Tests and improving the code.

## 2.1.0 ##

* [See Changes](https://github.com/PostgreSQLCopyHelper/PostgreSQLCopyHelper/compare/2.0.0...2.1.0)

This version attempted to fix a Bug regarding null value handling. It didn't fix the problem and *this version shouldn't be used*. Please update to more recent versions of the library.

## 2.0.0 ##

* [See Changes](https://github.com/PostgreSQLCopyHelper/PostgreSQLCopyHelper/compare/1.3.0...2.0.0)

Added support for Npgsql `4.0.0`. Thanks to @marklahn for updating the COPY API usage to 4.0 and @wejto for bringing up the Npgsql issues regarding Nullable Types in early Npgsql `4.0.0` releases.

## 1.3.0 ##

* [See Changes](https://github.com/PostgreSQLCopyHelper/PostgreSQLCopyHelper/compare/1.2.0...1.3.0)

Updated the package depdencies to Npgsql `3.2.6`.

## 1.2.0 ##

* [See Changes](https://github.com/PostgreSQLCopyHelper/PostgreSQLCopyHelper/compare/1.1.0...1.2.0)

Added Support for mapping String Arrays with the ``MapArray`` method:

* ``MapArray("col_array", x => x.Array)``

## 1.1.0 ##

* [See Changes](https://github.com/PostgreSQLCopyHelper/PostgreSQLCopyHelper/compare/1.0.0...1.1.0)

Adding `netstandard1.3` as Target Framework to support .NET Core.

## 1.0.0 ##

* [See Changes](https://github.com/PostgreSQLCopyHelper/PostgreSQLCopyHelper/compare/0.2...1.0.0)

Initial Release.
