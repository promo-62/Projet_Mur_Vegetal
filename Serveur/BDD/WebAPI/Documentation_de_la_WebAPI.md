# Documentation de la WebAPI

### ➢ WebAPI v6

**Résumé des fonctionnalités :**

- Lecture seule des éléments de toutes les collections sauf UsersHololens, UsersAdmin, UsersAPI  (requêtes GET).
- Lecture seule custom selon les demandes de la team interface web (requêtes GET).
- Insertion d’éléments dans toutes les collections (requêtes POST).
- Modification d’élément dans toutes les collections (requêtes PUT).
- Suppression d'un élément dans toutes les collections sauf UsersHololens, UsersAdmin, UsersAPI  (requêtes DELETE).
- Envoi d'une erreur 401 Unauthorized pour toutes les requêtes GET, POST, PUT, DELETE, HEAD, OPTIONS, PATCH non prises en charge.

**Utilisation :**  `api/web/[Route]`



| Méthode |         Route          |                        Fonctionnalité                        |
| :-----: | :--------------------: | :----------------------------------------------------------: |
|   GET   |       /sensors/        |     Obtenir [tous les champs] des capteurs fonctionnels      |
|         |     /sensors/{id}      |      Obtenir [tous les champs] du capteur d'Object {id}      |
|         |      /sensors/ids      | Obtenir les champs [IdSensor, IdSensorType, Name, Project] des capteurs fonctionnels |
|         |       /samples/        | Obtenir [tous les champs] de tous les éléments de la collection Samples |
|         |     /samples/{id}      |          Obtenir [tous les champs] du relevé d'{id}          |
|         |     samples/last/      | Obtenir les champs [IdSensor, Value] du relevé le plus récent pour chaque capteur |
|         |  /samples/sensor/{id}  | Obtenir [tous les champs] des relevés dont l'IdSensor est {id} |
|         |   /protocolversions/   | Obtenir [tous les champs] de tous les éléments de la collection ProtocolVersions |
|         | /protocolversions/{id} | Obtenir [tous les champs] de la version du protocole d'{id}  |
|         |        /plants/        | Obtenir [tous les champs] de tous les éléments de la collection Plants |
|         |      /plants/{id}      |        Obtenir [tous les champs] de la plante d'{id}         |
|         |        /events/        | Obtenir [tous les champs] de tous les éléments de la collection Events |
|         |      /events/{id}      |       Obtenir [tous les champs] de l'évènement d'{id}        |
|         |     /events/last/      | Obtenir [tous les champs] du dernier élément ajouté à Events |
|         |       /socials/        | Obtenir [tous les champs] de tous les éléments de la collection Socials |
|         |     /socials/{id}      |      Obtenir [tous les champs] du compte social d'{id}       |
|         |        /tables/        | Obtenir [tous les champs] de tous les éléments de la collection Tables |
|         |      /tables/{id}      |         Obtenir [tous les champs] du tableau d'{id}          |
|         |        /medias/        | Obtenir [tous les champs] de tous les éléments de la collection Medias |
|         |      /medias/{id}      |          Obtenir [tous les champs] du media d'{id}           |
|         |      /countdowns/      | Obtenir [tous les champs] de tous les éléments de la collection Countdowns |
|         |    /countdowns/{id}    |     Obtenir [tous les champs] du compte à rebours d'{id}     |
|         |     /sensortypes/      | Obtenir [tous les champs] de tous les éléments de la collection SensorTypes |
|         |   /sensortypes/{id}    |     Obtenir [tous les champs] du type de capteur d'{id}      |
|         |        /alerts/        | Obtenir [tous les champs] de tous les éléments de la collection Alerts |
|         |      /alerts/{id}      |         Obtenir [tous les champs] de l'alerte d'{id}         |
|         |       /screens/        | Obtenir [tous les champs] de tous les éléments de la collection Screens |
|         |     /screens/{id}      |         Obtenir [tous les champs] de l'écran d'{id}          |



| Méthode |       Route        |                     Fonctionnalité                     |
| :-----: | :----------------: | :----------------------------------------------------: |
|  POST   |     /sensors/      |     Insérer un élément dans la collection Sensors      |
|         |     /samples/      |     Insérer un élément dans la collection Samples      |
|         | /protocolversions/ | Insérer un élément dans la collection ProtocolVersions |
|         |      /plants/      |      Insérer un élément dans la collection Plants      |
|         |      /events/      |      Insérer un élément dans la collection Events      |
|         |  /usershololens/   |  Insérer un élément dans la collection UsersHololens   |
|         |    /usersadmin/    |    Insérer un élément dans la collection UsersAdmin    |
|         |     /usersapi/     |     Insérer un élément dans la collection UsersAPI     |
|         |     /socials/      |     Insérer un élément dans la collection Socials      |
|         |      /tables/      |      Insérer un élément dans la collection Tables      |
|         |      /medias/      |      Insérer un élément dans la collection Medias      |
|         |    /countdowns/    |    Insérer un élément dans la collection Countdowns    |
|         |   /sensortypes/    |   Insérer un élément dans la collection SensorTypes    |
|         |      /alerts/      |      Insérer un élément dans la collection Alerts      |
|         |     /screens/      |     Insérer un élément dans la collection Screens      |



| Méthode |         Route          |                        Fonctionnalité                        |
| :-----: | :--------------------: | :----------------------------------------------------------: |
|   PUT   |     /sensors/{id}      |    Modifier [tous les champs] du capteur d’ObjectId {id}     |
|         |     /samples/{id}      |     Modifier [tous les champs] du relevé d’ObjectId {id}     |
|         | /protocolversions/{id} | Modifier [tous les champs] de la version du protocole d’ObjectId {id} |
|         |      /plants/{id}      |   Modifier [tous les champs] de la plantes d’ObjectId {id}   |
|         |      /events/{id}      |  Modifier [tous les champs] de l'évènement d’ObjectId {id}   |
|         |     /socials/{id}      | Modifier [tous les champs] du compte social d’ObjectId {id}  |
|         |      /tables/{id}      |    Modifier [tous les champs] du tableau d’ObjectId {id}     |
|         |      /medias/{id}      |     Modifier [tous les champs] du media d’ObjectId {id}      |
|         |    /countdowns/{id}    | Modifier [tous les champs] du compte à rebours d’ObjectId {id} |
|         |   /sensortypes/{id}    | Modifier [tous les champs] du type de capteur d’ObjectId {id} |
|         |      /alerts/{id}      |    Modifier [tous les champs] de l'alerte d’ObjectId {id}    |
|         |     /screens/{id}      |    Modifier [tous les champs] de l'écran d’ObjectId {id}     |



| Méthode |         Route          |                        Fonctionnalité                        |
| :-----: | :--------------------: | :----------------------------------------------------------: |
| DELETE  |     /sensors/{id}      | Supprimer l'élément d’ObjectId {id} de la collection Sensors |
|         |     /samples/{id}      | Supprimer l'élément d’ObjectId {id} de la collection Samples |
|         | /protocolversions/{id} | Supprimer l'élément d’ObjectId {id} de la collection ProtocolVersions |
|         |      /plants/{id}      | Supprimer l'élément d’ObjectId {id} de la collection Plants  |
|         |      /events/{id}      | Supprimer l'élément d’ObjectId {id} de la collection Events  |
|         |     /socials/{id}      | Supprimer l'élément d’ObjectId {id} de la collection Socials |
|         |      /tables/{id}      | Supprimer l'élément d’ObjectId {id} de la collection Tables  |
|         |      /medias/{id}      | Supprimer l'élément d’ObjectId {id} de la collection Medias  |
|         |    /countdowns/{id}    | Supprimer l'élément d’ObjectId {id} de la collection Countdowns |
|         |   /sensortypes/{id}    | Supprimer l'élément d’ObjectId {id} de la collection SensorTypes |
|         |      /alerts/{id}      | Supprimer l'élément d’ObjectId {id} de la collection Alerts  |
|         |     /screens/{id}      | Supprimer l'élément d’ObjectId {id} de la collection Screens |

[^Dev]: Etienne Schelfhout

