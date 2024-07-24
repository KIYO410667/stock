using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Stock;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Newtonsoft.Json;

namespace api.Service
{
    public class FMPService : IFMPService
    {
        private HttpClient _httpClient;
        private IConfiguration _config;
        public FMPService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<Stock?> FindStockBySymbolAsync(string symbol)
        {
            try
        {
            var result = await _httpClient.GetAsync($"https://financialmodelingprep.com/api/v3/profile/{symbol}?apikey={_config["FMPapikey"]}");

            if (result.IsSuccessStatusCode)
            {
                var jsonString = await result.Content.ReadAsStringAsync();
                var stocks = JsonConvert.DeserializeObject<FMPStock[]>(jsonString);

                if (stocks != null && stocks.Length > 0)
                {
                    var stock = stocks[0];
                    return stock.ToStockFromFMPStockDto();
                }

                Console.WriteLine($"No stocks found or deserialization returned null for symbol {symbol}. Response: {jsonString}");
                return null;
            }

            Console.WriteLine($"Request failed for symbol {symbol}. Status Code: {result.StatusCode}. Reason: {result.ReasonPhrase}");
            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception occurred while fetching stock data for symbol {symbol}: {e.Message}");
            return null;
        }
        }
    }
}