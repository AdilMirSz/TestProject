using System;
using System.Collections.Generic;
using System.Linq;

namespace CSVWorker
{
    public class ActiveSessionsCSVReportMaker : BaseCSVReportMaker
    {
        public class SessionsRanges
        {
            public List<int> StartTimes { get; set; }
            public List<int> EndTimes { get; set; }
        }

        public ActiveSessionsCSVReportMaker(string path) : base(path) { }
        public override void MakeReport()
        {
            var data = GetData();
            
            Dictionary<DateTime, SessionsRanges> timespansByDays = new Dictionary<DateTime, SessionsRanges>();

            for (int i = 0; i < _linesCount; i++)
            {
                var dayAndStartTime = data[CSVTypes.SessionStartTime][i].Split(' ');
                var dayAndEndTime = data[CSVTypes.SessionEndTime][i].Split(' ');

                var dateTime = Convert.ToDateTime(dayAndStartTime[0]);
                if (!timespansByDays.ContainsKey(dateTime))
                {
                    timespansByDays[dateTime] = new SessionsRanges { StartTimes = new List<int>(), EndTimes = new List<int>() };
                }

                timespansByDays[dateTime].StartTimes.Add((int)TimeSpan.Parse(dayAndStartTime[1]).TotalSeconds);
                timespansByDays[dateTime].EndTimes.Add((int)TimeSpan.Parse(dayAndEndTime[1]).TotalSeconds);
            }


            timespansByDays.OrderBy(x => x.Key);

            foreach (var e in timespansByDays)
            {
                MaxConcurrentSessionsPerDay(e);
            }
        }

        private static void MaxConcurrentSessionsPerDay(KeyValuePair<DateTime, SessionsRanges> day)
        {
            var startTimes = day.Value.StartTimes;
            var endTimes = day.Value.EndTimes;
            var length = startTimes.Count;

            startTimes.Sort();
            endTimes.Sort();

            var concurrentSessions = 1;
            var currentMax = 1;
            var initialTime = startTimes[0];
            var i = 1;
            var j = 0;


            while (i < length && j < length)
            {
                if (startTimes[i] <= endTimes[j])
                {
                    concurrentSessions++;

                    if (concurrentSessions > currentMax)
                    {
                        currentMax = concurrentSessions;
                        initialTime = startTimes[i];
                    }

                    i++;
                }

                else
                {
                    concurrentSessions--;
                    j++;
                }
            }

            Console.WriteLine("Maximum Number of Concurrent Sessions = " + currentMax +
                              " at time " + day.Key.Add(TimeSpan.FromSeconds(initialTime)));
        }
    }
}