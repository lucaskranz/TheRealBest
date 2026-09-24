namespace TheRealBest.Infrastructure.Data.Configurations;

internal static class ConfigurationConstants
{
    public const int ExternalIdMaxLength = 50;
    public const int NameMaxLength = 150;
    public const int ShortNameMaxLength = 50;
    public const int RoundPhaseMaxLength = 100;
    public const int CountryMaxLength = 100;
    public const int UrlMaxLength = 500;
    public const int DescriptionMaxLength = 500;
    public const int TranslationMaxLength = 255;
    public const int EnumMaxLength = 30;
    public const int LocaleMaxLength = 5;

    // Pontuações e multiplicadores (ex.: 87.4567, 1.2500)
    public const int ScorePrecision = 10;
    public const int ScoreScale = 4;

    // Somatórios da temporada (ex.: FSS com dezenas de partidas acumuladas)
    public const int AggregatePrecision = 14;

    // Métricas estatísticas (ex.: xG 0.87, precisão de passe 91.50)
    public const int StatPrecision = 6;
    public const int StatScale = 2;
}
