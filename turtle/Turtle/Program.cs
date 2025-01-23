using Logging;

namespace Turtle1 {
class Program
{
    
    static void Main(string[] args)
    {
        Logger.ConfigureLogging();

        Logger.LogInfo("Программа запущена");

        Console.WriteLine("""
            Commands:
            move N: command to change turtle’s position on N steps.
            angle N: command to change turtle’s angle of direction to N degrees.
            history: command to show program history.
            help: command to show all commands.
            pd: command to put down the pen.
            pu: command to put up the pen.
            color {colorName}: command to change turtle’s color of the pen to {colorName} color.
            steps: command to show all executed steps.
            figures: command to show all properties of completed figures.
            uexit: command to exit without saving state.
            cexit: command to exit and clear all the states.
            exit: command to exit the program.
            """);

        Field field = null;
        
        if (File.Exists(StateManager.filePath) && !string.IsNullOrEmpty(File.ReadAllText(StateManager.filePath)))
        {
            field = StateManager.DeserializeFromJson<Field>();
        }
        else
        {
            Turtle turtle = new Turtle(); 
            field = new Field(turtle);
        }
        
        field.StartDrawing();
    }
}
}

