namespace SpottedZebra.PowerTools.Core
{
    internal interface ILogAdapter
    {
        void Info(string format, params object[] args);

        void Warn(string format, params object[] args);

        void Error(string format, params object[] args);
    }
}