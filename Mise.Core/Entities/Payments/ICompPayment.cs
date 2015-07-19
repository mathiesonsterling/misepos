using System;

using Mise.Core.Entities.People;
namespace Mise.Core.Entities.Payments
{
	public interface ICompPayment : IPayment
	{
		/// <summary>
		/// Reason given for the comp, if any
		/// </summary>
		/// <value>The reason.</value>
		string Reason{get;}
	}
}

