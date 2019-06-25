## PumpAnalyser

First-of-all this static class handle, for each pumps,  when it needs to water the plants.

For that you need to call the static function MainHandlingPumps which for each pumps we call another function called HandlingPumps.

To diversified the method of analysing if a pump will be active or not, the strategy pattern is implemented in AnalysisForPumpStrategy.cs.

Now there are 4 methods of analysis which are used as object and their main function is Analysis which return the number of minutes a pump needs to be activated (=< 0 is inactive):

- StandardAnalysisForPumpStrategy : which use a marking system depending of humidity's intervals. Each interval is link to a mark and the mean of the mark is the final value.

- MeanAnalysisForPumpStrategy : take the mean of the humidity's lasts samples and compare them to a value (by default = 20). In the mean is inferior, it will activate a pump during 5 minutes.
- LazyAnalysisForPumpStrategy : it activates the pumps every x time (default = 24h)

- NothingAnalysisForPumpStrategy : it doesn't turn on pumps. Need to be used only to water the plants manually. 

