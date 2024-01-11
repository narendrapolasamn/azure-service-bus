using EventBus.Events;
using System;

namespace Reservation.API.Core.IntegrationEvents.Events
{
    public class CarPricePerDayChangedIntegrationEvent : IntegrationEvent
    {
        public Guid CarId { get; private set; }

        public decimal NewPricePerDay { get; private set; }

        public decimal OldPricePerDay { get; private set; }

        public CarPricePerDayChangedIntegrationEvent(Guid carId, decimal newPricePerDay, decimal oldPricePerDay)
        {
            CarId = carId;
            NewPricePerDay = newPricePerDay;
            OldPricePerDay = oldPricePerDay;
        }
    }
}
