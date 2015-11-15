using Microsoft.WindowsAzure.Storage;
using NDomain.Configuration;
using NDomain.Bus.Transport.Azure.Queues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDomain.Configuration
{
    public static class AzureConfigurator
    {
        public static BusConfigurator WithAzureQueues(this BusConfigurator configurator,
                                                      CloudStorageAccount account,
                                                      string queueName)
        {

            configurator.MessagingFactory = new QueueTransportFactory(account, queueName);

            return configurator;
        }
    }
}
