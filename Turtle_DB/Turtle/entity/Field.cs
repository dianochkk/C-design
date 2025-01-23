
using System.Numerics;
using System.Text.Json.Serialization;
using Logging;

namespace Turtle1 {
    [Serializable]
    public class Field 
    { 
        
        
        public Turtle turtle { get; set; }

    public DatabaseManager db { get; set; }
    
    public List<Line> steps { get; set; } = new(); // Только нарисованные шаги
    
    public List<List<Coordinates>> figures { get; set; } = new();
    
    public List<Line> all_steps { get; set; } = new(); // Все шаги
    
    
    public Field()
    {
        db = new DatabaseManager();
        turtle = db.GetTurtles().Result[0];
        
        Logger.LogInfo("Создан объект Field для рисования.");
    }

    
    public async Task StartDrawing()
    {
        try
        {
            Logger.LogInfo("Начато рисование.");
            Console.WriteLine($"Current color: {turtle.Color}, pen down: {turtle.Tail}, location ({turtle.X}; {turtle.Y}), direction: {turtle.Angle} degrees.");

            string command;
            while ((command = Console.ReadLine()) != "exit")
            {
                
                await db.AddCommandToHistory(command);
                await ProcessCommand(command);
            }
            await db.UpdateFirstTurtleAsync(turtle);
        }
        catch (Exception ex)
        {
            Logger.LogError($"Ошибка во время рисования: {ex.Message}");
        }
        finally
        {
            Logger.LogInfo("Программа завершает работу");
        }
    }



    public async Task ProcessCommand(string command1)
    {
        Logger.LogInfo($"Получена команда: {command1}");
        string[] parsedCommand = command1.Split(' ');
        
        double x, y;

        switch (parsedCommand[0])
            {
                case "move":
                    try
                    {
                        if (parsedCommand.Length == 2)
                        {
                            double distance = double.Parse(parsedCommand[1]);
                            x = turtle.X + Math.Round(distance * Math.Cos((turtle.Angle / 180) * Math.PI), 2);
                            y = turtle.Y + Math.Round(distance * Math.Sin((turtle.Angle / 180) * Math.PI), 2);

                            Logger.LogInfo($"Черепаха перемещается на расстояние {distance}. Новые координаты: ({x}; {y})");

                            if (turtle.Tail)
                            {
                                Line line = new Line(new Coordinates(turtle.X, turtle.Y), new Coordinates(x, y),
                                    turtle.Color);
                                await db.AddLineToSteps(turtle.X, turtle.Y, x, y, turtle.Color);
                                steps.Add(line);
                                if (steps.Count >= 2)
                                {
                                    double x1 = turtle.X, y1 = turtle.Y, x2, y2, x3, y3;
                                    int startLine = -1;
                                    Coordinates intersection = null;
                                    for (int i = 0; i < steps.Count - 1; i++)
                                    {
                                        x2 = steps[i].FirstCoordinate.X;
                                        y2 = steps[i].FirstCoordinate.Y;
                                        x3 = steps[i].SecondCoordinate.X;
                                        y3 = steps[i].SecondCoordinate.Y;
                                        intersection = FindIntersection(x, y, x1, y1, x2, y2, x3, y3);
                                        if (intersection != null)
                                        {
                                            startLine = i;
                                            Console.WriteLine("Образовалась фигура, построение новой фигуры начато");
                                            break;
                                        }
                                    }
                                    if (startLine != -1)
                                    {
                                        List<Coordinates> figure = new List<Coordinates>();
                                        List<int> ids = new List<int>();
                                        int a = await db.AddCoordinate(intersection.X, intersection.Y);
                                        ids.Add(a);
                                        figure.Add(intersection);
                                        if (intersection.X == steps[startLine].SecondCoordinate.X && intersection.Y == steps[startLine].SecondCoordinate.Y)
                                        {
                                            startLine++;
                                        }
                                        for (int j = startLine; j < steps.Count; j++)
                                        {
                                            a = await db.AddCoordinate(steps[j].SecondCoordinate.X, steps[j].SecondCoordinate.Y);
                                            ids.Add(a);
                                            figure.Add(steps[j].SecondCoordinate);
                                        }
                                        figures.Add(figure);
                                        await db.AddFigure(ids);
                                        await db.ClearSteps();
                                        steps.Clear();
                                    }
                                }
                                
                            }
                            Line step = new Line(new Coordinates(turtle.X, turtle.Y), new Coordinates(x, y), turtle.Color);
                            await db.AddLineToAllSteps(turtle.X, turtle.Y, x, y, turtle.Color);
                            all_steps.Add(step);
                            

                            turtle.X = x;
                            turtle.Y = y;

                            Console.WriteLine($"Current color: {turtle.Color}, pen down: {turtle.Tail}, location ({turtle.X}; {turtle.Y}), direction: {turtle.Angle} degrees.");
                        }
                        else
                        {
                            Logger.LogWarning("Неверное количество аргументов для команды move.");
                            Console.WriteLine("Неверное количество аргументов");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"Ошибка в команде move: {ex.Message}");
                        Console.WriteLine("Неверный ввод");
                    }
                    break;

                case "angle":
                    try
                    {
                        if (parsedCommand.Length == 2)
                        {
                            double newAngle = double.Parse(parsedCommand[1]);
                            if (newAngle <= 360)
                            {
                                
                                turtle.Angle = newAngle;
                                Logger.LogInfo($"Угол поменялся и стал равен {turtle.Angle}");
                                Console.WriteLine($"Current color: {turtle.Color}, pen down: {turtle.Tail}, location ({turtle.X}; {turtle.Y}), direction: {turtle.Angle} degrees.");
                            }
                            else
                            {
                                Logger.LogInfo($"Пользователь ввел угол больше 360");
                                Console.WriteLine("Угол должен быть в пределах 360");
                            }
                        }
                        else
                        {
                            Logger.LogInfo($"Пользователь ввел неверное количество аргументов");
                            Console.WriteLine("Неверное количество аргументов");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"Ошибка в команде angle: {ex.Message}");
                        Console.WriteLine("Неверный ввод");
                    }
                    break;

                case "pd":
                    turtle.Tail = true;
                    Logger.LogInfo("Перо опущено");
                    Console.WriteLine($"Current color: {turtle.Color}, pen down: {turtle.Tail}, location ({turtle.X}; {turtle.Y}), direction: {turtle.Angle} degrees.");
                    break;

                case "pu":
                    turtle.Tail = false;
                    Logger.LogInfo("Перо поднято");
                    Console.WriteLine($"Current color: {turtle.Color}, pen down: {turtle.Tail}, location ({turtle.X}; {turtle.Y}), direction: {turtle.Angle} degrees.");
                    break;

                case "color":
                    if (parsedCommand.Length == 2)
                    {
                        turtle.Color = parsedCommand[1];
                        Logger.LogInfo($"Цвет изменился на {turtle.Color}");
                        Console.WriteLine($"Current color: {turtle.Color}, pen down: {turtle.Tail}, location ({turtle.X}; {turtle.Y}), direction: {turtle.Angle} degrees.");
                    }
                    else
                    {
                        Logger.LogInfo("Цвет не изменился, ошибка в команде");
                        Console.WriteLine("Неверное количество аргументов");
                    }
                    break;

                case "steps":
                    Console.WriteLine("Координаты вершин линий:");
                    for (int i = 0; i < all_steps.Count; i++)
                    {
                        Console.WriteLine($"Линия {i + 1}: \n {all_steps[i]}");
                    }
                    Logger.LogInfo($"Координаты вершин выведены");
                    break;

                case "figures":
                    Console.WriteLine("Координаты вершин:");
                    for (int i = 0; i < figures.Count; i++)
                    {
                        Console.WriteLine($"Фигура {i + 1}:");
                        for (int j = 0; j < figures[i].Count; j++) {
                            Console.WriteLine($"{j + 1}. {figures[i][j]}");
                        }
                    }
                    Logger.LogInfo($"Координаты вершин фигур");
                    break;
                case "history":
                    Console.WriteLine("История команд:");
                    
                    Logger.LogInfo($"История команд");
                    break;
                case "clear":
                    await db.DropAllTablesAsync();
                    break;
                case "help":
                    Console.WriteLine("""
                                      
                                      Commands:
                                      move N: command to change turtle’s position on N steps.
                                      angle N: command to change turtle’s angle of direction to N degrees.
                                      history: command to show program history.
                                      help: command to show all commands.
                                      clear: command to clear saved state.
                                      pd: command to put down the pen.
                                      pu: command to put up the pen.
                                      color {colorName}: command to change turtle’s color of the pen to {colorName} color.
                                      steps: command to show all executed steps.
                                      figures: command to show all properties of completed figures.
                                      exit: command to exit the program.
                                      """);
                    break;

                default:
                    Console.WriteLine("Неверная команда!");
                    break;
            }
    }
    

    public Coordinates FindIntersection(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
    {
        double denominator = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);

        if (Math.Abs(denominator) < 1e-10)
        {
            return null;
        }

        double ix = ((x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4)) / denominator;
        double iy = ((x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4)) / denominator;

        return new Coordinates(ix, iy);
    }
    
}
    
}