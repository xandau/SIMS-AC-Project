using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs;
using WebAPI.Models;

namespace WebAPI.Repository
{
    public interface ITicketRepository : IRepository<Ticket>
    {
        public Task<List<Ticket>> GetAssignedTickets(string access_token);

        public Task<List<Ticket>> GetCreatedTickets(string access_token);

        public Task<bool> StopInstance(string access_token, StopInstance request);
    }
}
