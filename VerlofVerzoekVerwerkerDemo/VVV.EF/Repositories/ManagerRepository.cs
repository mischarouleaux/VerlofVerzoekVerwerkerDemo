using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVV.Interfaces.Repositories;
using VVV.Models;

namespace VVV.EF.Repositories
{
    public class ManagerRepository : BaseRepository<Manager>, IManagerRepository
    {
        public ManagerRepository(VerlofVerzoekVerwerkerContext context) : base(context)
        {

        }
    }
}
