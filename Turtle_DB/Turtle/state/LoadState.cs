namespace Turtle1;

public class LoadState
{
    public static Field LoadFieldState()
    {
        List<Coordinates> list_c = new List<Coordinates>();
        List<List<Coordinates>> list_figures = new List<List<Coordinates>>();
        List<Line> lines_list = new List<Line>();
        List<Line> steps = new List<Line>();
        List<Line> all_steps = new List<Line>();
        
        DatabaseManager db = new DatabaseManager();
        Field fieldState = new Field();
        fieldState.turtle = db.GetTurtles().Result[0];
        
        var coordinates = db.GetCoordinates().Result;
        for (int i = 0; i < coordinates.Count; i++)
        {
            var coordinate = coordinates[i];
            string[] coordinate_split = coordinate.Split(", ");
            Coordinates c = new Coordinates(Double.Parse(coordinate_split[1]), Double.Parse(coordinate_split[2])); 
            list_c.Add(c);
        }
        
        var figures = db.GetFigures().Result;
        for (int i = 0; i < figures.Count; i++)
        {
            string figure = figures[i].Substring(4, figures[i].Length - 5);
            string[] figure_split = figure.Split(", ");
            List<Coordinates> listFigure = new List<Coordinates>();
            for (int j = 0; j < figure_split.Length; j++)
            {
                listFigure.Add(list_c[int.Parse(figure_split[j]) - 1]);
            }
            list_figures.Add(listFigure);
            
        }
        fieldState.figures = list_figures;
        
        var lines = db.GetLines().Result;
        for (int i = 0; i < lines.Count; i++)
        {
            string lineString = lines[i];
            string[] parsed_line = lineString.Split(", ");
            Line line = new Line(list_c[int.Parse(parsed_line[1]) - 1], list_c[int.Parse(parsed_line[2]) - 1], parsed_line[3]);
            
            lines_list.Add(line);
        }
        
        var stepsDB = db.GetSteps().Result;
        for (int i = 0; i < steps.Count; i++)
        {
            string step = stepsDB[i];
            string[] parsed_step = step.Split(", ");
            steps.Add(lines_list[int.Parse(parsed_step[1]) - 1]);
        }
        fieldState.steps = steps;
        
        var allStepsDB = db.GetAllSteps().Result;
        for (int i = 0; i < steps.Count; i++)
        {
            string allSteps = allStepsDB[i];
            string[] parsed_steps = allSteps.Split(", ");
            all_steps.Add(lines_list[int.Parse(parsed_steps[1]) - 1]);
        }
        fieldState.all_steps = all_steps;
        
        return fieldState;
    }
}