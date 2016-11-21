namespace SpottedZebra.PowerTools.Core.Data
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Parameters used by the OverlayText PowerTool.
    /// </summary>
    [DataContract]
    internal sealed class OverlayTextJob : IJobDescription
    {
        [DataMember(IsRequired = true)]
        public string Name { get; set; }

        [DataMember(IsRequired = true)]
        [RelativePath]
        public string ImagePath { get; set; }

        [DataMember(IsRequired = true)]
        public string FontName { get; set; }

        [DataMember(IsRequired = true)]
        public string FontColor { get; set; }

        [DataMember]
        public float MaxFontSize { get; set; }

        [DataMember]
        public bool DisableAntiAliasing { get; set; }

        [DataMember(IsRequired = true)]
        public string OutputImageNameTemplate { get; set; }

        [DataMember(IsRequired = true)]
        public BoundingBox BoundingBox { get; set; }

        [DataMember(IsRequired = true)]
        public Label[] Labels { get; set; }
    }
}
