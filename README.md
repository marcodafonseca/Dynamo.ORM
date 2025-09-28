# Dynamo.ORM

An async ORM built for Amazon Web Service's DynamoDb in .Net Standard
**This is still in beta**

Please don't hesitate to log issues or requests on GitHub.
We are working everyday to make this more and more robust to ensure it will help more developers.

The examples below use the local Amazon DynamoDb you would setup on your machine

## Example Usage

### Example setup of a table model called 'People'

```c#
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

### Example configuration

```c#
var config = new AmazonDynamoDBConfig
{
    ServiceURL = "http://localhost:8000/"
};
var client = new AmazonDynamoDBClient(config);
var repository = new Repository(client);
```

### Example adding a Person with the HashKey '1'

```c#
var model = new PersonModel();

model.Id = 1;
model.FirstName = "John";
model.LastName = "Smith";
model.CreatedDate = DateTime.Now;

await repository.Add(model);
```

### Example getting a Person entry with the HashKey '1'

```c#
var entity = await repository.Get<PersonModel>(1);
```

### Example updating a Person entry with the HashKey '1'

```c#
var model = new PersonModel();

model.Id = 1;
model.FirstName = "John";
model.LastName = "Smith";
model.CreatedDate = DateTime.Now;

await repository.Update(model);
```

### Example deleting a Person entry with the HashKey '1'

```c#
await repository.Delete<PersonModel>(1);
```

### If you don't want to use the Table Name attribute you can provide the Table Name as a parameter on repository Call. Examples of calls made with additional parameter

```c#

var model = new PersonModel();

model.Id = 1;
model.FirstName = "John";
model.LastName = "Smith";
model.CreatedDate = DateTime.Now;

await repository.Add(model, "tableName");
await repository.Get<PersonModel>(model.Id, "tableName");
await repository.Get<PersonModel>(x => x.FirstName == "Fake", "tableName");
await repository.List<PersonModel>(x => x.Id < 20 && x.Id >= 10, "tableName");
await repository.Update(model, "tableName");
await repository.Delete(model, "tableName");
```

## Release Notes

Click [here](ReleaseNotes.md) to view all the release notes

## Version 0.4.0

- **! Breaking Changes !** - Changed return type of List function from List to IList
- Added pagination parameters to existing List method
- Added custom exception for paging, PageNotFoundException
- Added method for retrieving approximate item count for a table.

## Important Links

- [Contributing Guidlines](CONTRIBUTING.md)
- [Code Of Conduct](CODE_OF_CONDUCT.md)
- [License](LICENSE)
