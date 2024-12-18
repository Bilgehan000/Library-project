using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kütüphane_yönetim_1
{
        public class KullanıcıBilgisi
        {

            public KullanıcıBilgisi(string name, string email, string password)
            {
                Name = name;
                Email = email;
                Password = password;
            }

            public string Name { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }
}
