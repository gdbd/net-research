using System;
using System.Collections.Generic;
using System.Linq;
using Korus.TestApplication.Common;
using Korus.TestApplication.Common.TestApplication.Common;
using Microsoft.SharePoint;

namespace Korus.TestApplication.EventReceivers
{
    public class ContractsDocSetReceiver : SPItemEventReceiver
    {
        public override void ItemAdded(SPItemEventProperties properties)
        {
            base.ItemAdded(properties);
            try
            {
                using (var site = new SPSite(properties.SiteId, SPUserToken.SystemAccount))
                {
                    using (var web = site.OpenWeb(properties.Web.ID))
                    {
                        var numerator = new Common.Numerator(web);
                        var nextNumber = numerator.GetNextContractNumber();

                        var origFiring = EventFiringEnabled;
                        try
                        {
                            EventFiringEnabled = false;
                            var item = properties.List.GetItemById(properties.ListItemId);

                            item[Fields.ContractNumber] = nextNumber;
                            item[Fields.ContractName] = nextNumber;

                            item[Fields.ContractInitiator] = new SPFieldUserValue(properties.Web,
                                properties.CurrentUserId, string.Empty);

                            var allApprovers = GetAllApproversMembers(web);
                            var usersValue = new SPFieldUserValueCollection();
                            usersValue.AddRange(allApprovers.Select(a => new SPFieldUserValue(web, a.ID, a.Name)));

                            item[Fields.ContractApprovers] = usersValue;
                            //var group = web.SiteGroups[]

                            item.SystemUpdate(false);
                        }
                        finally
                        {
                            EventFiringEnabled = origFiring;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                throw;
            }
        }

        private static IEnumerable<SPUser> GetAllApproversMembers(SPWeb web)
        {
            var approversGroupTitle = Common.ResourceAccessor.GetString("ApproversGroupTitle");
            var group = web.SiteGroups.Cast<SPGroup>().FirstOrDefault(g => g.Name == approversGroupTitle);
            if(group == null) throw new Exception($"Group '{approversGroupTitle}' not exists!");
            return group.Users.Cast<SPUser>();
        }

    }
}