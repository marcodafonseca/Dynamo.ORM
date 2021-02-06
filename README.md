# Dynamo.ORM

An async ORM built for Amazon Web Service's DynamoDb in .Net Standard

**This is still in beta**

Please don't hesitate to log issues or requests on GitHub.
We are working everyday to make this more and more robust to ensure it will help more developers.

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

### Version 0.1.4.5

- Updated dependency version numbers

### Version 0.1.4.4

- Added support for null/empty strings

## Important Links

- [Contributing Guidlines](CONTRIBUTING.md)
- [Code Of Conduct](CODE_OF_CONDUCT.md)
- [License](LICENSE)
