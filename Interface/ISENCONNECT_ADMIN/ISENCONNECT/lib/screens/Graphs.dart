import 'package:ISENCONNECT/BDD/SensorType.dart';
import "package:flutter/material.dart";
import "package:bezier_chart/bezier_chart.dart";
import 'package:ISENCONNECT/BDD/Sample.dart';
import 'package:ISENCONNECT/Global.dart';

class Graph extends StatelessWidget{

  SensorType sensorType;

  Graph(SensorType sensorType){
    this.sensorType = sensorType;
  }

  @override
  Widget build(BuildContext ctxt) {

    Future<List<Sample>> data = Global.lightData;
    Color color = Colors.amber[200];

    if (sensorType == SensorType.AIR_QUALITY){
      data = Global.airQualityData;
      color = Colors.lightGreen[300];
    }
    else if (sensorType == SensorType.HUMIDITY){
      data = Global.humidityData;
      color = Colors.blue[200];
    }
    else if (sensorType == SensorType.HIVE_PRESSURE){
      data = Global.hivePressureData;
      color = Colors.brown[200];
    }
    else if (sensorType == SensorType.HIVE_TEMPERATURE){
      data = Global.hiveTemperatureData;
      color = Colors.red[100];
    }
    else if (sensorType == SensorType.HIVE_TRAFFIC){
      data = Global.hiveTrafficData;
      color = Colors.yellow;
    }

    return Scaffold(
      body: DefaultTabController(
        length: 3,
        child: Scaffold(
          appBar: AppBar(
            backgroundColor: Colors.red[700],
            title: TabBar(
              indicatorColor: Colors.white,
              tabs: [
                Tab(text: 'J'),
                Tab(text: 'S'),
                Tab(text: 'A'),
              ],
            ),
          ),
          body: FutureBuilder(
              future: data,
              builder: (ctxt, snapshot){
                if (snapshot.hasData)
                  return
                  TabBarView(
                    children: [
                      dayGraph(ctxt, snapshot.data, color),
                      weekGraph(ctxt, snapshot.data, color),
                      yearGraph(ctxt, snapshot.data, color)
                    ]
                  );
                else 
                  return CircularProgressIndicator();
              }
          )
        )
      )
    );
  }

  
  Widget dayGraph(BuildContext context, List<Sample> data, Color color) {
    return Center(
      child: Container(
        color: Colors.red,
        height: MediaQuery.of(context).size.height / 2,
        width: MediaQuery.of(context).size.width * 0.9,
        child: BezierChart(
          bezierChartScale: BezierChartScale.CUSTOM,
          xAxisCustomValues: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24],
          footerValueBuilder: (double value){
            if (value%3 == 0){
              DateTime time = DateTime.now().subtract(Duration(hours: 24-value.toInt()));
              String minute = "";
              String hour = "";
              if (time.minute < 10)
                minute = "0" + time.minute.toString();
              else
                minute = time.minute.toString();
              if (time.hour < 10)
                hour = "0" + time.hour.toString();
              else
                hour = time.hour.toString();
              return hour + ":" + minute;
            }
            return "";
          },
          series: [
            BezierLine(
              onMissingValue: (dateTime){
                return 1;
              },
              data: dayData(data),
            ),
          ],
          config: BezierChartConfig(
            verticalIndicatorStrokeWidth: 3.0,
            verticalIndicatorColor: Colors.black26,
            showVerticalIndicator: true,
            backgroundColor: color,
            snap: false,
          ),
        ),
      ),
    );
  }

  Widget weekGraph(BuildContext context, List<Sample> data, Color color) {
    return Center(
      child: Container(
        color: Colors.red,
        height: MediaQuery.of(context).size.height / 2,
        width: MediaQuery.of(context).size.width * 0.9,
        child: BezierChart(
          bezierChartScale: BezierChartScale.CUSTOM,
          xAxisCustomValues: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28],
          footerValueBuilder: (double value){
            if (value%4 == 0){
              DateTime time = DateTime.now().subtract(Duration(days: (28-value.toInt()) ~/ 4));
              String day = "";
              String month = "";
              if (time.day < 10)
                day = "0" + time.day.toString();
              else
                day = time.day.toString();
              if (time.month < 10)
                month = "0" + time.month.toString();
              else
                month = time.month.toString();
              return day + "/" + month;
            }
            return "";
          },
          series: [
            BezierLine(
              onMissingValue: (dateTime){
                return 1;
              },
              data: weekData(data),
            ),
          ],
          config: BezierChartConfig(
            verticalIndicatorStrokeWidth: 3.0,
            verticalIndicatorColor: Colors.black26,
            showVerticalIndicator: true,
            backgroundColor: color,
            snap: false,
          ),
        ),
      ),
    );
  }

  Widget yearGraph(BuildContext context, List<Sample> data, Color color) {
    return Center(
      child: Container(
        color: Colors.red,
        height: MediaQuery.of(context).size.height / 2,
        width: MediaQuery.of(context).size.width * 0.9,
        child: BezierChart(
          bezierChartScale: BezierChartScale.CUSTOM,
          xAxisCustomValues: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24],
          footerValueBuilder: (double value){
            if (value%3 == 0){
              DateTime time = DateTime.now().subtract(Duration(days: (24-value.toInt()) * 15));
              String year = time.year.toString();
              String month = "";
              if (time.month < 10)
                month = "0" + time.month.toString();
              else
                month = time.month.toString();
              return month + "/" + year;
            }
            return "";
          },
          series: [
            BezierLine(
              onMissingValue: (dateTime){
                return 1;
              },
              data: yearData(data),
            ),
          ],
          config: BezierChartConfig(
            verticalIndicatorStrokeWidth: 3.0,
            verticalIndicatorColor: Colors.black26,
            showVerticalIndicator: true,
            backgroundColor: color,
            snap: false,
          ),
        ),
      ),
    );
  }
  

  List<DataPoint> dayData(List<Sample> data){
    List<DataPoint> list = new List();
    
    if (data.length == 0){
      for (int i = 0 ; i < 24 ; i++)
        list.add(DataPoint<double>(value: 1, xAxis: (i.toDouble() + 1)));
      return list;
    }
    data.sort((a, b) => b.timestamp.compareTo(a.timestamp)); // Sort from last to first registered sample
    List<int> values = new List();
    int j = 0;
    for (int i = 0; i < 24; i++){
      int sum = 0;
      int buffer = 0;
      while (j < data.length && data[j].timestamp > Global.dateTimeToTimestamp(DateTime.now().subtract(Duration(hours: i)))){
        sum += data[j].value;
        buffer++;        
        j++;
      }
      if (buffer == 0)
        values.add(0);
      else
        values.add(sum ~/ buffer); // Truncate integers division
    }
    values = values.reversed.toList();
    
    for (int i = 0 ; i < values.length ; i++)
      list.add(DataPoint<double>(value: values[i].toDouble(), xAxis: (i.toDouble() + 1)));
    return list;
  }

  List<DataPoint> weekData(List<Sample> data){
    List<DataPoint> list = new List();
    
    if (data.length == 0){
      for (int i = 0 ; i < 28 ; i++)
        list.add(DataPoint<double>(value: 1, xAxis: i.toDouble() + 1));
      return list;
    }
    data.sort((a, b) => b.timestamp.compareTo(a.timestamp)); // Sort from last to first registered sample
    List<int> values = new List();
    int j = 0;
    for (int i = 0; i < 28; i++){
      int sum = 0;
      int buffer = 0;
      while (j < data.length && data[j].timestamp > Global.dateTimeToTimestamp(DateTime.now().subtract(Duration(hours: i * 6)))){
        sum += data[j].value;
        buffer++;        
        j++;
      }
      if (buffer == 0)
        values.add(0);
      else
        values.add(sum ~/ buffer); // Truncate integers division
    }
    values = values.reversed.toList();
    
    for (int i = 0 ; i < values.length ; i++)
      list.add(DataPoint<double>(value: values[i].toDouble(), xAxis: i.toDouble() + 1));
    return list;
  }

  List<DataPoint> yearData(List<Sample> data){
    List<DataPoint> list = new List();
    
    if (data.length == 0){
      for (int i = 0 ; i < 24 ; i++)
        list.add(DataPoint<double>(value: 1, xAxis: i.toDouble()));
      return list;
    }
    data.sort((a, b) => b.timestamp.compareTo(a.timestamp)); // Sort from last to first registered sample
    List<int> values = new List();
    int j = 0;
    for (int i = 0; i < 24; i++){
      int sum = 0;
      int buffer = 0;
      while (j < data.length && data[j].timestamp > Global.dateTimeToTimestamp(DateTime.now().subtract(Duration(days: i * 15)))){
        sum += data[j].value;
        buffer++;        
        j++;
      }
      if (buffer == 0)
        values.add(0);
      else
        values.add(sum ~/ buffer); // Truncate integers division
    }
    values = values.reversed.toList();
    
    for (int i = 0 ; i < values.length ; i++)
      list.add(DataPoint<double>(value: values[i].toDouble(), xAxis: i.toDouble() + 1));
    return list;
  }
}