using Microsoft.SharePoint;

namespace Korus.TestApplication.Common
{
    public class Numerator
    {
        private readonly SPWeb _web;
     

        private const string ContractNumberPropertyName = "prop_23bca670c6a2421c91f872366ede1887";

        public Numerator(SPWeb web)
        {
            _web = web;
        }

        public string GetNextContractNumber()
        {

            var contractNumber = int.Parse(_web.Properties[ContractNumberPropertyName] ?? "0");

            contractNumber += 1;

            var nextNumer = contractNumber.ToString().PadLeft(6, '0');

            _web.Properties[ContractNumberPropertyName] = nextNumer;
            _web.Properties.Update();

            return nextNumer;

        }
    }
}
