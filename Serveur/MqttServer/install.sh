#!/bin/bash
cd certificates
./generate_certificates.sh
cd ..

mkdir log
touch log/mosquitto.log
chmod u-w,g-r,o+w,o-r log/mosquitto.log
echo
echo "--> Building docker..."
sudo docker build -t server_mqtt ./

echo
echo "--> Run the server's container with:" 
echo "sudo docker run -p 8883:8883 -v $(pwd)/log:/mosquitto/log server_mqtt"
echo
echo "--> You can get the logs of the mosquitto server at log/mosquitto.log"
echo