import 'package:ISENCONNECT/BDD/SensorType.dart';

class Sample {

  SensorType sampleType;
  int value;
  int timestamp;

  Sample(SensorType sampleType, int value, int timestamp){
    this.sampleType = sampleType;
    this.value = value;
    this.timestamp = timestamp;
  }
  
  static Sample fromJson(Map<String, dynamic> json)
  {
    SensorType dataType;
    int tmp = json['idSampleType'];
    switch(tmp){
      case 0:
        dataType = SensorType.HUMIDITY;
        break;
      case 1:
        dataType = SensorType.AIR_QUALITY;
        break;
      case 2:
        dataType = SensorType.LIGHT;
        break;
      case 3:
        dataType = SensorType.HIVE_PRESSURE;
        break;
      case 4:
        dataType = SensorType.HIVE_TEMPERATURE;
        break;
      case 5:
        dataType = SensorType.HIVE_TRAFFIC;
        break;

// --- Add a sensorType here ---

      default:
        dataType = SensorType.NULL;
    }
    return new Sample(dataType, json["value"], json["sampleDate"]);
  }

}
