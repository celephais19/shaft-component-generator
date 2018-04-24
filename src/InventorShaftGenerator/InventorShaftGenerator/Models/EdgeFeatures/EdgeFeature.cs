using System;

namespace InventorShaftGenerator.Models.EdgeFeatures
{
    public enum EdgeFeature
    {
        NotAvailable,
        None,
        Chamfer,
        Fillet,
        Thread,
        LockNutGroove,
        PlainKeywayGroove,
        KeywayGrooveRoundedEnd,
        ReliefSI,
        ReliefDIN,
        ReliefGOST
    }

    public static class EdgeFeatureEnumExtensions
    {
        public static Type ToEdgeFeatureType(this EdgeFeature edgeFeature)
        {
            switch (edgeFeature)
            {
                case EdgeFeature.Chamfer:
                    return typeof(ChamferEdgeFeature);
                case EdgeFeature.Fillet:
                    return typeof(FilletEdgeFeature);
                case EdgeFeature.Thread:
                    return typeof(ThreadEdgeFeature);
                case EdgeFeature.LockNutGroove:
                    return typeof(LockNutGrooveEdgeFeature);
                case EdgeFeature.PlainKeywayGroove:
                    return typeof(PlainKeywayGrooveEdgeFeature);
                case EdgeFeature.KeywayGrooveRoundedEnd:
                    return typeof(KeywayGrooveRoundedEndEdgeFeature);
                default:
                    throw new ArgumentException($"Cannot find a corresponding edge feature for {edgeFeature}",
                        nameof(edgeFeature));
            }
        }

        public static EdgeFeature ToEdgeFeatureEnumMember(this ISectionFeature sectionFeature)
        {
            if (!sectionFeature.IsEdgeFeature())
            {
                throw new ArgumentException("A specified section feature type must be a type of an edge feature",
                    nameof(sectionFeature));
            }

            switch (sectionFeature)
            {
                case ChamferEdgeFeature _:
                    return EdgeFeature.Chamfer;
                case FilletEdgeFeature _:
                    return EdgeFeature.Fillet;
                case ThreadEdgeFeature _:
                    return EdgeFeature.Thread;
                case LockNutGrooveEdgeFeature _:
                    return EdgeFeature.LockNutGroove;
                case PlainKeywayGrooveEdgeFeature _:
                    return EdgeFeature.PlainKeywayGroove;
                case KeywayGrooveRoundedEndEdgeFeature _:
                    return EdgeFeature.KeywayGrooveRoundedEnd;
                case ReliefSIEdgeFeature _:
                    return EdgeFeature.ReliefSI;
                case ReliefDinEdgeFeature _ :
                    return EdgeFeature.ReliefDIN;
                case ReliefGostEdgeFeature _:
                    return EdgeFeature.ReliefGOST;


                default:
                    throw new ArgumentException(
                        $"Cannot find a corresponding edge feature enum member for {sectionFeature}",
                        nameof(sectionFeature));
            }
        }
    }
}