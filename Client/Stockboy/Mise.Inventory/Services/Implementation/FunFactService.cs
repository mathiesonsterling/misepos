using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Mise.Inventory.Services.Implementation
{
	public class FunFactService : IFunFactService
	{
		private readonly List<string> _hardcodedFunFacts;
		public FunFactService ()
		{
			_hardcodedFunFacts = new List<string>{
				"The full size pears inside some bottles of liquor are actually grown in there by hanging the bottles from the tree.",
				"In professional shooting, alcohol is actually considered to be a performance enhancing drug because shooters can drink it to relax themselves and slow their heart rate to give them an edge.",
				"In Russia many doctors “treat” alcoholism by surgically implanting a small capsule into their patients. The capsules react so severely with alcohol that when the patient touches a single drop of alcohol, they instantly acquire an excruciating illness of similar intensity to acute heroin withdrawal.",
				"A Russian chimp had to go to rehab because he had developed an alcohol addiction after too many visitors had given him alcoholic 'treats'.",
				"Until 2011, in Russia, anything under 10% alcohol was considered foodstuff and not alcoholic",
				"Germany, instead of Father’s Day, has a “Men’s Day” where men young and old cart wagons filled with booze and food around.",
				"During Prohibition, the U.S. government poisoned alcohol to discourage alcoholism, killing as many as 10,000 people.",
				"The State of Georgia was originally colonized with three prohibitions. No alcohol, no slavery, and no Catholics.",
				"Scientists are researching a chemical/drug that dramatically reduces alcohol/BAC levels in the blood, which has the potential to be used to sober people up in a very short time.",
				"The original Playboy Bunnies were required to be able to identify 143 brands of liquor and how to make 20 different cocktails as part of their job requirements.",
				"Absinthe, The Green Fairy, was invented by the Swiss in the 19th century. Though it contains wormwood, it’s no more hallucinogenic than vodka or whiskey. Economics/politics are the true culprits for Absinthe once being banned across the world.",
				"Walgreens pharmacy franchise grew from 20 to 400 stores during Prohibition due to liquor prescriptions. ",
				"On Christmas Day 1826, cadets at the US Military Academy at West Point including future Confederacy President Jefferson Davis threw a booze-fueled party and eventual turned into mutiny that ended with 20 Court Martials.",
				"Mountain Dew was made to be mixed with whiskey.",
				"Bill Wilson, the well-known co-founder of Alcoholics Anonymous, demanded whiskey in the last days of his life and became belligerent when he was denied.",
				"In Russia, in times of economic disparity or high inflation, teachers can be paid in Vodka.",
				"Scientist Nikola Tesla drank whiskey every day, because he thought it would make him live to 150.",
				"In 1830, the average person over 15 years old drank 88 bottles of whiskey per year. That is one bottle every 4.2 days.",
				"When King Charles II of Navarre was deathly ill, his physician ordered him to be wrapped and sewn into bed sheets that had been pre-soaked in brandy. A careless servant used a candle to burn the loose thread away, which set fire to the cloth and burned the king alive.",
				"After Germany surrendered in WWII, everyone in Moscow partied until the entire city actually ran out of vodka. ",
				"Gin and tonic originated in British colonial India when the British population would mix their medicinal quinine tonic (malaria medicine) with gin to make it more palatable.",
				"The world’s hottest vodka at 250,000 scovilles is so hot the manufacturers put a lead seal on the bottle that has to be cut with an wire cutter; and they insist on their website that you don’t buy it.",
				"Rum was the main currency in early colonial Australia. New South Wales Corps officers bought up all the imported rum and established a 17 year monopoly on its trade. When Governor William Bligh tried to end it, he was deposed in the only armed takeover of government in Aussie history. It was known as The Rum Rebellion.",
				"There is a traditional British army drink called gunfire made of black tea and rum. Officers serve it to lower ranks on Christmas. During the Korean war, British soldiers once gave gunfire to some American MPs, causing them to drive an ARV and some jeeps into a fence.",
				"A gin & tonic will glow under a UV light because tonic contains quinines, which are UV light reactive.",
				"The most expensive scotch in the world is sold for $460,000 and that’s $14,000 per shot.",
				"There is a cruise ship that runs between Stockholm and Helsinki with the primary purpose of purchasing and consuming cheap alcohol.",
				"During prohibition, Herbert Hoover would go to the Belgian embassy to have martinis, as it was not illegal to possess alcohol on foreign soil.",
				"Mississippi permits drivers to consume alcohol while driving. However, the driver must still stay below the 0.08% blood alcohol content limit for DUI."
			};
		}

		#region IFunFactService implementation

		public Task<string> GetRandomFunFact ()
		{
			var rand = new Random ();
			var pos = rand.Next (_hardcodedFunFacts.Count);

			return Task.FromResult (_hardcodedFunFacts [pos]);
		}

		#endregion
	}
}

