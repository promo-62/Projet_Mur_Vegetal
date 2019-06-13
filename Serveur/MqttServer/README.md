# Partie création du serveur Eclipse Mosquitto MQTT et insertion dans la BDD

## Installation de Docker

Vous pouvez suivre les instructions sur le site de Docker, en fonction de votre OS : https://docs.docker.com/install/

## Installation du Serveur MQTT

On clone l'ensemble du projet : 

+ `git clone https://github.com/promo-62/Projet_Mur_Vegetal.git`

On bascule de branche, si la branche n'as pas encore été fusionnée avec master

+ `cd Projet_Mur_Vegetal`
+ `git checkout -b ServeurMqtt origin/ServeurMqtt`

On utilise le script d'initialisation du serveur et on build le container Docker :  

+ `cd Serveur/MqttServer`
+ `./install.sh`

On lance le container :  
+ `sudo docker run -d -p 8883:8883 -v $(pwd)/log:/mosquitto/log server_mqtt`
