using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain;

namespace Service
{
    public interface IUserService
    {
        public Task<List<User>> GetUsers();
        public Task<User> GetUserById(Guid id);
        public Task AddUser(User user);
        public Task UpdateUserById(Guid id, User updatedUser);
        public Task DeleteUserById(Guid id);
    }

    public class UserService : IUserService
    {
        private readonly WebshopDBContext _context;

        public UserService(WebshopDBContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetUserById(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task AddUser(User user)
        {
            user.UserType = Domain.enums.UserType.User;
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserById(Guid id, User updatedUser)
        {
            updatedUser.Id = id;
            _context.Users.Update(updatedUser);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserById(Guid id)
        {
            _context.Users.Remove(new User { Id = id });
            await _context.SaveChangesAsync();
        }
    }
}
