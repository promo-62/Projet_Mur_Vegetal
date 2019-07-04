import 'Content.dart';
import 'package:flutter/material.dart';


class Event extends Content {

  String text = "";
  int position = 0;

  Event(String title, String text, String image, int beginningDate, int endingDate, int position, String id): 
  super(title, image, beginningDate, endingDate, id){
    this.text = text;
    this.position = position;
  }
  
  static Event fromJson(Map<String, dynamic> json){
    return new Event(
      json['name'], 
      json['text'], 
      json['eventImage'], 
      json['beginningDate'], 
      json['endingDate'], 
      json['position'],
      json['id']);
  }

  Map<String, dynamic> toJson(){
    return {
      'Name' : title,
      'EventDate' : endingDate,
      'BeginningDate' : beginningDate,
      'EventImage' : image,
      'EndingDate' : endingDate,
      'Text' : text,
      'Position' : position
    };
  }
  

  Column showEvent(BuildContext context){
    return Column(
      mainAxisAlignment: MainAxisAlignment.center,
      children: [
        Container(
          height: (MediaQuery.of(context).size.height / 2),
          child: showImage(),
        ),
        ListTile(
          title:Text(
            title,
            style:TextStyle(
              fontFamily: "AbrilFatface-Regular",
              fontSize: MediaQuery.of(context).size.height/25,
            )
          ),
          subtitle: Text(text),
        )

      ]
    );
  }

  

}