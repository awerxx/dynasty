# Blake — specyfikacja MVP

## 1. Kontekst i stack

Blake to aplikacja do prowadzenia budżetu domowego: planowanie miesięcznych przychodów i wydatków, rejestrowanie wydatków faktycznych, porównanie planu z wykonaniem oraz śledzenie sald na kontach oszczędnościowych.

- .NET 10, C#
- Blazor Web App, tryb renderowania **Interactive Server**
- ASP.NET Core Identity, uwierzytelnianie cookies
- EF Core + PostgreSQL (Npgsql)
- Testy: TUnit
- UI po polsku, kwoty w PLN (kultura `pl-PL`)

## 2. Architektura (w skrócie)

| Warstwa | Odpowiedzialność | Zależy od |
|---|---|---|
| **Domain** | Encje, value objects (`Money`, `YearMonth`), reguły biznesowe. Bez zależności od frameworków. | — |
| **Application** | Interfejsy serwisów, DTO z DataAnnotations, interfejsy repozytoriów. | Domain |
| **Infrastructure** | `DbContext` (EF Core + Postgres), migracje, Identity, implementacje serwisów z Application. | Application, Domain |
| **Api** | Minimal API (JSON) wywołujące serwisy z Application, autoryzacja cookie. | Application, Infrastructure |
| **Web** | Strony Blazor (Interactive Server), strony Identity; wywołuje serwisy z Application in-process. | Application, Infrastructure |

- Każda metoda serwisu przyjmuje `ownerId` jako jawny parametr; Web/Api przekazują Id zalogowanego użytkownika.
- Ownership sprawdzany w zapytaniu (`Where(... && x.OwnerId == ownerId)`); brak rekordu i cudzy rekord dają ten sam wynik „nie znaleziono”.
- Role: `User`, `Admin`. Polityki autoryzacji zdefiniowane centralnie i współdzielone przez Web i Api.

## 3. Wymagania funkcjonalne MVP

**Konto i dostęp**
- F1. Rejestracja, logowanie, wylogowanie (Identity, cookies).
- F2. Admin: lista użytkowników, blokowanie/odblokowanie konta, nadawanie/odbieranie roli `Admin`.

**Kategorie**
- F3. CRUD kategorii wydatków i przychodów użytkownika; przy rejestracji tworzony jest zestaw domyślny.

**Plan miesiąca**
- F4. Utworzenie planu dla wybranego miesiąca: pozycje planowanych przychodów i wydatków (kategoria, kwota, opis).
- F5. Utworzenie planu kolejnego miesiąca przez skopiowanie pozycji z poprzedniego.
- F6. Edycja i usuwanie pozycji planu w otwartym miesiącu.

**Wydatki faktyczne**
- F7. Dodawanie, edycja, usuwanie wydatków/przychodów (data, kwota, kategoria, opis).
- F8. Lista transakcji miesiąca z filtrem po kategorii.

**Stan bieżący**
- F9. Pulpit miesiąca: plan vs wykonanie per kategoria i łącznie, pozostały budżet, procent wykorzystania; przekroczenia oznaczone wizualnie.

**Oszczędności**
- F10. CRUD kont oszczędnościowych (nazwa, opcjonalny opis).
- F11. Aktualizacja salda konta (kwota, data) — zapisywana jako nowy wpis historii.
- F12. Widok sumy oszczędności oraz historii sald każdego konta.

## 4. User stories

1. Jako **użytkownik** chcę zaplanować przychody i wydatki na nadchodzący miesiąc, żeby z góry wiedzieć, ile mogę wydać.
2. Jako **użytkownik** chcę skopiować plan z poprzedniego miesiąca, żeby nie wpisywać co miesiąc tych samych stałych pozycji.
3. Jako **użytkownik** chcę szybko dodać wydatek z kategorią, żeby na bieżąco rejestrować wykonanie budżetu.
4. Jako **użytkownik** chcę widzieć plan vs wykonanie per kategoria, żeby wiedzieć, gdzie przekraczam budżet.
5. Jako **użytkownik** chcę aktualizować salda kont oszczędnościowych, żeby znać łączną wartość oszczędności i jej zmiany w czasie.
6. Jako **użytkownik** chcę mieć pewność, że nikt inny nie widzi moich danych, żeby bezpiecznie prowadzić w aplikacji finanse.
7. Jako **admin** chcę blokować konta i zarządzać rolami, żeby kontrolować dostęp do aplikacji.

## 5. Kryteria akceptacji

- **AK1 (izolacja):** Żądanie użytkownika A o zasób użytkownika B (Web i Api) zwraca 404; pokryte testami integracyjnymi dla każdego typu zasobu (kategoria, plan, transakcja, konto oszczędnościowe).
- **AK2 (role):** Strony i endpointy admina zwracają 403 dla roli `User`; niezalogowany użytkownik jest przekierowywany na logowanie (Web) lub dostaje 401 (Api).
- **AK3 (kopiowanie planu):** Kopia planu zawiera dokładnie te same pozycje (kategoria, kwota, opis) co miesiąc źródłowy; próba skopiowania do miesiąca, który ma już plan, kończy się błędem walidacji bez zmian w danych.
- **AK4 (pulpit):** Dla danych testowych suma „wykonanie” na pulpicie równa się sumie transakcji miesiąca co do grosza; pozostały budżet = plan wydatków − wykonanie wydatków.
- **AK5 (oszczędności):** Po aktualizacji salda wyświetlana kwota konta to wpis z najpóźniejszą datą; poprzednie wpisy są widoczne w historii; suma oszczędności = suma bieżących sald aktywnych kont.
- **AK6 (walidacja):** Kwota ≤ 0, więcej niż 2 miejsca po przecinku, brak kategorii lub data spoza miesiąca są odrzucane z komunikatem po polsku.
- **AK7 (wydajność):** Pulpit miesiąca z 500 transakcjami ładuje się < 1 s na lokalnym Postgresie.
- **AK8 (jakość):** Reguły biznesowe z sekcji 6 mają testy jednostkowe w TUnit; `dotnet test` przechodzi na czysto.

## 6. Reguły biznesowe

1. **Własność danych:** każda encja domenowa ma `OwnerId`; użytkownik odczytuje i modyfikuje wyłącznie własne dane. Admin zarządza kontami, ale **nie** ma wglądu w dane finansowe użytkowników.
2. **Kwoty:** wyłącznie PLN, `decimal(18,2)`, zawsze dodatnie; kierunek (przychód/wydatek) wynika z typu kategorii, nie ze znaku kwoty.
3. **Jeden plan na miesiąc:** użytkownik ma co najwyżej jeden plan dla danego `YearMonth`; transakcja należy do miesiąca wyznaczonego przez jej datę.
4. **Historia sald jest tylko do dopisywania:** aktualizacja salda tworzy nowy wpis; saldo bieżące = wpis z najpóźniejszą datą (przy równej dacie — ostatnio dodany).
5. **Kategoria w użyciu:** kategorii przypisanej do pozycji planu lub transakcji nie można usunąć — można ją jedynie zarchiwizować (znika z list wyboru, zostaje w historii).

## 7. Poza zakresem MVP

- Import wyciągów bankowych (CSV/MT940) i integracje z bankami (PSD2).
- Wiele walut i przeliczenia kursowe.
- Budżety współdzielone (gospodarstwo domowe z wieloma użytkownikami).
- Transakcje cykliczne generowane automatycznie.
- Cele oszczędnościowe, prognozy i rekomendacje.
- Powiadomienia (e-mail, push) o przekroczeniu budżetu.
- Eksport raportów (PDF/Excel) i zaawansowane wykresy wielomiesięczne.
- Aplikacja mobilna / PWA, tryb offline.
- Logowanie zewnętrzne (Google, Microsoft) i 2FA.
- Lokalizacja inna niż polska.

## 8. Backlog pierwszych zadań

1. Szkielet solucji: projekty Domain, Application, Infrastructure, Api, Web + projekty testowe TUnit; `global.json`, `Directory.Packages.props`, `docker-compose.yml` z Postgresem.
2. Infrastructure: `DbContext` na Npgsql, pierwsza migracja, connection string z konfiguracji.
3. Identity: rejestracja/logowanie cookies, role `User`/`Admin`, seed konta admina.
4. Wzorzec ownership: `OwnerId` na encjach, jawny `ownerId` w serwisach, filtr w `Where`; testy izolacji na Testcontainers (AK1) jako szablon dla kolejnych encji.
5. Domain: value objects `Money`, `YearMonth` z testami reguł 2–3.
6. Kategorie: encja, CRUD, zestaw domyślny przy rejestracji, archiwizacja (reguła 5).
7. Plan miesiąca: encje `MonthlyPlan`/`PlannedItem`, CRUD pozycji, widok planu.
8. Kopiowanie planu z poprzedniego miesiąca (F5, AK3).
9. Transakcje: CRUD, lista miesiąca z filtrem kategorii.
10. Pulpit plan vs wykonanie (F9, AK4, AK7).
11. Konta oszczędnościowe i historia sald (F10–F12, AK5).
12. Panel admina: lista użytkowników, blokowanie, role (F2, AK2).
13. Api: endpointy dla planu, transakcji i oszczędności z tą samą autoryzacją co Web.
14. Walidacja i komunikaty po polsku w całej aplikacji (AK6); przegląd testów (AK8).
