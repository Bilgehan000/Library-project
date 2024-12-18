using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kütüphane_yönetim_1
{
    internal class Kullanıcı
    {
        KutuphaneDbContext context = new KutuphaneDbContext();

        public async Task<List<User>> GetUsersAsync(User user)
        {
            return await context.Users.ToListAsync();
        }

        public async Task AddUsers(User user)
        {
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        }

        public async Task RemoveUser(User user)
        {
            var usersil = await context.Users
                .FirstOrDefaultAsync(m=>m.Id == user.Id);
            if (usersil != null)
            {
                context.Users.Remove(usersil);
                await context.SaveChangesAsync();
            }
        }

        public async Task Update(User user)
        {
            var userGuncel = await context.Users
                .FirstOrDefaultAsync(m => m.Id == user.Id);

            if (userGuncel != null)
            {
                userGuncel.Name = user.Name;
                userGuncel.Email = user.Email;
                userGuncel.Password = user.Password;
            }
            else
            {
                MessageBox.Show("Manager Bulunamadı");
            }

        }

        public async Task<bool> Login(string name, string email, string password)
        {
            var user = await context.Users
                                         .FirstOrDefaultAsync(m => m.Name == name && m.Email == email && m.Password == password);
            return user != null;
        }

        public async Task<int> GetUserId(string email)
        {
                var User = await context.Users
                                        .FirstOrDefaultAsync(m => m.Email == email);
                return User?.Id ?? 0;
        }

        public async Task<User> GetUserById(int userId)
        {
            var user = await context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            return user;
        }

    }

}

