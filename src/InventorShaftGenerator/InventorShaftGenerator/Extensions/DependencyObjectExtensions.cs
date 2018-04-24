using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace InventorShaftGenerator.Extensions
{
    public static class DependencyObjectExtensions
    {
        /*public static T GetVisualParent<T>(this DependencyObject child) where T : Visual
        {
            while ((child != null) && !(child is T))
            {
                child = VisualTreeHelper.GetParent(child);
            }

            return child as T;
        }*/

        public static IEnumerable<DependencyObject> EnumerateVisualChildren(this DependencyObject dependencyObject)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++)
            {
                yield return VisualTreeHelper.GetChild(dependencyObject, i);
            }
        }

        public static IEnumerable<DependencyObject> EnumerateVisualDescendents(this DependencyObject dependencyObject)
        {
            yield return dependencyObject;

            foreach (DependencyObject child in dependencyObject.EnumerateVisualChildren())
            {
                foreach (DependencyObject descendent in child.EnumerateVisualDescendents())
                {
                    yield return descendent;
                }
            }
        }

        public static void UpdateBindingSources(this DependencyObject dependencyObject)
        {
            foreach (DependencyObject element in dependencyObject.EnumerateVisualDescendents())
            {
                LocalValueEnumerator localValueEnumerator = element.GetLocalValueEnumerator();
                while (localValueEnumerator.MoveNext())
                {
                    BindingExpressionBase bindingExpression =
                        BindingOperations.GetBindingExpressionBase(element, localValueEnumerator.Current.Property);
                    bindingExpression?.UpdateSource();
                }
            }
        }

        public static T FindChild<T>(this DependencyObject depObj, Func<T, bool> predicate = null)
            where T : DependencyObject
        {
            if (depObj == null)
            {
                return null;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var result = (child as T) ?? FindChild(child, predicate);
                if (result != null && (predicate?.Invoke(result) ?? true))
                {
                    return result;
                }
            }

            return null;
        }

        public static T FindAncestor<T>(this DependencyObject depObj, int? ancestorLevel,
                                        Func<T, bool> condition = null) where T : DependencyObject
        {
            if (depObj == null)
            {
                return null;
            }

            int level = 0;
            while (true)
            {
                var parent = VisualTreeHelper.GetParent(depObj);

                switch (parent)
                {
                    case null:
                        return null;

                    case T neededParent when (!ancestorLevel.HasValue || ++level == ancestorLevel) &&
                                             (condition?.Invoke(neededParent) ?? true):
                        return neededParent;
                }

                depObj = parent;
            }
        }

        public static T FindAncestor<T>(this DependencyObject depObj, Func<T, bool> condition)
            where T : DependencyObject
        {
            return depObj.FindAncestor(null, condition);
        }

        public static T FindAncestor<T>(this DependencyObject depObj) where T : DependencyObject
        {
            return depObj.FindAncestor<T>(null);
        }

        public static IEnumerable<T> FindChildren<T>(this DependencyObject depObj, Func<T, bool> condition = null)
            where T : DependencyObject
        {
            if (depObj == null)
            {
                yield break;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                if (child is T neededChild && (condition?.Invoke(neededChild) ?? true))
                {
                    yield return neededChild;
                }

                foreach (T childOfChild in FindChildren<T>(child, condition))
                {
                    yield return childOfChild;
                }
            }
        }

        public static Rect BoundsRelativeTo(this FrameworkElement element,
                                            Visual relativeTo)
        {
            return
                element.TransformToVisual(relativeTo)
                       .TransformBounds(LayoutInformation.GetLayoutSlot(element));
        }
    }
}