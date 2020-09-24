using System;
using System.Linq;
using Microsoft.SharePoint;

namespace Korus.TestApplication.Common
{
    public class EventReceiverHelper
    {

        public static void EnsureReceiver(SPList list, Type receiverClass, int sequence = 10000,
            SPEventReceiverSynchronization sync = SPEventReceiverSynchronization.Default, 
            string name = null,
            params SPEventReceiverType[] receiverTypes)
        {
            if (receiverClass == null)
                throw new ArgumentNullException(nameof(receiverClass));

            if (name == null)
            {
                name = $"Event receiver of type {receiverClass.FullName}";
            }

            foreach (var receiverType in receiverTypes)
            {
                var receiverClassName = receiverClass.FullName;
                var receiverAssembly = receiverClass.Assembly.FullName;
                var receiver =
                    list.EventReceivers.OfType<SPEventReceiverDefinition>().FirstOrDefault(
                        x => x.Class == receiverClassName && x.Type == receiverType);

                if (receiver == null)
                {
                    var rec = list.EventReceivers.Add();
                    rec.Name = name;
                    rec.Assembly = receiverAssembly;
                    rec.Class = receiverClassName;
                    rec.Synchronization = sync;
                    rec.Type = receiverType;
                    rec.SequenceNumber = sequence;
                    rec.Update();
                }
            }
        }

        public static void UnRegisterReceiver(SPList list, Type eventReceiverClass, SPEventReceiverType receiverType)
        {
            string sClassName = eventReceiverClass.FullName;

            int count = 0;
            bool found = true;
            while ((found) && (count < 100))
            {
                var eventReceivers = list.EventReceivers;

                count++;
                found = false;
                foreach (SPEventReceiverDefinition def in eventReceivers)
                {
                    if ((def.Class == sClassName) && (def.Type == receiverType))
                    {
                        def.Delete();
                        found = true;
                        break;
                    }
                }
            }
        }
    }
}
