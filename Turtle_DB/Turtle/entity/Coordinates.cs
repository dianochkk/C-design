using Logging;
namespace Turtle1 {
    [Serializable]
    public class Coordinates 
{
    public double X { get; }
    public double Y { get; }

    public Coordinates(double x, double y)
    {
        X = x;
        Y = y;
        Logger.LogInfo("Новая координата создана");
    }

    public override string ToString()
    {
        return $"x={X}, y={Y}";
    }
}
}