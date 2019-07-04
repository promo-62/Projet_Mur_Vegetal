import 'package:flutter/material.dart';
import 'package:validators/validators.dart';
import 'package:youtube_player/youtube_player.dart';

import 'Content.dart';

class Media extends Content{

  String videoURL = "xx";

  Media(String title, String image, int beginningDate, int endingDate, String videoURL, String id):
  super(title, image, beginningDate, endingDate, id){
    this.videoURL = videoURL;
  }

  static Media fromJson(Map<String, dynamic> json){
    return new Media(
      json['name'], 
      json['image'],
      json['beginningDate'], 
      json['endingDate'], 
      json['video'],
      json['id']);
  }

  Map<String, dynamic> toJson(){
    return {
      'Name' : title,
      'BeginningDate' : beginningDate,
      'EndingDate' : endingDate,
      'Video' : videoURL,
      'Image' : image
    };
  }

  Card showMedia(BuildContext ctxt){
    if (videoURL != null && isURL(videoURL)){
      if (contains(videoURL, "https://www.youtube.com" ) || contains(videoURL,"https://youtu.be"))
        return
        Card(
          child: YoutubePlayer(
          context: ctxt,
          source: videoURL,
          quality: YoutubeQuality.HD,
          width: MediaQuery.of(ctxt).size.width/2,
          autoPlay: false,
          reactToOrientationChange: false,
          startFullScreen: false,
          showThumbnail: true,
          playerMode: YoutubePlayerMode.DEFAULT
        ));
        
      return 
        Card(
          child: Image.network(videoURL)
        );
        
    }
    if (image != null && isBase64(image))
      return 
        Card(
          child: showImage()
        );
        
    return
    Card(); 
  }
}