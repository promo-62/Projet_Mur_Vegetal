# Documentation de la WebAPI

### WebAPI v1 : Partie Web

<u>Fonctionnalités :</u> Lecture de données (requêtes GET)
<u>Utilisation</u> : api/web/[Route]

| Route              |                                                              |
| ------------------ | ------------------------------------------------------------ |
| /capteurs/         | Obtenir les [IdCapteur, TypeCapteur, Nom, Projet] des capteurs fonctionnels |
| /releves/derniers/ | Obtenir les [IdCapteur, Valeur] du relevé le plus récent pour chaque capteur |
| /releves/{id}      | Obtenir [tous les relevés] pour un capteur d'IdCapteur {id}  |
| /comptearebours/   | Obtenir [tous les champs] de tous les éléments de la collection CompteARebours |
| /events/           | Obtenir [tous les champs] de tous les éléments de la collection Event |
| /event/dernier/    | Obtenir [tous les champs] du dernier élément ajouté à Event  |
| /medias/           | Obtenir [tous les champs] de tous les éléments de la collection Media |
| /tableaux/         | Obtenir [tous les champs] de tous les éléments de la collection Tableau |