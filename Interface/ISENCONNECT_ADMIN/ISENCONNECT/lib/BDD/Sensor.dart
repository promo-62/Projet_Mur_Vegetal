

class Sensor {

  String name;
  int idSensor;
  int idSensorType;
  bool isWorking;

  Sensor(
    this.idSensor,
    this.idSensorType,
    this.name,
    this.isWorking
  );

  static Sensor fromJson(Map<String, dynamic> json){
    return new Sensor(
      json['IdSensor'],
      json['IdSensorType'],
      json['Name'],
      json['IsWorking']
    );
  }

}