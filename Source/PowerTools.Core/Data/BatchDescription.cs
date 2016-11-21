namespace SpottedZebra.PowerTools.Core.Data
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Base configuration element. Defines common things like logging. Has a list of 
    /// tool-specific jobs.
    /// </summary>
    /// <typeparam name="J">The PowerTool's job type.</typeparam>
    [DataContract]
    internal sealed class BatchDescription<J> where J : IJobDescription
    {
        [DataMember(IsRequired = true)]
        [RelativePath]
        public string OutputFolderPath { get; set; }

        [DataMember]
        public bool Debug { get; set; }

        [DataMember]
        [RelativePath]
        public string LogFilePath { get; set; }

        [DataMember(IsRequired = true)]
        public J[] Jobs { get; set; }
    }
}
