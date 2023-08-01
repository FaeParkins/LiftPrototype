using System;
// for list use
using System.Collections.Generic;

namespace LiftPrototype
{
    /// <summary>
    /// <c>LogData</c> is a struct containing the lifts logging data for use in the output csv.
    /// Use of a struct simplifies handling.
    /// </summary>
    struct LogData
    {
        /// <value><c>time</c> stores the time at which the log data was extracted. The value is stored as seconds since lift start.</value>
        public int time;

        /// <value><c>people</c> contains a list of the people in the lift when the log was extracted. Kept as a list of IDs.</value>
        public string[] people;

        /// <value><c>floor</c> stores the floor the lift was at when the log was extracted.</value>
        public int floor;

        /// <value><c>route</c> stores the lifts current call queue/route at the moment the log was extracted. Stored as an ordered list of floors. </value>
        public int[] route;
    }
}
