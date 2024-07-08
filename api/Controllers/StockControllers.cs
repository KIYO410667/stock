using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/stock")]
    [ApiController]
    public class StockControllers : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        //Bring in database by using Dbcontext as a parameter
        public StockControllers(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet] //Read
        public IActionResult GetAll()
        {
            var stocks = _context.Stock.ToList();

            //Stocks would take in DBcontext and tolist is deferred execution
            return Ok(stocks);
        }
        
        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var stock = _context.Stock.Find(id);

            if(stock == null)
            {
                return NotFound();
            }

            return Ok(stock);
        }
    }
}