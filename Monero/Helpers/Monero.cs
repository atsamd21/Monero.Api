namespace Monero.Helpers;

public static class Monero
{
    private const int _lowestBase = 12;

    public static decimal PiconeroToMonero(this ulong amount)
    {
        decimal piconero = amount;
        const decimal conversion = 1_000_000_000_000;
        return piconero / conversion;
    }

    public static ulong MoneroToPiconero(this decimal amount)
    {
        if (amount < decimal.Zero)
            throw new InvalidOperationException("Cannot have a negative amount of Monero");

        int decimalPlaces = GetDecimalPlaces(amount);

        if (decimalPlaces > _lowestBase)
            throw new InvalidOperationException($"{amount} has more than {_lowestBase} decimal places. " + $"{amount} can only have 12 decimal places at most.");

        amount *= (decimal)Math.Pow(10, _lowestBase);

        return checked((ulong)amount);
    }

    private static int GetDecimalPlaces(decimal number)
    {
        int decimalPlaces = 0;
        number = Math.Abs(number);
        number -= (int)number;
        while (number > 0)
        {
            decimalPlaces++;
            number *= 10;
            number -= (int)number;
        }
        return decimalPlaces;
    }
}
