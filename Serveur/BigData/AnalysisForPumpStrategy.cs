using System;
using System.Collections.Generic;
using WebAPI.Models;

namespace nsPump
{   
    /*DO NOT CALL THIS ONE, Used for Strategy Pattern, if you don't know how to handle it,
      don't create new classes in that file. */
    public abstract class IAnalysisForPumpStrategy
    {
        public abstract int Analysis(List<Samples> ListSamplesHumidity, Samples LastSamplePump);
    }
    /*Analyser using notation and weighted mean.*/
    public class StandardAnalysisForPumpStrategy : IAnalysisForPumpStrategy
    {  
        public class markingInterval
        {
            public markingInterval(int f_low, int f_high, float f_notation)
            {
                lowInterval = f_low; highInterval = f_high; notation = f_notation;
            }
            public int lowInterval { get; set; }
            public int highInterval { get; set; }
            public float notation { get; set; }
            public bool isInside(int value)
            { if (value >= lowInterval && value < highInterval) return true; return false;}
        }

        public StandardAnalysisForPumpStrategy()
        {

        }
        public override int Analysis(List<Samples> listSummaries, Samples LastSamplePum)
        {
            float finalTime = 0;



            List<markingInterval> listIntervall = new List<markingInterval>
            { new markingInterval(0,9,3), new markingInterval(10,19,1.5f), new markingInterval(20,59,0),
              new markingInterval(60,79,-1.5f), new markingInterval(80,89,-2), new markingInterval(90,101,-3)};
            
            foreach(Samples Sample in listSummaries)
            {
                foreach(markingInterval Interval in listIntervall)
                {
                    if(Interval.isInside(Sample.Value))
                        finalTime += Interval.notation;
                }
            }
            finalTime /= Math.Max(1, listSummaries.Count);

            return (int)Math.Round(finalTime);
        }
    }
    public class MeanAnalysisForPumpStrategy : IAnalysisForPumpStrategy
    {  
        int mean = 20;
        public MeanAnalysisForPumpStrategy()
        {

        }
        public override int Analysis(List<Samples> listSummaries, Samples LastSamplePum)
        {
            float finalTime = 0;

            foreach(Samples Sample in listSummaries)
                finalTime += Sample.Value;

            finalTime /= Math.Max(1, listSummaries.Count);
            if(finalTime < mean)
                return 5;
            else
                return 0;

            ///return (int)Math.Round(finalTime);
        }
    }
    /*Analyser allowing pump to water each x time, 24hours by default.*/
    public class LazyAnalysisForPumpStrategy : IAnalysisForPumpStrategy
    {
        public int hoursBetweenWattering { get; set; }
        public LazyAnalysisForPumpStrategy(int f_hoursBetweenWattering = 24*3600)
        {
            hoursBetweenWattering = f_hoursBetweenWattering;
        }
        public override int Analysis(List<Samples> ListSamplesHumidity, Samples LastSamplePump)
        {
            int finalTime = 0;
            if(DateTimeOffset.Now.ToUnixTimeSeconds() - LastSamplePump.SampleDate > hoursBetweenWattering)
            {
                finalTime = 5;
            }

            return finalTime;
        }
    }
    /*Analyser disallowing the pumps to being active.*/
    public class NothingAnalysisForPumpStrategy : IAnalysisForPumpStrategy
    {
        public NothingAnalysisForPumpStrategy()
        {
            
        }
        public override int Analysis(List<Samples> ListSamplesHumidity, Samples LastSamplePump)
        {
            int finalTime = 0;
            if(DateTimeOffset.Now.ToUnixTimeSeconds() - LastSamplePump.SampleDate < 1*3600)
            {
                finalTime = LastSamplePump.Value;
            }
            return 0;
        }
    }
}