# INF27523 — Boutique en microservices

Projet de cours (TP2). API en **.NET 8** : passage d’Ocelot, **JWT**, **Stripe**, **EF Core** (une base par service).

**Démarrer**  
Ouvrir `Code/EC_MicroServices.sln` et lancer les 7 projets, ou exécuter `Code/start-all.ps1`.

- Passerelle / page d’accueil : [http://localhost:5000](http://localhost:5000)  
- Swagger : [http://localhost:5000/swagger](http://localhost:5000/swagger)

**Configuration** : chaînes SQL dans les `appsettings.json` (LocalDB). Clés Stripe en dev : `Code/EC_PaymentService/appsettings.Development.json` (ne pas commiter des secrets).

Simon Kasongo — UQAR, INF27523
