using EventBus.Events;
using System;

namespace Reservation.API.Core.IntegrationEvents.Events
{
    public class CarRentalConfirmedAndPaidIntegrationEvent : IntegrationEvent
    {
        public Guid UserId { get; set; }

        public CarRentalConfirmedAndPaidIntegrationEvent(Guid userId)
            => UserId = userId;
    }
}
