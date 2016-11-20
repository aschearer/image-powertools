namespace SpottedZebra.PowerTools.CommandLine
{
    using System;
    using Core;
    using NLog;

    internal class NLogAdapter : ILogAdapter
    {
        private Logger logger;

        public NLogAdapter(Logger logger)
        {
            this.logger = logger;
        }

        public void Error(string format, params object[] args)
        {
            this.logger.Error(format, args);
        }

        public void Info(string format, params object[] args)
        {
            this.logger.Info(format, args);
        }

        public void Warn(string format, params object[] args)
        {
            this.logger.Warn(format, args);
        }
    }
}