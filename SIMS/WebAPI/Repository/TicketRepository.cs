using WebAPI.Models;

namespace WebAPI.Repository
{
    public class TicketRepository : ARepository<Ticket>
    {
        public TicketRepository(SIMSContext context) : base(context)
        {
        }
    }
}
