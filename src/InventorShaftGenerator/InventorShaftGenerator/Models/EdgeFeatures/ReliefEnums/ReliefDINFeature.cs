using System;
using InventorShaftGenerator.Models.EdgeFeatures.ReliefsDINUnits;

namespace InventorShaftGenerator.Models.EdgeFeatures.ReliefEnums
{
    public enum ReliefDINFeature
    {
        ReliefA,
        ReliefB,
        ReliefC,
        ReliefD,
        ReliefE,
        ReliefF
    }

    public static class ReliefDINFeatureEnumExtensions
    {
        public static Type ToReliefDINEdgeFeatureType(this ReliefDINFeature reliefDinEnumMember)
        {
            switch (reliefDinEnumMember)
            {
                case ReliefDINFeature.ReliefA:
                    return typeof(ReliefADinEdgeFeature);
                case ReliefDINFeature.ReliefB:
                    return typeof(ReliefBDinEdgeFeature);
                case ReliefDINFeature.ReliefC:
                    return typeof(ReliefCDinEdgeFeature);
                case ReliefDINFeature.ReliefD:
                    return typeof(ReliefDDinEdgeFeature);
                case ReliefDINFeature.ReliefE:
                    return typeof(ReliefEDinEdgeFeature);
                case ReliefDINFeature.ReliefF:
                    return typeof(ReliefFDinEdgeFeature);

                default:
                    throw new ArgumentException($"Cannot find a corresponding edge feature for {reliefDinEnumMember}",
                        nameof(reliefDinEnumMember));
            }
        }
    }
}