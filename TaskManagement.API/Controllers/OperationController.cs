using Microsoft.AspNetCore.Mvc;
using TaskManagement.Core.Entities;
using TaskManagement.Infrastructure.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Core.Repositories;
using TaskManagement.Core.Helpers;
using TaskManagement.Core.DTOs.Task;
using TaskManagement.Core.DTOs.Project;
using TaskManagement.Infrastructure.Repositories;

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

 
        //[HttpPost("create")]
        //public async Task<IActionResult> Create()
        //{
        //  var task  = new TaskEntity { Id = Guid.NewGuid(),
        //      Title = "task2x",
        //      Description = "Description2x",
        //      Status = Task_Status.NotStarted,
        //      Priority = TaskPriority.Medium,
        //      ProjectId = new Guid("B0E0F55B-DEBE-4D57-8EF2-6032E916C227"),
        //      StartDate  = DateTime.Now,
        //      EndDate = DateTime.Now.AddDays(1)
        //  };   

        //    _context.Add(task);
        //    await _context.SaveChangesAsync();
        //    return Ok(task.Id);
        //}



    }
}
