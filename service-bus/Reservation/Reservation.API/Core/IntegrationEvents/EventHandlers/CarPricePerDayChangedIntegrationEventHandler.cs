using EventBus.Events;
using EventBus.Events.Interface;
using Reservation.Domain.Model;
using Reservation.Domain.Repositories.Interfaces;
using Reservation.API.Core.IntegrationEvents.Events;

namespace Reservation.API.Core.IntegrationEvents.EventHandlers
{
    public class CarPricePerDayChangedIntegrationEventHandler : IIntegraionEventHandler<CarPricePerDayChangedIntegrationEvent>
    {
        private readonly ILogger<CarPricePerDayChangedIntegrationEventHandler> _logger;
        private readonly IReservationRepository _reservationRepository;

        public CarPricePerDayChangedIntegrationEventHandler(ILogger<CarPricePerDayChangedIntegrationEventHandler> logger,
                                                            IReservationRepository reservationRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _reservationRepository = reservationRepository ?? throw new ArgumentNullException(nameof(reservationRepository));
        }

        public async Task HandleAsync(CarPricePerDayChangedIntegrationEvent @event)
        {
            _logger.LogInformation("Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", @event.Id, @event);

            var userIds = _reservationRepository.GetUsers();

            foreach (var id in userIds)
            {
                var customerReservation = await _reservationRepository.GetReservationAsync("Narendra");

                await UpdatePriceInCustomerReservation(@event.CarId, @event.NewPricePerDay, @event.OldPricePerDay, customerReservation);
            }
        }

       private async Task UpdatePriceInCustomerReservation(Guid carId, decimal newPrice,
                                                            decimal oldPrice, CustomerReservation reservation)
        {
            if (carId == reservation.Car.Id)
            {
                _logger.LogInformation($"{nameof(CarPricePerDayChangedIntegrationEventHandler)} - Updating car price in reservation for the customer: {reservation.CustomerId}", reservation.CustomerId);

                if (reservation.Car.PricePerDay == oldPrice)
                {
                    reservation.Car.PricePerDay = newPrice;
                }
                await _reservationRepository.UpdateReservationAsync(reservation);
            }
        }
    }
}
