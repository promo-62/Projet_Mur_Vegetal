FROM debian

EXPOSE 1883 8883

RUN export DEBIAN_FRONTEND=noninteractive \
&& apt-get update && apt-get -y -q upgrade \
&& apt-get -y -q install mosquitto \
&& apt-get -y -q install mosquitto-clients

RUN service mosquitto stop

ADD mosquitto.conf /mosquitto/config/
ADD log/mosquitto.log /mosquitto/log/
ADD data/mosquitto.db /mosquitto/data/
ADD credentials /mosquitto/credentials/
ADD /certificates/DigiCertCA.crt /mosquitto/ca_certificates/
ADD /certificates/iotdata.yhdf.fr.crt /mosquitto/certs/
ADD /certificates/iotdata.yhdf.fr.key /mosquitto/certs/

RUN mosquitto_passwd -U /mosquitto/credentials/credentials \
&& chmod u+r /mosquitto/ca_certificates/DigiCertCA.crt \
&& chmod u+r /mosquitto/certs/iotdata.yhdf.fr.crt \
&& chmod u+r /mosquitto/certs/iotdata.yhdf.fr.key \
&& chown -R mosquitto /mosquitto/data/ \
&& chgrp -R mosquitto /mosquitto/data/ 

CMD [ "mosquitto", "-v", "-c", "/mosquitto/config/mosquitto.conf"]
