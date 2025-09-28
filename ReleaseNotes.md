# Release Notes

## Version 0.5.0

- Documented the Repository class and interface
- Upgraded dependencies and fixed code to work with newer dependencies
- Fixed typo in README

## Version 0.4.0

- **! Breaking Changes !** - Changed return type of List function from List to IList
- Added pagination parameters to existing List method
- Added custom exception for paging, PageNotFoundException
- Added method for retrieving approximate item count for a table.

## Version 0.3.3

- Implemented fix for handling of nullable IDictionary<,>

## Version 0.3.2

- Improved handling of `null` properties

## Version 0.3.1

- Added CancellationToken for better handling of async calls

## Version 0.3.0

- Added ability to pass in custom table names to the repository functions at runtime

## Version 0.2.1

- Ensured support for v6.0

## Version 0.2.0

- Added support for IDictionary

## Version 0.1.4.6

- Changed AWSSDK.DynamoDBv2 version reference to 3.\*

## Version 0.1.4.5

- Updated dependency version numbers

## Version 0.1.4.4

- Resolved [Does not supporting saving of IEnumerable](https://github.com/marcodafonseca/Dynamo.ORM/issues/4)

## Version 0.1.4.3

- Resolved [Added support for null/empty strings](https://github.com/marcodafonseca/Dynamo.ORM/issues/3)

## Version 0.1.3.0

- Added transaction support for Add, Update and Delete

## Version 0.1.2.0

- Added Guid support

## Version 0.1.1.0

- Added ability to read and write mapped property fields

## Version 0.1.0.0

- Resolved [Can't call inline functions in expressions](https://github.com/marcodafonseca/Dynamo.ORM/issues/1)
- Resolved [Can't query database with particular types](https://github.com/marcodafonseca/Dynamo.ORM/issues/2)

## Version 0.0.4.0

- Added support for more data types
  - byte
  - byte?
  - byte[]
  - char
  - char?
  - decimal
  - decimal?
  - double
  - double?
  - float
  - float?
  - Int16 (short)
  - Int16? (short?)
  - Int64 (long)
  - Int64? (long?)
  - UInt16 (ushort)
  - UInt16? (ushort?)
  - UInt32 (uint)
  - UInt32? (uint?)
  - UInt64 (ulong)
  - UInt64? (ulong?)
- Increased support for nullable types

## Version 0.0.3.0

- Changed Add, Update and Delete functions to be run asynchronously as Tasks
- Added "null" support for expressions
- Updated repository tests
- Updated NuGet packages
- Added simple benchmarking application (Dynamo.ORM.Benchmarks)
