namespace SpottedZebra.PowerTools.Core.Data
{
    using System;

    /// <summary>
    /// Properties on BatchDescription and IJobDescription's with this attribute
    /// will be modified at runtime. The configuration file's directory will be
    /// prepended to each path.
    /// </summary>
    internal class RelativePathAttribute : Attribute
    {
    }
}