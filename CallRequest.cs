using System;

namespace LiftPrototype
{
    /// <summary>
    /// <c>CallRequest</c> is a struct containing the lift call data provided in the input csv.
    /// This parsed form makes operating on the events easier.
    /// </summary>
    struct CallRequest
    {
        /// <value><c>caller_id</c> stores the ID of the person calling the lift. A string is used to accomadate future possibility of alphanumeric codes.</value>
        public string caller_ID;

        /// <value><c>start_floor</c> represents the floor the lift call was recieved from, stored in integer form.</value>
        public int start_floor;

        /// <value><c>end_floor</c> represents the floor the person calling wishes to go to, stored in integer form.</value>
        public int end_floor;

        /// <value><c>call_time</c> represents the time at which the lift is called, in seconds since start, stored in integer form.</value>
        // NOTE: currently the value is interpreted as "seconds since start" however the actual format is not specified
        // NOTE: if this is changed out for a more complex datatype later a lot of the surrounding maths needs updating to accomadate
        public int call_time;
    }
}