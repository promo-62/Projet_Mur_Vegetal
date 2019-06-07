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
ADD CA.crt /mosquitto/ca_certificates/
ADD server.crt /mosquitto/certs/
ADD server.key /mosquitto/certs/

RUN mosquitto_passwd -U /mosquitto/credentials/credentials
RUN chmod 777 /mosquitto/log/mosquitto.log
RUN chmod 777 /mosquitto/config/mosquitto.conf 

CMD [ "mosquitto", "-c", "/mosquitto/config/mosquitto.conf", "-v"]