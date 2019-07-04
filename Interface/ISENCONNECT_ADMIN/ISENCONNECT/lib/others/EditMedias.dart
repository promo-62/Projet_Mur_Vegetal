import 'dart:convert';
import 'package:ISENCONNECT/main.dart';
import 'package:ISENCONNECT/Global.dart';
import 'package:ISENCONNECT/others/Media.dart';
import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';
import 'package:ISENCONNECT/BDD/Bdd.dart';
import 'dart:io';
import 'dart:core';
import 'package:validators/validators.dart';

class EditPicturesPage extends StatefulWidget {
  @override
  EditPicturesPageState createState() => EditPicturesPageState();
}

class EditPicturesPageState extends State < EditPicturesPage > {
  File _imageFile;
  String _imageString = "";
  String _urlYoutube = "";
  bool isSwitched = false;
  DateTime tmp = new DateTime.now();
  DateTime tmp2 = new DateTime(2025);


  void getImage() async {
    //on cherche la photo dans la galerie téléphone
    _imageFile = await ImagePicker.pickImage(source: ImageSource.gallery);
    setState(() {
      if (_imageFile != null)
        _imageString = base64.encode(_imageFile.readAsBytesSync());
    });
  }

  Widget showImage() {
    //permet d'afficher l'image
    if (_imageString != "") {
      return Image.memory(base64.decode(_imageString));
    } else {
      return const Text('Cliquez pour ajouter',
        style: TextStyle(
          fontFamily: 'AbrilFatface-Regular',
          fontSize: 40
        ),
        textAlign: TextAlign.center,
      );
    }
  }

  @override
  Widget build(BuildContext ctxt) {

    return Scaffold(
      appBar: AppBar(
        backgroundColor: Colors.amber[500],
        title: Text("Ajouter un media"),
      ),
      body: ListView(
        children: < Widget > [

          Row(
            mainAxisAlignment: MainAxisAlignment.center,
            children: < Widget > [
              Text("Photos"),
              //Bouton Switch
              Switch(
                value: isSwitched,
                activeColor: Colors.amber[600],
                onChanged: (value) {
                  setState(() {
                    isSwitched = value;
                  });
                }
              ),
              Text("Vidéo Youtube")
            ],
          ),

          isSwitched
          // --- ZONE LIEN URL --- //
          ?
          Padding(
            padding: EdgeInsets.only(left: 5, right: 5),
            child: TextField(
              enabled: true,
              decoration: InputDecoration(
                labelStyle: TextStyle(color: Colors.black),
                labelText: "Youtube",
                focusedBorder: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(5),
                    borderSide: BorderSide(color: Colors.amber[500])),
                enabledBorder: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(5),
                ),
              ),
              obscureText: false,
              onChanged: (String txt) {
                _urlYoutube = txt;
              },
            )
          )

          // --- ZONE D'IMAGE --- //
          :
          Container(
            height: MediaQuery.of(ctxt).size.height / 3,
            padding: EdgeInsets.all(MediaQuery.of(ctxt).size.height / 50),
            child: InkWell(
              onTap: () {
                getImage();
              },
              child: Card(
                elevation: 10.0,
                child: showImage(),
              ),
            ),
          ),


          // --- SAUVEGARDE --- //
          Padding(
            padding: EdgeInsets.only(left: 5, right: 5),
            child: RaisedButton(
              color: Colors.amber[600],
              child: Text(
                "Ajouter",
                style: TextStyle(color: Colors.white),
              ),
              onPressed: () {
                if (!((!isBase64(_imageString)) &&
                  ((!contains(_urlYoutube, "https://www.youtube.com") &&
                    !contains(_urlYoutube, "https://youtu.be")))))
                  setState(() async {

                    await Bdd.postMedia(body: new Media(
                      "",
                      _imageString,
                      Global.dateTimeToTimestamp(tmp),
                      Global.dateTimeToTimestamp(tmp2),
                      _urlYoutube,
                      ""
                    ).toJson());

                    Navigator.pop(ctxt);
                  });
              }
            ),
          ),

        ]
      )
    );
  }


}