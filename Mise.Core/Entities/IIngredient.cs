using Mise.Core.Entities.Base;

namespace Mise.Core.Entities
{
    /// <summary>
    /// Represents ingredients for food
    /// </summary>
    public interface IIngredient : IRestaurantEntityBase
    {
		string Name{get;}
    }
}
