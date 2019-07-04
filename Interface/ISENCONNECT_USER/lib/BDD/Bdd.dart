import 'package:ISENCONNECT/BDD/Alert.dart';

import 'Event.dart';
import 'Countdown.dart';
import 'Sample.dart';
import 'SensorType.dart';
import 'Alert.dart';
import 'package:ISENCONNECT/Global.dart';
import 'package:ISENCONNECT/Others/Media.dart';
// imports for json and http request
import 'package:validators/validators.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';
import 'dart:async';
import 'dart:core';
import 'dart:io';


class Bdd{

// Returns the list of Event objects contained in the server

  static Future<List<Event>> getEvents() async
  {
    List<Event> list = new List();
    try{
      var res = await http.get(Global.url + Global.eventsURL, headers: {
        HttpHeaders.authorizationHeader: Global.authentificationID,
        HttpHeaders.userAgentHeader: "Mozilla"
      });
        if (res.statusCode == 200) {
          List<dynamic> data = json.decode(res.body) as List;
          list = data.map<Event>((json) => Event.fromJson(json)).toList();
        }
        else{
          print("Error, events statusCode not valid : " + res.statusCode.toString());
        }
    }
    catch(e){
      print ("Error during events loading");
    }
    for(int i = 0 ; i < list.length ; i++){
      if (list[i].image == null || !isBase64(list[i].image) ||
      (Global.dateTimeFromTimeStamp(list[i].endingDate).difference(DateTime.now()).inSeconds < 0) || 
      (Global.dateTimeFromTimeStamp(list[i].beginningDate).difference(DateTime.now()).inSeconds > 0)){
        print("Event non conforme : " + list[i].title);
        list.removeAt(i);
        i--;
      }
      else
        print("Event conforme : " + list[i].title);
    }
    return list;
  }

  static Future<List<Media>> getMedias() async{
    List<Media> list = new List();
    try{
      var res = await http.get(Global.url + Global.mediasURL, headers: {
        HttpHeaders.authorizationHeader: Global.authentificationID,
        HttpHeaders.userAgentHeader: "Mozilla"
      });
        if (res.statusCode == 200) {
          List<dynamic> data = json.decode(res.body) as List;
          list = data.map<Media>((json) => Media.fromJson(json)).toList();
        }
        else{
          print("Error, medias statusCode not valid : " + res.statusCode.toString());
        }
    }
    catch(e){
      print ("Error during medias loading");
    }
    for(int i = 0 ; i < list.length ; i++){
      if ((Global.dateTimeFromTimeStamp(list[i].endingDate).difference(DateTime.now()).inSeconds < 0) || 
      (Global.dateTimeFromTimeStamp(list[i].beginningDate).difference(DateTime.now()).inSeconds > 0) || 
      (list[i].image == null || !isBase64(list[i].image)) && 
      (list[i].videoURL == null || (!contains(list[i].videoURL, "https://www.youtube.com") &&
      !contains(list[i].videoURL, "https://youtu.be")))){
        print("Media non conforme : " + list[i].title);
        list.removeAt(i);
        i--;
      }
      else
        print("Media conforme : " + list[i].title);
    }
    return list;
  }

  static Future<List<Countdown>> getCountdowns() async
  {
    List<Countdown> list = new List();
    try{
      var res = await http.get(Global.url + Global.countdownsURL, headers: {
        HttpHeaders.authorizationHeader: Global.authentificationID,
        HttpHeaders.userAgentHeader: "Mozilla"
      });
        if (res.statusCode == 200) {
          List<dynamic> data = json.decode(res.body) as List;
          list = data.map<Countdown>((json) => Countdown.fromJson(json)).toList();
        }
        else{
          print("Error, countdown statusCode not valid : " + res.statusCode.toString());
        }
    }
    catch(e){
      print ("Error during countdowns loading");
    }
    for(int i = 0 ; i < list.length ; i++)
      if (list[i].image != null && !isBase64(list[i].image) || list[i].deadline == null ||
      (Global.dateTimeFromTimeStamp(list[i].endingDate).difference(DateTime.now()).inSeconds < 0) ||
       (Global.dateTimeFromTimeStamp(list[i].beginningDate).difference(DateTime.now()).inSeconds > 0)){
        print("Countdown non conforme : " + list[i].title + " / " + Global.dateTimeFromTimeStamp(list[i].deadline).toString());
        list.removeAt(i);
        i--;
      }
      else
        print("Countdown conforme : " + list[i].title + " / " + Global.dateTimeFromTimeStamp(list[i].deadline).toString());
    return list;
  }

  static Future<List<Sample>> getSensorData(SensorType sensorType) async
  {
    List<Sample> tmp = new List();
    List<Sample> list = new List();

    try{
      var res = await http.get(Global.url + Global.sensorsDataURL, headers: {
        HttpHeaders.authorizationHeader: Global.authentificationID,
        HttpHeaders.userAgentHeader: "Mozilla"
      });
        if (res.statusCode == 200) {
          List<dynamic> data = json.decode(res.body) as List;
          tmp = data.map<Sample>((json) => Sample.fromJson(json)).toList();
          print("samples received");
        }
        else{
          print("Error, samples statusCode not valid : " + res.statusCode.toString());
        }
    }
    catch(e){
      print ("Error during sensor data loading");
    }

    for (Sample s in tmp){
      if (s.sampleType == sensorType){
        list.add(s);
      }
    }
    list.sort((a, b) => b.timestamp.compareTo(a.timestamp)); // Sort from last to first registered sample
    
    return list;
  }

  static Future<List<Alert>> getAlert() async
  {
    List<Alert> list = new List();
    try{
      var res = await http.get(Global.url + Global.alertsURL, headers: {
        HttpHeaders.authorizationHeader: Global.authentificationID,
        HttpHeaders.userAgentHeader: "Mozilla"
      });
      if (res.statusCode == 200) {
        List<dynamic> data = json.decode(res.body) as List;
        list = data.map<Alert>((json) => Alert.fromJson(json)).toList();
      }
      else{
        print("Error, alerts statusCode not valid : " + res.statusCode.toString());
      }
    }
    catch(e){
      print ("Error during alerts loading");
    }
    print("Alerts loaded");
    return list;
  }

  static postEvent({Map body}) async
  {
    await http.post(Global.url + Global.eventsURL, headers: {
      HttpHeaders.contentTypeHeader: 'application/json',
      HttpHeaders.authorizationHeader: Global.authentificationID,
      HttpHeaders.userAgentHeader: "Mozilla"
    }, body: json.encode(body));

    Global.events = getEvents();
  }

  static postMedia({Map body}) async{
    await http.post(Global.url + Global.mediasURL, headers: {
      HttpHeaders.contentTypeHeader: 'application/json',
      HttpHeaders.authorizationHeader: Global.authentificationID,
      HttpHeaders.userAgentHeader: "Mozilla"
    }, body: json.encode(body));

    Global.medias = getMedias();
  }

  static postCountdown({Map body}) async
  {
    await http.post(Global.url + Global.countdownsURL, headers: {
      HttpHeaders.contentTypeHeader: 'application/json',
      HttpHeaders.authorizationHeader: Global.authentificationID
    }, body: json.encode(body));

    Global.countdowns = getCountdowns();
  }
}