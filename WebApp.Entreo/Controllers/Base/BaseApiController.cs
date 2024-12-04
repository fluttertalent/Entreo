using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Entreo.Data;

namespace WebApp.Entreo.Controllers.Base
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        protected readonly ILogger<BaseApiController> _logger;
        protected readonly ApplicationDbContext _context;

        public BaseApiController(ILogger<BaseApiController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        protected async Task<bool> EntityExists<T>(int id) where T : class
        {
            return await _context.Set<T>().AnyAsync(e => EF.Property<int>(e, "Id") == id);
        }
    }
}