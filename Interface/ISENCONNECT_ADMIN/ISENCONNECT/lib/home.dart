import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flip_box_bar/flip_box_bar.dart';
import 'package:ISENCONNECT/screens/EventsPage.dart';
import 'package:ISENCONNECT/screens/WallPage.dart';
import 'package:ISENCONNECT/screens/Reseaux.dart';
import 'package:ISENCONNECT/screens/MediasPage.dart';

class HomePage extends StatefulWidget {
  @override
  _HomeState createState() => _HomeState();
}

class _HomeState extends State<HomePage> {
  int selectedIndex = 0;

  @override
  Widget build(BuildContext ctxt) {
    SystemChrome.setPreferredOrientations([
        DeviceOrientation.portraitUp,
        DeviceOrientation.portraitDown,
      ]);
    return new Scaffold(
      //contenu principal
      appBar: AppBar(
        backgroundColor: Colors.red[700],
        title: Container(
          child:Image.asset('connect.png'),
        ),
        centerTitle: true,
      ),
      body: [
        EventsPage(),
        MediasPage(),
        WallPage(),
        SocialPage(),
      ].elementAt(selectedIndex),
      bottomNavigationBar: FlipBoxBar(
        selectedIndex: selectedIndex,
        items: [
          FlipBarItem(
            icon: Icon(Icons.event),
            text: Text("Événements"),
            frontColor: Colors.orange[700],
            backColor: Colors.orange[500]),
          FlipBarItem(
            icon: Icon(Icons.photo),
            text: Text("Medias"),
            frontColor: Colors.amber[500],
            backColor: Colors.amber[400]),
          FlipBarItem(
            icon: Icon(Icons.spa),
            text: Text("Données"),
            frontColor: Colors.green[600],
            backColor: Colors.green[500]),
          FlipBarItem(
            icon: Icon(Icons.people),
            text: Text(
            "Réseaux Sociaux",
            textAlign: TextAlign.center,
            ),
            frontColor: Colors.blue[500],
            backColor: Colors.blue[400]),
        ],
        onIndexChanged: (newIndex) {
          setState(() {
            selectedIndex = newIndex;
          });
        },
      ),
    );
  }
}
