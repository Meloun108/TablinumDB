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

`mongoexport.exe --collection=collection_name --db=database_name --out=filename.json`
