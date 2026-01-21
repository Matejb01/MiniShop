# MiniShop – Web aplikacija (ASP.NET Core MVC)

MiniShop je web aplikacija izrađena u ASP.NET Core MVC tehnologiji koja demonstrira razvoj modernog web rješenja s administracijskim sučeljem, korisničkim dijelom aplikacije i REST API servisom. Projekt je izrađen u sklopu kolegija *Napredne tehnike programiranja web servisa*.

## Funkcionalnosti

### Administratorski dio

* CRUD upravljanje proizvodima
* CRUD upravljanje kategorijama
* Rich Text Editor (TinyMCE) za unos opisa proizvoda
* Autentikacija i autorizacija (Admin rola)

### Korisnički dio

* Javni prikaz proizvoda (shop view)
* Prikaz detalja proizvoda
* Wishlist funkcionalnost za prijavljene korisnike

### API

* REST API endpointi za dohvat proizvoda i kategorija
* JSON format podataka
* Odvojeni API controlleri (bez ovisnosti o UI-ju)

## Tehnologije

* ASP.NET Core 8 MVC
* Entity Framework Core
* ASP.NET Identity
* SQL Server (LocalDB)
* Bootstrap 5
* TinyMCE Rich Text Editor
* REST API (JSON)

## Arhitektura

* MVC controlleri za HTML prikaz (admin i public dio)
* API controlleri za REST komunikaciju
* DTO sloj za API izlaz
* Entity Framework Core za rad s bazom podataka

## Pokretanje projekta

1. Otvoriti rješenje u Visual Studio 2022
2. Pokrenuti `Update-Database` za kreiranje baze
3. Pokrenuti aplikaciju (IIS Express ili Kestrel)

Projekt je demonstracijske prirode i ne uključuje sustav naplate.
