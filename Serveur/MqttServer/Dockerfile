FROM debian

EXPOSE 1883 8883

RUN export DEBIAN_FRONTEND=noninteractive \
&& apt-get update && apt-get -y -q upgrade \
&& apt-get -y -q install mosquitto \
&& apt-get -y -q install mosquitto-clients \
&& apt-get -y -q install openssl 

RUN service mosquitto stop

ADD mosquitto.conf /mosquitto/config/
ADD log/mosquitto.log /mosquitto/log/
ADD credentials /mosquitto/credentials/
ADD /certificates/ca.crt /mosquitto/ca_certificates/
ADD /certificates/server.crt /mosquitto/certs/
ADD /certificates/server.key /mosquitto/certs/

RUN mosquitto_passwd -U /mosquitto/credentials/credentials \
&& chmod u+r /mosquitto/ca_certificates/ca.crt \
&& chmod u+r /mosquitto/certs/server.crt \
&& chmod u+r /mosquitto/certs/server.key

CMD [ "mosquitto", "-v", "-c", "/mosquitto/config/mosquitto.conf"]