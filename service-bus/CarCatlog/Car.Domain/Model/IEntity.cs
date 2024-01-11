using System;

namespace Car.Domain.Model
{
    public interface IEntity
    {
        public Guid Id { get; set; }
    }
}
