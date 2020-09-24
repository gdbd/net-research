using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace Korus.TestApplication.Features.TestApplication
{

    [Guid("dda83ef9-dd3e-4b00-932f-5abf2db81444")]
    public class TestApplicationEventReceiver : SPFeatureReceiver
    {
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            base.FeatureActivated(properties);

             var site = (SPSite) properties.Feature.Parent;
             var web = site.RootWeb;


            //var web = (SPWeb)properties.Feature.Parent;
            
            EnsureGroup(web);

            EnsureReceivers(web);
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            base.FeatureDeactivating(properties);

            // var web = (SPWeb)properties.Feature.Parent;
            var site = (SPSite)properties.Feature.Parent;

            var web = site.RootWeb;

            RemoveGroup(web);

            RemoveReceivers(web);
        }

        private static void EnsureReceivers(SPWeb web)
        {
            var list = web.GetList(SPUtility.ConcatUrls(web.Url, Common.Urls.ContractsList));
            Common.EventReceiverHelper.EnsureReceiver(list, typeof(EventReceivers.ContractsDocSetReceiver),
                sync: SPEventReceiverSynchronization.Synchronous,
                receiverTypes: new [] {  SPEventReceiverType.ItemAdded, });
        }

        private static void RemoveReceivers(SPWeb web)
        {
            var list = web.GetList(SPUtility.ConcatUrls(web.Url, Common.Urls.ContractsList));
         
            Common.EventReceiverHelper.UnRegisterReceiver(list, typeof(EventReceivers.ContractsDocSetReceiver),
                receiverType: SPEventReceiverType.ItemAdded);
        }

        private static void RemoveGroup(SPWeb web)
        {
            var approversGroupTitle = Common.ResourceAccessor.GetString("ApproversGroupTitle");
            var group = web.SiteGroups.Cast<SPGroup>().FirstOrDefault(g => g.Name == approversGroupTitle);
            if (group != null)
            {
                web.SiteGroups.RemoveByID(@group.ID);
            }
        }

        private static void EnsureGroup(SPWeb web)
        {
            var approversGroupTitle = Common.ResourceAccessor.GetString("ApproversGroupTitle");

            var group = web.SiteGroups.Cast<SPGroup>().FirstOrDefault(g => g.Name == approversGroupTitle);
            if (group == null)
            {
                web.SiteGroups.Add(approversGroupTitle, web.Author, web.Author, string.Empty);
                group = web.SiteGroups.Cast<SPGroup>().Single(g => g.Name == approversGroupTitle);

                var approversField = (SPFieldUser)web.Fields.GetFieldByInternalName(Common.Fields.ContractApprovers);
                approversField.SelectionGroup = group.ID;
                approversField.Update();
            }
        }
    }
}
