using System;
using System.Collections.Generic;
using System.IO;

namespace CSVWorker
{
    public abstract class BaseCSVReportMaker
    {
        protected enum CSVTypes
        {
            SessionStartTime,
            SessionEndTime,
            ProjectName, 
            OperatorName,
            SessionState,
            SessionStateDuration
        }

        protected static long _linesCount = 0;

        private static Dictionary<CSVTypes, List<string>> _dataCache = null;

        private readonly string _path;

        protected BaseCSVReportMaker(string path)
        {  
            _path = path;
        }
        protected Dictionary<CSVTypes, List<string>> GetData()
        {
            if (_dataCache != null)
                return _dataCache;

            var dictionary = new Dictionary<CSVTypes, List<string>>();

            var enumCount = Enum.GetNames(typeof(CSVTypes)).Length;

            using (var reader = new StreamReader(_path))
            {
                reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    for (int i = 0; i < enumCount; i++)
                    {
                        if (!dictionary.ContainsKey((CSVTypes)i))
                            dictionary[(CSVTypes)i] = new List<string>();

                        dictionary[(CSVTypes)i].Add(values[i]); 
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