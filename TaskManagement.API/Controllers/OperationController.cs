using Microsoft.AspNetCore.Mvc;
using TaskManagement.Core.Entities;
using TaskManagement.Infrastructure.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Core.Repositories;
using TaskManagement.Core.Helpers;
using TaskManagement.Core.DTOs.Task;
using TaskManagement.Core.DTOs.Project;

namespace Task_Management_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OperationController : ControllerBase
    {
        private readonly AppDbContext _context;
        public OperationController(AppDbContext context )
        {
                _context = context;
        }

      

    }
}
