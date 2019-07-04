import 'package:flutter/material.dart';
import 'package:ISENCONNECT/screens/home.dart';
import 'dart:io';
import 'Global.dart';
import 'BDD/Bdd.dart';
import 'BDD/SensorType.dart';
import 'BDD/Event.dart';
import 'BDD/Countdown.dart';
import 'package:ISENCONNECT/Others/Media.dart';

List<Event> listEvent = new List();
List<Countdown> listTimer=  new List();
List<Media> listMedia = new List();

int skipTexte = 1;
int skipImage = 1;
int skipTimer = 1;
int emptyImage = 1;
int containerButton = 1;

String editTitle = "";
String editText = "";
DateTime editTimer;
DateTime editBeginningDate;
DateTime editEndingDate;
String editImage;
File imageFile;
String imageInString;


main() {
  runApp(MyApp());
}


class MyApp extends StatelessWidget {

  @override
  build(BuildContext ctxt) {

    Global.events = Bdd.getEvents();
    Global.countdowns = Bdd.getCountdowns();
    Global.medias = Bdd.getMedias();
    
    Global.lightData = Bdd.getSensorData(SensorType.LIGHT);
    Global.airQualityData = Bdd.getSensorData(SensorType.AIR_QUALITY);
    Global.humidityData = Bdd.getSensorData(SensorType.HUMIDITY);
    Global.hiveTrafficData = Bdd.getSensorData(SensorType.HIVE_TRAFFIC);
    Global.hivePressureData = Bdd.getSensorData(SensorType.HIVE_PRESSURE);
    Global.hiveTemperatureData = Bdd.getSensorData(SensorType.HIVE_TEMPERATURE);

    return new MaterialApp(
      debugShowCheckedModeBanner: false,
      home: HomePage(),
    );
  }
}
