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

### Authentication

The login endpoint is `POST /api/auth/login`. JWT signing keys must not be stored in `appsettings.json` or committed to source control. For local PowerShell development, generate a temporary key in the same terminal used to start the API:

```powershell
$rng = [Security.Cryptography.RandomNumberGenerator]::Create()
$bytes = New-Object byte[] 32
$rng.GetBytes($bytes)
$rng.Dispose()

$env:Jwt__SigningKey = [Convert]::ToBase64String([Security.Cryptography.RandomNumberGenerator]::GetBytes(32))
dotnet run
```

The JWT expires after 15 minutes by default. It is returned only in the `readiness_auth` cookie (`HttpOnly`, `Secure`, `SameSite=Strict`); the login response contains the user's public profile, not the token.

Because browsers attach cookies automatically, browser clients must first call `GET /api/auth/csrf-token`, retain the returned `token`, and send it as the `X-CSRF-TOKEN` header on `POST /api/auth/login` and `POST /api/auth/logout`. Future authenticated state-changing endpoints must use the same anti-forgery validation. Run the API over HTTPS, as the authentication and anti-forgery cookies are marked `Secure`.

`POST /api/auth/logout` requires a valid JWT cookie and anti-forgery token. It stores the JWT's `jti` in the `RevokedTokens` table until its expiration and clears the browser cookie. The authentication middleware checks this table for each authenticated request, so revocation remains effective after application restarts and across API instances. Apply the pending EF Core migration before using login/logout. Logout without a valid token returns `401 Unauthorized`.

`POST /api/auth/change-password` requires the current password, a new password following the registration rules, a valid JWT cookie, and an anti-forgery token. The user ID is read from the token's `sub` claim; clients must not send a user ID. On success, the new password is stored as a password hash.

`PUT /api/users/me` updates the authenticated user's email, name, and year of birth. It requires a valid JWT cookie and anti-forgery token. The user ID comes from the JWT `sub` claim; the request cannot change the user ID, role, or password hash. Email addresses must be unique.
