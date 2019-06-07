FROM debian

EXPOSE 1883 8883

RUN export DEBIAN_FRONTEND=noninteractive \
&& apt-get update && apt-get -y -q upgrade \
&& apt-get -y -q install mosquitto \
&& apt-get -y -q install mosquitto-clients \
&& apt-get -y -q install openssl 

RUN service mosquitto stop

ADD mosquitto.conf /mosquitto/config/
ADD mosquitto.log /mosquitto/log/
ADD credentials /mosquitto/credentials/
ADD ca.crt /mosquitto/ca_certificates/
ADD server.crt /mosquitto/certs/
ADD server.key /mosquitto/certs/

RUN mosquitto_passwd -U /mosquitto/credentials/credentials \
&& chmod 777 /mosquitto/log/mosquitto.log \
&& chmod 777 /mosquitto/config/mosquitto.conf \
&& chmod 777 /mosquitto/ca_certificates/ca.crt \
&& chmod 777 /mosquitto/certs/server.crt \
&& chmod 777 /mosquitto/certs/server.key

CMD [ "mosquitto", "-v"]