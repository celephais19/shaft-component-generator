using System;
using InventorShaftGenerator.Models.EdgeFeatures.ReliefGOSTUnits;

namespace InventorShaftGenerator.Models.EdgeFeatures.ReliefEnums
{
    public enum ReliefGOSTFeature
    {
        ReliefA,
        ReliefB,
        ReliefC,
        ReliefD,
        ReliefE
    }

    public static class ReliefGOSTFeatureEnumExtensions
    {
        public static Type ToReliefGOSTEdgeFeatureType(this ReliefGOSTFeature reliefGOSTEnumMember)
        {
            switch (reliefGOSTEnumMember)
            {
                case ReliefGOSTFeature.ReliefA:
                    return typeof(ReliefAGostEdgeFeature);
                case ReliefGOSTFeature.ReliefB:
                    return typeof(ReliefBGostEdgeFeature);
                case ReliefGOSTFeature.ReliefC:
                    return typeof(ReliefCGostEdgeFeature);
                case ReliefGOSTFeature.ReliefD:
                    return typeof(ReliefDGostEdgeFeature);
                case ReliefGOSTFeature.ReliefE:
                    return typeof(ReliefEGostEdgeFeature);
                default:
                    throw new ArgumentException($"Cannot find a corresponding edge feature for {reliefGOSTEnumMember}",
                        nameof(reliefGOSTEnumMember));
            }
        }
    }
}