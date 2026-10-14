# NWSDB Billing and Payment System

An academic Service Oriented Computing project demonstrating water billing and third-party payment collection. This is a local learning application, not an official NWSDB product. The Bank API simulates approval and decline; it does not transfer money or connect to a real bank.

## Applications

| Application | Solution | Responsibility |
| --- | --- | --- |
| NWSDB API | `nwsdb API/nwsdb_API/nwsdb_API.slnx` | Customer accounts, readings, bills, payment coordination and receipts |
| Bank API | `bank_API/bank_API/bank_API.slnx` | Simulated payment outcomes and lookup by payment reference |
| Website | `nwsdb webclient/website/website.slnx` | Customer dashboard and staff management |
| Desktop | `nwsdb desktop client/desktop client/desktop client.slnx` | Partner cashier bill lookup, payments and receipts |

```text
Customer/staff website ----> NWSDB API ----> NWSDB database
Partner desktop client --->     |
                                +---------> Bank API ----> Bank database
```

Clients call the NWSDB API rather than connecting to SQL Server. Calling the Bank API directly does not settle an NWSDB bill.

## Stack and requirements

- Windows and Visual Studio with ASP.NET/web and .NET desktop development workloads, .slnx support, and the .NET 10 SDK.
- .NET Framework 4.7.2 targeting pack for the Windows Forms project.
- SQL Server (Express is sufficient for this local demonstration); SSMS is useful for database setup.
- NuGet restore for the versions in each project file. APIs use EF Core, AutoMapper and Scalar; the NWSDB API also uses JWT bearer authentication.
- Trust the local HTTPS development certificate using `dotnet dev-certs https --trust` on the development machine.

## Local configuration

Private connection settings, password notes and database files are intentionally not tracked. Moving/cloning the source does not copy SQL Server databases or another user's User Secrets.

For each API, copy `appsettings.Development.example.json` to `appsettings.Development.json` in the same project directory. Replace the placeholder values locally:

- Set the same new service key in Bank API `ServiceAuthentication:Key` and NWSDB API `BankApi:ServiceKey`.
- Set a separate random NWSDB `Jwt:Key` of at least 32 characters.
- Set `Seed:Password` to a local demonstration password. All initially seeded users receive this password.

Use Visual Studio **Manage User Secrets** on each API to set its connection string. The configuration property is `ConnectionStrings:conn`. Templates in `appsettings.json` are placeholders, not usable SQL credentials.

Example shape (replace the placeholders on your machine):

```json
{
  "ConnectionStrings": {
    "conn": "Server=YOUR_SERVER;Database=NwsdbCoursework;User Id=YOUR_SQL_LOGIN;Password=YOUR_SQL_PASSWORD;Encrypt=True;TrustServerCertificate=True"
  }
}
```

For Bank API, use `Database=BankDemoCoursework` and its separate SQL login. `TrustServerCertificate=True` is for this local development setup, not a production certificate strategy.

Create the two databases using a database administrator account. Map each runtime SQL login only to its corresponding database, with read/write permissions. Schema creation and migrations require additional privileges; use an administrator-controlled connection for those operations instead of granting the runtime login sysadmin rights.

## Database schema and initial data

Open each API solution and restore NuGet packages. In Visual Studio's Package Manager Console, select the API as the default project and apply the existing migrations:

```powershell
# Bank API solution; use an administrator connection for YOUR_SERVER.
Update-Database -Context BankDbContext -Connection "Server=YOUR_SERVER;Database=BankDemoCoursework;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True"

# NWSDB API solution
Update-Database -Context AppDbContext -Connection "Server=YOUR_SERVER;Database=NwsdbCoursework;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True"
```

The Windows account running these commands must have the necessary SQL permissions. Adapt the connection for your environment.

From the NWSDB API project directory (`nwsdb API/nwsdb_API/nwsdb_API`), initialise the fictional demo data:

```powershell
dotnet run --launch-profile https -- --seed --ConnectionStrings:conn="Server=YOUR_SERVER;Database=NwsdbCoursework;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True"
```

This command performs migrations before seeding and exits afterwards. The command-line connection applies to that invocation only. Normal runs use your configured runtime connection. Seeding skips insertion if users already exist; it is not a reset or a database backup.

| Username | Application / role |
| --- | --- |
| `staff` | Website: staff |
| `customer` | Website: customer for W-1001 |
| `customer2` | Website: second customer for W-1002 |
| `cashier` | Desktop: partner cashier |
| `cashier2` | Desktop: second partner cashier |

Use the `Seed:Password` configured before seeding. Changing that setting later does not change existing users' passwords. Records added during local manual practice are not recreated by the initial seed.

## Run locally

1. Start Bank API using its `https` launch profile: `https://localhost:7182/scalar/v1`.
2. Start NWSDB API using its `https` launch profile: `https://localhost:7180/scalar/v1`.
3. Start the website: `https://localhost:7181`.
4. Start the Windows Forms desktop project.

If ports change, update NWSDB `BankApi:BaseUrl`, website `NwsdbApi:BaseUrl`, and desktop `NwsdbApiUrl` in `App.config`.

Scalar uses the login response's access token for NWSDB bearer authentication. Bank requests use `X-Service-Key`. Do not publish these values in screenshots.

## Payment workflow

1. The cashier looks up an unpaid bill through NWSDB API.
2. NWSDB validates the caller and full bill amount, then saves a pending attempt.
3. NWSDB sends the saved reference to Bank API.
4. Bank API records an approved or declined simulated outcome.
5. NWSDB validates the bank response and records completion/decline. Completed payments provide receipts.
6. A pending/uncertain outcome is checked using the original reference rather than assuming failure and creating another payment.

Use `demo-approve` and `demo-decline` only as demonstration inputs. Repeating the same bank reference with identical details returns the recorded result; conflicting details are rejected. Database constraints and transactions support duplicate protection, but this is not a claim of production-grade exactly-once processing across services.

## Verification and limitations

Manual exercises during development covered login/account retrieval, malformed GUID rejection, role-restricted deletion, searching, removal of unused test records and desktop-to-website payment processing. Code includes duplicate-bill and linked-record checks. This repository does not include a comprehensive automated test suite or verified production deployment.

All three desktop forms were previously loadable in a separate Windows Forms design host, but Visual Studio Designer intermittently reported that the Form base class could not be loaded. Resolution of the IDE issue is not confirmed.

The website uses in-memory sessions. Multi-instance hosting needs shared session storage and data-protection configuration. Load testing, concurrency/failure testing, production secret management, deployment automation and monitoring are future work. Check third-party package licensing before non-development use.

## Source layout

API code is organised into `Model`, `Data` (contexts/repositories), `DTO`, `Profiles`, `Services`, `Controllers` and `Migrations`. The website uses Razor Pages and a dedicated HTTP API client. The desktop uses Windows Forms and its own HTTP API client.
