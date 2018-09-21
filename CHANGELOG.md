# CHANGELOG #

## 2.5.0 ##

* [See Changes](https://github.com/bytefish/PostgreSQLCopyHelper/compare/2.4.0...2.5.0)

- Drops support for `net45` and `net451`
- Upgrades Npgsql to 4.1
- SavelAll now returns a count of rows written

## 2.4.0 ##

* [See Changes](https://github.com/bytefish/PostgreSQLCopyHelper/compare/2.3.0...2.4.0)

This release reverted the behavior of automatically quoting all identifiers. In order to quote case-sensitive identifiers the new method ``UsePostgresQuoting`` can be used like this:

```csharp
var copyHelper = new PostgreSQLCopyHelper<MixedCaseEntity>("sample", "MixedCaseEntity")
                     .UsePostgresQuoting()
                     .MapInteger("Property_One", x => x.Property_One)
                     .MapText("Property_Two", x => x.Property_Two);
```

## 2.3.0 ##

* [See Changes](https://github.com/bytefish/PostgreSQLCopyHelper/compare/2.2.0...2.3.0)

In this release PostgreSQL-compatible quoting for case-sensitive identifiers is added as default. That means: All identifiers, such as schema names, table names and column names containing upper-case (or other special characters) will be quoted automatically.

### Please Note ###

* Release 2.4.0 reverts this behavior and requires it to be enabled explicitly with the UsePostgresQuoting method.

## 2.2.0 ##

* [See Changes](https://github.com/bytefish/PostgreSQLCopyHelper/compare/2.1.0...2.2.0)

Fixes the Null Value Handling with Npgsql 4.0.0, see PostgreSQLCopyHelper Issue #22.

Thanks to @say25 for fixing the bug, adding Unit Tests and improving the code.

## 2.1.0 ##

* [See Changes](https://github.com/bytefish/PostgreSQLCopyHelper/compare/2.0.0...2.1.0)

This version attempted to fix a Bug regarding null value handling. It didn't fix the problem and *this version shouldn't be used*. Please update to more recent versions of the library.

## 2.0.0 ##

* [See Changes](https://github.com/bytefish/PostgreSQLCopyHelper/compare/1.3.0...2.0.0)

Added support for Npgsql 4.0.0. Thanks to @marklahn for updating the COPY API usage to 4.0 and @wejto for bringing up the Npgsql issues regarding Nullable Types in early Npgsql 4.0.0 releases.

## 1.3.0 ##

* [See Changes](https://github.com/bytefish/PostgreSQLCopyHelper/compare/1.2.0...1.3.0)

Updated the package depdencies to Npgsql 3.2.6.

## 1.2.0 ##

* [See Changes](https://github.com/bytefish/PostgreSQLCopyHelper/compare/1.1.0...1.2.0)

Added Support for mapping String Arrays with the ``MapArray`` method:

* ``MapArray("col_array", x => x.Array)``

## 1.1.0 ##

* [See Changes](https://github.com/bytefish/PostgreSQLCopyHelper/compare/1.0.0...1.1.0)

Adding .netstandard1.3 as Target Framework to support .NET Core.

## 1.0.0 ##

* [See Changes](https://github.com/bytefish/PostgreSQLCopyHelper/compare/0.2...1.0.0)

Initial Release.
