using System.Collections.Generic;
using System.IO;

namespace CSVWorker
{
    public abstract class BaseCSVReportMaker
    {
        public static string[] CSVTypes = new[]
        {
            "SessionStartTime",
            "SessionEndTime",
            "ProjectName",
            "OperatorName",
            "SessionState",
            "SessionStateDuration"
        };
        
        protected static long _linesCount = 0;

        private static Dictionary<string, List<string>> _dataCache = null;

        private readonly string _path;

        protected BaseCSVReportMaker(string path)
        {  
            _path = path;
        }
        protected Dictionary<string, List<string>> GetData()
        {
            if (_dataCache != null)
                return _dataCache;

            var dictionary = new Dictionary<string, List<string>>();

            using (var reader = new StreamReader(_path))
            {
                reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    for (int i = 0; i < CSVTypes.Length; i++)
                    {
                        if (!dictionary.ContainsKey(CSVTypes[i]))
                            dictionary[CSVTypes[i]] = new List<string>();
                        dictionary[CSVTypes[i]].Add(values[i]);
                    }

                    _linesCount++;
                }
            }

            _dataCache = dictionary;
            return dictionary;
        }

        public abstract void MakeReport();
    }
}