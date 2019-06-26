# Big data

### What it does:

The programs in the big data section are launch by a single program (BigDataHub.cs) and have several purposes. It runs continuously and have the following programs:

- The control the pump on the vegetal wall (PumpAnalyser.cs and AnalysisForPumpStrategy.cs). 
- The detection of dead sensors and/or battery issues (Sensorchecking.cs)
- The detection of aberrant values in samples (Statementchecking.cs)

### How to run it ?

You only need to launch "BigDataHub.cs" (with BigDataProj.csproj), which will use the other programs. It uses CRUD_BigData.cs to interact with the data base.

Every time a problem is encountered, an alert is created in the data base.

### How to setup ?

You can choose different timing and threshold options in the properties file : "Configuration.JSON"

- turnAroundTime (default 1800) : time between two system checks.
- toleranceThreshold (default 3) : tolerance threshold, number of missing statements acceptable before an alert.
- deathThreshold (default 30): number of missing statements acceptable before delete from database.
- warningThreshold (default 3) : number of aberrant values acceptable before to send an alert.
- errorThreshold (default 8): number of aberrant values acceptable before to consider a sensor as a defected sensor.
- repetitionThreshold (default 20): number of value repetition acceptable before to send an alert.
- alertUpdateTime (default 5270400): maximum time interval at the end of which an alert message remains in the database.
- goodAlertTime (default 259200): maximum time interval at the end of which a good alert message (aka sensor back online) remains in the database.

### PumpAnalyser

First-of-all this static class handle, for each pumps,  when it needs to water the plants.

For that you need to call the static function MainHandlingPumps which for each pumps we call another function called HandlingPumps.

To diversified the method of analysing if a pump will be active or not, the strategy pattern is implemented in AnalysisForPumpStrategy.cs.

Now there are 4 methods of analysis which are used as object and their main function is Analysis which return the number of minutes a pump needs to be activated (=< 0 is inactive):

- StandardAnalysisForPumpStrategy : which use a marking system depending of humidity's intervals. Each interval is link to a mark and the mean of the mark is the final value.
- MeanAnalysisForPumpStrategy : take the mean of the humidity's lasts samples and compare them to a value (by default = 20). In the mean is inferior, it will activate a pump during 5 minutes.
- LazyAnalysisForPumpStrategy : it activates the pumps every x time (default = 24h)
- NothingAnalysisForPumpStrategy : it doesn't turn on pumps. Need to be used only to water the plants manually. 

### 