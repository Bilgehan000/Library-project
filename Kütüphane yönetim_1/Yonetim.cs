using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kütüphane_yönetim_1
{
    internal class Yonetim
    {
        KutuphaneDbContext context = new KutuphaneDbContext();

        public async Task<List<Manager>> GetManagersAsync(Manager manager)
        {
            return await context.Managers.ToListAsync();
        }

        public async Task AddManager(Manager manager)
        {
            await context.Managers.AddAsync(manager);
            await context.SaveChangesAsync();
        }

        public async Task Remove(Manager manager)
        {
            var managersil = await context.Managers
                .FirstOrDefaultAsync(m => m.Id == manager.Id);

            if (managersil != null)
            {
                context.Managers.Remove(managersil);
                await context.SaveChangesAsync();
            }
        }
        
        public async Task Update(Manager manager)
        {
            var ManagerGuncel = await context.Managers
                .FirstOrDefaultAsync(m=>m.Id == manager.Id);

            if (ManagerGuncel != null)
            {
                ManagerGuncel.Name = manager.Name;
                ManagerGuncel.Email = manager.Email;
                ManagerGuncel.Password = manager.Password;
            }
            else
            {
                MessageBox.Show("Manager Bulunamadı");
            }

        }

        public async Task<bool> Login(string name, string email,string password)
        {
            var Manager = await context.Managers
                                         .FirstOrDefaultAsync(m => m.Name == name && m.Email == email && m.Password == password);
            return Manager != null;
        }
        
    }
}
