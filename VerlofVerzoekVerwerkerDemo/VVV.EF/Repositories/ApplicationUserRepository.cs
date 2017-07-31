using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVV.Interfaces.Repositories;
using VVV.Models;

namespace VVV.EF.Repositories
{
    public class ApplicationUserRepository : BaseRepository<ApplicationUser>, IApplicationUserRepository
    {
        public ApplicationUserRepository(VerlofVerzoekVerwerkerContext context) : base(context)
        {

        }
    }

}
