using System;
using InventorShaftGenerator.Models.SubFeatures;

namespace InventorShaftGenerator.Models
{
    public enum SubFeature
    {
        ThroughHole,
        RetainingRingGroove,
        KeywayGroove,
        Wrench,
        ReliefDSI,
        GrooveA,
        GrooveB,
        RetainingRingBore
    }

    public static class SubfeatureEnumExtensions
    {
        public static Type ToSubFeatureType(this SubFeature subFeatureEnumMember)
        {
            switch (subFeatureEnumMember)
            {
                case SubFeature.ThroughHole:
                    return typeof(ThroughHoleSubFeature);
                case SubFeature.RetainingRingGroove:
                    return typeof(RetainingRingGrooveSubFeature);
                case SubFeature.RetainingRingBore:
                    return typeof(RetainingRingGrooveSubFeature);
                case SubFeature.KeywayGroove:
                    return typeof(KeywayGrooveSubFeature);
                case SubFeature.Wrench:
                    return typeof(WrenchSubFeature);
                case SubFeature.ReliefDSI:
                    return typeof(ReliefDSISubFeature);
                case SubFeature.GrooveA:
                    return typeof(GrooveASubFeature);
                case SubFeature.GrooveB:
                    return typeof(GrooveBSubFeature);
                default:
                    throw new ArgumentException($"Cannot find a corresponding sub feature for {subFeatureEnumMember}",
                        nameof(subFeatureEnumMember));
            }
        }
    }
}