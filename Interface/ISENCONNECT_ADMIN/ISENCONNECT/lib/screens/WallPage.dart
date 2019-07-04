import 'package:ISENCONNECT/others/Thumbnails.dart';
import 'package:flutter/material.dart';
import 'package:flutter/widgets.dart';
import 'package:ISENCONNECT/Global.dart';
import 'package:ISENCONNECT/BDD/Alert.dart';

class Personal {
  Personal._();

  static const _kFontFam = 'Personal';

  static const IconData watering_can =
      const IconData(0xe800, fontFamily: _kFontFam);
  static const IconData temperature =
      const IconData(0xe801, fontFamily: _kFontFam);
  static const IconData bee = const IconData(0xe802, fontFamily: _kFontFam);
}

class WallPage extends StatelessWidget {

  @override
  Widget build(BuildContext ctxt) {

    List <InkWell> listInkWell = new List();

// humidity sensor :
   ThumbNails thumbNails = new ThumbNails('Humidité du Mur',Icons.local_drink,Colors.blue[900],Colors.blue[200]);
   listInkWell.add(thumbNails.showThumbnails(ctxt));

// luminosity sensor:
  thumbNails = new ThumbNails('Luminosité du Mur',Icons.brightness_6,Colors.amber[900],Colors.amber[200]);
  listInkWell.add(thumbNails.showThumbnails(ctxt));

 // bees sensor : 

   thumbNails = new ThumbNails('Abeilles dans la ruche',Personal.bee,Colors.grey[700],Colors.yellow);
   listInkWell.add(thumbNails.showThumbnails(ctxt));
    
// air quality sensor : 

  thumbNails = new ThumbNails('Qualité de l\'air',Icons.toys,Colors.lightGreen[900],Colors.lightGreen[300]);
  listInkWell.add(thumbNails.showThumbnails(ctxt));

  // atmospheric pressure sensor :

  thumbNails = new ThumbNails('Pression atmosphérique sur le toit',Icons.cloud,Colors.brown,Colors.brown[200]);
  listInkWell.add(thumbNails.showThumbnails(ctxt));

  // outside temperature sensor :
  thumbNails = new ThumbNails('Température sur le toit',Personal.temperature,Colors.red[400],Colors.red[100]);
  listInkWell.add(thumbNails.showThumbnails(ctxt));

  return Scaffold(
      body: Center(
        child: GridView.extent(
          maxCrossAxisExtent: 300,
          padding: const EdgeInsets.all(4),
          mainAxisSpacing: 4,
          crossAxisSpacing: 4,
          children: listInkWell,
        ),
      ),
      floatingActionButton: FloatingActionButton(
        backgroundColor: Colors.white,
        child: Icon(Icons.error, color: Colors.red),
        onPressed: () {
          showDialog(
            context: ctxt,
            builder: (BuildContext context) {
              return FutureBuilder(
                future: Global.alerts,
                builder: (context, snapshot) {
                  if (snapshot.hasData) {
                    return alertLoaded(snapshot.data, context);
                  }
                   else{
                     return alertLoading(context);
                   }
                }
              );
          });
        },
      ),
      floatingActionButtonLocation: FloatingActionButtonLocation.endFloat,
    );
  }

  AlertDialog alertLoaded(List<Alert> alerts, BuildContext context){
    if(alerts.isNotEmpty) {
      return AlertDialog(
          shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(20.0)),
          elevation: 10.0,
          title: new Text("Alerte capteur n°" +
              getLastAlertId(alerts).toString() +
              " nommé " + getLastAlertName(alerts)),
          content: new Text("Raison : " +
              getLastAlertReason(alerts)),
          actions: <Widget>[
            new FlatButton(
              child: new Text("OK",
                  style: TextStyle(color: Colors.red[700])),
              onPressed: () {
                Navigator.of(context).pop();
              },
            )
          ]
      );
    }
    else if(alerts.isEmpty){
      return AlertDialog(
          shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(20.0)),
          elevation: 10.0,
          title: new Text("Aucune alerte"),
          actions: <Widget>[
            new FlatButton(
              child: new Text("OK", style :TextStyle(color: Colors.red[700])),
              onPressed: () {
                Navigator.of(context).pop();
              },
            )
          ]
      );
    }
    return null;
  }

  AlertDialog alertLoading(BuildContext context){
    return AlertDialog(
          shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(20.0)),
          elevation: 10.0,
          title: new Text("Alerts loading ..."),
          actions: <Widget>[
            new FlatButton(
              child: new Text("OK", style :TextStyle(color: Colors.red[700])),
              onPressed: () {
                Navigator.of(context).pop();
              },
            )
          ]
      );
  }
  int getLastAlertId(List<Alert> alerts){
    if(alerts.isNotEmpty)
      return alerts[alerts.length - 1].idSensor;
    return 0;
  }
  String getLastAlertName(List<Alert> alerts){
    if(alerts.isNotEmpty)
      return alerts[alerts.length - 1].name;
    return "null";
  }
  String getLastAlertReason(List<Alert> alerts){
    if(alerts.isNotEmpty)
      return alerts[alerts.length - 1].alertReason;
    return "null";
  }
}
