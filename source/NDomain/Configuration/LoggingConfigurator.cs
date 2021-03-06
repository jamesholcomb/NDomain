﻿using NDomain.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDomain.Configuration
{
    public class LoggingConfigurator : Configurator
    {
        public ILoggerFactory LoggerFactory { get; set; }

        public LoggingConfigurator(ContextBuilder builder)
            : base(builder)
        {
            builder.Configuring += this.OnConfiguring;
        }

        private void OnConfiguring(ContextBuilder builder)
        {
            builder.LoggerFactory = new Lazy<ILoggerFactory>(
                () => this.LoggerFactory ?? new NullLoggerFactory());
        }
    }
}
