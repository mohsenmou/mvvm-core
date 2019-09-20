using System;
using System.Windows;

namespace Mohsenmou.MVVM.Core
{
    public static class FocusExtension
    {
        public static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.RegisterAttached("IsFocused", typeof(bool?), typeof(FocusExtension), 
                new FrameworkPropertyMetadata(new PropertyChangedCallback(OnIsFocusedChanged)));

        public static bool? GetIsFocused(DependencyObject element)
        {
            if (element==null)
                throw new ArgumentNullException("element");

            return (bool?)element.GetValue(IsFocusedProperty);
        }
        public static void SetIsFocused(DependencyObject element, bool? value)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            element.SetValue(IsFocusedProperty, value);
        }
        private static void Fe_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var fe = (FrameworkElement)sender;
            if (fe.IsVisible && (bool)((FrameworkElement)sender).GetValue(IsFocusedProperty))
            {
                fe.IsVisibleChanged -= Fe_IsVisibleChanged;
                fe.Focus();
            }
        }
        private static void GotFocus(object sender, RoutedEventArgs e)
        {
            ((FrameworkElement)sender).SetValue(IsFocusedProperty, true);
        }
        private static void LostFocus(object sender, RoutedEventArgs e)
        {
            ((FrameworkElement)sender).SetValue(IsFocusedProperty, false);
        }
        private static void OnIsFocusedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var fe = (FrameworkElement)d;
            if (e.OldValue==null)
            {
                fe.GotFocus += GotFocus;
                fe.LostFocus += LostFocus;
            }
            if (!fe.IsVisible)
            {
                fe.IsVisibleChanged += new DependencyPropertyChangedEventHandler(Fe_IsVisibleChanged);
            }
            if (e.NewValue!=null)
            {
                if ((bool)e.NewValue)
                {
                    fe.Focus();
                }
            }
        }
    }
}
