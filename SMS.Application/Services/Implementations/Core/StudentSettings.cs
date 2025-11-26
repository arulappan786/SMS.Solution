namespace SMS.Application.Services.Implementations.Core
{
    public record StudentSettings
    {
        public required string CodePrefix { get; init; }
        public required int CodeLength { get; init; }

    }
}