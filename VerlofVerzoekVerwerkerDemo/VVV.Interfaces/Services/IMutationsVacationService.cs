using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVV.Models;

namespace VVV.Interfaces.Services
{
    public interface IMutationsVacationService
    {
        IQueryable<MutationsVacation> GetMutationsByUserID(long id);
        int GetMinutesVacationByID(long id);
        void AddMinutes(long userid, long currentuser, int minutes);
        void SubtractMinutes(long userid, long currentuser, int minutes);
        void SubtractMinutes(long userid, long currentuser, int minutes, VacationRequest vacreq);
        void Save(Models.MutationsVacation mutationsvacation);
    }
}
