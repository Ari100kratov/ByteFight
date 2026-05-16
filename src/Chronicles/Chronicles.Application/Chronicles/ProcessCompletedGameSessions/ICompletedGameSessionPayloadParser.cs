using Chronicles.Application.Abstractions.Data;

namespace Chronicles.Application.Chronicles.ProcessCompletedGameSessions;

public interface ICompletedGameSessionPayloadParser
{
    CompletedGameSessionData Parse(string payload);
}
