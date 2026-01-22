namespace TestProject.CurrencyUpdater.Cbr;

using System.Globalization;
using System.Xml.Linq;

public static class CbrXmlParser
{
    public static IReadOnlyList<CbrCurrencyRate> Parse(string xml)
    {
        var doc = XDocument.Parse(xml);

        // В XML ЦБ обычно есть Valute -> CharCode / Value / Nominal.
        // Мы берём Name = CharCode (USD, EUR, …)
        // Rate нормализуем: Value / Nominal
        var list = new List<CbrCurrencyRate>();

        var valutes = doc.Root?.Elements("Valute") ?? Enumerable.Empty<XElement>();
        foreach (var v in valutes)
        {
            var charCode = v.Element("CharCode")?.Value?.Trim();
            var valueStr = v.Element("Value")?.Value?.Trim();
            var nominalStr = v.Element("Nominal")?.Value?.Trim();

            if (string.IsNullOrWhiteSpace(charCode) ||
                string.IsNullOrWhiteSpace(valueStr) ||
                string.IsNullOrWhiteSpace(nominalStr))
                continue;

            // ЦБ часто отдаёт число с запятой: "98,1234"
            if (!decimal.TryParse(valueStr, NumberStyles.Number, new CultureInfo("ru-RU"), out var value))
                continue;

            if (!int.TryParse(nominalStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out var nominal) || nominal <= 0)
                nominal = 1;

            var rate = value / nominal;

            list.Add(new CbrCurrencyRate(charCode, rate));
        }

        return list;
    }
}
