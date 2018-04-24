using System;
using System.Collections.Specialized;
using InventorShaftGenerator.Infrastructure;

namespace InventorShaftGenerator.Models
{
    public class ShaftSectionFeatureError : IEquatable<ShaftSectionFeatureError>
    {
        public ShaftSectionFeatureError(ShaftSection section, ISectionFeature feature, string errorMessage)
        {
            this.Section = section;
            this.Feature = feature;
            this.ErrorMessage = errorMessage;

            Shaft.Sections.CollectionChanged += ProvideSelfDestroy;
        }

        public Guid Id { get; } = Guid.NewGuid();

        public ShaftSection Section { get; }

        public ISectionFeature Feature { get; }

        public string ErrorMessage { get; }

        private void ProvideSelfDestroy(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action != NotifyCollectionChangedAction.Remove)
            {
                return;
            }

            bool linkedSectionWasDeleted = args.OldItems[0].Equals(this.Section);
            if (linkedSectionWasDeleted)
            {
                Shaft.ShaftFeaturesErrors.Remove(this);
            }
        }

        public bool Equals(ShaftSectionFeatureError other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((ShaftSectionFeatureError) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (this.Section != null ? this.Section.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.Feature != null ? this.Feature.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.ErrorMessage != null ? this.ErrorMessage.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}