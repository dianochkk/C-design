using Logging;

namespace Turtle1;
using System.Text.Json;

public class StateManager
{
    public static string filePath { get; set; } = "../../../state/state.json";
    
    public static T DeserializeFromJson<T>()
    {
        try
        {
            // Чтение содержимого файла
            string jsonString = File.ReadAllText(filePath);
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                IncludeFields = true
            };

            // Десериализация JSON-строки в объект
            T obj = JsonSerializer.Deserialize<T>(jsonString, options);

            Logger.LogInfo("Объект успешно десериализован.");
            return obj;
        }
        catch (Exception ex)
        {
            Logger.LogError($"Ошибка при десериализации JSON: {ex.Message}");
            throw;
        }
    }
    public static void SerializeToJson<T>(T obj)
    {
        try
        {
            // Настройка опций для форматированного вывода JSON
            var options = new JsonSerializerOptions
            {
                WriteIndented = true // Делает JSON читаемым для человека
            };

            // Сериализация объекта в строку JSON
            string jsonString = JsonSerializer.Serialize(obj, options);

            // Запись строки JSON в файл
            File.WriteAllText(filePath, jsonString);

            Logger.LogInfo($"Объект успешно записан в файл: {filePath}");
        }
        catch (Exception ex)
        {
            Logger.LogError($"Ошибка при записи JSON: {ex.Message}");
        }
    }
}