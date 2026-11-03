# READINESS

## GoodReads look a like API

### How to run everything

There are two main ways to run the Rest API, the first one with the run scripts and the second one with sequential commands
**1) Run scripts**
You can choose to run either depending on you operating system, the -d flag allows you to also call the docker compose file (detached from terminal)
```bash
./run.bat # This is for windows (ends in .bat)
sh ./run.sh # This is for linux users (if you use git bash)
##Docker Flag
./run.bat -d | --d | docker
sh ./run.sh -d | --d | docker
```

**2) Sequential commands**
```bash
# First execute the docker container
docker compose up -d # -d = detach, it simply doens't put you into a console
# Secondly just run the dotnet API, you can either run from the editor itself, or use the cli as follows
dotnet run # If only for debug purposes run 'dotnet build', it WON'T apply the migrations
```

#### How to stop the program with a clean db
Simply run:
```bash
docker compose down
```
This will remove the container, but keep the images stored locally to be used another time.
Afterwards, you can simply re-run the program as specified before and you will have a clean db

### Precautions

**DO NOT** run the following commands unless really necessary, and you should warn other people.
```bash
dotnet ef migrations add <name of migration> ## This creates a new migration, normally they are done when there are changes on the entities for the database
```

```bash
dotnet ef database update ## This updates the migration on the database, it's an apply for the newly created migration, this isn't that bad to apply, but the Program.cs file already does that for us
```


### Testing

In order to run the tests call `dotnet test readiness.test`, this will call and run the unit tests present in that folder

If you want to check the coverage report run the following command:
```bash
dotnet test readiness.Tests --collect:"XPlat Code Coverage" --settings readiness.Tests/coverage.runsettings
```

This will generate a "coverage.cobertura.xml" file which is not very readable, for this you can run the following command:

```bash
#Linux or git bash
dotnet reportgenerator \
  -reports:"readiness.Tests/TestResults/**/coverage.cobertura.xml" \
  -targetdir:readiness.Tests/coveragereport \
  -classfilters:"+*Service" \
  -reporttypes:Html

#Windows
dotnet reportgenerator -reports:"readiness.Tests/TestResults/**/coverage.cobertura.xml" -targetdir:readiness.Tests/coveragereport -classfilters:"+*Service" -reporttypes:Html
``` 

*Note*: To run this report generator tool, you have to install it with `dotnet tool install -g dotnet-reportgenerator-globaltool`

