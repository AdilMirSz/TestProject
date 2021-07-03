using System;
using System.Collections.Generic;

namespace CSVWorker
{
    public class OperatorSessionsDurationsCSVReportMaker : BaseCSVReportMaker
    { 
        public OperatorSessionsDurationsCSVReportMaker(string path) : base(path) { }
        public override void MakeReport()
        {
            var operatorsStatesDict = new Dictionary<string, Dictionary<string, long>>();
            var data = GetData();

            for (int i = 0; i < _linesCount; i++)
            {
                var operatorName = data[CSVTypes.OperatorName][i];
                var sessionState = data[CSVTypes.SessionState][i];
                var stateDuration = data[CSVTypes.SessionStateDuration][i];

                if (!operatorsStatesDict.ContainsKey(operatorName))
                {
                    operatorsStatesDict[operatorName] = new Dictionary<string, long>();
                }

                if (!operatorsStatesDict[operatorName].ContainsKey(sessionState))
                {
                    operatorsStatesDict[operatorName][sessionState] = long.Parse(stateDuration);
                }

                operatorsStatesDict[operatorName][sessionState] += long.Parse(stateDuration);
            }
            
            foreach (var dict in operatorsStatesDict)
            {
                Console.Write("For operator " + "<"+dict.Key+">");
                foreach (var kvp in dict.Value)
                {
                    Console.Write(" State " + "<"+kvp.Key+">" + " lasted for " + (kvp.Value / 60 + kvp.Value % 60) + " minutes; ");
                    Console.WriteLine();
                }
            }
        }
    }
}