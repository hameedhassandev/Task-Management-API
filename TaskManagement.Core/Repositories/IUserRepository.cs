using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.DTOs.Users.Repository;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Helpers;

namespace TaskManagement.Core.Repositories
{
    public interface IUserRepository
    {
        Task<Result<Guid>> AddUserAsync(AddUserDto dto);
        Task<Result<BlockUserDto>> BlockUserAsync(BlockUserDto dto);
        Task<Result<Nothing>> UnBlockUserAsync(Guid userId);
        Task<Result<bool>> IsEmailExist(string email);
        Task<Result<bool>> IsUserBlocked(Guid userId);


    }
}
