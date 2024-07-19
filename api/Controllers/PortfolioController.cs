using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/portfolio")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IStockRepository _stockRepo;

        private readonly IPortfolioRepository _portfolioRepo;

        //private readonly Portfolio
        public PortfolioController(UserManager<AppUser> userManager
        , IStockRepository stockRepo, IPortfolioRepository portfolioRepo)
        {
            _stockRepo = stockRepo;
            _userManager = userManager;
            _portfolioRepo = portfolioRepo;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserPortfolio()
        {
             var userId = User.FindFirstValue(ClaimTypes.GivenName);

             if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated or userId claim not found.");
            }

            var appUser = await _userManager.FindByNameAsync(userId);

            var userPortfolio = await _portfolioRepo.GetAllUserPortfolio(appUser);

            return Ok(userPortfolio);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPortfolio(string symbol)
        {
            var username = User.FindFirstValue(ClaimTypes.GivenName);
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("User not authenticated or userId claim not found.");
            }

            var appUser = await _userManager.FindByNameAsync(username);

            var stock = await _stockRepo.GetBySymbolAsync(symbol);
            if(stock == null) return BadRequest("Stock not found");

            var userPortfolio = await _portfolioRepo.GetAllUserPortfolio(appUser);

            if(userPortfolio.Any(e => e.Symbol.ToLower() == symbol.ToLower())) return BadRequest("Cannot add same stock to portfolio");


            var portfolioModel = new Portfolio
            {
                StockId = stock.Id,
                AppUserId = appUser.Id
            };

            var portfolio = await _portfolioRepo.CreateAsync(portfolioModel);

            if(portfolio == null)
            {
                return StatusCode(500, "Could not create");
            }
            else
            {
                return Created();
            }
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeletePortfolio(string symbol)
        {
            var username = User.FindFirstValue(ClaimTypes.GivenName);
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("User not authenticated or userId claim not found.");
            }

            var appUser = await _userManager.FindByNameAsync(username);

            var userPortfolio = await _portfolioRepo.GetAllUserPortfolio(appUser);

            var filteredStock = userPortfolio.Where(s => s.Symbol.ToLower() == symbol.ToLower());

            if(filteredStock.Count() == 1)
            {
                await _portfolioRepo.DeletePortfolio(appUser, symbol);
            }
            else
            {
                return BadRequest("Stock is not in your Portfolio");
            }

            return Ok();

        }
    }
}
