namespace Dynasty.Carrington.Blake.Infrastructure.Persistence.Configurations;

internal static class ColumnLimits
{
    public const int Name = 200;
    public const int Comment = 2000;
    public const int Number = 50;
    public const int Enum = 20;
    public const int Currency = 3;
    public const string Money = "decimal(18,2)";
}
