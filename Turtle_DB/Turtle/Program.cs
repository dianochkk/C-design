using Logging;
using Microsoft.VisualBasic;


namespace Turtle1 {
class Program
{
    
    public static async Task Main(string[] args)
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
                       exit: command to exit the program.
                       """);

        DatabaseManager db = new DatabaseManager();
        await db.InitializeDatabase();
        
        if (db.GetFields().Result.Count == 0)
        {
            Turtle turtle = new Turtle();
            await db.InsertTurtle(turtle);
            await db.AddFieldAsync(1);
            Field f = new Field();
            await f.StartDrawing();
        }
        else
        {
            Field f = LoadState.LoadFieldState();
            await f.StartDrawing();
        }
        

    }
}
}

