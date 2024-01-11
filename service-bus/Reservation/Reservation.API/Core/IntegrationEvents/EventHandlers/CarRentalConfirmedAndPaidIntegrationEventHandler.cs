using Reservation.Domain.Repositories.Interfaces;
using EventBus.Events.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Reservation.API.Core.IntegrationEvents.Events
{
    public class CarRentalConfirmedAndPaidIntegrationEventHandler : IIntegraionEventHandler<CarRentalConfirmedAndPaidIntegrationEvent>
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly ILogger<CarRentalConfirmedAndPaidIntegrationEventHandler> _logger;

        public CarRentalConfirmedAndPaidIntegrationEventHandler(
            IReservationRepository reservationRepository,
            ILogger<CarRentalConfirmedAndPaidIntegrationEventHandler> logger)
        {
            _reservationRepository = reservationRepository ?? throw new ArgumentNullException(nameof(reservationRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task HandleAsync(CarRentalConfirmedAndPaidIntegrationEvent @event)
        {
            _logger.LogInformation("Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", @event.Id, @event);
            await _reservationRepository.DeleteReservationAsync(@event.UserId.ToString());
        }
    }
}
