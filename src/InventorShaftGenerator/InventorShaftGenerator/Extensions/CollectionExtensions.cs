using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Documents;
using InventorShaftGenerator.Infrastructure;
using InventorShaftGenerator.Models;
using InventorShaftGenerator.Models.SubFeatures;

namespace InventorShaftGenerator.Extensions
{
    public static class CollectionExtensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumerableList)
        {
            return enumerableList != null ? new ObservableCollection<T>(enumerableList) : null;
        }

        public static void RemoveByFeatureId(this ObservableCollection<ShaftSectionFeatureError> source, Guid featureId)
        {
            var error = source.SingleOrDefault(e => e.Feature.Id == featureId);
            if (error != null)
            {
                source.Remove(error);
            }
        }

        public static void AddError(this ObservableCollection<ShaftSectionFeatureError> source,
                                    ShaftSectionFeatureError newError,
                                    Func<bool> errorCancellationCondition)
        {
            void ResolveCondition()
            {
                bool satisfies = errorCancellationCondition.Invoke();
                if (satisfies)
                {
                    source.Remove(newError);
                    Shaft.ShaftFeaturesErrors.Remove(newError);
                }
            }

            void TryResolve(object sender, PropertyChangedEventArgs args)
            {
                if (sender.Equals(newError.Section) && args.PropertyName != nameof(ShaftSection.NextSection) &&
                    args.PropertyName != nameof(ShaftSection.PreviousSection) &&
                    args.PropertyName != nameof(ShaftSection.FirstLine) &&
                    args.PropertyName != nameof(ShaftSection.SecondLine) &&
                    args.PropertyName != nameof(ShaftSection.ThirdLine) &&
                    args.PropertyName != nameof(ShaftSection.AvailableFirstEdgeFeatures) &&
                    args.PropertyName != nameof(ShaftSection.AvailableSecondEdgeFeatures))
                {
                    ResolveCondition();
                }
            }

            source.Add(newError);
            Shaft.ShaftFeaturesErrors.Add(newError);
            if (newError.Section.IsBore)
            {
                if (newError.Section.BoreFromEdge == BoreFromEdge.FromLeft)
                {
                    Shaft.BoreOnTheLeft.ItemPropertyChanged += TryResolve;
                }
                else
                {
                    Shaft.BoreOnTheRight.ItemPropertyChanged += TryResolve;
                }
            }
            else
            {
                Shaft.Sections.ItemPropertyChanged += TryResolve;
            }

            newError.Feature.PropertyChanged += (sender, args) => ResolveCondition();

            if (newError.Feature is ISectionSubFeature)
            {
                newError.Section.SubFeatures.CollectionChanged += (sender, args) => ResolveCondition();
            }
        }

        public static bool ContainsError(this IEnumerable<ShaftSectionFeatureError> source,
                                         ShaftSectionFeatureError errorToCheck)
        {
            return source.Contains(errorToCheck);
        }

        public static ObservableCollectionEx<T> ToObservableCollectionEx<T>(this IEnumerable<T> enumerableList)
            where T : INotifyPropertyChanged
        {
            return enumerableList != null ? new ObservableCollectionEx<T>(enumerableList) : null;
        }

        public static int IndexOf<T>(this IEnumerable<T> source, T value)
        {
            int index = 0;
            var comparer = EqualityComparer<T>.Default; // or pass in as a parameter
            foreach (T item in source)
            {
                if (comparer.Equals(item, value)) return index;
                index++;
            }

            return -1;
        }

        public static IEnumerable<T> FastReverse<T>(this IList<T> source)
        {
            for (int i = source.Count - 1; i >= 0; i--)
            {
                yield return source[i];
            }
        }
    }
}