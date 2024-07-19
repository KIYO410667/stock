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
    }
}