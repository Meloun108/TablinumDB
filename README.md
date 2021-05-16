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
      GroupId: ObjectId,
      NumberGroup: String,
      NumberGroupDate: ISODate,
      Location: Boolean,
    ]
  },
  From: ObjectId,
  Executor: ObjectId,
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
  GroupId: ObjectId,
  Name: String,
  RoleId: ObjectId,
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
