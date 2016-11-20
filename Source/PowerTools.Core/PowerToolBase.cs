namespace SpottedZebra.PowerTools.Core
{
    using Data;

    /// <summary>
    /// Provide logging and other common functionality for PowerTools.
    /// Provide something concrete for adapters like PowerToolConsoleProgramBase
    /// to use to bootstrap things.
    /// </summary>
    /// <typeparam name="T">The PowerTool's job type.</typeparam>
    internal abstract class PowerToolBase<T> where T : IJobDescription
    {
        private BatchDescription<T> batch;

        private ILogAdapter logger;

        protected BatchDescription<T> Batch
        {
            get { return this.batch; }
        }

        internal void Setup(BatchDescription<T> batch, ILogAdapter logger)
        {
            this.batch = batch;
            this.logger = logger;
        }

        internal void Process(T jobDescription)
        {
            this.OnProcess(jobDescription);
        }

        protected abstract void OnProcess(T jobDescription);

        protected void Info(string format, params object[] args)
        {
            this.logger.Info(format, args);
        }

        protected void Warn(string format, params object[] args)
        {
            this.logger.Warn(format, args);
        }

        protected void Error(string format, params object[] args)
        {
            this.logger.Error(format, args);
        }
    }
}