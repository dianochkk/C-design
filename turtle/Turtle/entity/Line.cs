using Logging;
namespace Turtle1 {
    [Serializable]
class Line 
{
    public Coordinates FirstCoordinate { get; }
    public Coordinates SecondCoordinate { get; }
    public string Color { get; }

    public Line(Coordinates firstCoordinate, Coordinates secondCoordinate, string color)
    {
        FirstCoordinate = firstCoordinate;
        SecondCoordinate = secondCoordinate;
        Color = color;
        Logger.LogInfo("Создана линия");
    }

    public override string ToString()
    {
        return $"Первая координата= {FirstCoordinate} \n Вторая координата= {SecondCoordinate} \n Цвет='{Color}' \n";
    }
}
}