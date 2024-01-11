using System;

namespace Reservation.API.Services.Interfaces
{
    internal interface IIdentityService
    {
        Guid GetUserIdentity();
    }
}
