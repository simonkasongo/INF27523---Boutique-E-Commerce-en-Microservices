# INF27523 - Boutique e-commerce en microservices (TP2)

Projet de cours (UQAR) : transformation de la logique d’une boutique monolithique (TP1) en **architecture en microservices** - plusieurs **ASP.NET Core Web API** indépendantes, une **passerelle (API Gateway)**, de la persistance par service et des appels HTTP entre services.

**Auteur** : Simon Kasongo

---

## Travail réalisé

- **Passerelle** (`EC_Gateway`, port 5000) : **Ocelot** pour router `/api/...` vers le bon service ; **MMLib.SwaggerForOcelot** pour une **documentation Swagger unifiée** ; petite **page web** (connexion / inscription) servie en statique sur la même origine.
- **Authentification** (`EC_AuthService`, 5001) : inscription, connexion, **JWT** (HS256), hachage des mots de passe ; rôles Client / Vendeur.
- **Catalogue** (`EC_ProductService`, 5002) : CRUD produits, intégration d’**APIs externes** (ex. FakeStore, DummyJSON) et import côté serveur.
- **Panier** (`EC_CartService`, 5003) : persistance des lignes de panier ; appels HTTP vers le service produits pour valider / enrichir les articles.
- **Commandes** (`EC_OrderService`, 5004) : commandes + lignes ; **orchestration du checkout** (panier, paiement, notification).
- **Paiement** (`EC_PaymentService`, 5005) : intégration **Stripe** (mode test / clés de développement), enregistrement des paiements côté base.
- **Notifications** (`EC_NotificationService`, 5006, bonus) : enregistrement d’événements (envoi simulé / journalisé).
- **Données** : **Entity Framework Core** avec **une base SQL Server (LocalDB) par service** et **migrations** appliquées au démarrage (`Database.Migrate()`).
- **Lancement** : solution Visual Studio (7 projets) ou script PowerShell `Code/start-all.ps1` (puis `stop-all.ps1` pour arrêter les ports 5000-5006).

---

## Technologies

| Domaine | éléments |
|--------|----------|
| Langage & runtime | C#, **.NET 8** |
| API | **ASP.NET Core** (Web API, contrôleurs) |
| Passerelle & doc | **Ocelot**, **MMLib.SwaggerForOcelot**, **Swashbuckle** (Swagger + JWT dans l’UI) |
| Sécurité | **JWT** (`Microsoft.AspNetCore.Authentication.JwtBearer`, génération de jeton côté service auth) |
| Données | **Entity Framework Core 8**, **SQL Server LocalDB** |
| Paiement | **Stripe.net** (API côté serveur) |
| Inter-services | `HttpClient` (clients dédiés dans les services concernés) |
| Front léger | HTML / JS statique hébergé sur la passerelle (pas de SPA séparée) |

---

## Démarrer en local

1. **Prérequis** : SDK .NET 8, SQL Server **LocalDB**, Visual Studio 2022 (recommandé) ou CLI.
2. Ouvrir `Code/EC_MicroServices.sln` et démarrer les **sept** projets (passerelle **après** les services si le Swagger unifié ne charge pas tout de suite - rafraîchir la page au besoin).  
3. **URLs utiles**  
   - Application / connexion : [http://localhost:5000](http://localhost:5000)  
   - **Swagger** unifié : [http://localhost:5000/swagger](http://localhost:5000/swagger)

**Chaînes de connexion** : `appsettings.json` de chaque service (ajuster l’instance SQL si besoin).  
**Stripe (développement)** : placer les clés de test dans `EC_PaymentService/appsettings.Development.json` (fichier volontairement exclu du dépôt par `.gitignore`).

Un déploiement **Azure** (App Service) peut compléter le travail : publier chaque service, mettre à jour les URLs dans la configuration Ocelot et les paramètres d’application (chaînes SQL, JWT, Stripe).
