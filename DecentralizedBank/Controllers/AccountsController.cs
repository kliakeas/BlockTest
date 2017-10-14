using DecentralizedBank.Models;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DecentralizedBank.Controllers
{
    [RoutePrefix("api/v1/Accounts")]
    [SwaggerResponse(HttpStatusCode.BadRequest, Description = "In case something is wrong with the request data")]
    public class AccountsController : ApiController
    {
        /// <summary>
        /// Get all the accounts in the current node
        /// </summary>
        /// <remarks></remarks>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.OK, Description = "Returns an array of accounts", Type = typeof(AccountsResponse))]
        public HttpResponseMessage GetAccounts()
        {
            try
            {
                var web3 = new Nethereum.Web3.Web3(ConfigurationManager.AppSettings["GethUrl"]);
                var accountsTask = web3.Eth.Accounts.SendRequestAsync();
                accountsTask.Wait();

                return ResponseHelper.Create(HttpStatusCode.OK, new AccountsResponse()
                {
                    Accounts = accountsTask.Result
                });
            }
            catch (Exception ex)
            {
                return ResponseHelper.Create(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        /// <summary>
        /// Get the current balance of the selected account
        /// </summary>
        /// <remarks></remarks>
        [HttpGet]
        [Route("{account}/balance")]
        [SwaggerResponse(HttpStatusCode.OK, Description = "Returns the current balance of the selected account", Type = typeof(AccountBalanceResponse))]
        public HttpResponseMessage GetAccountBalance(string account)
        {
            try
            {
                var web3 = new Nethereum.Web3.Web3(ConfigurationManager.AppSettings["GethUrl"]);
                var balanceTask = web3.Eth.GetBalance.SendRequestAsync(account);
                balanceTask.Wait();

                return ResponseHelper.Create(HttpStatusCode.OK, new AccountBalanceResponse()
                {
                    Account = account,
                    Balance = Nethereum.Util.UnitConversion.Convert.FromWei(balanceTask.Result, Nethereum.Util.UnitConversion.EthUnit.Ether)
                });
            }
            catch (Exception ex)
            {
                return ResponseHelper.Create(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        /// <summary>
        /// Perform an amount transfer between two accounts
        /// </summary>
        /// <remarks></remarks>
        [HttpPost]
        [Route("transfer")]
        [SwaggerResponse(HttpStatusCode.OK, Description = "Returns the transaction hash of the tranfer transaction that is posted to the blockchain", Type = typeof(TransferResponse))]
        public HttpResponseMessage AccountsTransfer([FromBody]TransferRequest transferRequest)
        {
            try
            {
                var web3 = new Nethereum.Web3.Web3(ConfigurationManager.AppSettings["GethUrl"]);

                web3.Personal.UnlockAccount.SendRequestAsync(transferRequest.Sender, ConfigurationManager.AppSettings["GethAccountsPassword"], 60).Wait();

                var wei = Nethereum.Util.UnitConversion.Convert.ToWei(transferRequest.Amount, Nethereum.Util.UnitConversion.EthUnit.Ether);
                var transferTask = web3.TransactionManager.SendTransactionAsync(transferRequest.Sender, transferRequest.Receiver, new Nethereum.Hex.HexTypes.HexBigInteger(wei));
                transferTask.Wait();

                return ResponseHelper.Create(HttpStatusCode.OK, new TransferResponse()
                {
                    TxHash = transferTask.Result
                });
            }
            catch (Exception ex)
            {
                return ResponseHelper.Create(HttpStatusCode.BadRequest, ex.Message);
            }
        }
    }
}
