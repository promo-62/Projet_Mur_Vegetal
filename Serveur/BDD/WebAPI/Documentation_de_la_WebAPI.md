# Documentation de la WebAPI

### ➢ WebAPI v4.1

**Résumé des fonctionnalités :**

- Lecture seule des éléments de toutes les collections (requêtes GET).
- Lecture seule custom selon les demandes de la team interface web (requêtes GET).
- Insertion d’éléments dans toutes les collections (requêtes POST).
- Modification d’élément dans toutes les collections (requêtes PUT).
- Envoi d'une erreur 401 Unauthorized pour toutes les requêtes GET, POST, PUT, DELETE, HEAD, OPTIONS, PATCH non prises en charge.

**Utilisation :**  `api/web/[Route]`



| Méthode |          Route           |                        Fonctionnalité                        |
| :-----: | :----------------------: | :----------------------------------------------------------: |
|   GET   |        /capteurs/        |     Obtenir [tous les champs] des capteurs fonctionnels      |
|         |      /capteurs/{id}      |     Obtenir [tous les champs] du capteur d'ObjectId {id}     |
|         |      /capteurs/ids       |  Obtenir [IdCapteur, TypeCapteur, Nom, Projet] des capteurs  |
|         |        /releves/         | Obtenir [tous les champs] de tous les éléments de la collection Releves |
|         |      /releves/{id}       |     Obtenir [tous les champs] du relevé d'ObjectId {id}      |
|         |    releves/derniers/     | Obtenir [IdCapteur, Valeur] du relevé le plus récent pour chaque capteur |
|         |  /releves/capteur/{id}   | Obtenir [tous les relevés] pour un capteur d'IdCapteur {id}  |
|         |   /versionsprotocoles/   | Obtenir [tous les champs] de tous les éléments de la collection VersionsProtocoles |
|         | /versionsprotocoles/{id} | Obtenir [tous les champs] de la version de protocole d'ObjectId {id} |
|         |        /plantes/         | Obtenir [tous les champs] de tous les éléments de la collection Plantes |
|         |      /plantes/{id}       |    Obtenir [tous les champs] de la plante d'ObjectId {id}    |
|         |         /events/         | Obtenir [tous les champs] de tous les éléments de la collection Events |
|         |       /events/{id}       |   Obtenir [tous les champs] de l’évènement d'ObjectId {id}   |
|         |     /events/dernier/     | Obtenir [tous les champs] du dernier élément ajouté à Events |
|         |     /usershololens/      | Obtenir [tous les champs] du dernier élément ajouté à UsersHololens |
|         |   /usershololens/{id}    | Obtenir [tous les champs] de l'utilisateur de l'Hololens d'ObjectId {id} |
|         |       /usersadmin/       | Obtenir [tous les champs] du dernier élément ajouté à UsersAdmin |
|         |     /usersadmin/{id}     | Obtenir [tous les champs] de l'utilisateur admin d'ObjectId {id} |
|         |        /usersapi/        | Obtenir [tous les champs] du dernier élément ajouté à UsersAPI |
|         |      /usersapi/{id}      | Obtenir [tous les champs] de l'utilisateur de l'API d'ObjectId {id} |
|         |        /socials/         | Obtenir [tous les champs] du dernier élément ajouté à Socials |
|         |      /socials/{id}       |  Obtenir [tous les champs] du compte social d'ObjectId {id}  |
|         |         /medias/         | Obtenir [tous les champs] de tous les éléments de la collection Medias |
|         |       /medias/{id}       |      Obtenir [tous les champs] du media d'ObjectId {id}      |
|         |        /tableaux/        | Obtenir [tous les champs] de tous les éléments de la collection Tableaux |
|         |      /tableaux/{id}      |     Obtenir [tous les champs] du tableau d'ObjectId {id}     |
|         |    /comptesarebours/     | Obtenir [tous les champs] de tous les éléments de la collection ComptesARebours |
|         |  /comptesarebours/{id}   | Obtenir [tous les champs] du compte à rebours d'ObjectId {id} |
|         |     /typescapteurs/      | Obtenir [tous les champs] de tous les éléments de la collection TypesCapteurs |
|         |   /typescapteurs/{id}    |  Obtenir [tous les champs] du type capteur d'ObjectId {id}   |
|         |        /alertes/         | Obtenir [tous les champs] de tous les éléments de la collection Alertes |
|         |      /alertes/{id}       |    Obtenir [tous les champs] de l'alerte d'ObjectId {id}     |



| Méthode |        Route         |                      Fonctionnalité                      |
| :-----: | :------------------: | :------------------------------------------------------: |
|  POST   |      /capteurs/      |      Insérer un élément dans la collection Capteurs      |
|         |      /releves/       |      Insérer un élément dans la collection Releves       |
|         | /versionsprotocoles/ | Insérer un élément dans la collection VersionsProtocoles |
|         |      /plantes/       |      Insérer un élément dans la collection Plantes       |
|         |       /events/       |       Insérer un élément dans la collection Events       |
|    !    |   /usershololens/    |   Insérer un élément dans la collection UsersHololens    |
|    !    |     /usersadmin/     |     Insérer un élément dans la collection UsersAdmin     |
|         |      /usersapi/      |      Insérer un élément dans la collection UsersAPI      |
|         |      /socials/       |      Insérer un élément dans la collection Socials       |
|         |      /tableaux/      |      Insérer un élément dans la collection Capteurs      |
|         |       /medias/       |       Insérer un élément dans la collection Medias       |
|         |  /comptesarebours/   |  Insérer un élément dans la collection ComptesARebours   |
|    !    |   /typescapteurs/    |   Insérer un élément dans la collection TypesCapteurs    |
|         |      /alertes/       |      Insérer un élément dans la collection Alertes       |



| Méthode |          Route           |                        Fonctionnalité                        |
| :-----: | :----------------------: | :----------------------------------------------------------: |
|   PUT   |      /capteurs/{id}      | Modifier [tous les champs] de l’élément d’ObjectId {id} de la collection Capteurs |
|         |      /releves/{id}       | Modifier [tous les champs] de l’élément d’ObjectId {id} de la collection Releves |
|         | /versionsprotocoles/{id} | Modifier [tous les champs] de l’élément d’ObjectId {id} de la collection VersionsProtocoles |
|         |      /plantes/{id}       | Modifier [tous les champs] de l’élément d’ObjectId {id} de la collection Plantes |
|         |       /events/{id}       | Modifier [tous les champs] de l’élément d’ObjectId {id} de la collection Events |
|         |   /usershololens/{id}    | Modifier [tous les champs] de l’élément d’ObjectId {id} de la collection UsersHololens |
|         |     /usersadmin/{id}     | Modifier [tous les champs] de l’élément d’ObjectId {id} de la collection UsersAdmin |
|         |      /usersapi/{id}      | Modifier [tous les champs] de l’élément d’ObjectId {id} de la collection UsersAPI |
|         |      /socials/{id}       | Modifier [tous les champs] de l’élément d’ObjectId {id} de la collection Socials |
|         |      /tableaux/{id}      | Modifier [tous les champs] de l’élément d’ObjectId {id} de la collection Capteurs |
|         |       /medias/{id}       | Modifier [tous les champs] de l’élément d’ObjectId {id} de la collection Medias |
|         |  /comptesarebours/{id}   | Modifier [tous les champs] de l’élément d’ObjectId {id} de la collection ComptesARebours |
|         |   /typescapteurs/{id}    | Modifier [tous les champs] de l’élément d’ObjectId {id} de la collection TypesCapteurs |
|         |      /alertes/{id}       | Modifier [tous les champs] de l’élément d’ObjectId {id} de la collection Alertes |