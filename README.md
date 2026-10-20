# READINESS

## GoodReads look a like API

### How to run everything
```bash
# First execute the docker container
docker compose up -d # -d = detach, it simply doens't put you into a console
# Secondly just run the dotnet API, you can either run from the editor itself, or use the cli as follows
dotnet run # If only foor debug purposes run dotnet build, it ***WON'T*** apply the migrations
```

#### How to stop the program with a clean db
Simply run:
```bash
docker compose down
```
This will remove the container, but keep the images stored locally to be used another time.
Afterwards, you can simply re-run the program as specified before and you will have a clean db

### Precautions

**DO NOT** run the following commands unless really necessary, and you should warn other people on that they were run.
```bash
dotnet ef migrations add <name of migration> ## This creates a new migration, normally they are done when there are changes on the entities for the database
```

```bash
dotnet ef database update ## This updates the migration on the database, it's an apply for the newly created migration, this isn't that bad to apply, but the Program.cs file already does that for us
```
