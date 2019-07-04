import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:flutter_multi_carousel/carousel.dart';
import 'package:ISENCONNECT/main.dart';
import 'package:ISENCONNECT/others/EditTimer.dart';
import 'package:ISENCONNECT/Global.dart';
import 'package:ISENCONNECT/BDD/Event.dart';
import 'package:ISENCONNECT/BDD/Countdown.dart';
import 'dart:convert';

class EventsPage extends StatefulWidget {
  @override
  EventsPageState createState() => EventsPageState();
}

class EventsPageState extends State<EventsPage>
    with TickerProviderStateMixin {
  String dropDownStringItem;
  bool isOpened = false;
  AnimationController _animationController;
  AnimationController controller;
  Animation<Color> _buttonColor;
  Animation<double> _animateIcon;
  Animation<double> _translateButton;
  Curve _curve = Curves.easeOut;
  double _fabHeight = 56.0;

  @override
  initState() {
    controller = AnimationController(
      vsync: this,
      duration: Duration(days: 0, hours: 0, minutes: 0, seconds: 0),
    );
    _animationController =
        AnimationController(vsync: this, duration: Duration(milliseconds: 500))
          ..addListener(() {
            setState(() {});
          });

    _animateIcon =
        Tween<double>(begin: 0.0, end: 1.0).animate(_animationController);
    _buttonColor = ColorTween(
      begin: Colors.red[700],
      end: Colors.red[500],
    ).animate(CurvedAnimation(
      parent: _animationController,
      curve: Interval(
        0.00,
        1.00,
        curve: Curves.linear,
      ),
    ));
    _translateButton = Tween<double>(
      begin: _fabHeight,
      end: -14.0,
    ).animate(CurvedAnimation(
      parent: _animationController,
      curve: Interval(
        0.0,
        0.75,
        curve: _curve,
      ),
    ));
    super.initState();
  }

  @override
  dispose() {
    controller.dispose();
    _animationController.dispose();
    super.dispose();
  }

  animate() {
    if (!isOpened) {
      _animationController.forward();
    } else {
      _animationController.reverse();
    }
    isOpened = !isOpened;
  }

  Widget reload(BuildContext ctxt) {
    return Container(
      child: FloatingActionButton(
        heroTag: "btn1",
        onPressed: () {},
        tooltip: 'Image',
        child: Icon(Icons.refresh),
      ),
    );
  }

  Widget editMenu(BuildContext ctxt) {
    return Container(
      child: FloatingActionButton(
        backgroundColor: Colors.orange[500],
        heroTag: "btn3",
        onPressed: () {
          imageInString = "";
          Navigator.push(
            ctxt, MaterialPageRoute(builder: (ctxt) => EditTimer()));
        },
        tooltip: 'Inbox',
        child: Icon(Icons.add_circle),
      ),
    );
  }

  Widget toggle() {
    return FloatingActionButton(
      heroTag: "btn2",
      backgroundColor: _buttonColor.value,
      onPressed: animate,
      tooltip: 'Toggle',
      child: AnimatedIcon(
        icon: AnimatedIcons.menu_close,
        progress: _animateIcon,
      ),
    );
  }

  Widget build(BuildContext ctxt) {
    return new Scaffold(
      floatingActionButton: editMenu(ctxt),

      body: FutureBuilder(
        future: Global.events,
        builder: (ctxt, snapshot){
          if (snapshot.hasData)
          {
            return FutureBuilder(
              future: Global.countdowns,
              builder: (ctxt, snapshot2){
                if (snapshot2.hasData){
                  return Carousel(
                    height: MediaQuery.of(ctxt).size.height / 1.3,
                    width: MediaQuery.of(ctxt).size.width,
                    type: "slideswiper",
                    showIndicator: false,
                    arrowColor: Colors.red[900],
                    axis: Axis.horizontal,
                    showArrow: true,
                    children: loadedEvents(snapshot.data, snapshot2.data, ctxt, controller)
                  );
                }
                else
                  return Carousel(
                    height: MediaQuery.of(ctxt).size.height / 1.3,
                    width: MediaQuery.of(ctxt).size.width,
                    type: "slideswiper",
                    showIndicator: false,
                    arrowColor: Colors.red[900],
                    axis: Axis.horizontal,
                    showArrow: true,
                    children: loadingEvents(ctxt),
                  );
              }
            );
          }
          else 
          {
            return Carousel(
              height: MediaQuery.of(ctxt).size.height / 1.3,
              width: MediaQuery.of(ctxt).size.width,
              type: "slideswiper",
              showIndicator: false,
              arrowColor: Colors.red[900],
              axis: Axis.horizontal,
              showArrow: true,
              children: loadingEvents(ctxt),
            );
          }
        }
      ),
    );
  }

  List<Column> loadedEvents(List<Event> events, List<Countdown> countdowns, BuildContext ctxt, AnimationController controller)
  {
    List<Column> tmp = new List();
    for(Event elem in events){
      tmp.add(elem.showEvent(context));
    }

    for(Countdown elem in countdowns){
      DateTime selectedDate;
      if (elem.deadline > Global.dateTimeToTimestamp(DateTime.now()))
        selectedDate = new DateTime.fromMillisecondsSinceEpoch(elem.deadline*1000);
      else
        selectedDate = DateTime.now().add(Duration(seconds: 1));
      Duration eventDate = selectedDate.difference(DateTime.now());
      controller = AnimationController(
        vsync: this,
        duration: eventDate,
      );
      tmp.add(elem.showTimer(context, controller));
    }

    return tmp;
  }

  List<Column> loadingEvents(BuildContext ctxt)
  {
    List<Column> tmp = new List();

    tmp.add(Column(
      mainAxisAlignment: MainAxisAlignment.center,
      children: [

        Container(
          child: CircularProgressIndicator(),
        ),

        ListTile(
          title:Text(
            "",
            style:TextStyle(
              fontFamily: "AbrilFatface-Regular",
              fontSize: MediaQuery.of(ctxt).size.height/25,
            )
          ),
          subtitle: Text(""),
        )
    ]));
    return tmp;
  }
}
