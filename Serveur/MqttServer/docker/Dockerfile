FROM debian

EXPOSE 1883 8883

RUN export DEBIAN_FRONTEND=noninteractive \
&& apt-get update && apt-get -y -q upgrade \
&& apt-get -y -q install mosquitto

ADD mosquitto.conf /mosquitto/config/
ADD mosquitto.log /mosquitto/log/
ADD users_password mosquitto/users/

RUN chmod 777 /mosquitto/log/mosquitto.log

CMD [ "mosquitto", "-c", "/mosquitto/config/mosquitto.conf"]