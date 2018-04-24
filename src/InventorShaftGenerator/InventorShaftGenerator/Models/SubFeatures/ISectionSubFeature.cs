namespace InventorShaftGenerator.Models.SubFeatures
{
    public interface ISectionSubFeature : ISectionFeature
    {
        float Distance { get; set; }
        DistanceFrom DistanceFrom { get; set; }
    }
}