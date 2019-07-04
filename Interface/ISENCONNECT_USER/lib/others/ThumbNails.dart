import 'package:ISENCONNECT/screens/Graphs.dart';
import 'package:flutter/material.dart';
import 'package:flutter/painting.dart';
import 'package:ISENCONNECT/Global.dart';
import 'package:ISENCONNECT/BDD/Sample.dart';
import 'package:ISENCONNECT/BDD/SensorType.dart';

class ThumbNails{
  String title;
  IconData icon;
  Color textColor;
  Color backgroundColor;
  bool alert;

  ThumbNails(String title, IconData icon, Color textColor, Color backgroundColor, bool alert){
    this.title = title;
    this.icon = icon;
    this.textColor = textColor;
    this.backgroundColor = backgroundColor;
    this.alert = alert;
  }

  InkWell showThumbnails(BuildContext context){
    
    Future<List<Sample>> list;
    String unit = "";
    SensorType sensorType;

    switch (title){
      case "Humidité du Mur": 
        list = Global.humidityData;
        unit = " %";
        sensorType = SensorType.HUMIDITY;
        break;
      case "Abeilles dans la ruche":
        list = Global.hiveTrafficData;
        sensorType = SensorType.HIVE_TRAFFIC;
        break;
      case "Qualité de l\'air":
        list = Global.airQualityData;
        unit = " %";
        sensorType = SensorType.AIR_QUALITY;
        break;
      case "Pression atmosphérique sur le toit":
        list = Global.hivePressureData;
        unit = " bar";
        sensorType = SensorType.HIVE_PRESSURE;
        break;
      case "Température sur le toit":
        list = Global.hiveTemperatureData;
        unit = " °C";
        sensorType = SensorType.HIVE_TEMPERATURE;
        break;
      default:
        list = Global.lightData;
        unit = " lux";
        sensorType = SensorType.LIGHT;
        break;
    }

    return InkWell(
      customBorder: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(20.0)),
      highlightColor: textColor,
      onTap: () {
        Navigator.push(
            context, MaterialPageRoute(builder: (ctxt) => Graph(sensorType)));
      },

      child: Card(
        shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(20.0)),
        color: backgroundColor,
        elevation: 10.0,
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            FutureBuilder(
              future: list,
              builder: (context, snapshot){
                if (snapshot.hasData){
                  if (snapshot.data.length > 0)
                    return 
                      Text(snapshot.data[0].value.toString() + unit,
                        style: TextStyle(
                        fontFamily: 'AbrilFatface-Regular',
                        fontSize: MediaQuery.of(context).size.height / 14,
                        color: textColor),
                        textAlign: TextAlign.center
                      );
                  else 
                    return
                      Icon(Icons.signal_wifi_off,
                        size: MediaQuery.of(context).size.height / 14,
                        color: textColor);
                }
                else
                  return  CircularProgressIndicator();
              },),
            Text(title,
                  style: TextStyle(
                      fontFamily: 'AbrilFatface-Regular',
                      fontSize: MediaQuery.of(context).size.height / 50,
                      color: textColor),
                  textAlign: TextAlign.center),
            Icon(icon,
                size: MediaQuery.of(context).size.height / 14,
                color: textColor),
          ],
        ),
      ),
    );
  }

}