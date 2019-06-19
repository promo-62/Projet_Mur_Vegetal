# Documentation de la WebAPI

### ➢ WebAPI v2

Résumé des fonctionnalités :

- Lecture seule des éléments de toutes les collections (requêtes GET)
- Lecture seule custom selon les demandes de la team interface web (requêtes GET)
- Insertion d’éléments dans certaines collections (requêtes POST)
- Modification d’élément dans certaines collections (requêtes PUT)
Utilisation : api/web/[Route]

| Méthode |          Route          |                        Fonctionnalité                        |
| ------: | :---------------------: | :----------------------------------------------------------: |
|     GET |       /capteurs/        |     Obtenir [tous les champs] des capteurs fonctionnels      |
|         |     /capteurs/{id}      |     Obtenir [tous les champs] du capteur d'ObjectId {id}     |
|         |      /capteurs/ids      |  Obtenir [IdCapteur, TypeCapteur, Nom, Projet] des capteurs  |
|         |        /releves/        | Obtenir [tous les champs] de tous les éléments de la collection Releve |
|         |      /releves/{id}      |     Obtenir [tous les champs] du relevé d'ObjectId {id}      |
|         |    releves/derniers/    | Obtenir [IdCapteur, Valeur] du relevé le plus récent pour chaque capteur |
|         |  /releves/capteur/{id}  | Obtenir [tous les relevés] pour un capteur d'IdCapteur {id}  |
|         |   /versionsprotocole/   | Obtenir [tous les champs] de tous les éléments de la collection Version_protocole |
|         | /versionsprotocole/{id} | Obtenir [tous les champs] de la Version_protocole d'ObjectId {id} |
|         |        /plantes/        | Obtenir [tous les champs] de tous les éléments de la collection Plantes |
|         |      /plantes/{id}      |    Obtenir [tous les champs] de la plante d'ObjectId {id}    |
|         |        /events/         | Obtenir [tous les champs] de tous les éléments de la collection Event |
|         |      /events/{id}       |   Obtenir [tous les champs] de l’évènement d'ObjectId {id}   |
|         |     /event/dernier/     | Obtenir [tous les champs] du dernier élément ajouté à Event  |
|         |         /users/         |  Obtenir [tous les champs] du dernier élément ajouté à User  |
|         |       /users/{id}       |      Obtenir [tous les champs] du user d'ObjectId {id}       |
|         |        /socials/        | Obtenir [tous les champs] du dernier élément ajouté à Social |
|         |      /socials/{id}      |  Obtenir [tous les champs] du compte social d'ObjectId {id}  |
|         |        /medias/         | Obtenir [tous les champs] de tous les éléments de la collection Media |
|         |      /medias/{id}       |      Obtenir [tous les champs] du media d'ObjectId {id}      |
|         |       /tableaux/        | Obtenir [tous les champs] de tous les éléments de la collection Tableau |
|         |     /tableaux/{id}      |     Obtenir [tous les champs] du tableau d'ObjectId {id}     |
|         |    /comptearebours/     | Obtenir [tous les champs] de tous les éléments de la collection CompteARebours |
|         |  /comptearebours/{id}   | Obtenir [tous les champs] du compteARebours d'ObjectId {id}  |

| Méthode |      Route       |                    Fonctionnalité                    |
| :-----: | :--------------: | :--------------------------------------------------: |
|  POST   |    /capteurs/    |    Insérer un élément dans la collection Capteurs    |
|         |    /releves/     |     Insérer un élément dans la collection Releve     |
|         |    /plantes/     |    Insérer un élément dans la collection Plantes     |
|         |     /events/     |     Insérer un élément dans la collection Event      |
|         |    /socials/     |     Insérer un élément dans la collection Social     |
|         |    /tableaux/    |    Insérer un élément dans la collection Capteurs    |
|         |     /medias/     |     Insérer un élément dans la collection Media      |
|         | /comptearebours/ | Insérer un élément dans la collection CompteARebours |

| Méthode |        Route         |                        Fonctionnalité                        |
| :-----: | :------------------: | :----------------------------------------------------------: |
|   PUT   |    /capteurs/{id}    | Modifier [tous les champs] de l’élément d’ObjectId {id} de la collection Capteurs |
|         |    /releves/{id}     | Modifier [tous les champs] de l’élément d’ObjectId {id} de la collection Releve |
|         |    /plantes/{id}     | Modifier [tous les champs] de l’élément d’ObjectId {id} de la collection Plantes |
|         |     /events/{id}     | Modifier [tous les champs] de l’élément d’ObjectId {id} de la collection Event |
|         |    /socials/{id}     | Modifier [tous les champs] de l’élément d’ObjectId {id} de la collection Social |
|         |    /tableaux/{id}    | Modifier [tous les champs] de l’élément d’ObjectId {id} de la collection Capteurs |
|         |     /medias/{id}     | Modifier [tous les champs] de l’élément d’ObjectId {id} de la collection Media |
|         | /comptearebours/{id} | Modifier [tous les champs] de l’élément d’ObjectId {id} de la collection CompteARebours |