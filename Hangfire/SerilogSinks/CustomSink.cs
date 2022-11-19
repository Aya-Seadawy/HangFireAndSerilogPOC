using Serilog.Core;
using Serilog.Events;

namespace HangfirProject.SerilogSinks;

public class CustomSink: ILogEventSink
{
    public void Emit(LogEvent logEvent)
    {
        var result = logEvent.RenderMessage();

        Console.ForegroundColor = logEvent.Level switch
        {
            LogEventLevel.Debug => ConsoleColor.Green,
            LogEventLevel.Information => ConsoleColor.Magenta,
            LogEventLevel.Error =>ConsoleColor.Red,
            LogEventLevel.Warning => ConsoleColor.Yellow,
            LogEventLevel.Fatal => ConsoleColor.DarkRed,
            LogEventLevel.Verbose => ConsoleColor.Blue,
            _ => ConsoleColor.White,
        };
        Console.WriteLine($"{logEvent.Timestamp} - {logEvent.Level}: {result}"); 
    }
}
