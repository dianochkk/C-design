using System.Data;
using Dapper;
using Npgsql;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;
using Logging;

namespace Turtle1
{
    public class DatabaseManager
    {
        private readonly string _connectionString;

        public DatabaseManager()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("/Users/dianathebest/RiderProjects/Turtle_DB/Turtle/resources/appsettings.json")
                .Build();

            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        private NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }

        public async Task InitializeDatabase()
        {
            try
            {
                
                using var connection = GetConnection();
                await connection.OpenAsync();
                Logger.LogInfo("Connection opened.");

                var createTurtleTable = @"
            CREATE TABLE IF NOT EXISTS turtles (
                id SERIAL PRIMARY KEY,
                x DOUBLE PRECISION,
                y DOUBLE PRECISION,
                color VARCHAR(50),
                tail BOOLEAN,
                angle DOUBLE PRECISION
            );";
                
                var createLineTable = @"
            CREATE TABLE IF NOT EXISTS lines (
                id SERIAL PRIMARY KEY,
                first_coordinate_id INT NOT NULL,
                second_coordinate_id INT NOT NULL,
                FOREIGN KEY (first_coordinate_id) REFERENCES coordinates (id) ON DELETE CASCADE,
                FOREIGN KEY (second_coordinate_id) REFERENCES coordinates (id) ON DELETE CASCADE,
                color VARCHAR(50)
            );";

                var createCoordinatesTable = @"
            CREATE TABLE IF NOT EXISTS coordinates (
                id SERIAL PRIMARY KEY,
                x DOUBLE PRECISION,
                y DOUBLE PRECISION
            );";
                var createFieldTable = @"
            CREATE TABLE IF NOT EXISTS fields (
                id SERIAL PRIMARY KEY,
                turtle_id INTEGER,
                FOREIGN KEY (turtle_id) REFERENCES turtles(id) ON DELETE CASCADE
            );";
                var createAllStepsTable = @"
            CREATE TABLE IF NOT EXISTS all_steps (
                id SERIAL PRIMARY KEY,
                line_id INTEGER,
                FOREIGN KEY (line_id) REFERENCES lines(id) ON DELETE CASCADE
            );";
                var createStepsTable = @"
            CREATE TABLE IF NOT EXISTS steps (
                id SERIAL PRIMARY KEY,
                line_id INTEGER,
                FOREIGN KEY (line_id) REFERENCES lines(id) ON DELETE CASCADE
            );";
                var createFiguresTable = @"
            CREATE TABLE IF NOT EXISTS figures (
                id SERIAL PRIMARY KEY,
                coordinate_ids INTEGER[]
            );";
                var createHistoryTable = @"
            CREATE TABLE IF NOT EXISTS history (
                id SERIAL PRIMARY KEY,
                command VARCHAR(50) NOT NULL
            );";
                
                await connection.ExecuteAsync(createTurtleTable);
                await connection.ExecuteAsync(createCoordinatesTable);
                await connection.ExecuteAsync(createLineTable);
                

                await connection.ExecuteAsync(createFieldTable);
                await connection.ExecuteAsync(createAllStepsTable);
                await connection.ExecuteAsync(createStepsTable);
                await connection.ExecuteAsync(createFiguresTable);
                await connection.ExecuteAsync(createHistoryTable);
                
                
                
                
                

                Logger.LogInfo("Все таблицы успешно инициализированы.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                Logger.LogError(ex.Message);
            }
        }


        public async Task InsertTurtle(Turtle turtle)
        {
            var query = "INSERT INTO turtles (x, y, color, tail, angle) VALUES (@X, @Y, @Color, @Tail, @Angle)";

            using var connection = GetConnection();
            await connection.OpenAsync();
            await connection.ExecuteAsync(query, turtle);
            Logger.LogInfo("Turtle inserted successfully.");
        }
        
        public async Task AddFieldAsync(int turtleId)
        {
            string query = @"
        INSERT INTO fields (turtle_id)
        VALUES (1)";

            try
            {
                using var connection = GetConnection(); // Получение соединения
                await connection.OpenAsync();

                // Выполнение запроса
                await connection.ExecuteAsync(query);

                Logger.LogInfo($"Field with turtle_id {turtleId} successfully added.");
            }
            catch (Exception ex)
            {
                Logger.LogInfo($"An error occurred while adding field: {ex.Message}");
                Console.WriteLine($"An error occurred while adding field: {ex.Message}");
            }
        }


        public async Task<List<Turtle>> GetTurtles()
        {
            var query = "SELECT * FROM turtles";

            using var connection = GetConnection();
            await connection.OpenAsync();
            return (await connection.QueryAsync<Turtle>(query)).AsList();
        }
        
        public async Task AddCommandToHistory(string command)
        {
            const string query = "INSERT INTO history (command) VALUES (@Command)";

            try
            {
                using var connection = GetConnection(); // Метод для получения соединения
                await connection.OpenAsync();

                await connection.ExecuteAsync(query, new { Command = command });

                Logger.LogInfo("Command added to history successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while adding to history: {ex.Message}");
            }
        }
        
        public async Task<int> AddCoordinate(double x, double y)
        {
            const string query = "INSERT INTO coordinates (x, y) VALUES (@X, @Y) RETURNING id";
            try
            {
                using var connection = GetConnection();
                await connection.OpenAsync();

                var id = await connection.ExecuteScalarAsync<int>(query, new { X = x, Y = y });
                Logger.LogInfo($"Coordinate added with ID: {id}");
                return id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding coordinate: {ex.Message}");
                throw;
            }
        }

        public async Task<int> AddLine(int firstCoordinateId, int secondCoordinateId, string color)
        {
            const string query = "INSERT INTO lines (first_coordinate_id, second_coordinate_id, color) VALUES (@FirstId, @SecondId, @Color) RETURNING id";
            try
            {
                using var connection = GetConnection();
                await connection.OpenAsync();

                var id = await connection.ExecuteScalarAsync<int>(query, new { FirstId = firstCoordinateId, SecondId = secondCoordinateId, Color = color });
                Logger.LogInfo($"Line added with ID: {id}");
                return id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding line: {ex.Message}");
                throw;
            }
        }

        public async Task<int> AddStepInAllSteps(int lineId)
        {
            const string query = "INSERT INTO all_steps (line_id) VALUES (@LineId) RETURNING id";
            try
            {
                using var connection = GetConnection();
                await connection.OpenAsync();

                var id = await connection.ExecuteScalarAsync<int>(query, new { LineId = lineId });
                Logger.LogInfo($"Step added with ID: {id}");
                return id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding step: {ex.Message}");
                throw;
            }
        }

        public async Task AddLineToAllSteps(double x1, double y1, double x2, double y2, string color)
        {
            try
            {
                // Добавляем первую и вторую координаты
                var firstCoordinateId = await AddCoordinate(x1, y1);
                var secondCoordinateId = await AddCoordinate(x2, y2);

                // Добавляем линию с этими координатами
                var lineId = await AddLine(firstCoordinateId, secondCoordinateId, color);

                // Добавляем шаг с этой линией
                var stepId = await AddStepInAllSteps(lineId);

                Logger.LogInfo($"Successfully added step with ID: {stepId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding line with steps: {ex.Message}");
            }
        }
        
        public async Task ClearAllSteps()
        {
            const string deleteStepsQuery = "DELETE FROM all_steps";

            try
            {
                using var connection = GetConnection();
                await connection.OpenAsync();
                
                await connection.ExecuteAsync(deleteStepsQuery);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clearing all steps: {ex.Message}");
                throw;
            }
        }
        
        public async Task ClearSteps()
        {
            const string deleteStepsQuery = "DELETE FROM steps";

            try
            {
                using var connection = GetConnection();
                await connection.OpenAsync();
                
                await connection.ExecuteAsync(deleteStepsQuery);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clearing all steps: {ex.Message}");
                throw;
            }
        }
        
        public async Task<int> AddFigure(List<int> coordinateIds)
        {
            const string query = @"
        INSERT INTO figures (coordinate_ids)
        VALUES (@CoordinateIds)
        RETURNING id";

            try
            {
                using var connection = GetConnection(); // Метод для получения соединения
                await connection.OpenAsync();

                // Выполняем запрос
                var id = await connection.ExecuteScalarAsync<int>(query, new
                {
                    CoordinateIds = coordinateIds.ToArray() // Преобразуем список в массив
                });

                Logger.LogInfo($"Figure added with ID: {id}");
                return id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding figure: {ex.Message}");
                throw;
            }
        }

        public async Task<int> AddStep(int lineId)
        {
            const string query = "INSERT INTO steps (line_id) VALUES (@LineId) RETURNING id";
            try
            {
                using var connection = GetConnection();
                await connection.OpenAsync();

                var id = await connection.ExecuteScalarAsync<int>(query, new { LineId = lineId });
                Logger.LogInfo($"Step added with ID: {id}");
                return id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding step: {ex.Message}");
                throw;
            }
        }
        public async Task AddLineToSteps(double x1, double y1, double x2, double y2, string color)
        {
            try
            {
                // Добавляем первую и вторую координаты
                var firstCoordinateId = await AddCoordinate(x1, y1);
                var secondCoordinateId = await AddCoordinate(x2, y2);

                // Добавляем линию с этими координатами
                var lineId = await AddLine(firstCoordinateId, secondCoordinateId, color);

                // Добавляем шаг с этой линией
                var stepId = await AddStep(lineId);

                Logger.LogInfo($"Successfully added step with ID: {stepId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding line with steps: {ex.Message}");
            }
        }
        
        
        public async Task<List<string>> GetLines()
        {
            var query = "SELECT * FROM lines";

            using var connection = GetConnection();
            await connection.OpenAsync();
            return ConvertToStringList(await connection.QueryAsync(query));
        }
        public async Task<List<string>> GetAllSteps()
        {
            var query = "SELECT * FROM all_steps";

            using var connection = GetConnection();
            await connection.OpenAsync();
            return ConvertToStringList(await connection.QueryAsync(query));
        }
        public async Task<List<string>> GetCoordinates()
        {
            var query = "SELECT * FROM coordinates";

            using var connection = GetConnection();
            await connection.OpenAsync();
            return ConvertToStringList(await connection.QueryAsync(query));
        }
        public async Task<List<string>> GetFields()
        {
            var query = "SELECT * FROM fields";

            using var connection = GetConnection();
            await connection.OpenAsync();
            return ConvertToStringList(await connection.QueryAsync(query));
        }
        public async Task<List<string>> GetFigures()
        {
            var query = "SELECT * FROM figures";

            using var connection = GetConnection();
            await connection.OpenAsync();
            return ConvertToStringList(await connection.QueryAsync(query));
        }
        public async Task<List<string>> GetHistory()
        {
            var query = "SELECT * FROM history";

            using var connection = GetConnection();
            await connection.OpenAsync();
            return ConvertToStringList(await connection.QueryAsync(query));
        }
        public async Task<List<string>> GetSteps()
        {
            var query = "SELECT * FROM steps";

            using var connection = GetConnection();
            await connection.OpenAsync();
            return ConvertToStringList(await connection.QueryAsync(query));
        }
        
        private List<string> ConvertToStringList(IEnumerable<dynamic> result)
        {
            var stringList = new List<string>();

            foreach (var row in result)
            {
                // Преобразуем каждую строку таблицы
                var values = ((IDictionary<string, object>)row)
                    .Select(kv =>
                    {
                        if (kv.Value is Array array) // Если значение — массив
                        {
                            var arrayString = string.Join(", ", array.Cast<object>());
                            return $"[{arrayString}]";
                        }
                        return $"{kv.Value}";
                    })
                    .ToArray();

                stringList.Add(string.Join(", ", values));
            }

            return stringList;
        }

        public async Task UpdateFirstTurtleAsync(Turtle turtle)
        {
            
            const string query = @"
            UPDATE turtles
            SET x = @X, y = @Y, color = @Color, tail = @Tail, angle = @Angle
            WHERE id = (SELECT id FROM turtles ORDER BY id LIMIT 1)";

            try
            {
                using var connection = GetConnection();
                await connection.OpenAsync(); 
                var affectedRows = await connection.ExecuteAsync(query, new
                {
                    X = turtle.X,
                    Y = turtle.Y,
                    Color = turtle.Color,
                    Tail = turtle.Tail,
                    Angle = turtle.Angle
                });

                if (affectedRows > 0)
                {
                    Logger.LogInfo("First turtle updated successfully.");
                }
                else
                {
                    Logger.LogInfo("No turtles found to update.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while updating the first turtle: {ex.Message}");
            }
            
        }
        
        public async Task DropAllTablesAsync()
        {
            const string dropTablesQuery = @"
            DROP TABLE IF EXISTS fields CASCADE;
            DROP TABLE IF EXISTS turtles CASCADE;
            DROP TABLE IF EXISTS all_steps CASCADE;
            DROP TABLE IF EXISTS steps CASCADE;
            DROP TABLE IF EXISTS lines CASCADE;
            DROP TABLE IF EXISTS coordinates CASCADE;
            DROP TABLE IF EXISTS figures CASCADE;
            DROP TABLE IF EXISTS history CASCADE;";

            try
            {
                using var connection = GetConnection();
                await connection.OpenAsync(); 

                await connection.ExecuteAsync(dropTablesQuery); 

                Logger.LogInfo("All tables have been dropped successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while dropping tables: {ex.Message}");
            }
            
        }
        
        

    }
}
