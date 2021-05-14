# TablinumDB
## Use database:
`use Tablinum`

## DataTables find data:
`db.collection_name.find({}).pretty();`

## Default paths:
### Windows
`C:\data\db`
### Linux & MacOS
`/data/db`

## Export/Import collection databases:
### Export
`mongoexport.exe --collection=collection_name --db=database_name --out=filename.json`
or
`mongodump.exe -d database_name -o directory_backup`
### Import
`mongoimport.exe -d database_name -c collection_name --file filename.json`
or
`mongorestore.exe -d database_name directory_backup`

# Schema
## Documents
```
{
  _id: ObjectId,
  Number: String,
  NumberDate: ISODate,
  NumberCenter: String,
  NumberCenterDate: ISODate,
  NumberDepartment: String,
  NumberDeprtmentDate: ISODate,
  GroupInfo:
  {
    [
      Group:
      {
        _id: ObjectId,
        dept: String,
      },
      NumberGroup: String,
      NumberGroupDate: ISODate,
      LocationThis: Boolean,
    ]
  },
  From:
  {
    _id: ObjectId,
    InitioName: String,
  },
  Executor:
  {
    _id: ObjectId,
    Login: String,
    Password: Hash,
    Group:
    {
      _id: ObjectId,
      Dept: String,
    },
    Name: String,
    Role:
    {
      _id: ObjectId,
      RoleName: String,
    },
  },
  ExecutionDate: ISODate,
  Status: Boolean,
  View: String,
  Speed: String,
  Control: Boolean,
  Comment: String,
  Created: ISODate,
  Updated:
  {
    [
      ISODate,
    ],
  }
}
```

## Users
```
{
  _id: ObjectId,
  Login: String,
  Password: Hash,
  Group:
  {
    _id: ObjectId,
    Dept: String,
  },
  Name: String,
  Role:
  {
    _id: ObjectId,
    RoleName: String,
  },
}
```

## Roles
```
{
  _id: ObjectId,
  RoleName: String,
}
```

## Groups
```
{
  _id: ObjectId,
  Dept: String,
}
```

## Initio
```
{
  _id: ObjectId,
  InitioName: String,
}
```
