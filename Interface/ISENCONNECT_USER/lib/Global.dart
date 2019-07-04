import 'package:flutter/material.dart';
import 'package:ISENCONNECT/BDD/Countdown.dart';

import 'BDD/Event.dart';
import 'BDD/Sample.dart';
import 'BDD/Countdown.dart';
import 'package:ISENCONNECT/Others/Media.dart';

import 'dart:convert';
import 'dart:io';

class Global
{

  static String url = "http://iotdata.yhdf.fr/api/web";
  static String eventsURL = "/events";
  static String countdownsURL = "/countdowns";
  static String mediasURL = "/medias";
  static String alertsURL = "/alerts";
  static String sensorsDataURL = "/samples";
  static String authentificationID = "Basic aG9sb2xlbnM6dXJiQGIwaXMyMDE5";

  static Future < List < Event >> events;
  static Future < List < Countdown >> countdowns;
  static Future < List < Media >> medias;
  
  static Future < List < Sample >> lightData;
  static Future < List < Sample >> airQualityData;
  static Future < List < Sample >> humidityData;
  static Future < List < Sample >> hiveTrafficData;
  static Future < List < Sample >> hivePressureData;
  static Future < List < Sample >> hiveTemperatureData;

  static int dateTimeToTimestamp(DateTime dateTime){
    return dateTime.difference(new DateTime(1970)).inSeconds;
  }
  static DateTime dateTimeFromTimeStamp(int timestamp){
    return DateTime.fromMillisecondsSinceEpoch(timestamp * 1000);
  }

  static Image imageFromString(String string){
    return Image.memory(base64.decode(string));
  }
  static String stringFromFile(File file){
    return base64.encode(file.readAsBytesSync());
  }

// -- Add a sensor data here --
}