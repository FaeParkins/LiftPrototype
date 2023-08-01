# LiftPrototype

This project is my solution to the task of constructing a prototype lift control system as per the provided specification.
The project is a C# Console application.

## Usage
The program is designed for and should be run in the Console/Terminal.
- When inputting the filepaths as arguments input as follows:

      Path\LiftPrototype.exe "input.csv" "output.csv"

- When inputting the filepaths via the console during runtime, input as follows:

      Please enter the filepath for the input csv:
      input.csv
      Please enter the filepath for the output csv:
      output.csv

## Assumptions Made
- The input and output csv filepaths are provided to the program via either input args or in response to the program
requesting them as console input.
- The file handling should be managed externally to the lfit system itself as it should make the prototype easier to adapt
into a fuller solution.
- Events provided by the input csv will always be in ascending order by call time.
- The lift starts at the lowest floor.
- The lift is only aware of the requested destinations once the relevent person has boarded it.
- The 10 second time to move up or down one floor is assumed to include any time for boarding/departure.
This removes uncertainty and undefined times from the current model.
- As this is a prototype, and any real application would have its inputs constrained by hardware, error checking
is not considered at this stage.
- As the output logs are extracted on floor arrival, boarding/departures at that floor will not be reflected in the
respective output log.
- The maximum lift capacity of 8 people from the previous task is still present and requires consideration.

## General Notes
- As per the provided data the lift system indexes floors starting at 1.
- Due to the current implementation of full lift handling, it is possible a person waiting
may have the lift pass by them multiple times. As based on the provided data this scenario is notably unlikely
the current solution has be kept. Should the client wish it, a more complex solution could be implemented to resolve
this by assigning priority to missed individuals.
- The output csv contains lists as some of its elements, to allow for readability and easy extraction these have been
delimited with a space character.
