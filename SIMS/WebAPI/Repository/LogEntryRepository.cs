using WebAPI.Models;

namespace WebAPI.Repository
{
    public class LogEntryRepository : ARepository<LogEntry>
    {
        public LogEntryRepository(SIMSContext context) : base(context)
        {
        }
    }
}
