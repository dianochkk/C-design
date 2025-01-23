using System.Diagnostics;

namespace Logging
{
    public static class Logger
    {
        public static void ConfigureLogging()
        {
            // Указываем логирование в файл
            Trace.Listeners.Clear();
            Trace.Listeners.Add(new TextWriterTraceListener("./log.txt"));
            Trace.AutoFlush = true;
        }

        // Метод для записи информации в лог
        public static void LogInfo(string message)
        {
            Trace.WriteLine($"INFO: {message} [{DateTime.Now}]");
        }

        // Метод для записи ошибок в лог
        public static void LogError(string message)
        {
            Trace.WriteLine($"ERROR: {message} [{DateTime.Now}]");
        }

        // Метод для записи предупреждений в лог
        public static void LogWarning(string message)
        {
            Trace.WriteLine($"WARNING: {message} [{DateTime.Now}]");
        }
    }
}
