# Partie création du serveur Eclipse Mosquitto MQTT et insertion dans la BDD

## Installation du Serveur MQTT
+ `git clone https://github.com/promo-62/Projet_Mur_Vegetal.git`
+ `cd Projet_Mur_Vegetal`
+ `git checkout -b ServeurMqtt origin/ServeurMqtt` (À faire uniquement si la branche n'a pas été Merged sur Master)
+ `cd Serveur/MqttServer`
+ `./install.sh`
+ `sudo docker run -p 8883:8883 -v $(pwd)/log:/mosquitto/log server_mqtt`

Equipe : 
Corentin D
Loïc D
Antoine C
P-E H
Romain D
