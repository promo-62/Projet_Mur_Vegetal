import 'package:flutter/material.dart';
import 'package:ISENCONNECT/others/Countdown.dart';

import 'Event.dart';
import 'Sample.dart';
import 'Countdown.dart';

import 'dart:convert';
import 'dart:io';

class Global
{

  static String url = "http://iotdata.yhdf.fr/api/web";
  //static String url = "http://192.168.43.109:4001/api/web";
  static String eventsURL = "/events";
  static String timersURL = "/countdowns";
  static String alertsURL = "/alerts";
  static String sensorsDataURL = "/samples";

  static Future < List < Event >> events;
  static Future < List < Countdown >> countdowns;
  
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