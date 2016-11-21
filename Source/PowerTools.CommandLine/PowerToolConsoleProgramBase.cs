namespace SpottedZebra.PowerTools.CommandLine
{
    using SpottedZebra.PowerTools.Core.Tools;

    /// <summary>
    /// This exists so we can have a non-generic type to call Run on in MainProgram.
    /// </summary>
    internal abstract class PowerToolConsoleProgramBase
    {
        public abstract ExitCode Run(string[] args);
    }
}
