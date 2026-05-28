using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository.Interface;

public interface IComplaintTicketRepository
{
    Task<IEnumerable<ComplaintTicket>> GetAsync();
    Task<ComplaintTicket?> GetByIdAsync(int complaintTicketId);

    Task<int> AddAsync(ComplaintTicket complaintTicket);

    Task UpdateAsync(ComplaintTicket complaintTicket);

    Task UpdateStatusAsync(int complaintTicketId, ComplaintTicketStatusEnum newStatus);

    Task UpdateUrgencyAsync(int complaintTicketId, ComplaintTicketUrgencyLevelEnum newUrgencyLevel);

    Task DeleteAsync(int complaintTicketId);
}