namespace Bookify.Domain.Shared;


public record Currency
{
    private Currency(string code) => Code = code;

    public string Code { get; init; }

    public static readonly Currency Usd = new("USD");
    public static readonly Currency Eur = new("EUR");
    internal static readonly Currency None = new("");

    public static Currency FromCode(string code)
    {
        return code.ToUpper() switch
        {
            "USD" => Usd,
            "EUR" => Eur,
            _ => throw new ApplicationException("The currency code is invalid")
        };
    }

    public static readonly IReadOnlyCollection<Currency> All =
    [
        Usd,
        Eur
    ];
}



