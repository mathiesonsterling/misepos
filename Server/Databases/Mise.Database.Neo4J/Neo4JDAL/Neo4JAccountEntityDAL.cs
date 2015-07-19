using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Accounts;
using Mise.Core.Entities.Accounts;
using Mise.Core.ValueItems;
using Mise.Database.StorableEntities.Accounts;
using Mise.Database.StorableEntities.ValueItems;
using Neo4jClient.Cypher;

namespace Mise.Neo4J.Neo4JDAL
{
    public partial class Neo4JEntityDAL
    {
        #region Account
        public async Task<IEnumerable<IAccount>> GetAccountsAsync()
        {

            var nodes = await _graphClient.Cypher
                .Match("(account:MiseAccount)")
                .Return(
                    account => account.As<AccountGraphNode>())
                .ResultsAsync;

            var res = new List<IAccount>();
            foreach (var node in nodes)
            {
                var emails = await GetEmailsAssociatedWithEntity(node.ID);

                var acctID = node.ID;
                //get the credit cards
                var ccQuery =  _graphClient.Cypher
                    .Match("(account:MiseAccount)-[:CURRENT_PAYMENT_CARD]-(cc:CreditCard)")
                    .Where((AccountGraphNode account) => account.ID == acctID)
                    .Return(cc => cc.As<CreditCardGraphNode>());
                var currentCreditCardNodes = (await ccQuery.ResultsAsync).ToList();

                if (currentCreditCardNodes.Count > 1)
                {
                    throw new Exception("More than one current credit card!");
                }


                CreditCard currentCard = null;
                var creditCardGraphNode = currentCreditCardNodes.FirstOrDefault();
                if (creditCardGraphNode != null)
                {
                    currentCard = creditCardGraphNode.Rehydrate();
                }

                //get the referral codes in and out
                var referralUsedToCreates = await _graphClient.Cypher
                    .Match("(account:MiseAccount)-[:REFERRED_BY]->(r:ReferralCode)")
                    .Where((AccountGraphNode account) => account.ID == acctID)
                    .Return(r => r.As<ReferralCode>())
                    .ResultsAsync;

                var referralToGiveOut = await _graphClient.Cypher
                    .Match("(account:MiseAccount)-[:HAS_REFERRAL_CODE]->(r:ReferralCode)")
                    .Where((AccountGraphNode account) => account.ID == acctID)
                    .Return(r => r.As<ReferralCode>())
                    .ResultsAsync;

                //get the charges
                var chargeNodes = await _graphClient.Cypher
                    .Match("(charge:AccountCharge)-[:ACCOUNT_CHARGED]->(account:MiseAccount)")
                    .Where((AccountGraphNode account) => account.ID == acctID)
                    .Return(charge => charge.As<AccountChargeGraphNode>())
                    .ResultsAsync;
                var charges = chargeNodes.Select(cn => cn.Rehydrate(acctID));

                //get the payments
                var paymentNodes = await _graphClient.Cypher
                    .Match("(p:AccountPayment)-[:PAYMENT_MADE]->(account:MiseAccount)")
                    .Where((AccountGraphNode account) => account.ID == acctID)
                    .AndWhere((AccountPaymentGraphNode p) => p.PaymentType == PaymentType.InternalCreditCard.ToString())
                    .Return(p => p.As<AccountPaymentGraphNode>())
                    .ResultsAsync;

                //we need to get each of our credit cards here
                var payments = new List<AccountCreditCardPayment>();
                foreach (var pNode in paymentNodes)
                {
                    var cards = await GetCreditCardsRelatingToEntity("PAID_WITH_CARD", pNode.ID);
                    var ccPay = pNode.Rehydrate(cards.FirstOrDefault()) as AccountCreditCardPayment;
                    payments.Add(ccPay);
                }

                var creditNodes = await _graphClient.Cypher
                    .Match("(p:AccountPayment)-[:PAYMENT_MADE]->(account:MiseAccount)")
                    .Where((AccountGraphNode account) => account.ID == acctID)
                    .AndWhere((AccountPaymentGraphNode p) => p.PaymentType == PaymentType.MiseCredit.ToString())
                    .Return(p => p.As<AccountPaymentGraphNode>())
                    .ResultsAsync;
                var credits = creditNodes.Select(n => n.Rehydrate(null) as AccountCredit);
                res.Add(node.Rehydrate(emails, currentCard, charges, payments, credits, referralToGiveOut.FirstOrDefault(), referralUsedToCreates.FirstOrDefault()));
            }

            return res; 
        }


        public async Task AddAccountAsync(IAccount account)
        {

            await _graphClient.Cypher
                .Create("(a:MiseAccount {account})")
                .WithParam("account", new AccountGraphNode(account))
                .ExecuteWithoutResultsAsync()
                .ConfigureAwait(false);

            foreach (var email in account.Emails)
            {
                await SetEmailAddressOnEntityAsync(email, account.ID);
            }

            if (account.PhoneNumber != null)
            {
                await SetPhoneNumber(account.PhoneNumber, account.ID);
            }

            if (account.CurrentCard != null)
            {
                await SetCreditCardOnEntity(account.CurrentCard, "CURRENT_PAYMENT_CARD", account.ID);
            }

            //tie us to a referral code
            if (account.ReferralCodeForAccountToGiveOut != null)
            {
                await SetReferralCodeOnEntity(account.ReferralCodeForAccountToGiveOut, "HAS_REFERRAL_CODE", account.ID);
            }

            if (account.ReferralCodeUsedToCreate != null)
            {
                await SetReferralCodeOnEntity(account.ReferralCodeUsedToCreate, "REFERRED_BY", account.ID);
            }
            //set our payments, credits, and charges
            foreach (var charge in account.GetCharges())
            {
                var chargeNode = new AccountChargeGraphNode(charge);
                await _graphClient.Cypher
                    .Create("(c:AccountCharge {node})")
                    .WithParam("node", chargeNode)
                    .ExecuteWithoutResultsAsync()
                    .ConfigureAwait(false);

                var linkQuery = _graphClient.Cypher
                    .Match("(c:AccountCharge)", "(a:MiseAccount)")
                    .Where((AccountGraphNode a) => a.ID == account.ID)
                    .AndWhere((AccountChargeGraphNode c) => c.ID == chargeNode.ID)
                    .CreateUnique("(c)-[:ACCOUNT_CHARGED]->(a)");

                await linkQuery.ExecuteWithoutResultsAsync()
                    .ConfigureAwait(false);
            }
            foreach (var payment in account.GetPayments())
            {
                var payNode = new AccountPaymentGraphNode(payment);
                await _graphClient.Cypher
                    .Create("(c:AccountPayment {node})")
                    .WithParam("node", payNode)
                    .ExecuteWithoutResultsAsync()
                    .ConfigureAwait(false);

                await _graphClient.Cypher
                    .Match("(pay:AccountPayment)", "(a:MiseAccount)")
                    .Where((AccountGraphNode a) => account.ID == a.ID)
                    .AndWhere((AccountPaymentGraphNode pay) => pay.ID == payNode.ID)
                    .CreateUnique("(pay)-[:PAYMENT_MADE]->(a)")
                    .ExecuteWithoutResultsAsync()
                    .ConfigureAwait(false);

                if (payment is AccountCreditCardPayment)
                {
                    var down = payment as AccountCreditCardPayment;
                    if (down.CardUsed != null)
                    {
                        await SetCreditCardOnEntity(down.CardUsed, "PAID_WITH_CARD", down.ID);
                    }
                }
            }
        }

        public async Task UpdateAccountAysnc(IAccount account)
        {
            //delete any charges or payments for this account
            await _graphClient.Cypher
                .Match("(c:AccountCharge)-[r:ACCOUNT_CHARGED]-(a:MiseAccount)")
                .Where((AccountGraphNode a) => a.ID == account.ID)
                .Delete("c, r")
                .ExecuteWithoutResultsAsync()
                .ConfigureAwait(false);

            await _graphClient.Cypher
                .Match("(p:AccountPayment)-[:PAYMENT_MADE]->(a:MiseAccount)")
                .Where((AccountGraphNode a) => a.ID == account.ID)
                .Delete("p, r")
                .ExecuteWithoutResultsAsync()
                .ConfigureAwait(false);


            //delete the account
            await _graphClient.Cypher
                .Match("(a:MiseAccount)-[r]-()")
                .Where((AccountGraphNode a) => a.ID == account.ID)
                .Delete("a, r")
                .ExecuteWithoutResultsAsync()
                .ConfigureAwait(false);

            //add it
            await AddAccountAsync(account);
        }

        #endregion

        private async Task SetReferralCodeOnEntity(ReferralCode code, string relationship, Guid entityID)
        {
            var found = await _graphClient.Cypher
                .Match("(r:ReferralCode)")
                .Where((ReferralCode r) => r.Code == code.Code)
                .Return(r => r.As<ReferralCode>())
                .ResultsAsync;

            var dbCode = found.FirstOrDefault();
            if (dbCode == null)
            {
                var createQuery = _graphClient.Cypher
                   .Create("(r:ReferralCode {code})")
                   .WithParam("code", code);

                await createQuery.ExecuteWithoutResultsAsync().ConfigureAwait(false);
            }

            //tie it to the entity
            var linkQuery = _graphClient.Cypher
                .Match("(m)", "(r:ReferralCode)")
                .Where("m.ID = {guid}")
                .AndWhere("r.Code = {code}")
                .WithParam("guid", entityID)
                .WithParam("code", code.Code)
                .CreateUnique("(m)-[:" + relationship + "]->(r)");

            await linkQuery.ExecuteWithoutResultsAsync().ConfigureAwait(false);
        }

        private async Task SetCreditCardOnEntity(CreditCard card, string relationship, Guid entityID)
        {
            ICypherFluentQuery<CreditCardGraphNode> query;
            if (card.ProcessorToken != null)
            {
                query = _graphClient.Cypher
                    .Match("(cc:CreditCard)")
                    .Where((CreditCardGraphNode cc) => card.ProcessorToken.Processor.ToString() == cc.ProcessorName
                                                       && card.ProcessorToken.Token == cc.ProcessorToken
                    )
                    .Return(cc => cc.As<CreditCardGraphNode>());
            }
            else
            {
                query = _graphClient.Cypher
                    .Match("(cc:CreditCard)")
                    .Where((CreditCardGraphNode cc) => cc.FirstName == card.Name.FirstName
                                                       && cc.MiddleName == card.Name.MiddleName
                                                       && cc.LastName == card.Name.LastName
                    )
                    .Return(cc => cc.As<CreditCardGraphNode>());

            }

            var found = (await query.ResultsAsync).ToList();

            CreditCardGraphNode node;
            if (found.Any() == false)
            {
                node = new CreditCardGraphNode(card);
                var createQuery = _graphClient.Cypher
                    .Create("(cc:CreditCard {card})")
                    .WithParam("card", node);

                await createQuery.ExecuteWithoutResultsAsync().ConfigureAwait(false);
            }
            else
            {
                node = found.First();
            }

            var linkQuery =  _graphClient.Cypher
                .Match("(m)", "(cc:CreditCard)")
                .Where("m.ID = {guid}")
                .AndWhere("cc.ID = {ccID}")
                .WithParam("guid", entityID)
                .WithParam("ccID", node.ID)
                .CreateUnique("(m)-[:" + relationship + "]->(cc)");

            await linkQuery.ExecuteWithoutResultsAsync().ConfigureAwait(false);
        }

        private async Task<IEnumerable<CreditCard>> GetCreditCardsRelatingToEntity(string relationship, Guid entityID)
        {
            var nodes = await _graphClient.Cypher
                .Match("(m)-[:" + relationship + "]-(cc:CreditCard)")
                .Where("m.ID={guid}")
                .WithParam("guid", entityID)
                .Return(cc => cc.As<CreditCardGraphNode>())
                .ResultsAsync;

            var cards = nodes.Select(n => n.Rehydrate());
            return cards;
        }
    }
}
