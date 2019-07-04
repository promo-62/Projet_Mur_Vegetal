import 'package:flutter/material.dart';

import 'package:ISENCONNECT/main.dart';
import 'package:ISENCONNECT/others/EditMedias.dart';
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

      floatingActionButton: FloatingActionButton(
        backgroundColor: Colors.amber[500],
        onPressed: () {
          imageInString = "";
          Navigator.push(
            ctxt, MaterialPageRoute(builder: (ctxt) => EditPicturesPage()));
        },
        tooltip: 'Inbox',
        child: Icon(Icons.add_a_photo),
      ),

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
              return InkWell(
                child: snapshot.data[index].showMedia(ctxt),
                onLongPress: () {
                  showDialog(
                    context: ctxt,
                    builder: (BuildContext context) {
                      return AlertDialog(
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(20.0)),
                        elevation: 10.0,
                        title: new Text("Supprimer le media ?"),
                        content: new Text(
                          "Êtes-vous sûr(e) de vouloir supprimer la photo/vidéo ?"),
                        actions: < Widget > [
                          new FlatButton(
                            child: new Text("Fermer", style: TextStyle(color: Colors.blue[700])),
                            onPressed: () {
                              Navigator.of(context).pop();
                            },
                          ),
                          new FlatButton(
                            child: new Text("Supprimer", style: TextStyle(color: Colors.red[700])),
                            onPressed: () async {
                              setState(() {
                                Bdd.deleteMedia(snapshot.data[index].id);
                                snapshot.data.removeAt(index);
                              });
                              Navigator.of(context).pop();
                            },
                          ),
                        ],
                      );
                    }
                  );
                }
              );
            }
          );
        } else return CircularProgressIndicator();
      }
    );
  }

}