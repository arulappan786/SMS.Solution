namespace SMS.Application.Services.Implements.Core
{
    public record StudentSettings
    {
        public required string CodePrefix { get; init; }
        public required int CodeLength { get; init; }

    }
}