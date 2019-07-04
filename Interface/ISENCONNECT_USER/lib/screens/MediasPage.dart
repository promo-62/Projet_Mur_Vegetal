import 'package:flutter/material.dart';

import 'package:ISENCONNECT/main.dart';

import 'package:ISENCONNECT/Global.dart';
import 'package:ISENCONNECT/BDD/Bdd.dart';

class MediasPage extends StatefulWidget {
  @override
  PicturesPageState createState() => PicturesPageState();
}

class PicturesPageState extends State < MediasPage > {

  @override
  Widget build(BuildContext ctxt) {
    return Scaffold(

      body: Center(
        child: getElementListMediaView()
      )
    );
  }

  Widget getElementListMediaView() {

    return FutureBuilder(
      future: Global.medias,
      builder: (ctxt, snapshot) {
        if (snapshot.hasData) {
          return GridView.builder(
            gridDelegate: SliverGridDelegateWithFixedCrossAxisCount(crossAxisCount: 2),
            itemCount: snapshot.data.length,
            itemBuilder: (BuildContext ctxt, index) {
              return Card(
                child: snapshot.data[index].showMedia(ctxt),
              );
            }
          );
        } else return CircularProgressIndicator();
      }
    );
  }

}