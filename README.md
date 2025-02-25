
# Plateforme de distribution de contenu + Éditeur

## But

- Construire un web service avec son client Windows pour gérer une plateforme de distribution de contenu limitée aux jeux vidéo.
-  Ajouter à celui-ci un jeu multijoueur comprenant le serveur ainsi que le jeu correspondant.

## A rendre

-  Un web service de stockage et de gestion des jeux en ligne.
-  Un logiciel sous Windows pour parcourir les jeux, en télécharger un et jouer à celui-ci.
-  Un serveur de jeu orchestrant le fonctionnement d’au moins un jeu.
- Une application permettant de jouer à un jeu.

## Contrainte

- Langages autorisés : C#, HTML, JavaScript, CSS, TypeScript
- Serveur web : ASP.Net Core
-  Logiciel Windows : WPF
- Serveur de jeux : C#
-  Jeu : C# avec Godot, Unity, Winform, WPF, MAUI, etc.

## Projet de départ

Votre solution devra être basée sur le projet **Library.sln**.

- La partie serveur est dans le projet **Gauniv.WebServer**.
-  La partie client est dans le projet **Gauniv.Client**.
-  La connexion entre votre client et votre serveur est dans le projet **Gauniv.Network**.
-  Vous devrez créer deux projets supplémentaires :
  -  **Gauniv.GameServer** pour le serveur de jeu.
  -  **Gauniv.Game** pour le jeu lui-même.

## Aide

### Base de données

- Pour des informations sur Entity Framework, consultez : [EF Core Migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=vs)

### MAUI – Gesture

-  Pour les éléments qui prennent en charge l'évènement click, utilisez :
  ```xml
  <Button Clicked="" />
  ```
-  Pour les éléments qui ne le supportent pas, utilisez les Gesture :
  ```xml
  <Label>
      <Label.GestureRecognizers>
          <TapGestureRecognizer Command="{Binding AppearingCommand}" />
      </Label.GestureRecognizers>
  </Label>
  ```

### MAUI – Évènement

-  Pour transmettre un évènement depuis une View vers un ViewModel, utilisez par exemple :
  ```xml
  <Label>
      <Label.Behaviors>
          <toolkit:EventToCommandBehavior EventName="Focused"
                                          Command="{Binding FocusedCommand}"
                                          x:TypeArguments="FocusedEventArgs" />
      </Label.Behaviors>
  </Label>
  ```
- N'oubliez pas d'ajouter le namespace approprié pour CommunityToolkit.

### DTO

-  Pour renvoyer un objet différent de celui contenu dans votre BDD, vous pouvez :
  - Créer votre DTO à la main (voir [ce tutoriel](https://learn.microsoft.com/en-us/aspnet/web-api/overview/data/using-web-api-with-entity-framework/part-5))
  - Ou utiliser la librairie [AutoMapper](https://automapper.org/)

### Entity Framework

-  Pour récupérer les catégories avec les jeux, utilisez la méthode `Include` :
  ```csharp
  appDbContext.Games.Include(g => g.Categories).Where(x => x.Price > 0);
  ```
- Ou activez le Lazy Loading.

## Développement

-  Injecter des données de test dans la base de données.
-  Avant d'utiliser des requêtes HTTP depuis le client, créez une interface avec des données statiques.
- Pour tester le téléchargement d'un jeu, utilisez temporairement un fichier texte à ouvrir avec votre éditeur par défaut.

## Fonctionnalités attendues

### Web Service (Plateforme de distribution)

#### Modèle de données

- [ ]  Stocker une liste des jeux accessibles.
- [ ]  Stocker une liste des jeux achetés par les utilisateurs.
- [ ]  Stocker une liste de genres (catégories) pour caractériser les jeux.

Sachant que :

- **Un jeu** contient au minimum :
  - [x]  Un Id
  - [x]  Un nom
  - [x]  Une description
  - [x]  Un payload (binaire du jeu)
  - [x]  Un prix
  - [x]  Des catégories (un jeu peut avoir plusieurs catégories)

- **Un utilisateur** contient au minimum :
  - [x]  Un Id
  - [x]  Un nom
  - [x]  Un prénom
  - [x]  Une liste des jeux achetés

#### Administration

-  Un administrateur peut :
  - [ ]  Ajouter des jeux
  - [ ]  Supprimer des jeux
  - [ ]  Modifier un jeu
  - [ ]  Ajouter de nouvelles catégories
  - [ ]  Modifier une catégorie
  - [ ]  Supprimer une catégorie

-  Un utilisateur peut :
  - [ ]  Consulter la liste des jeux possédés
  - [ ]  Acheter un nouveau jeu
  - [ ]  Voir les jeux possédés
  - [ ]  Consulter la liste des autres joueurs inscrits et leurs statuts en temps réel

-  Tout le monde peut :
  - [ ]  Consulter la liste de tous les jeux (avec filtrage par nom, prix, catégorie, etc.)
  - [ ]  Consulter la liste de toutes les catégories

#### API REST

L'API doit permettre :

- [ ]  S'authentifier
- [ ]  Récupérer le binaire d'un jeu et le copier localement (sans charger l'ensemble du fichier en mémoire)
- [ ]  Lister les catégories disponibles
- [ ]  Lister les jeux (avec support des filtres et de la pagination)
  - Exemple d'URL : `/game`, `/game?offset=10&limit=15`, `/game?category=3`, etc.
- [ ]  Lister les jeux possédés (pour les utilisateurs connectés, avec filtres et pagination)

### Application Client (WPF, MAUI ou WINUI)

- [ ]  Lister les jeux avec pagination (scroll infini, bouton, etc.)
- [ ]  Filtrer les jeux par catégorie, prix, etc.
- [ ]  Afficher les détails d'un jeu (nom, description, statut, catégories)
- [ ]  Télécharger, supprimer et lancer un jeu
  - [ ]  Masquer les boutons "jouer" et "supprimer" si le jeu n'est pas téléchargé
  - [ ]  Masquer le bouton "télécharger" si le jeu est déjà disponible
- [ ]  Jouer à un jeu
  - [ ]  Afficher l'état du jeu (non téléchargé, prêt, en jeu, etc.)
  - [ ]  Contrôler le jeu (lancement, arrêt forcé, etc.)
- [ ]  Voir et mettre à jour le profil de l'utilisateur (dossier d'installation, identifiants, etc.)

### Serveur de jeu

- [ ]  Orchestrer le fonctionnement d'au moins un jeu.
- [ ]  Assurer la communication entre les joueurs et le serveur via TCP.
- [ ]  Implémenter les règles et le déroulement d'une partie (par exemple, gestion du temps de réaction, vérification d'éligibilité, etc.)

### Jeu

- [ ]  Fournir une application permettant de jouer (avec interface graphique et contrôles).
- [ ]  Gérer l'authentification et la sélection des joueurs.
- [ ]  Intégrer les mécanismes de lancement, arrêt, et contrôle du jeu.

## Options et améliorations

- [ ]  Ajouter des filtres dans la liste des jeux (catégorie, prix, etc.).
- [ ]  Créer une page de statistiques sur :
  - [ ]  Le nombre total de jeux disponibles.
  - [ ]  Le nombre de jeux par catégorie.
  - [ ]  Le nombre moyen de jeux joués par compte.
  - [ ]  Le temps moyen joué par jeu.
  - [ ]  Le maximum de joueurs en simultané sur la plateforme et par jeu.
- [ ]  Mettre en place un mécanisme pour stocker les jeux hors de la base de données (si nécessaire).
- [ ]  Implémenter le streaming du binaire pour réduire l’empreinte mémoire.
- [ ]  Permettre à chaque joueur de gérer une liste d'amis.

```

---

Ce README avec des cases à cocher vous permet de valider chaque étape du développement au fur et à mesure de l'avancement du projet. Vous pouvez le modifier ou l'étendre selon vos besoins.
# OPTIONNEL :
# Serveur de jeu (Console)

Le serveur est une application console qui coordonne tous les joueurs.

La communication entre les joueurs et le serveur se fait en TCP.

Pour simplifier la communication, je conseille l’utilisation de MessagePack ou autre (Protobuff, Thrift, Cap’n Proto, ...).


## Deroulement d’une partie

Le jeu se joue sur un damier N*N.

1. Le serveur attend que tous les joueurs soient prêts pour commencer la partie.
2. Le serveur décide du MJ et avertit tous les participants de leurs rôles.
3. Le MJ décide d'une case et valide son choix.
4. Les joueurs reçoivent le top départ.
5. Chaque joueur clique le plus vite possible sur la case choisie par le MJ.
6. Le serveur définit l'ordre final des joueurs grâce au temps de réaction de chaque joueur.
7. Pour chaque joueur, le serveur vérifie que la participation du joueur est valide grâce à la fonction ci-dessous. Si le joueur est exclu, la position de tous les joueurs doit être mise à jour en conséquence.
8. Le serveur communique le résultat final à tout le monde.


## Verifier l’eligibilité d’un joueur

```csharp
bool IsEligible(int pos, string name)
{
    Stopwatch sw = new();
    sw.Start();
    ECDsa key = ECDsa.Create();
    key.GenerateKey(ECCurve.NamedCurves.nistP521);
    int t = 5000 / pos;
    var k = new byte[t];
    var d = Encoding.UTF8.GetBytes(name);
    for (int i = 0; i < t; i++)
    {
        var s = key.SignData(d.Concat(BitConverter.GetBytes(pos)).ToArray(), HashAlgorithmName.SHA512);
        k[i] = s[i % s.Length];
    }
    var res = key.SignData(k, HashAlgorithmName.SHA512);
    sw.Stop();
    Console.WriteLine($"{pos} {sw.ElapsedMilliseconds} {res}");
    if (res[(int)Math.Truncate(res.Length / 4.0)] > 0x7F)
        return true;
    return false;
}
```
## Le joueur

- Un joueur doit être authentifié par login / mot de passe auprès du serveur d’identification.
  - Le serveur d’authentification doit retourner un token prouvant l’authentification.
- Un joueur est composé d’un nom et d’un token d’authentification.

## Option

- Le serveur sait gérer plusieurs parties en même temps (et donc il sait gérer des salons).
- Séparer la partie serveur de la partie jeu :
  - Le serveur est générique et charge des plugins, chaque plugin est un jeu.
  - Le serveur peut gérer plusieurs jeux en même temps.
  - On peut rajouter un jeu sans redémarrer le serveur.
- Lancer plusieurs serveurs en même temps pour augmenter la capacité maximale de joueurs :
  - Un joueur peut se connecter à n'importe quel serveur et jouer à n'importe quelle partie.
  - Si le serveur ne sait gérer qu'une partie à la fois, alors tous les joueurs de tous les serveurs rejoignent la même partie en même temps.
  - Si le serveur sait gérer plusieurs parties à la fois, alors le joueur peut choisir la partie à rejoindre quel que soit son serveur d'origine.


# Jeu (Godot, UNITY, Winform, Console, …)


Le jeu doit mettre en place les IHM permettant aux joueurs de jouer

### Commun

1. Entrer des identifiant de connexion
2. Sélection du nom
3. Ready check

# MJ ou #JOUEUR

1. Attente des autres joueurs
2. Affichage des résultats

### MJ

1. Sélection d’une case
2. Validation de la case sélectionné ou changement (ref #4)

### Joueur

1. Attente du choix du MJ
2. Affichage de la case sélectionné par le MJ
3. Clic !

## Option

- Ajout d’un temp maximal pour cliquer
- Géré les joueurs dans la liste d’ami avec le statut correspondant
- Remplacer le damier par une map créer par le MJ
