using System;
namespace Mise.Core.Client.Entities
{
    public interface IMiseRestaurantClientEntity : IMiseClientEntity
    {
        string RestaurantId { get; }
    }
}

