import 'Event.dart';
import 'package:flutter/material.dart';

class Countdown extends Event {

  int deadline = 0;

  Countdown(String title, String text, String image, int beginningDate, int endingDate, int position, String id, int deadline) : 
  super(title, text, image, beginningDate, endingDate, position, id){
    this.deadline = deadline;
  }
  
  static Countdown fromJson(Map<String, dynamic> json){
    return new Countdown(
      json['name'], 
      json['text'], 
      json['image'], 
      json['beginningDateEvent'], 
      json['endingDateEvent'], 
      json['position'],
      json['id'],
      json['endingDateCountdown']
    );
  }

  Map<String, dynamic> toJson(){
    return {
      'Name' : title,
      'EndingDateCountdown' : deadline,
      'BeginningDateEvent' : beginningDate,
      'Image' : image,
      'EndingDateEvent' : endingDate,
      'Text' : text,
      'Position' : position
    };
  }
  
  Column showTimer(BuildContext context, AnimationController controller){
    
    controller.reverse(
        from: controller.value == 0.0 ? 1.0 : controller.value);
    return Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: <Widget>[

              Container(
                child: Center(
                  child:AnimatedBuilder(
                    animation: controller,
                    builder: (BuildContext ctxt, Widget child) {
                      return Text(
                        _timerString(controller),
                        textAlign: TextAlign.center,
                        style: Theme.of(context).textTheme.display1,
                      );
                  }),
                )
              ),

              Container(
                height: MediaQuery.of(context).size.height/2.5,
                child: showImage()),

              ListTile(
                title:Text(
                  title,
                  style: TextStyle(
                    fontFamily: "AbrilFatface-Regular",
                    fontSize: MediaQuery.of(context).size.height/25
                  ),
                ),
                subtitle: Text(text),
              ),

            ]
            );
                    
  }

  String _timerString(AnimationController controller){
    Duration duration = controller.duration * controller.value;
    return '${duration.inDays}j:${duration.inHours % 24}h:${duration.inMinutes % 60}m:${(duration.inSeconds % 60).toString().padLeft(2, '0')}s';
  }

}