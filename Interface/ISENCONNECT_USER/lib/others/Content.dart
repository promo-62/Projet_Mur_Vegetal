import 'package:flutter/material.dart';
import 'dart:convert';

abstract class Content{

  String title = "";
  String image = "";
  int beginningDate = 0;
  int endingDate = 0;
  String id;

  Content(String title, String image, int beginningDate, int endingDate, String id){
    this.title = title;
    this.image = image;
    this.beginningDate = beginningDate;
    this.endingDate = endingDate;
    this.id = id;
  }

  Image showImage() {
    return Image.memory(base64.decode(image));
  }
}