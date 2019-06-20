#!/bin/bash

if [ ! -f certificates/DigiCertCA.crt ] || [ ! -f certificates/iotdata.yhdf.fr.crt ] || [ ! -f certificates/iotdata.yhdf.fr.key ]; then
    echo "--> ERROR : Files are missing. You must provide DigiCertCA.crt, iotdata.yhdf.fr.crt, iotdata.yhdf.fr.key into the certificates folder"
    exit 99
fi

mkdir log
touch log/mosquitto.log
chmod u-w,g-r,o+w,o-r log/mosquitto.log
echo
echo "--> Building docker..."
sudo docker build -t server_mqtt ./

echo
echo "--> Run the server's container with:" 
echo "sudo docker run -d -p 8883:8883 -v $(pwd)/log:/mosquitto/log server_mqtt"
echo
echo "--> You can get the logs of the mosquitto server at log/mosquitto.log"
echo
