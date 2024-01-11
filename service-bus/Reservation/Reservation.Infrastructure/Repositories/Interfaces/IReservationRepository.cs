using Reservation.Domain.Model;

namespace Reservation.Domain.Repositories.Interfaces
{
    public interface IReservationRepository
    {
        Task<CustomerReservation> GetReservationAsync(string customerId);
        Task<CustomerReservation> UpdateReservationAsync(CustomerReservation reservation);
        Task<bool> DeleteReservationAsync(string id);
        IEnumerable<string> GetUsers();
    }
}
