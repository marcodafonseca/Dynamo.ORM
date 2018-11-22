# Dynamo.ORM
An async ORM built for Amazon Web Service's DynamoDb in .Net Standard

**This is still in alpha**

The examples below use the local Amazon DynamoDb you would setup on your machine

## Example Usage

###### Example setup of a table model called 'People'
```
[DynamoDBTable("People")]
public class PersonModel : Base
{
    [DynamoDBHashKey]
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime CreatedDate { get; set; }
}
```
###### Example configuration
```
var config = new AmazonDynamoDBConfig
{
    ServiceURL = "http://localhost:8000/"
};
var client = new AmazonDynamoDBClient(config);
var repository = new Repository(client);
```
###### Example adding a Person with the HashKey '1'
```
var model = new PersonModel();

model.Id = 1;
model.FirstName = "John";
model.LastName = "Smith";
model.CreatedDate = DateTime.Now;

await repository.Add(model);
```
###### Example getting a Person entry with the HashKey '1'
```
var entity = await repository.Get<PersonModel>(1);
```
###### Example updating a Person entry with the HashKey '1'
```
var model = new PersonModel();

model.Id = 1;
model.FirstName = "John";
model.LastName = "Smith";
model.CreatedDate = DateTime.Now;

await repository.Update(model);
```
###### Example deleting a Person entry with the HashKey '1'
```
await repository.Delete<PersonModel>(1);
```

## Release Notes

Click [here](ReleaseNotes.md) to view all the release notes

### Version 0.0.4.0
* Added support for more data types
* * byte
* * byte?
* * byte[]
* * char
* * char?
* * decimal
* * decimal?
* * double
* * double?
* * float
* * float?
* * Int16 (short)
* * Int16? (short?)
* * Int64 (long)
* * Int64? (long?)
* * UInt16 (ushort)
* * UInt16? (ushort?)
* * UInt32 (uint)
* * UInt32? (uint?)
* * UInt64 (ulong)
* * UInt64? (ulong?)
* Increased support for nullable types

## Important Links
* [Contributing Guidlines](CONTRIBUTING.md)
* [Code Of Conduct](CODE_OF_CONDUCT.md)
* [License](LICENSE)
