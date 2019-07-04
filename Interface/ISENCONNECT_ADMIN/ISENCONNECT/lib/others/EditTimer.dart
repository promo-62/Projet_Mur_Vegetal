import 'package:ISENCONNECT/BDD/Countdown.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:ISENCONNECT/main.dart';
import 'package:flutter/painting.dart';
import 'package:image_picker/image_picker.dart';
import 'package:ISENCONNECT/BDD/Event.dart';
import 'dart:convert';
import 'dart:core';
import 'package:ISENCONNECT/Global.dart';
import 'package:ISENCONNECT/BDD/Bdd.dart';

class EditTimer extends StatefulWidget {
  @override
  EditTimerState createState() => EditTimerState();
}

class EditTimerState extends State<EditTimer> with TickerProviderStateMixin {
  //---- INITIALISATION VARIABLE ---//
  String _titre = "";
  String _text = "";
  int _beginButton = 0;
  AnimationController controller;

  DateTime selectedDate;
  DateTime today = DateTime.now();
  Duration eventDate;
  DateTime _beginningDate = DateTime.now();
  DateTime _endingDate = DateTime.now().add(new Duration(days: 365));
  TextEditingController _controllerTitre;
  TextEditingController _controllerText;
  var _menus = ['Ajout de Event', 'Ajout de timer avec photo'];
  var _currentItemSelected = 'Ajout de Event';

  String get timerString {
    Duration duration = controller.duration * controller.value;
    return '${duration.inDays}j:${duration.inHours % 24}h:${duration.inMinutes % 60}m:${(duration.inSeconds % 60).toString().padLeft(2, '0')}s';
  }

  @override
  void initState() {
    _controllerTitre = new TextEditingController(text: '');
    _controllerText = new TextEditingController(text: '');

    controller = AnimationController(
      vsync: this,
      duration: Duration(days: 0, hours: 0, minutes: 0, seconds: 0),
    );
    super.initState();
  }

  @override
  dispose() {
    controller.dispose();
    super.dispose();
  }

  void getImage() async {
    //on cherche la photo dans la galerie téléphone
    imageFile = await ImagePicker.pickImage(source: ImageSource.gallery);
    setState(() {
      if (imageFile != null)
        imageInString = base64.encode(imageFile.readAsBytesSync());
    });
  }

  Widget showImage() {
    //permet de charger l'image pour modification
    if (skipImage == 0) {
      skipImage = 1;
      imageInString = editImage;
      return Image.memory(base64.decode(imageInString));
    }
    //permet d'afficher l'image
    else if (imageInString != "") {
      return Image.memory(base64.decode(imageInString));
    } else {
      return const Text(
        'Cliquez pour ajouter',
        style: TextStyle(fontFamily: 'AbrilFatface-Regular', fontSize: 40),
        textAlign: TextAlign.center,
      );
    }
  }

  // ---- FONCTION SELECTION DATE D'EVENEMENT---- //
  Future<Null> _selectDate(BuildContext context) async {
    final DateTime picked = await showDatePicker(
        context: context,
        initialDate: today,
        firstDate: DateTime(2015, 8),
        lastDate: DateTime(2101));
    if (picked != null &&
        picked != selectedDate &&
        picked.difference(today) > Duration.zero) {
      setState(() {
        selectedDate = picked;
        eventDate = selectedDate.difference(today);
        controller = AnimationController(
          vsync: this,
          duration: eventDate,
        );
        controller.reverse(
            from: controller.value == 0.0 ? 1.0 : controller.value);
      });
    }
  }

  // ---- FONCTION SELECTION DATE D'AFFICHAGE ---- //
  Future<Null> _selectDate2(BuildContext context) async {
    final DateTime picked = await showDatePicker(
        context: context,
        initialDate: today,
        firstDate: DateTime(2015, 8),
        lastDate: DateTime(2101));
    if (picked != null &&
        picked != selectedDate &&
        picked.difference(today) > Duration.zero) {
      setState(() {
        if (_beginButton == 1) {
          _beginningDate = picked;
        } else {
          _endingDate = picked;
        }
      });
    }
  }

  @override
  Widget build(BuildContext ctxt) {
    //permet de charger le titre, le texte,le timer et la date d'affichage pour modification
    if (skipTexte == 0) {
      skipTexte = 1;

      _controllerTitre.text = editTitle;
      _controllerText.text = editText;
      _titre = editTitle;
      _text = editText;
      _beginningDate = editBeginningDate;
      _endingDate = editEndingDate;

      if (skipTimer == 0) {
        skipTimer = 1;
        eventDate = editTimer.difference(today);
        selectedDate = editTimer;
        controller.duration = eventDate;
        controller.reverse(
            from: controller.value == 0.0 ? 1.0 : controller.value);
      }
    }

    ThemeData themeData = Theme.of(ctxt);

    return Scaffold(
        appBar: AppBar(
          backgroundColor: Colors.orange[400],
          title: Text("Editing Menu"),
        ),
        body: ListView(children: <Widget>[
          // --- MENU DEROULANT --- //
          Padding(
            padding: EdgeInsets.only(left: 11, top: 10),
            child: DropdownButton<String>(
              items: _menus.map((String dropDownStringItem) {
                return DropdownMenuItem<String>(
                  value: dropDownStringItem,
                  child: new Text(dropDownStringItem),
                );
              }).toList(),
              onChanged: (String newValueSelected) {
                setState(() {
                  this._currentItemSelected = newValueSelected;
                });
              },
              value: _currentItemSelected,
            ),
          ),

          // --- ZONE TITRE --- //
          Padding(
            padding: EdgeInsets.all(10),
            child: TextField(
              controller: _controllerTitre,
              cursorColor: Colors.orange[700],
              enabled: true,
              decoration: InputDecoration(
                labelText: "Titre d'évènement",
                labelStyle: TextStyle(color: Colors.black),
                focusedBorder: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(5),
                    borderSide: BorderSide(color: Colors.orange)),
                enabledBorder: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(5),
                ),
              ),
              obscureText: false,
              onChanged: (String txt) {
                _titre = txt;
              },
            ),
          ),

          // --- ZONE TEXTE --- //
          Padding(
            padding: EdgeInsets.all(10),
            child: TextField(
              keyboardType: TextInputType.multiline,
              maxLines: null,
              controller: _controllerText,
              enabled: true,
              decoration: InputDecoration(
                labelStyle: TextStyle(color: Colors.black),
                labelText: "Texte",
                focusedBorder: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(5),
                    borderSide: BorderSide(color: Colors.orange)),
                enabledBorder: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(5),
                ),
              ),
              obscureText: false,
              onChanged: (String txt) {
                _text = txt;
              },
            ),
          ),

          // -->AFFICHAGE DIFFERENT SELON LE CHOIX DE PAGE<-- //
          // Si on ne veut qu'un Event => :
          _currentItemSelected == 'Ajout de Event'
              ? Padding(
                  padding: EdgeInsets.only(bottom: 5),
                )

              //Si on ne veut un timer avec/sans photo => :
              : Column(
                  mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                  crossAxisAlignment: CrossAxisAlignment.center,
                  children: <Widget>[
                    //--- BOUTON Selection Date ---//
                    RaisedButton(
                        color: Colors.orange[400],
                        child: Text(
                          "Selectionner la date",
                          style: TextStyle(color: Colors.white),
                        ),
                        onPressed: () {
                          setState(() {
                            _selectDate(ctxt);
                          });
                        }),

                    Text(
                      "Prochain event dans :",
                      style: themeData.textTheme.subhead,
                    ),

                    AnimatedBuilder(
                        animation: controller,
                        builder: (BuildContext ctxt, Widget child) {
                          return Text(
                            timerString,
                            style: themeData.textTheme.display1,
                          );
                        }),
                  ],
                ),
          // --- FIN DES PAGES ADAPTATIVES --- //

          // --- ZONE D'IMAGE --- //
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
          Row(mainAxisAlignment: MainAxisAlignment.center, children: <Widget>[
            // --- BOUTON Selection Date Début ---//
            Padding(
              padding: EdgeInsets.only(right: 5),
              child: RaisedButton(
                  color: Colors.orange[400],
                  child: Text(
                    "Afficher à partir du : \n${_beginningDate.day.toString()} / ${_beginningDate.month.toString()} / ${_beginningDate.year.toString()}",
                    style: TextStyle(color: Colors.white),
                  ),
                  onPressed: () {
                    setState(() {
                      _beginButton = 1;
                      _selectDate2(ctxt);
                    });
                  }),
            ),

            // --- BOUTON Selection Date Fin ---//
            Padding(
              padding: EdgeInsets.only(right: 5),
              child: RaisedButton(
                  color: Colors.orange[400],
                  child: Text(
                    "Jusqu'au : \n${_endingDate.day.toString()} / ${_endingDate.month.toString()} / ${_endingDate.year.toString()}",
                    textAlign: TextAlign.center,
                    style: TextStyle(color: Colors.white),
                  ),
                  onPressed: () {
                    setState(() {
                      _beginButton = 0;
                      _selectDate2(ctxt);
                    });
                  }),
            ),
          ]),

          //--- BOUTON Ajouter ---//
          Padding(
            padding: EdgeInsets.only(left: 5, right: 5),
            child: RaisedButton(
              child: Text(
                "Ajouter",
                style: TextStyle(color: Colors.white),
              ),
              color: Colors.orange[700],
              onPressed: () async {
                if (_currentItemSelected == 'Ajout de Event') {
                  if (imageInString != null && imageInString != "") {
                    Event tmp = new Event(
                        _titre,
                        _text,
                        imageInString,
                        Global.dateTimeToTimestamp(_beginningDate),
                        Global.dateTimeToTimestamp(_endingDate),
                        4,
                        "");
                    await Bdd.postEvent(body: tmp.toJson());
                    Navigator.pop(ctxt);
                  } else
                    ; // pop up "Veuillez sélectionner une image"
                } else {
                  Countdown tmp = new Countdown(
                      _titre,
                      _text,
                      imageInString,
                      Global.dateTimeToTimestamp(_beginningDate),
                      Global.dateTimeToTimestamp(_endingDate),
                      4,
                      "",
                      Global.dateTimeToTimestamp(selectedDate));
                  Bdd.postCountdown(body: tmp.toJson());
                  Navigator.pop(ctxt);
                }
              },
            ),
          ),

          _currentItemSelected == 'Ajout de Event'
              // --- LISTES EVENEMENTS AJOUTES --- //
              ? Container(
                  height: MediaQuery.of(ctxt).size.height / 3,
                  child: FutureBuilder(
                      future: Global.events,
                      builder: (ctxt, snapshot) {
                        if (snapshot.hasData)
                          return getElementListEventView(snapshot.data);
                        else
                          return CircularProgressIndicator();
                      }))
              : // --- LISTES TIMERS AJOUTES --- //
              Container(
                  height: MediaQuery.of(ctxt).size.height / 3,
                  child: FutureBuilder(
                      future: Global.countdowns,
                      builder: (ctxt, snapshot) {
                        if (snapshot.hasData)
                          return getElementListTimerView(snapshot.data);
                        else
                          return CircularProgressIndicator();
                      }))
        ]));
  }

  ListView getElementListEventView(List<Event> events) {
    events.sort((a, b) => a.position.compareTo(b.position));
    return ListView.builder(
        itemCount: events.length,
        itemBuilder: (BuildContext ctxt, index) {
          final item = events[index].title;

          return Dismissible(
            child: Card(
                elevation: 5,
                shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(5.0)),
                child: Container(
                    decoration: BoxDecoration(
                        border: Border.all(color: Colors.orange[400], width: 3),
                        borderRadius: BorderRadius.circular(5.0)),
                    width: MediaQuery.of(ctxt).size.width,
                    child: ListTile(
                      title: Text(events[index].title,
                          style: TextStyle(
                              fontFamily: "AbrilFatface-Regular",
                              fontSize: 30)),
                      subtitle: Text("du ${DateTime.fromMillisecondsSinceEpoch(events[index].beginningDate * 1000).day.toString()}" +
                          " / ${DateTime.fromMillisecondsSinceEpoch(events[index].beginningDate * 1000).month.toString()}" +
                          " / ${DateTime.fromMillisecondsSinceEpoch(events[index].beginningDate * 1000).year.toString()}" +
                          " au ${DateTime.fromMillisecondsSinceEpoch(events[index].endingDate * 1000).day.toString()}" +
                          " / ${DateTime.fromMillisecondsSinceEpoch(events[index].endingDate * 1000).month.toString()}" +
                          " / ${DateTime.fromMillisecondsSinceEpoch(events[index].endingDate * 1000).year.toString()}"),
                    ))),

            //background edit
            background: Container(
                alignment: Alignment.centerLeft,
                color: Colors.lightGreen,
                child: Padding(
                  padding: EdgeInsets.only(left: 10),
                  child: Icon(
                    Icons.edit,
                    color: Colors.white,
                  ),
                )),

            //background suppress
            secondaryBackground: Container(
                alignment: Alignment.centerRight,
                color: Colors.redAccent,
                child: Padding(
                  padding: EdgeInsets.only(right: 10),
                  child: Icon(
                    Icons.delete_forever,
                    color: Colors.white,
                  ),
                )),

            key: Key(item),
            onDismissed: (direction) {
              // delete
              if (direction == DismissDirection.endToStart) {
                showDialog(
                    context: ctxt,
                    builder: (BuildContext context) {
                      return AlertDialog(
                        shape: RoundedRectangleBorder(
                            borderRadius: BorderRadius.circular(20.0)),
                        elevation: 10.0,
                        title: new Text("Supprimer l'événment ?"),
                        content: new Text(
                            "Êtes-vous sûr de vouloir supprimer l'événement ?"),
                        actions: <Widget>[
                          new FlatButton(
                            child: new Text("Fermer",
                                style: TextStyle(color: Colors.blue[700])),
                            onPressed: () {
                              Navigator.of(context).pop();
                            },
                          ),
                          new FlatButton(
                            child: new Text("Supprimer",
                                style: TextStyle(color: Colors.red[700])),
                            onPressed: () {
                              setState(() {
                                Bdd.deleteEvent(events[index].id);
                                events.removeAt(index);
                                Scaffold.of(ctxt).showSnackBar(SnackBar(
                                  content: Text("Évènement Supprimé"),
                                ));
                              });
                              Navigator.of(context).pop();
                            },
                          ),
                        ],
                      );
                    });
              }
              // edit
              else {
                setState(() {
                  editTitle = events[index].title;
                  editText = events[index].text;
                  editImage = events[index].image;
                  editBeginningDate = DateTime.fromMillisecondsSinceEpoch(
                      events[index].beginningDate * 1000);
                  editEndingDate = DateTime.fromMillisecondsSinceEpoch(
                      events[index].endingDate * 1000);

                  skipTexte = 0;
                  skipImage = 0;

                  Bdd.deleteEvent(events[index].id);
                  events.removeAt(index);
                });
              }
            },
          );
        });
  }

  ListView getElementListTimerView(List<Countdown> countdowns) {
    countdowns.sort((a, b) => a.position.compareTo(b.position));
    return ListView.builder(
        itemCount: countdowns.length,
        itemBuilder: (BuildContext ctxt, index) {
          final item = countdowns[index].title;

          return Dismissible(
            child: Card(
                elevation: 5,
                shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(5.0)),
                child: Container(
                    decoration: BoxDecoration(
                        border: Border.all(color: Colors.orange[400], width: 3),
                        borderRadius: BorderRadius.circular(5.0)),
                    width: MediaQuery.of(ctxt).size.width,
                    child: ListTile(
                      title: Text(countdowns[index].title,
                          style: TextStyle(
                              fontFamily: "AbrilFatface-Regular",
                              fontSize: 30)),
                      subtitle: Text("du ${DateTime.fromMillisecondsSinceEpoch(countdowns[index].beginningDate * 1000).day.toString()}" +
                          " / ${DateTime.fromMillisecondsSinceEpoch(countdowns[index].beginningDate * 1000).month.toString()}" +
                          " / ${DateTime.fromMillisecondsSinceEpoch(countdowns[index].beginningDate * 1000).year.toString()}" +
                          " au ${DateTime.fromMillisecondsSinceEpoch(countdowns[index].endingDate * 1000).day.toString()}" +
                          " / ${DateTime.fromMillisecondsSinceEpoch(countdowns[index].endingDate * 1000).month.toString()}" +
                          " / ${DateTime.fromMillisecondsSinceEpoch(countdowns[index].endingDate * 1000).year.toString()}"),
                    ))),

            //background edit
            background: Container(
                alignment: Alignment.centerLeft,
                color: Colors.lightGreen,
                child: Padding(
                  padding: EdgeInsets.only(left: 10),
                  child: Icon(
                    Icons.edit,
                    color: Colors.white,
                  ),
                )),

            //background suppress
            secondaryBackground: Container(
                alignment: Alignment.centerRight,
                color: Colors.redAccent,
                child: Padding(
                  padding: EdgeInsets.only(right: 10),
                  child: Icon(
                    Icons.delete_forever,
                    color: Colors.white,
                  ),
                )),

            key: Key(item),
            onDismissed: (direction) {
              // delete
              if (direction == DismissDirection.endToStart) {
                showDialog(
                    context: ctxt,
                    builder: (BuildContext context) {
                      return AlertDialog(
                        shape: RoundedRectangleBorder(
                            borderRadius: BorderRadius.circular(20.0)),
                        elevation: 10.0,
                        title: new Text("Supprimer le compte à rebours ?"),
                        content: new Text(
                            "Êtes-vous sûr de vouloir supprimer le compte à rebours ?"),
                        actions: <Widget>[
                          new FlatButton(
                            child: new Text("Fermer",
                                style: TextStyle(color: Colors.blue[700])),
                            onPressed: () {
                              Navigator.of(context).pop();
                            },
                          ),
                          new FlatButton(
                            child: new Text("Supprimer",
                                style: TextStyle(color: Colors.red[700])),
                            onPressed: () {
                              setState(() {
                                Bdd.deleteCountdown(countdowns[index].id);
                                countdowns.removeAt(index);

                                Scaffold.of(ctxt).showSnackBar(SnackBar(
                                  content: Text("Compte à rebours supprimé"),
                                ));
                              });
                              Navigator.of(context).pop();
                            },
                          ),
                        ],
                      );
                    });
              } else {
                setState(() {
                  editTitle = countdowns[index].title;
                  editText = countdowns[index].text;
                  editImage = countdowns[index].image;
                  editBeginningDate = DateTime.fromMillisecondsSinceEpoch(
                      countdowns[index].beginningDate * 1000);
                  editEndingDate = DateTime.fromMillisecondsSinceEpoch(
                      countdowns[index].endingDate * 1000);
                  editTimer = DateTime.fromMillisecondsSinceEpoch(
                      countdowns[index].deadline * 1000);
                  skipTexte = 0;
                  skipImage = 0;
                  skipTimer = 0;

                  Bdd.deleteCountdown(countdowns[index].id);
                  countdowns.removeAt(index);
                });
              }
            },
          );
        });
  }
}
