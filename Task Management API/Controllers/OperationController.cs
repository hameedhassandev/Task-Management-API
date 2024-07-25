using Microsoft.AspNetCore.Mvc;
using TaskManagement.Core.Entities;
using TaskManagement.Infrastructure.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Task_Management_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OperationController : ControllerBase
    {
        private readonly AppDbContext _context;
        public OperationController(AppDbContext context)
        {
                _context = context;
        }

        [HttpGet("SeedRoles")]
        public IActionResult SeedRoles()
        {
            var roles = new List<Role>
            {
                new Role{Id = Guid.NewGuid(), Name = "Admin"},
                new Role{Id = Guid.NewGuid(), Name = "User"}
            };
             _context.Roles.AddRange(roles);

            var result =  _context.SaveChanges();
            return Ok(result);
        }

        [HttpGet("GetRoles")]
        public IActionResult GetRoles() 
        {
            var roles = _context.Roles.AsNoTracking().ToList();
            return Ok(roles);
        }
    }
}
