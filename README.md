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

### Version 0.0.3.0

* Changed Add, Update and Delete functions to be run asynchronously as Tasks
* Added "null" support for expressions
* Updated repository tests
* Updated NuGet packages
* Added simple benchmarking application (Dynamo.ORM.Benchmarks)

## Important Links
* [Contributing Guidlines](CONTRIBUTING.md)
* [Code Of Conduct](CODE_OF_CONDUCT.md)
* [License](LICENSE)
