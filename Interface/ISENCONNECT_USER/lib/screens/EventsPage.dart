import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:flutter_multi_carousel/carousel.dart';
import 'package:ISENCONNECT/Global.dart';
import 'package:ISENCONNECT/BDD/Event.dart';
import 'package:ISENCONNECT/BDD/Countdown.dart';

class EventsPage extends StatefulWidget {
  @override
  EventsPageState createState() => EventsPageState();
}

class EventsPageState extends State<EventsPage>
    with TickerProviderStateMixin {
  String dropDownStringItem;
  bool isOpened = false;
  AnimationController controller;

  @override
  initState() {
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


  Widget build(BuildContext ctxt) {
    return new Scaffold(

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
