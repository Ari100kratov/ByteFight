using Application.Abstractions.Messaging;

namespace Application.Assets.GetAssetFile;

public sealed record GetAssetFileQuery(string Key) : IQuery<StreamResult>;
