namespace SpottedZebra.PowerTools.Core.Data
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Label used by the OverlayText PowerTool.
    /// </summary>
    [DataContract]
    internal sealed class Label
    {
        [DataMember(IsRequired = true)]
        public string Name { get; set; }

        [DataMember(IsRequired = true)]
        public string Value { get; set; }

        [DataMember]
        public string FontName { get; set; }

        [DataMember]
        public bool DisableAntiAliasing { get; set; }
    }
}
