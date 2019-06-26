# Installation et utilisation du programme MQTT Serveur



## Installation des dépendances

#### Serveur Mosquitto

Vous pouvez vous référrez au ReadME du serveur Mosquitto pour installer celui-ci si cela n'est pas déja fait. Ce serveur permettra de transmettre les messages par protocole MQTT.

#### .NET et son environnement

Pour permettre la compilation du programme il est nécessaire que l'environnement de travail puisse compiler des projets en .NET.



## Installation du programme

Le programme, situé dans le dossier **Recuperation**, n'a pas besoin d'être changé et peut être utilisé directement. Dans le cas où les sources ont été déplacées dans un nouveau projet, il sera nécessaire d'installer les dépendances du projets.

Les codes sources contenant le code principal sont **Recuperation.cs** et **Protocol.cs**.

Il faut se rendre dans le dossier du projet, par défaut **Recuperation** si cela n'est pas déja fait:

- `cd Recuperation`

Nous installons ensuite les dépendances pour le JSON, le client MQTT et l'api MongoDB:

* `dotnet add package Newtonsoft.Json` 
* `dotnet add package MQTTnet` 
* `dotnet add package MongoDB` 

Afin d'éviter des conflit, nous éxécutons la commande:

* `dotnet restore`



## Lancement du programme

Pour lancer le programme, nous nous rendons à nouveau dans le dossier du projet, par défaut **Recupération**. Nous lançons ensuite la commande pour démarrer le programme:

* `dotnet run`

Une console attendant les messages sur la branche **Rpi/#**.

Seuls les messages des branches **Rpi/DemandeID/Server**, **Rpi/EnvoiInfos/Server** et **Rpi/DemandeAction/Server** seront pris en compte et traités. 