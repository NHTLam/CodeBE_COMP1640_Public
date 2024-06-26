﻿using CodeBE_COMP1640.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CodeBE_COMP1640.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> List();
        Task<User> Get(long Id);
        Task<bool> Create(User User);
        Task<bool> Update(User User);
        Task<bool> Delete(User User);
        Task<List<User>> GetUsersByDepartmentId(int departmentId,List<int> userIds);      
        Task<bool> UpdateCheckbox(int id, bool isChecked);     

    }

    public class UserRepository : IUserRepository
    {
        private DataContext DataContext;

        public UserRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        public async Task<List<User>> List()
        {
            List<User> Users = await DataContext.Users.AsNoTracking().ToListAsync();

            List<RoleUserMapping> RoleUserMappings = await DataContext.RoleUserMappings.AsNoTracking().ToListAsync();

            foreach (User User in Users)
            {
                User.RoleUserMappings = RoleUserMappings
                    .Where(x => x.UserId == User.UserId)
                    .ToList();
            }

            return Users;
        }

        public async Task<User> Get(long Id)
        {
            User? User = await DataContext.Users.AsNoTracking()
            .Where(x => x.UserId == Id).FirstOrDefaultAsync();

            if (User == null)
                return null;
            User.RoleUserMappings = await DataContext.RoleUserMappings.AsNoTracking().Where(x => x.UserId == User.UserId).ToListAsync();

            return User;
        }

        public async Task<bool> Create(User User)
        {
            DataContext.Users.Add(User);
            await DataContext.SaveChangesAsync();
            User.UserId = User.UserId;
            await SaveReference(User);
            return true;
        }

        public async Task<bool> Update(User User)
        {
            User? NewUser = DataContext.Users
                .Where(x => x.UserId == User.UserId)
                .FirstOrDefault();
            if (NewUser == null)
                return false;
            NewUser.UserId = User.UserId;
            NewUser.Username = User.Username;
            NewUser.Password = User.Password;
            NewUser.Email = User.Email;
            NewUser.Class = User.Class;
            NewUser.Phone = User.Phone;
            NewUser.Address = User.Address;
            NewUser.DepartmentId = User.DepartmentId;
            await DataContext.SaveChangesAsync();
            await SaveReference(User);
            return true;
        }

        public async Task<bool> Delete(User User)
        {
            await DataContext.RoleUserMappings
                    .Where(x => x.UserId == User.UserId)
                    .DeleteFromQueryAsync();
            User? CurrentUser = DataContext.Users
                .Where(x => x.UserId == User.UserId)
                .FirstOrDefault();
            if (User == null)
                return false;
            DataContext.Users.Remove(CurrentUser);
            await DataContext.SaveChangesAsync();
            return true;
        }

        private async Task SaveReference(User User)
        {
            if (User.RoleUserMappings == null || User.RoleUserMappings.Count == 0)
                await DataContext.RoleUserMappings
                    .Where(x => x.UserId == User.UserId)
                    .DeleteFromQueryAsync();
            else
            {
                List<RoleUserMapping> RoleUserMappings = new List<RoleUserMapping>();
                var PermissonUserMappingIds = User.RoleUserMappings.Select(x => x.Id).Distinct().ToList();
                await DataContext.RoleUserMappings
                    .Where(x => x.UserId == User.UserId)
                    .Where(x => !PermissonUserMappingIds.Contains(x.Id))
                    .DeleteFromQueryAsync();
                foreach (RoleUserMapping PermissonUserMapping in User.RoleUserMappings)
                {
                    RoleUserMapping NewPermissonUserMapping = new RoleUserMapping();
                    NewPermissonUserMapping.UserId = User.UserId;
                    NewPermissonUserMapping.RoleId = PermissonUserMapping.RoleId;
                    RoleUserMappings.Add(NewPermissonUserMapping);
                }
                await DataContext.AddRangeAsync(RoleUserMappings);
                await DataContext.SaveChangesAsync();
            }
        }
        public async Task<List<User>> GetUsersByDepartmentId(int departmentId,List<int> userIds)
    {
        return await DataContext.Users.Where(u => u.DepartmentId == departmentId || userIds.Contains(u.UserId)).ToListAsync();
    }
    public async Task<bool> UpdateCheckbox(int id, bool isChecked)
        {
            var user = await DataContext.Users.FindAsync(id);
            if (user == null)
                return false;

            user.Check = isChecked;
            await DataContext.SaveChangesAsync();
            return true;
        }
    }
}
