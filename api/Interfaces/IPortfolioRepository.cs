using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Portfolio;
using api.Models;

namespace api.Interfaces
{
    public interface IPortfolioRepository
    {
        Task<List<PortfolioDto>> GetAllUserPortfolio(AppUser user);
    }
}