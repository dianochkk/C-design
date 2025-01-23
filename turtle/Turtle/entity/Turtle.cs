using Logging;
namespace Turtle1 {
    [Serializable]
public class Turtle  : ITurtle 
{
    public double X { get; set; }
    public double Y { get; set; }
    public string Color { get; set; } = "black";
    public bool Tail { get; set; }
    public double Angle { get; set; }

    public Turtle()
    {
        X = 0;
        Y = 0;
        Angle = 0;
        Tail = false;
        Logger.LogInfo("Создана черепаха");
    }
}
}