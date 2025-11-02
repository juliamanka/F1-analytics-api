# F1 Analytics API

**F1 Analytics** to backendowy projekt REST API stworzony w technologii **.NET 9**, umożliwiający gromadzenie, analizę oraz zarządzanie danymi pomiarowymi serii testowych związanych z Formułą 1.

Aplikacja została zaprojektowana w architekturze warstwowej z wykorzystaniem:
- **ASP.NET Core Web API**
- **Entity Framework Core**
- wzorca **Repository**
- własnych **middleware’ów** do obsługi wyjątków.

---

## Spis treści

1. [Opis ogólny](#opis-ogólny)  
2. [Technologie](#technologie)  
3. [Architektura i struktura katalogów](#architektura-i-struktura-katalogów)  
4. [Wymagania i konfiguracja lokalna](#wymagania-i-konfiguracja-lokalna)  
5. [Uruchomienie aplikacji lokalnie](#uruchomienie-aplikacji-lokalnie)  
6. [Paginacja](#paginacja)  
7. [API Endpoints](#api-endpoints)  
   - [Seria pomiarowa (Series)](#seria-pomiarowa-series)  
   - [Pomiary (Measurements)](#pomiary-measurements)  
   - [Autoryzacja (Auth / Users)](#autoryzacja-auth--users)  
8. [Modele danych](#modele-danych)  
9. [Obsługa błędów](#obsługa-błędów)
10.  [Autentyfikacja i autoryzacja użytkowników](#autentyfikacja-i-autoryzacja-użytkowników)  


---

## Opis ogólny

Aplikacja F1 Analytics udostępnia REST API pozwalające:
- definiować serie pomiarowe (np. testy aerodynamiczne, testy mechaniczne, itp.),
- zapisywać i aktualizować pomiary w ramach tych serii,
- pobierać dane pomiarowe do dalszej analizy i wizualizacji,
- zarządzać użytkownikami i obsługiwać logowanie.

Docelowo API może być wykorzystywane przez frontend (np. dashboard analityczny).

---

## Technologie

- **.NET 9.0**
- **ASP.NET Core Web API**
- **C# 12**
- **Entity Framework Core** (warstwa dostępu do bazy danych)
- **EF Migrations** (kontrola schematu bazy danych)
- **SQL Server / SQLite** (w zależności od konfiguracji `appsettings.json`)
- **Middleware** do globalnej obsługi wyjątków (np. `GlobalExceptionsHandler`, `DatabaseErrorHandler`)

---

## Architektura i struktura katalogów

Poniżej uproszczona struktura projektu:

```text
F1Analytics/
 ├── Program.cs
 ├── appsettings.json
 ├── appsettings.Development.json
 ├── F1Analytics.csproj
 ├── Configuration/
 │   └── PagedResultConfiguration.cs     # Konfiguracja paginacji odpowiedzi
 ├── Database/
 │   ├── F1AnalyticsDbContext.cs         # Kontekst Entity Framework Core
 │   ├── Migrations/                     # Migracje bazy danych
 │   ├── Models/                         # Encje mapowane do tabel bazy danych
 │   │   ├── Measurement.cs
 │   │   ├── Series.cs
 │   │   ├── User.cs
 │   │   └── UserRole.cs
 │   └── Repositories/                   # Logika dostępu do danych (Repository)
 │       ├── MeasurementRepository.cs
 │       └── SeriesRepository.cs
 ├── Middlewares/
 │   ├── GlobalExceptionsHandler.cs      # Globalna obsługa wyjątków
 │   └── DatabaseErrorHandler.cs         # Obsługa błędów bazy danych
 ├── Requests/                           # Obiekty przyjmowane w żądaniach (DTO In)
 │   ├── Measurement/
 │   │   ├── CreateMeasurementRequest.cs
 │   │   └── UpdateMeasurementRequest.cs
 │   └── Series/
 │       ├── CreateSeriesRequest.cs
 │       └── UpdateSeriesRequest.cs
 ├── Responses/                          # Obiekty zwracane w odpowiedziach (DTO Out)
 │   ├── ApiResponse.cs
 │   ├── PagedResult.cs
 │   └── Measurement/
 │       └── MeasurementResponse.cs
 └── Controllers/                        # Kontrolery HTTP (warstwa API)
     ├── MeasurementsController.cs
     ├── SeriesController.cs
     └── AuthController.cs
```

Warstwy logiczne:
- **Controllers** – przyjmują żądania HTTP, zwracają odpowiedzi HTTP.
- **Repositories** – komunikacja z bazą danych przez `F1AnalyticsDbContext`.
- **Models** – encje odwzorowujące tabele bazy (`Measurement`, `Series`, `User`, `UserRole`).
- **Requests / Responses (DTO)** – kontrakty danych między klientem a API.
- **Middlewares** – spójna obsługa błędów i wyjątków.
- **Configuration** – konfiguracje pomocnicze (np. paginacja).

---

## Wymagania i konfiguracja lokalna

### Wymagania
- Zainstalowane **.NET 9 SDK**
- Dostępna instancja bazy danych zgodna z konfiguracją w `appsettings.json`
  - domyślnie projekt może korzystać z lokalnej bazy (np. SQL Server LocalDB / SQLite – zależnie od tego, co masz wpisane w ConnectionString)

### Konfiguracja `appsettings.json`
Plik `appsettings.json` zawiera m.in.:
- łańcuch połączenia do bazy danych (`ConnectionStrings`)
- ustawienia logowania
- (jeśli dotyczy) konfigurację uwierzytelniania/tokenów

> Upewnij się, że wartości `ConnectionStrings` są poprawne dla Twojego środowiska lokalnego.

### Migracje bazy danych

Aby utworzyć lub zaktualizować strukturę bazy danych zgodnie z aktualnym modelem:

```bash
dotnet ef database update
```

To polecenie użyje migracji znajdujących się w katalogu `Database/Migrations`.

> Uwaga: Musisz mieć zainstalowane globalne narzędzie `dotnet-ef`:
> ```bash
> dotnet tool install --global dotnet-ef
> ```

---

## Uruchomienie aplikacji lokalnie

1. Przygotuj bazę danych (sekcja powyżej: `dotnet ef database update`).
2. Uruchom projekt API:

```bash
dotnet run
```

Domyślnie aplikacja powinna nasłuchiwać pod adresem (lub podobnym, zależnie od konfiguracji Kestrel):

```text
http://localhost:5000
https://localhost:5001
```

W trybie Development dostępne może być również UI Swagger.

---

## Paginacja

API może zwracać wyniki w formie stronicowanej (zob. `PagedResult<T>` oraz `PagedResultConfiguration`).

Typowy wzorzec zapytania z paginacją wygląda tak:

```http
GET /api/measurements?page=1&pageSize=50
```

Parametry:
- `page` – numer strony (licząc od 1),
- `pageSize` – liczba elementów na stronie.

Typowa odpowiedź paginowana może wyglądać tak:

```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": 123,
        "seriesId": 1,
        "value": -1234.56,
        "timestamp": "2025-10-28T12:30:00Z"
      }
    ],
    "page": 1,
    "pageSize": 50,
    "totalCount": 342
  }
}
```

---

## API Endpoints

### Seria pomiarowa (Series)

Endpointy odpowiedzialne za definiowanie i zarządzanie seriami pomiarowymi (np. „Test Aerodynamiczny 2025”).

| Metoda  | Endpoint                  | Opis                                                         |
|---------|---------------------------|--------------------------------------------------------------|
| GET     | `/api/series`             | Pobiera listę wszystkich serii pomiarowych.                  |
| GET     | `/api/series/{id}`        | Pobiera szczegóły serii o podanym `id`.                      |
| POST    | `/api/series`             | Tworzy nową serię pomiarową.                                |
| PUT     | `/api/series/{id}`        | Aktualizuje istniejącą serię.                               |
| DELETE  | `/api/series/{id}`        | Usuwa serię pomiarową.                                      |

#### Przykład żądania `POST /api/series`

```json
{
  "name": "Test Aerodynamiczny 2025",
  "description": "Pomiar siły docisku przy różnych kątach natarcia",
  "minValue": -500.0,
  "maxValue": 1500.0,
  "unit": "N",
  "color": "#FF0000",
  "measurementType": "aero"
}
```

---

### Pomiary (Measurements)

Endpointy odpowiedzialne za pojedyncze wartości pomiarowe w ramach serii.

| Metoda  | Endpoint                        | Opis                                                                 |
|---------|---------------------------------|----------------------------------------------------------------------|
| GET     | `/api/measurements`             | Pobiera listę wszystkich pomiarów (z obsługą paginacji).             |
| GET     | `/api/measurements/{id}`        | Pobiera szczegóły pomiaru o danym `id`.                              |
| POST    | `/api/measurements`             | Dodaje nowy pomiar.                                                  |
| PUT     | `/api/measurements/{id}`        | Aktualizuje istniejący pomiar.                                       |
| DELETE  | `/api/measurements/{id}`        | Usuwa pomiar.                                                        |

#### Przykład żądania `POST /api/measurements`

```json
{
  "seriesId": 1,
  "value": -1234.56,
  "timestamp": "2025-10-28T12:30:00Z"
}
```

#### Przykład odpowiedzi `GET /api/measurements/{id}`

```json
{
  "success": true,
  "data": {
    "id": 987,
    "seriesId": 1,
    "value": -1234.56,
    "timestamp": "2025-10-28T12:30:00Z"
  }
}
```

---

### Autoryzacja (Auth / Users)

Endpointy powiązane z użytkownikami i dostępem.  
(W projekcie dostępne są klasy `User` i `UserRole`, a także requesty typu logowanie / zmiana hasła.)

| Metoda  | Endpoint                          | Opis                                              |
|---------|------------------------------------|---------------------------------------------------|
| POST    | `/api/auth/login`                 | Logowanie użytkownika.                            |
| POST    | `/api/auth/change-password`       | Zmiana hasła aktualnie zalogowanego użytkownika.  |

#### Przykład żądania `POST /api/auth/login`

```json
{
  "username": "jan.michalowski",
  "password": "haslo123"
}
```

> W odpowiedzi można spodziewać się tokenu / informacji o uwierzytelnieniu, zależnie od implementacji kontrolera `AuthController`.

---

## Modele danych

### Measurement

Reprezentuje pojedynczy pomiar.

| Pole        | Typ        | Opis                                   |
|-------------|------------|----------------------------------------|
| `id`        | int        | Identyfikator pomiaru.                 |
| `seriesId`  | int        | Identyfikator serii, do której należy. |
| `value`     | double     | Zmierzone dane/liczba.                 |
| `timestamp` | DateTime   | Czas wykonania pomiaru (UTC).          |

### Series

Reprezentuje logiczną serię pomiarową (np. „Test aero skrzydła przedniego”).

| Pole               | Typ        | Opis                                                  |
|--------------------|------------|-------------------------------------------------------|
| `id`               | int        | Identyfikator serii.                                 |
| `name`             | string     | Nazwa serii.                                         |
| `description`      | string     | Opis / kontekst testu.                               |
| `minValue`         | double     | Minimalna oczekiwana wartość pomiaru.                |
| `maxValue`         | double     | Maksymalna oczekiwana wartość pomiaru.               |
| `unit`             | string     | Jednostka (np. `N`, `°C`, `bar`).                    |
| `color`            | string     | Kolor sugerowany do wizualizacji (np. w dashboardzie).|
| `measurementType`  | string     | Typ pomiaru (np. `aero`, `mech`, itp.).              |

### User / UserRole

Podstawowe encje użytkowników systemu i ról uprawnień.

| Pole        | Typ        | Opis                              |
|-------------|------------|-----------------------------------|
| `id`        | int        | Identyfikator użytkownika.        |
| `username`  | string     | Login użytkownika.                |
| `password`  | string     | (Zahashowane) hasło użytkownika.  |
| `roleId`    | int        | Powiązana rola użytkownika.       |

---

## Obsługa błędów

Aplikacja korzysta z globalnych middleware’ów:
- `GlobalExceptionsHandler` – przechwytuje nieobsłużone wyjątki i zwraca spójny JSON z informacją o błędzie.
- `DatabaseErrorHandler` – obsługuje błędy związane z bazą danych (np. brak połączenia).

Standardowy format odpowiedzi błędu:

```json
{
  "success": false,
  "message": "Nie znaleziono elementu"
}
```

Standardowy format poprawnej odpowiedzi:

```json
{
  "success": true,
  "data": { ... }
}
```

---
## Autentyfikacja i autoryzacja użytkowników

Aplikacja korzysta z **ASP.NET Core Identity** oraz **JWT (JSON Web Token)** do zarządzania procesem logowania i autoryzacji użytkowników.

### Jak to działa

1. Użytkownik wysyła żądanie `POST /api/auth/login` z danymi logowania.  
2. Serwer weryfikuje dane i generuje **token JWT**.  
3. Token zawiera podstawowe informacje (`sub`, `role`, `exp`).  
4. Klient przesyła token w nagłówku `Authorization`:  

```http
Authorization: Bearer <token>
```

5. Serwer waliduje token przy każdym żądaniu (podpis, wystawca, odbiorca, czas ważności).

---

### Przykładowa konfiguracja JWT

```json
"JwtSettings": {
  "Issuer": "YourAppIssuer",
  "Audience": "YourAppAudience",
  "SecretKey": "<store securely in environment variable>"
}
```

> Wartości powyższe są przykładowe.  
> **Nie przechowuj prawdziwych kluczy w repozytorium.**  
> Używaj zmiennych środowiskowych lub usług typu **Azure Key Vault**.

---

### Przechowywanie sekretów

Zalecane metody przechowywania kluczy i sekretów:
- Zmienne środowiskowe (`Environment Variables`)
- **Azure Application Settings**
- **Azure Key Vault**

---

### Walidacja tokenu JWT

Weryfikowane są m.in.:
- podpis (`SecretKey`),
- `Issuer` i `Audience`,
- czas ważności (`exp`),
- integralność danych.

---

### Przykład payloadu tokenu JWT

```json
{
  "sub": "example.user",
  "role": "Admin",
  "iss": "YourAppIssuer",
  "aud": "YourAppAudience",
  "exp": 1767225600
}
```

---


