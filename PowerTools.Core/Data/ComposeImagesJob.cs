namespace SpottedZebra.PowerTools.Core.Data
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Parameters required for the ComposeImage PowerTool.
    /// </summary>
    [DataContract]
    public class ComposeImagesJob : IJobDescription
    {
        [DataMember(IsRequired = true)]
        public string Name { get; set; }

        [DataMember(IsRequired = true)]
        public string OutputImageNameTemplate { get; set; }

        [DataMember(IsRequired = true)]
        public string BaseImagesFolderPath { get; set; }

        [DataMember(IsRequired = true)]
        public string[] ImagesToCombinePaths { get; set; }
    }
}