using System;
using System.Threading.Tasks;
namespace Mise.Inventory.Services
{
	public interface IFunFactService
	{
		Task<string> GetRandomFunFact();
	}
}

