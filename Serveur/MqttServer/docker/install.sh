#!/bin/bash
cd certificates
./generate_certificates.sh
cd ..

chmod u-w,g-r,o+w,o-r log/mosquitto.log
echo
echo "--> Building docker..."
sudo docker build -t server_mqtt ./

echo
echo "--> Run the server's container with: sudo docker run -p 8883:8883 -v $(pwd)/log:/mosquitto/log mqtt"
echo "--> You can get the logs of the mosquitto server at log/mosquitto.log"
echo