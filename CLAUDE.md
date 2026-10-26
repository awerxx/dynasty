# Blake — instrukcje dla Claude

Specyfikacja produktu: `SPEC.md`. UI po polsku, kwoty w PLN (`pl-PL`).

## Stack i wersje
- .NET 10 / C# 14
- Blazor Web App, tryb **Interactive Server** (bez WASM)
- ASP.NET Core Identity, uwierzytelnianie cookies
- EF Core 10 + Npgsql (`Npgsql.EntityFrameworkCore.PostgreSQL` 10), PostgreSQL 17
- TUnit na Microsoft.Testing.Platform (`global.json`: `"test": { "runner": "Microsoft.Testing.Platform" }`)
- Wersje pakietów wyłącznie w `Directory.Packages.props`

## Architektura
- Projekty: `Domain`, `Application`, `Infrastructure`, `Api`, `Web`
- Referencje (nic poza tym):
  - `Domain` → nic
  - `Application` → `Domain`
  - `Infrastructure` → `Application`, `Domain`
  - `Api` → `Application`, `Infrastructure` (tylko rejestracja DI w `Program.cs`)
  - `Web` → `Application`, `Infrastructure` (tylko rejestracja DI w `Program.cs`)
  - `Web` i `Api` nie referują siebie nawzajem
- Gdzie co siedzi:
  - `Domain`: encje, value objects, reguły biznesowe; zero zależności od EF/ASP.NET
  - `Application`: interfejsy serwisów, DTO (z DataAnnotations), interfejsy repozytoriów
  - `Infrastructure`: `DbContext`, konfiguracje EF, migracje, Identity, implementacje serwisów z `Application`
  - `Api`: Minimal API wywołujące serwisy z `Application`
  - `Web`: komponenty Blazor wywołujące serwisy z `Application`

## Konwencje kodu
- PascalCase: klasy, metody, właściwości; `_camelCase`: pola prywatne
- Metody async mają sufiks `Async` i przyjmują `CancellationToken`
- Czas: `DateTime.UtcNow`
- PK encji domenowych: `Guid` v7 (`Guid.CreateVersion7()`), nadawany w domenie, nie przez bazę

## Wzorce
- Każda metoda serwisu przyjmuje `ownerId` jako jawny parametr, bez accessora bieżącego użytkownika w serwisach
- Ownership sprawdzany w zapytaniu: `.Where(x => x.Id == id && x.OwnerId == ownerId)`, nigdy pobranie + `if`
- Brak rekordu lub cudzy rekord → ta sama odpowiedź „nie znaleziono” (404 w Api)
- Walidacja: DataAnnotations na DTO (`EditForm` + `DataAnnotationsValidator` w Web, walidacja endpointów w Api)
- Migracje EF: nazwy opisowe w PascalCase (`AddSavingsAccountHistory`, nie `Migration2`)

## Zakazy
- `UserManager`/`SignInManager` nie są używane bezpośrednio w komponentach Blazor, tylko przez serwis z `Application`
- Zakaz `DateTime.Now`
- Zakaz hardcodowania sekretów i connection stringów; używaj `IConfiguration` i User Secrets
- Zakaz kontrolerów MVC; `Web` = Blazor, `Api` = Minimal API

## Testy
- TUnit; uruchamianie: `dotnet test`
- Integracyjne: Testcontainers (`Testcontainers.PostgreSql`), prawdziwy Postgres, bez EF InMemory
- Nazwy: `MethodName_Scenario_Expected` (np. `AddExpenseAsync_OtherOwnersCategory_ReturnsNotFound`)
- Struktura AAA z komentarzami `// Arrange`, `// Act`, `// Assert`
- Każdy nowy serwis z danymi użytkownika ma test izolacji: owner B nie widzi i nie modyfikuje danych ownera A

## Bezpieczeństwo
- User widzi tylko swoje dane: każde zapytanie filtruje po `OwnerId`
- Admin nie widzi dashboardu ani danych finansowych Usera: osobne layouty, `[Authorize(Roles = "...")]` na stronach i `RequireAuthorization` na endpointach
- Po dezaktywacji konta lub resecie hasła wywołaj `UserManager.UpdateSecurityStampAsync`, żeby unieważnić aktywne sesje
