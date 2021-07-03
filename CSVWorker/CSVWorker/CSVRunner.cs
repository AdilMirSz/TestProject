using System;
using System.IO;

namespace CSVWorker
{
    public class CSVRunner
    {
        public static void Run()
        {
            Console.WriteLine("Please enter file name (with type)");
            var currentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            var fileName = Console.ReadLine();
            var fullPath = Path.GetFullPath(Path.Combine(currentDirectory, $@"..\..\..\..\{fileName}"));
            var command = "";

            BaseCSVReportMaker reportMaker = null;

            bool finished = false;

            while (!finished)
            {
                Console.WriteLine("enter 1 to generate max concurrent active sessions report;" + "\n" + 
                                  "enter 2 to generate total operators' session durations;" + "\n" +
                                  "type anything else to close program");

                command = Console.ReadLine();
                switch (command)
                {
                    case "1":
                        reportMaker = new ActiveSessionsCSVReportMaker(fullPath);
                        break;
                    case "2":
                        reportMaker = new OperatorSessionsDurationsCSVReportMaker(fullPath);
                        break;
                    default:
                        finished = true;
                        break;
                }

                if (finished) break;

                reportMaker.MakeReport();
            }

        }
    }
}