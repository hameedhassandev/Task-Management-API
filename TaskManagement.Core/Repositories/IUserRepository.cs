﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.DTOs.Users;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Helpers;

namespace TaskManagement.Core.Repositories
{
    public interface IUserRepository
    {
        Task<Result<Guid>> AddUserAsync(AddUserDto dto);
        Task<Result<Guid>> UpdateUserAsync(UpdateUserInfoDto dto);
        Task<Result<BlockUserDto>> BlockUserAsync(BlockUserDto dto);
        Task<Result<bool>> IsEmailExist(string email);

    }
}