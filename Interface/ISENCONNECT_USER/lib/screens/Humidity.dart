import 'package:flutter/material.dart';
import 'package:charts_flutter/flutter.dart' as charts;

class HumidityPage extends StatelessWidget {
  @override
  Widget build(BuildContext ctxt) {
    return Scaffold(
      body: DefaultTabController(
        length: 5,
        child: Scaffold(
          appBar: AppBar(
            backgroundColor: Colors.red[700],
            title: TabBar(
              indicatorColor: Colors.white,
              tabs: [
                Tab(text: 'H'),
                Tab(text: 'J'),
                Tab(text: 'S'),
                Tab(text: 'M'),
                Tab(text: 'A'),
              ],
            ),
          ),
          body: TabBarView(
            children: [
              chartWidgetHour,
              chartWidgetDay,
              chartWidgetWeek,
              chartWidgetMonth,
              chartWidgetYear,
            ],
          ),
        ),
      ),
    );
  }
}

class Humidity {
  final String data;
  final int humidity;

  Humidity(this.data, this.humidity);
}

final dataHour = [
  new Humidity('01 min', 60),
  new Humidity('', 65),
  new Humidity(' ', 60),
  new Humidity('  ', 55),
  new Humidity('   ', 60),
  new Humidity('    ', 65),
  new Humidity('     ', 70),
  new Humidity('      ', 65),
  new Humidity('       ', 60),
  new Humidity('        ', 55),
  new Humidity('11 min', 60),
  new Humidity('         ', 65),
  new Humidity('          ', 60),
  new Humidity('           ', 65),
  new Humidity('            ', 60),
  new Humidity('             ', 55),
  new Humidity('              ', 60),
  new Humidity('               ', 65),
  new Humidity('                ', 70),
  new Humidity('                 ', 65),
  new Humidity('21 min', 60),
  new Humidity('                  ', 55),
  new Humidity('                   ', 60),
  new Humidity('                    ', 65),
  new Humidity('                     ', 65),
  new Humidity('                      ', 60),
  new Humidity('                       ', 55),
  new Humidity('                        ', 60),
  new Humidity('                         ', 65),
  new Humidity('                          ', 70),
  new Humidity('31 min', 65),
  new Humidity('                           ', 60),
  new Humidity('                            ', 65),
  new Humidity('                             ', 60),
  new Humidity('                              ', 55),
  new Humidity('                               ', 60),
  new Humidity('                                ', 65),
  new Humidity('                                 ', 70),
  new Humidity('                                  ', 65),
  new Humidity('                                   ', 60),
  new Humidity('41 min', 55),
  new Humidity('                                    ', 60),
  new Humidity('                                     ', 65),
  new Humidity('                                      ', 60),
  new Humidity('                                       ', 65),
  new Humidity('                                        ', 60),
  new Humidity('                                         ', 55),
  new Humidity('                                          ', 60),
  new Humidity('                                           ', 65),
  new Humidity('                                            ', 70),
  new Humidity('51 min', 65),
  new Humidity('                                             ', 60),
  new Humidity('                                              ', 55),
  new Humidity('                                               ', 60),
  new Humidity('                                                ', 65),
  new Humidity('                                                 ', 65),
  new Humidity('                                                  ', 60),
  new Humidity('                                                   ', 65),
  new Humidity('                                                    ', 60),
  new Humidity('60 min', 65),
];

final dataDay = [
  new Humidity('00 h', 30),
  new Humidity('', 35),
  new Humidity('  ', 40),
  new Humidity('03 h', 45),
  new Humidity('   ', 50),
  new Humidity('    ', 55),
  new Humidity('06 h', 60),
  new Humidity('     ', 65),
  new Humidity('      ', 70),
  new Humidity('09 h', 70),
  new Humidity('       ', 75),
  new Humidity('        ', 70),
  new Humidity('12 h', 65),
  new Humidity('         ', 60),
  new Humidity('          ', 60),
  new Humidity('15 h', 55),
  new Humidity('           ', 50),
  new Humidity('            ', 45),
  new Humidity('18 h', 45),
  new Humidity('             ', 50),
  new Humidity('              ', 45),
  new Humidity('21 h', 40),
  new Humidity('               ', 35),
  new Humidity('                ', 40),
];

final dataWeek = [
  new Humidity('L', 85),
  new Humidity('Ma', 65),
  new Humidity('Me', 50),
  new Humidity('J', 65),
  new Humidity('V', 60),
  new Humidity('S', 70),
  new Humidity('D', 80),
];

final dataMonth = [
  new Humidity('1', 60),
  new Humidity('', 65),
  new Humidity(' ', 60),
  new Humidity('  ', 55),
  new Humidity('   ', 60),
  new Humidity('6', 65),
  new Humidity('    ', 70),
  new Humidity('     ', 65),
  new Humidity('      ', 60),
  new Humidity('       ', 55),
  new Humidity('11', 60),
  new Humidity('        ', 65),
  new Humidity('         ', 60),
  new Humidity('          ', 65),
  new Humidity('           ', 60),
  new Humidity('16', 55),
  new Humidity('            ', 60),
  new Humidity('             ', 65),
  new Humidity('              ', 70),
  new Humidity('               ', 65),
  new Humidity('21', 60),
  new Humidity('                ', 55),
  new Humidity('                 ', 60),
  new Humidity('                  ', 65),
  new Humidity('                   ', 65),
  new Humidity('26', 60),
  new Humidity('                    ', 55),
  new Humidity('                     ', 60),
  new Humidity('                      ', 65),
  new Humidity('                       ', 70),
  new Humidity('31', 65),
];

final dataYear = [
  new Humidity('J', 60),
  new Humidity('F', 65),
  new Humidity('M', 60),
  new Humidity('A', 55),
  new Humidity('M ', 60),
  new Humidity('J ', 55),
  new Humidity('J  ', 50),
  new Humidity('A ', 55),
  new Humidity('S', 60),
  new Humidity('O', 65),
  new Humidity('N', 70),
  new Humidity('D', 65),
];

final hour = [
  new charts.Series(
    id: 'humidityHour',
    domainFn: (Humidity humidityHour, _) => humidityHour.data,
    measureFn: (Humidity humidityHour, _) => humidityHour.humidity,
    colorFn: (_, __) => charts.MaterialPalette.blue.shadeDefault,
    data: dataHour,
  ),
];

final day = [
  new charts.Series(
    id: 'humidityDay',
    domainFn: (Humidity humidityDay, _) => humidityDay.data,
    measureFn: (Humidity humidityDay, _) => humidityDay.humidity,
    colorFn: (_, __) => charts.MaterialPalette.blue.shadeDefault,
    data: dataDay,
  ),
];

final week = [
  new charts.Series(
    id: 'humidityWeek',
    domainFn: (Humidity humidityWeek, _) => humidityWeek.data,
    measureFn: (Humidity humidityWeek, _) => humidityWeek.humidity,
    colorFn: (_, __) => charts.MaterialPalette.blue.shadeDefault,
    data: dataWeek,
  ),
];

final month = [
  new charts.Series(
    id: 'humidityMonth',
    domainFn: (Humidity humidityMonth, _) => humidityMonth.data,
    measureFn: (Humidity humidityMonth, _) => humidityMonth.humidity,
    colorFn: (_, __) => charts.MaterialPalette.blue.shadeDefault,
    data: dataMonth,
  ),
];

final year = [
  new charts.Series(
    id: 'humidityYear',
    domainFn: (Humidity humidityYear, _) => humidityYear.data,
    measureFn: (Humidity humidityYear, _) => humidityYear.humidity,
    colorFn: (_, __) => charts.MaterialPalette.blue.shadeDefault,
    data: dataYear,
  ),
];

final chartWidgetHour = new Padding(
  padding: new EdgeInsets.all(8.0),
  child: new SizedBox(
    height: 200.0,
    child: new charts.BarChart(
      hour,
      animate: true,
    )
  ),
);

final chartWidgetDay = new Padding(
  padding: new EdgeInsets.all(8.0),
  child: new SizedBox(
    height: 200.0,
    child: new charts.BarChart(
      day,
      animate: true,
    )
  ),
);

final chartWidgetWeek = new Padding(
  padding: new EdgeInsets.all(8.0),
  child: new SizedBox(
    height: 200.0,
    child: new charts.BarChart(
      week,
      animate: true,
    )
  ),
);

final chartWidgetMonth = new Padding(
  padding: new EdgeInsets.all(8.0),
  child: new SizedBox(
    height: 200.0,
    child: new charts.BarChart(
      month,
      animate: true,
    )
  ),
);

final chartWidgetYear = new Padding(
  padding: new EdgeInsets.all(8.0),
  child: new SizedBox(
    height: 200.0,
    child: new charts.BarChart(
      year,
      animate: true,
    )
  ),
);