using System;
using InventorShaftGenerator.Models.EdgeFeatures.ReliefsSIUnits;

namespace InventorShaftGenerator.Models.EdgeFeatures.ReliefEnums
{
    public enum ReliefSIFeature
    {
        ReliefA,
        ReliefB,
        ReliefF,
        ReliefG
    }

    public static class ReliefSIFeatureEnumExtensions
    {
        public static Type ToReliefSIEdgeFeatureType(this ReliefSIFeature reliefSIEnumMember)
        {
            switch (reliefSIEnumMember)
            {
                case ReliefSIFeature.ReliefA:
                    return typeof(ReliefASIEdgeFeature);
                case ReliefSIFeature.ReliefB:
                    return typeof(ReliefBSIEdgeFeature);
                case ReliefSIFeature.ReliefF:
                    return typeof(ReliefFSIEdgeFeature);
                case ReliefSIFeature.ReliefG:
                    return typeof(ReliefGSIEdgeFeature);
                default:
                    throw new ArgumentException($"Cannot find a corresponding edge feature for {reliefSIEnumMember}",
                        nameof(reliefSIEnumMember));
            }
        }
    }
}