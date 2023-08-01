using System;
// for list use
using System.Collections.Generic;

namespace LiftPrototype
{
    /// <summary>
    /// <c>Lift</c> takes <c>CallRequest</c> structs and uses them to generate and follow a lift route.
    /// This class also provides data for logging and output purposes.
    /// </summary>
    class Lift
    {
        /// <value><c>current_floor</c> keeps track of the current floor the lift is at.</value>
        private int current_floor;

        /// <value><c>direction</c> is used to keep track of if the lift is heading up = 1 or down = 0.</value>
        private int direction;

        /// <value><c>current_time</c> keeps track of the current system time.</value>
        private int current_time;

        /// <value><c>travel_progress</c> is a variable used to keep track of how many seconds have been spent traveling towards the next floor. This value can also be used to infer the lift is in motion when the value is not 0.</value>
        private int travel_progress;

        /// <value><c>on_lift</c> keeps a list of all call requests that have boarded the lift, but not yet arrived at their destination.</value>
        private List<CallRequest> on_lift;

        /// <value><c>waiting</c> keeps a list of all call requests that have yet to board the lift.</value>
        private List<CallRequest> waiting;

        /// <value><c>call_queue</c> is a pair of lists that keep track of the lifts path between called/requested floors. One list is used to track the upwards path, the other the downward.</value>
        private List<int>[] call_queue;

        /// <value><c>floor_limits</c> is a pair of values denoting the minimum and maximum floors accessible by the lift system.</value>
        private readonly int[] floor_limits = { 1, 10 };

        /// <value><c>travel_time</c> is the amount of seconds it takes the lift to move from one floor to the next.</value>
        private readonly int travel_time = 10;

        /// <value><c>lift_capacity</c> is the maximum number of people the lift can hold.</value>
        private readonly int lift_capacity = 8;

        /// <summary>
        /// The constructor initialises all variables used to define the lifts state, and initialises the lists used for route planning opperations.
        /// </summary>
        public Lift()
        {
            // init lift to start at lowest floor
            current_floor = floor_limits[0];
            // init lift direction to be upwards
            direction = 1;
            // init system time to 0
            current_time = 0;
            // init travel progress to zero, representing stationary at a floor
            travel_progress = 0;

            // init the two request lists to empty
            on_lift = new List<CallRequest>();
            waiting = new List<CallRequest>();

            // init the up and down route queues to empty
            call_queue = new List<int>[2];
            call_queue[0] = new List<int>();
            call_queue[1] = new List<int>();
        }

        /// <summary>
        /// This getter method returns the last floor the lift passed. To check if the lift is stopped at the floor use in conjunction with <c>IsMoving()</c>.
        /// </summary>
        /// <returns>The last floor passed by the lift.</returns>
        public int GetCurrentFloor()
        {
            return current_floor;
        }

        /// <summary>
        /// This method returns the time since the lift process began.
        /// </summary>
        /// <returns>The time since lift start in seconds.</returns>
        public int GetCurrentTime()
        {
            return current_time;
        }

        /// <summary>
        /// This method allows the lifts movement status to be checked externally. Can be used in conjunction with <c>GetCurrentFloor()</c> to check when the lift has stopped at a floor.
        /// </summary>
        /// <returns>A bool representing if the lift is moving. </returns>
        public bool IsMoving()
        {
            return travel_progress != 0;
        }

        /// <summary>
        /// This method simplifies any checks for if the route is empty.
        /// </summary>
        /// <returns>A bool inidcating if the route is empty.</returns>
        public bool RouteEmpty()
        {
            return call_queue[0].Count == 0 && call_queue[1].Count == 0;
        }

        /// <summary>
        /// This method returns the current list of the people in the lift.
        /// </summary>
        /// <returns>The list of IDs present in the lift.</returns>
        public string[] GetLiftInhabitants()
        {
            // create an output array of the correct size
            string[] inhabitants = new string[on_lift.Count];

            // for every person in the lift
            for (int i = 0; i < on_lift.Count; i++)
            {
                // get the persons ID
                inhabitants[i] = on_lift[i].caller_ID;
            }

            // return the acquired list
            return inhabitants;
        }

        /// <summary>
        /// This method returns the current planned route for the lift.
        /// </summary>
        /// <returns>The list of floor numbers, in order of planned arrival.</returns>
        public int[] GetLiftQueue()
        {
            // create an output array with the same size as both directional queues
            int[] route = new int[call_queue[0].Count + call_queue[1].Count];

            // make an editable copy of direction for use as an index
            int temp_direction = direction;
            // keep track of how many floors have already been added to the route by previous direction
            int route_progress = 0;

            // for both directions
            for (int i = 0; i < 2; i++)
            {
                // for every floor in current directions queue
                for (int j = 0; j < call_queue[temp_direction].Count; j++)
                {
                    // add the floor to the route
                    route[route_progress + j] = call_queue[temp_direction][j];
                }

                // if first direction
                if (i == 0)
                {
                    // update the route progress value to account for already stored info
                    route_progress = call_queue[temp_direction].Count;
                    // swap temporary direction index
                    temp_direction = (temp_direction == 1) ? 0 : 1;
                }
            }

            // return the combined route list
            return route;
        }

        /// <summary>
        /// This method acts as the core of the lifts opepration, handling boarding and departure, updating the internal clock, and ensuring that the lift moves towards its current destination.
        /// </summary>
        public void Update()
        {
            // No action needed check

            // if the route is empty
            if (RouteEmpty())
            {
                // increment time
                current_time++;

                // early exit
                return;
            }

            // passenger update on stopped at floor

            // if lift is not moving, and has reached the next destination
            if (travel_progress == 0 && current_floor == call_queue[direction][0])
            {
                // remove anyone getting off
                Depart();

                // add destination for anyone getting on and swap them to on_lift
                Board();

                // as destination has been reached it can be removed from the route
                call_queue[direction].RemoveAt(0);
            }

            // travel opperations

            // if the lift is about to reach a new floor
            if (travel_progress == travel_time - 1)
            {
                // update the current floor based on current lift direction
                current_floor = (direction == 1) ? current_floor + 1 : current_floor - 1;

                // reset travel progress
                travel_progress = 0;
            }
            // if the lift is moving and is not about to arrive at a new floor
            else if (travel_progress != 0)
            {
                // increase travel progress
                travel_progress++;
            }
            // if stationary lift but current floor is not destination
            else if (travel_progress == 0)
            {
                // if there are destinations in current direction
                if(call_queue[direction].Count != 0)
                {
                    // begin moving towards next floor
                    travel_progress++;
                }
                // otherwise the lift must change direction
                else
                {
                    // swap direction
                    direction = (direction == 1) ? 0 : 1;

                    // being traveling in new direction
                    travel_progress++;
                }
            }

            // increment time by one second
            current_time++;
        }

        /// <summary>
        /// This method processes calls made to the lift, adding the calling floor to the route.
        /// The entire request struct is stored to allow for logging and correct provision of destination data once boarding has occured.
        /// If the lift is stopped at the calling floor the method will add the person to the on lift list and add their destination to the route.
        /// </summary>
        /// <param name="call">the struct containing the calls data. Also contains time of call for logging purposes, and destination for use during boarding event.</param>
        public void AddCall(CallRequest call)
        {
            // if request is at same floor as lift and lift is stoped
            if (call.start_floor == current_floor && travel_progress == 0)
            {
                // add destination to correct queue via helper method
                AddDestination(call.end_floor);

                // add request to on lift list
                on_lift.Add(call);

                // output to console the time taken to respond to the call
                Console.WriteLine("Call from " + call.caller_ID + " at floor " + current_floor.ToString() + " took 0 seconds to fulfill.");
            }
            // otherwise
            else
            {
                // add call to correct queue via helper method
                AddDestination(call.start_floor);

                // add request to waiting list
                waiting.Add(call);
            }
        }

        /// <summary>
        /// This method checks if anyone on the lift is currently departing the lift, and removes them from the relevant list.
        /// </summary>
        private void Depart()
        {
            // check every person on the lift
            for (int i = on_lift.Count - 1; i >= 0; i--)
            {
                // if they depart on the current floor
                if (on_lift[i].end_floor == current_floor)
                {
                    // remove the person from the lift
                    on_lift.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// This method checks if any of the calls to the lift are from the current floor, swapping people from the waiting list to the on lift list if so.
        /// Once a person is added to the lift the method also adds their destination to the route.
        /// Should the lift be full and people are left to board the method will block boarding, and add the floor back to the route as a future destination.
        /// </summary>
        private void Board()
        {
            // check every person in the waiting list
            for (int i = waiting.Count - 1; i >= 0; i--)
            {
                // if they board at this floor but the lift is full
                if (waiting[i].start_floor == current_floor && IsFull())
                {
                    // Do not add to the lift

                    // Add the call to the opposite directions route
                    AddFloorToQueue(current_floor, (direction == 1) ? 0 : 1);
                }
                // if they board at this floor and the lift has room
                else if (waiting[i].start_floor == current_floor)
                {
                    // output to console the time taken to respond to the call
                    Console.WriteLine("Call from " + waiting[i].caller_ID + " at floor " + current_floor.ToString() + " took " + (current_time - waiting[i].call_time).ToString() + " seconds to fulfill.");

                    // add their destination to the route
                    AddDestination(waiting[i].end_floor);

                    // swap them to the on lift list
                    on_lift.Add(waiting[i]);
                    waiting.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// This method provides a check for if the lift has reached its maximum capacity.
        /// </summary>
        /// <returns>A bool indicating if the lift is full.</returns>
        private bool IsFull()
        {
            return on_lift.Count > lift_capacity;
        }

        /// <summary>
        /// This method adds the provided floor to the lifts route.
        /// </summary>
        /// <param name="floor">the floor to be added to the route.</param>
        private void AddDestination(int floor)
        {
            // if request is above lift
            if (floor > current_floor)
            {
                // add floor to upwards route
                AddFloorToQueue(floor, 1);
            }
            // if request is below lift
            else if (floor < current_floor)
            {
                // add floor to downwards route
                AddFloorToQueue(floor, 0);
            }
            // otherwise request is at current floor
            else
            {
                // add to queue opposite of direction
                AddFloorToQueue(floor,(direction == 1)? 0 : 1);
            }
        }

        /// <summary>
        /// This method adds the provided floor to the <c>call_queue</c> in the provided direction.
        /// </summary>
        /// <param name="floor">the floor to be added to the routes queue.</param>
        /// <param name="direction">the direction the lift must be traveling in when it visits <c>floor</c>.</param>
        private void AddFloorToQueue(int floor, int direction)
        {
            // if the target queue is empty
            if (call_queue[direction].Count == 0)
            {
                // add floor to the target queue
                call_queue[direction].Add(floor);
            }
            // if the floor is already in the target queue
            else if (call_queue[direction].Contains(floor))
            {
                // no work needs to be done
                return;
            }
            // otherwise add to correct queue
            else
            {
                // the index at which the floor should be added
                int insertion_index = 0;

                // for upward queue
                if (direction == 1)
                {
                    // find last value in queue less than the floor
                    for (int i = 0; i < call_queue[direction].Count; i++)
                    {
                        // if floor greater than currently checked floor
                        if (floor > call_queue[direction][i])
                        {
                            // store the location where the floor should be inserted
                            insertion_index = i + 1;
                        }
                    }
                }
                // for downward queue
                else
                {
                    // find last value in queue greater than the floor
                    for (int i = 0; i < call_queue[direction].Count; i++)
                    {
                        // if floor less than currently checked floor
                        if (floor < call_queue[direction][i])
                        {
                            // store the location where the floor should be inserted
                            insertion_index = i + 1;
                        }
                    }
                }

                // insert floor into queue at correct location
                call_queue[direction].Insert(insertion_index, floor);
            }
        }
    }
}
