class Alert {

  String alertReason = "";
  String name = "";
  int idSensor = 0;
  int dateAlert = 0;
  bool isWorking = true;

  Alert(
    this.idSensor,
    this.name,
    this.dateAlert,
    this.isWorking,
    this.alertReason
  );

  static Alert fromJson(Map<String, dynamic> json){
    return new Alert(
        json['IdSensor'],
        json['Name'],
        json['DateAlert'],
        json['IsWorking'],
        json['AlertReason']
    );
  }

}