using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Stock;
using api.Models;

namespace api.Mappers
{
    public static class StockMappers
    {
        public static StockDto ToStockDto(this Stock stockModel)
        {
            return new StockDto
            {
                Id = stockModel.Id,
                Symbol = stockModel.Symbol,
                CompanyName = stockModel.CompanyName,
                Purchase = stockModel.Purchase,
                LastDiv = stockModel.LastDiv,
                Industry = stockModel.Industry,
                MarketCap = stockModel.MarketCap,
                Comments = stockModel.Comments.Select(c => c.ToCommentDto()).ToList()
            };
        }

        public static Stock ToStockFromCreateDto(this CreateStockRequestDto stockDto)
        { //allow you to transform CreateStockRequestDto to Stock
            return new Stock
            {
                Symbol = stockDto.Symbol,
                CompanyName = stockDto.CompanyName,
                Purchase = stockDto.Purchase,
                LastDiv = stockDto.LastDiv,
                Industry = stockDto.Industry,
                MarketCap = stockDto.MarketCap
            };
        }

        public static Stock ToStockFromFMPStockDto(this FMPStock fmpstock)
        { //allow you to transform CreateStockRequestDto to Stock
            return new Stock
            {
                Symbol = fmpstock.symbol,
                CompanyName = fmpstock.companyName,
                Purchase = (decimal)fmpstock.price,
                LastDiv = fmpstock.lastDiv,
                Industry = fmpstock.industry,
                MarketCap = fmpstock.mktCap
            };
        }
    }
}