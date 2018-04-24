using System.Collections.Generic;

namespace InventorShaftGenerator.Models.EdgeFeatures
{
    public class KeywayDimensions
    {
        public int Order { get; set; }
        public int MinInclusive { get; set; }
        public int MaxInclusive { get; set; }
        public float Width { get; set; }
        public List<Depth> Depths { get; set; }

        public enum DepthType
        {
            Type0,
            Type1,
            Type2,
            Type3,
            Type4,
            Type5,
            Type6
        }
    }

    public class Depth
    {
        public KeywayDimensions.DepthType DepthType { get; set; }
        public float Value { get; set; }
    }

    public class Keyway
    {
        public string Name { get; set; }
        public float MinMainDiameter { get; set; }
        public float MaxMainDiameter { get; set; }
        public KeywayDimensions.DepthType DepthType { get; set; }
    }
}