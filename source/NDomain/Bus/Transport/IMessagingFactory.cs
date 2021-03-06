﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDomain.Bus.Transport
{
    /// <summary>
    /// Factory that creates messaging clients
    /// </summary>
    public interface ITransportFactory
    {
        /// <summary>
        /// Creates an inbound transport for the specific endpoint
        /// </summary>
        /// <returns></returns>
        IInboundTransport CreateInboundTransport(string endpoint);

        /// <summary>
        /// Creates an outbound transport
        /// </summary>
        /// <returns></returns>
        IOutboundTransport CreateOutboundTransport();
    }
}
