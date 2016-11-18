namespace SpottedZebra.PowerTools.Core.Data
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Bounding box used by the OverlayText PowerTool.
    /// </summary>
    [DataContract]
    internal sealed class BoundingBox
    {
        public BoundingBox()
        {
        }

        public BoundingBox(float x, float y, float width, float height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        [DataMember(IsRequired = true)]
        public float X { get; set; }

        [DataMember(IsRequired = true)]
        public float Y { get; set; }

        [DataMember(IsRequired = true)]
        public float Width { get; set; }

        [DataMember(IsRequired = true)]
        public float Height { get; set; }
    }
}