using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Portfolio;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
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

        public async Task<Portfolio> CreateAsync(Portfolio portfolio)
        {
            await _context.Portfolios.AddAsync(portfolio);
            await _context.SaveChangesAsync();
            return portfolio;
        }

        public async Task<Portfolio> DeletePortfolio(AppUser appUser, string symbol)
        {
            var portfolioModel = await _context.Portfolios.FirstOrDefaultAsync(x => x.AppUserId == appUser.Id && x.Stock.Symbol.ToLower() == symbol.ToLower());
            if(portfolioModel == null)
            {
                return null;
            }

            _context.Portfolios.Remove(portfolioModel);
            await _context.SaveChangesAsync();

            return portfolioModel;
        }

        public async  Task<List<PortfolioDto>> GetAllUserPortfolio(AppUser user)
        {
            return await _context.Portfolios.Where(p => p.AppUserId == user.Id)
            .Include(p => p.Stock)
            .Select(p => new PortfolioDto
            {
                Id = p.StockId,
                Symbol = p.Stock.Symbol,
                CompanyName = p.Stock.CompanyName,
                Purchase = p.Stock.Purchase,
                LastDiv = p.Stock.LastDiv,
                Industry = p.Stock.Industry,
                MarketCap = p.Stock.MarketCap
            }).ToListAsync();
        }
    }
}