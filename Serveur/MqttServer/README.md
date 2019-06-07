# Tuto
git clone https://github.com/promo-62/Projet_Mur_Vegetal.git
cd Projet_Mur_Vegetal
### Debut partie temporaire
git fetch
git checkout -b ServeurMqtt remotes/origin/ServeurMqtt
### Fin partie temporaire
cd Serveur/MqttServer/docker
./install.sh
sudo docker run -p 8883:8883 -v $(pwd)/log:/mosquitto/log server_mqtt

#Partie création du serveur Eclipse Mosquitto MQTT et insertion dans la BDD

Equipe : 
Corentin D
Loïc D
Antoine C
P-E H
Romain D
