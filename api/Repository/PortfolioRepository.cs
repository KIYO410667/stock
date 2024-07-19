using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Portfolio;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly ApplicationDBContext _context;
        public PortfolioRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async  Task<List<Stock>> GetAllUserPortfolio(AppUser user)
        {
            return await _context.Portfolios.Where(u => u.AppUserId == user.Id)
            .Select(s => new Stock
            {
                Id = s.StockId,
                Symbol = s.Stock.Symbol,
                CompanyName = s.Stock.CompanyName,
                Purchase = s.Stock.Purchase,
                LastDiv = s.Stock.LastDiv,
                Industry = s.Stock.Industry,
                MarketCap = s.Stock.MarketCap
            }).ToListAsync();
        }
    }
}