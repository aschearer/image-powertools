namespace SpottedZebra.PowerTools.CommandLine
{
    /// <summary>
    /// This exists so we can have a non-generic type to call Run on in MainProgram.
    /// </summary>
    internal abstract class PowerToolConsoleProgramBase
    {
        public abstract void Run(string[] args);
    }
}
