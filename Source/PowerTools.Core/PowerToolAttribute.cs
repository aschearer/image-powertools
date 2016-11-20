namespace SpottedZebra.PowerTools.Core
{
    using System;

    /// <summary>
    /// Used in tools. E.g. to create the list of available tools in the command line program.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    internal class PowerToolAttribute : Attribute
    {
        public PowerToolAttribute(Type toolType, string name, string description)
        {
            this.ToolType = toolType;
            this.Name = name;
            this.Description = description;
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public Type ToolType { get; set; }
    }
}