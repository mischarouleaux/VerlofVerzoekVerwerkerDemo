using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVV.Interfaces.Repositories;
using VVV.Models;

namespace VVV.EF.Repositories
{
    public class HolidayRepository : BaseRepository<Holiday>, IHolidayRepository
    {
        public HolidayRepository(VerlofVerzoekVerwerkerContext context) : base(context)
        {

        }
    }
}
