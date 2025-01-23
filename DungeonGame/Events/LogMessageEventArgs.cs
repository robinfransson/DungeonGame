using Microsoft.Extensions.Logging;

namespace DungeonGame.Events;

public struct LogMessageEventArgs
{
    public FormattableString Message { get; }
    public LogLevel Level { get; }

    public string Format => Message.Format;
    public object?[] Arguments => Message.GetArguments();

    public LogMessageEventArgs(FormattableString message, LogLevel level)
    {
        Message = message;
        Level = level;
    }
}