using System;
// for file read/write
using System.IO;
// for list use
using System.Collections.Generic;


namespace LiftPrototype
{
    /// <summary>
    /// <c>Program</c> takes input from a csv file representing calls to the lift system, and generates the lifts responses.
    /// The responses are output to a csv file as well as the time taken to respond to each call being output to the console
    /// </summary>
    class Program
    {
        /// <value><c>input_filepath</c> stores the provided filepath for the lift event data.</value>
        private static string input_filepath;
        /// <value><c>output_filepath</c> stores the provided filepath for the lifts logging data.</value>
        private static string output_filepath;

        /// <value><c>events</c> keeps a list of all call/request events to be provided to the lift system.</value>
        private static List<CallRequest> events;

        /// <value><c>log</c> keeps a list of all the required logging data from the lift system.</value>
        private static List<LogData> log;

        /// <value><c>last_floor</c> keeps track of the last floor the lift visited. This is used to control when the log data is extracted.</value>
        private static int last_floor = 0;

        // the lift object that serves to process the call and request data
        /// <value><c>lift</c> holds the prototype lift system object. This object is where all the systems internal processing occurs.</value>
        private static Lift lift;

        /// <summary>
        /// The main method loads the input csv file, parses its input, provides the lift system with the data, and logs the resulting actions.
        /// </summary>
        /// <param name="args">either the input and output filepaths, or empty.</param>
        public static void Main(string[] args)
        {
            // get the input and output filepaths
            GetFilepaths(args);

            // process the csv into a list of structs sorted by time
            ParseCsvData();

            // initialise the lift object
            lift = new Lift();

            // initialise the data logging structure list
            log = new List<LogData>();

            // while there are still calls/requests to be processed
            while (events.Count != 0 || !lift.RouteEmpty())
            {
                // check if the lift has reached a new floor
                if (!lift.IsMoving() && lift.GetCurrentFloor() != last_floor)
                {
                    // update last floor
                    last_floor = lift.GetCurrentFloor();

                    // init new log data struct and insert the required data
                    LogData new_data = new LogData() { 
                        time = lift.GetCurrentTime(),
                        people = lift.GetLiftInhabitants(),
                        floor = lift.GetCurrentFloor(),
                        route = lift.GetLiftQueue()
                    };

                    // add data to log
                    log.Add(new_data);
                }

                // while there are still events, check if any calls are made at current lift time
                while (events.Count != 0 && lift.GetCurrentTime() == events[0].call_time)
                {
                    // add call to lift
                    lift.AddCall(events[0]);

                    // remove call from event list
                    events.RemoveAt(0);
                }

                // update lift to next second
                lift.Update();
            }

            // output log data to output csv
            OutputLog();
        }

        /// <summary>
        /// This method obtains the input and output paths required to function from either the passed args, or a user input from the console.
        /// </summary>
        /// <param name="args">taken from the main function input, either the input and output filepaths, or empty.</param>
        private static void GetFilepaths(string[] args)
        {
            // if one or both filepaths are missing from argument list
            if (args.Length < 2)
            {
                // if input filepath is missing
                if (args.Length == 0)
                {
                    // request input filepath from user via console
                    Console.WriteLine("Please enter the filepath for the input csv:");
                    input_filepath = Console.ReadLine();
                }
                // if input filepath is provided
                else if (args.Length == 1)
                {
                    // fetch the filepath from the argument list
                    input_filepath = args[0];
                }
                // as second argument is missing, request output filepath from user via console
                Console.WriteLine("Please enter the filepath for the output csv:");
                output_filepath = Console.ReadLine();
            }
            // if filepath provided as argument
            else
            {
                // fetch the filepaths from the argument list
                input_filepath = args[0];
                output_filepath = args[1];
            }
        }

        /// <summary>
        /// This method loads the data from <c>input_filepath</c> and processes it into an ordered list of structs.
        /// The resulting list is implicitly ordered by the time of request.
        /// </summary>
        private static void ParseCsvData()
        {
            // initialise the events list
            events = new List<CallRequest>();
            // open the input file
            using (StreamReader file = new StreamReader(input_filepath))
            {
                // init a var to hold each line of the csv file
                string request;
                // skip info line
                request = file.ReadLine();
                // for each lift call
                while ((request = file.ReadLine()) != null)
                {
                    // split the string into each value
                    string[] values = request.Split(',');
                    // create the requests structure
                    CallRequest current_event = new CallRequest() { 
                        caller_ID = values[0],
                        start_floor = int.Parse(values[1]),
                        end_floor = int.Parse(values[2]),
                        call_time = int.Parse(values[3]) 
                    };
                    // add the structure to the list
                    events.Add(current_event);
                }
            }
        }

        /// <summary>
        /// This method outputs the contents of <c>log</c> to a csv file at <c>output_filepath</c>.
        /// </summary>
        private static void OutputLog()
        {
            // open the output file
            using (StreamWriter file = new StreamWriter(output_filepath))
            {
                // write column headers to csv file
                file.WriteLine("Time,People,Current Floor,Lift Queue");

                // for every log
                for (int i = 0; i < log.Count; i++)
                {
                    // write data to csv

                    // time
                    file.Write(log[i].time);
                    file.Write(',');

                    // people delimited by spaces
                    for (int j = 0; j < log[i].people.Length;j++)
                    {
                        file.Write(log[i].people[j]);
                        file.Write(' ');
                    }
                    file.Write(',');

                    // lift floor
                    file.Write(log[i].floor);
                    file.Write(',');

                    // queue delimited by spaces
                    for (int j = 0; j < log[i].route.Length; j++)
                    {
                        file.Write(log[i].route[j]);
                        file.Write(' ');
                    }

                    // new line
                    file.WriteLine();
                }
            }
        }
    }
}

/*
NOTES:
-----------------------------
- lift indexes floors from 1 not 0
- it is possible for the current full lift behaviour to result in multiple passes of a call without responding to it, functionally prioritising highest and lowest floors.
Depending on usage volume it may be more desirable for a more complex solution to this problem. However this is out of scope for the prototype.
- The output csv delimits arrays by spaces
*/

/*
ASUMPTIONS:
-----------------------------
- Filepaths for input and output can be provided as args or requested from user running program
- It should be realtively easy to adapt and extend the provided code into a full solution
- provided events in a csv file will always be in ascending order by call time
- the lift starts at the ground floor
- the lift is only aware of detinations once the person has boarded the lift
- the 10 second movement time is assumed to include any boarding/departure times as otherwise they'd be unspecified and likely variable
- as this is a prototype and real input would be contrained by hardware error checking is not considered
- as log is taken on arrival to floor any boarding/departure is not processed until after the logging event
- the maximum lift capacity is still 8 and this should be accounted for
*/
