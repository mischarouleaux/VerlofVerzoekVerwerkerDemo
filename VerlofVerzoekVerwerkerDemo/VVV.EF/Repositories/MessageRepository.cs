using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVV.Interfaces.Repositories;
using VVV.Models;

namespace VVV.EF.Repositories
{
    public class MessageRepository : BaseRepository<Message>, IMessageRepository
    {
        public MessageRepository(VerlofVerzoekVerwerkerContext context) : base(context)
        {

        }
    }
}
