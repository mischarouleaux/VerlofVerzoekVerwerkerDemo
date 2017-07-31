using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVV.Models;
using VVV.Interfaces.Repositories;

namespace VVV.EF.Repositories
{
    public class VacationRequestRepository : BaseRepository<VacationRequest>, IVacationRequestRepository
    {
        public VacationRequestRepository(VerlofVerzoekVerwerkerContext context) : base(context)
        {

        }
    }
}
