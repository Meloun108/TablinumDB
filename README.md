# TablinumDB
## Use database:

`use Tablinum`

## DataTables:

`db.Documents.find({}).pretty();`
`db.Users.find({}).pretty();`

## Default paths:

### Windows
`C:\data\db`
### MacOS
`/data/db`

## Export/Import collection databases:

### Export
`mongoexport.exe --collection=collection_name --db=database_name --out=filename.json`
### Import
`mongoimport.exe -d database_name -c collection_name --file filename.json`
