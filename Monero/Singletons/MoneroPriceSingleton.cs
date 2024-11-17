using System.Collections.Concurrent;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Monero.Singletons;

public class MoneroPriceSingleton
{
    private readonly string _currentDirectory;
    private readonly string _currencies = "usd,aed,ars,aud,bdt,bhd,bmd,brl,cad,chf,clp,cny,czk,dkk,eur,gbp,gel,hkd,huf,idr,ils,inr,jpy,krw,kwd,lkr,mmk,mxn,myr,ngn,nok,nzd,php,pkr,pln,rub,sar,sek,sgd,thb,try,twd,uah,vef,vnd,zar";

    public ConcurrentDictionary<string, decimal> Prices { get; private set; } = [];

    public bool IsInitialized { get; private set; }

    public MoneroPriceSingleton()
    {
        #region Currencies
        Prices.TryAdd("USD", decimal.MaxValue);
        Prices.TryAdd("AED", decimal.MaxValue);
        Prices.TryAdd("ARS", decimal.MaxValue);
        Prices.TryAdd("AUD", decimal.MaxValue);
        Prices.TryAdd("BDT", decimal.MaxValue);
        Prices.TryAdd("BHD", decimal.MaxValue);
        Prices.TryAdd("BMD", decimal.MaxValue);
        Prices.TryAdd("BRL", decimal.MaxValue);
        Prices.TryAdd("CAD", decimal.MaxValue);
        Prices.TryAdd("CHF", decimal.MaxValue);
        Prices.TryAdd("CLP", decimal.MaxValue);
        Prices.TryAdd("CNY", decimal.MaxValue);
        Prices.TryAdd("CZK", decimal.MaxValue);
        Prices.TryAdd("DKK", decimal.MaxValue);
        Prices.TryAdd("EUR", decimal.MaxValue);
        Prices.TryAdd("GBP", decimal.MaxValue);
        Prices.TryAdd("GEL", decimal.MaxValue);
        Prices.TryAdd("HKD", decimal.MaxValue);
        Prices.TryAdd("HUF", decimal.MaxValue);
        Prices.TryAdd("IDR", decimal.MaxValue);
        Prices.TryAdd("ILS", decimal.MaxValue);
        Prices.TryAdd("INR", decimal.MaxValue);
        Prices.TryAdd("JPY", decimal.MaxValue);
        Prices.TryAdd("KRW", decimal.MaxValue);
        Prices.TryAdd("KWD", decimal.MaxValue);
        Prices.TryAdd("LKR", decimal.MaxValue);
        Prices.TryAdd("MMK", decimal.MaxValue);
        Prices.TryAdd("MXN", decimal.MaxValue);
        Prices.TryAdd("MYR", decimal.MaxValue);
        Prices.TryAdd("NGN", decimal.MaxValue);
        Prices.TryAdd("NOK", decimal.MaxValue);
        Prices.TryAdd("NZD", decimal.MaxValue);
        Prices.TryAdd("PHP", decimal.MaxValue);
        Prices.TryAdd("PKR", decimal.MaxValue);
        Prices.TryAdd("PLN", decimal.MaxValue);
        Prices.TryAdd("RUB", decimal.MaxValue);
        Prices.TryAdd("SAR", decimal.MaxValue);
        Prices.TryAdd("SEK", decimal.MaxValue);
        Prices.TryAdd("SGD", decimal.MaxValue);
        Prices.TryAdd("THB", decimal.MaxValue);
        Prices.TryAdd("TRY", decimal.MaxValue);
        Prices.TryAdd("TWD", decimal.MaxValue);
        Prices.TryAdd("UAH", decimal.MaxValue);
        Prices.TryAdd("VEF", decimal.MaxValue);
        Prices.TryAdd("VND", decimal.MaxValue);
        Prices.TryAdd("ZAR", decimal.MaxValue);
        #endregion

        _currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        if (string.IsNullOrEmpty(_currentDirectory))
            throw new Exception("Current directory was null or empty.");

        Task.Run(GetXMRPrice);

        while (!IsInitialized);
    }

    class PriceResponse
    {
        [JsonPropertyName("monero")]
        public Coin? Monero { get; set; }

        public class Coin
        {
            [JsonPropertyName("usd")]
            public decimal USD { get; set; }

            [JsonPropertyName("aed")]
            public decimal AED { get; set; }

            [JsonPropertyName("ars")]
            public decimal ARS { get; set; }

            [JsonPropertyName("aud")]
            public decimal AUD { get; set; }

            [JsonPropertyName("bdt")]
            public decimal BDT { get; set; }

            [JsonPropertyName("bhd")]
            public decimal BHD { get; set; }

            [JsonPropertyName("bmd")]
            public decimal BMD { get; set; }

            [JsonPropertyName("brl")]
            public decimal BRL { get; set; }

            [JsonPropertyName("cad")]
            public decimal CAD { get; set; }

            [JsonPropertyName("chf")]
            public decimal CHF { get; set; }

            [JsonPropertyName("clp")]
            public decimal CLP { get; set; }

            [JsonPropertyName("cny")]
            public decimal CNY { get; set; }

            [JsonPropertyName("czk")]
            public decimal CZK { get; set; }

            [JsonPropertyName("dkk")]
            public decimal DKK { get; set; }

            [JsonPropertyName("eur")]
            public decimal EUR { get; set; }

            [JsonPropertyName("gbp")]
            public decimal GBP { get; set; }

            [JsonPropertyName("gel")]
            public decimal GEL { get; set; }

            [JsonPropertyName("hkd")]
            public decimal HKD { get; set; }

            [JsonPropertyName("huf")]
            public decimal HUF { get; set; }

            [JsonPropertyName("idr")]
            public decimal IDR { get; set; }

            [JsonPropertyName("ils")]
            public decimal ILS { get; set; }

            [JsonPropertyName("inr")]
            public decimal INR { get; set; }

            [JsonPropertyName("jpy")]
            public decimal JPY { get; set; }

            [JsonPropertyName("krw")]
            public decimal KRW { get; set; }

            [JsonPropertyName("kwd")]
            public decimal KWD { get; set; }

            [JsonPropertyName("lkr")]
            public decimal LKR { get; set; }

            [JsonPropertyName("mmk")]
            public decimal MMK { get; set; }

            [JsonPropertyName("mxn")]
            public decimal MXN { get; set; }

            [JsonPropertyName("myr")]
            public decimal MYR { get; set; }

            [JsonPropertyName("ngn")]
            public decimal NGN { get; set; }

            [JsonPropertyName("nok")]
            public decimal NOK { get; set; }

            [JsonPropertyName("nzd")]
            public decimal NZD { get; set; }

            [JsonPropertyName("php")]
            public decimal PHP { get; set; }

            [JsonPropertyName("pkr")]
            public decimal PKR { get; set; }

            [JsonPropertyName("pln")]
            public decimal PLN { get; set; }

            [JsonPropertyName("rub")]
            public decimal RUB { get; set; }

            [JsonPropertyName("sar")]
            public decimal SAR { get; set; }

            [JsonPropertyName("sek")]
            public decimal SEK { get; set; }

            [JsonPropertyName("sgd")]
            public decimal SGD { get; set; }

            [JsonPropertyName("thb")]
            public decimal THB { get; set; }

            [JsonPropertyName("try")]
            public decimal TRY { get; set; }

            [JsonPropertyName("twd")]
            public decimal TWD { get; set; }

            [JsonPropertyName("uah")]
            public decimal UAH { get; set; }

            [JsonPropertyName("vef")]
            public decimal VEF { get; set; }

            [JsonPropertyName("vnd")]
            public decimal VND { get; set; }

            [JsonPropertyName("zar")]
            public decimal ZAR { get; set; }
        }
    }

    private async Task GetXMRPrice()
    {
        var httpClient = new HttpClient();

        httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36 OPR/45.0.2552.882");

        while (true)
        {
            try
            {
                var response = await httpClient.GetFromJsonAsync<PriceResponse>($"https://api.coingecko.com/api/v3/simple/price?ids=monero&vs_currencies={_currencies}&precision=18");

                if (response is not null && response.Monero is not null)
                {
                    Prices["USD"] = Math.Round((1m / response.Monero.USD), 12);
                    Prices["AED"] = Math.Round((1m / response.Monero.AED), 12);
                    Prices["ARS"] = Math.Round((1m / response.Monero.ARS), 12);
                    Prices["AUD"] = Math.Round((1m / response.Monero.AUD), 12);
                    Prices["BDT"] = Math.Round((1m / response.Monero.BDT), 12);
                    Prices["BHD"] = Math.Round((1m / response.Monero.BHD), 12);
                    Prices["BMD"] = Math.Round((1m / response.Monero.BMD), 12);
                    Prices["BRL"] = Math.Round((1m / response.Monero.BRL), 12);
                    Prices["CAD"] = Math.Round((1m / response.Monero.CAD), 12);
                    Prices["CHF"] = Math.Round((1m / response.Monero.CHF), 12);
                    Prices["CLP"] = Math.Round((1m / response.Monero.CLP), 12);
                    Prices["CNY"] = Math.Round((1m / response.Monero.CNY), 12);
                    Prices["CZK"] = Math.Round((1m / response.Monero.CZK), 12);
                    Prices["DKK"] = Math.Round((1m / response.Monero.DKK), 12);
                    Prices["EUR"] = Math.Round((1m / response.Monero.EUR), 12);
                    Prices["GBP"] = Math.Round((1m / response.Monero.GBP), 12);
                    Prices["GEL"] = Math.Round((1m / response.Monero.GEL), 12);
                    Prices["HKD"] = Math.Round((1m / response.Monero.HKD), 12);
                    Prices["HUF"] = Math.Round((1m / response.Monero.HUF), 12);
                    Prices["IDR"] = Math.Round((1m / response.Monero.IDR), 12);
                    Prices["ILS"] = Math.Round((1m / response.Monero.ILS), 12);
                    Prices["INR"] = Math.Round((1m / response.Monero.INR), 12);
                    Prices["JPY"] = Math.Round((1m / response.Monero.JPY), 12);
                    Prices["KRW"] = Math.Round((1m / response.Monero.KRW), 12);
                    Prices["KWD"] = Math.Round((1m / response.Monero.KWD), 12);
                    Prices["LKR"] = Math.Round((1m / response.Monero.LKR), 12);
                    Prices["MMK"] = Math.Round((1m / response.Monero.MMK), 12);
                    Prices["MXN"] = Math.Round((1m / response.Monero.MXN), 12);
                    Prices["MYR"] = Math.Round((1m / response.Monero.MYR), 12);
                    Prices["NGN"] = Math.Round((1m / response.Monero.NGN), 12);
                    Prices["NOK"] = Math.Round((1m / response.Monero.NOK), 12);
                    Prices["NZD"] = Math.Round((1m / response.Monero.NZD), 12);
                    Prices["PHP"] = Math.Round((1m / response.Monero.PHP), 12);
                    Prices["PKR"] = Math.Round((1m / response.Monero.PKR), 12);
                    Prices["PLN"] = Math.Round((1m / response.Monero.PLN), 12);
                    Prices["RUB"] = Math.Round((1m / response.Monero.RUB), 12);
                    Prices["SAR"] = Math.Round((1m / response.Monero.SAR), 12);
                    Prices["SEK"] = Math.Round((1m / response.Monero.SEK), 12);
                    Prices["SGD"] = Math.Round((1m / response.Monero.SGD), 12);
                    Prices["THB"] = Math.Round((1m / response.Monero.THB), 12);
                    Prices["TRY"] = Math.Round((1m / response.Monero.TRY), 12);
                    Prices["TWD"] = Math.Round((1m / response.Monero.TWD), 12);
                    Prices["UAH"] = Math.Round((1m / response.Monero.UAH), 12);
                    Prices["VEF"] = Math.Round((1m / response.Monero.VEF), 12);
                    Prices["VND"] = Math.Round((1m / response.Monero.VND), 12);
                    Prices["ZAR"] = Math.Round((1m / response.Monero.ZAR), 12);

                    IsInitialized = true;
                }
                else
                {

                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e);
                await Task.Delay(10_000);
                continue;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            await Task.Delay(600_000);
        }
    }
}
