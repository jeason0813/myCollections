using System;
using System.Windows;
using System.Windows.Controls;

namespace myCollections.Controls
{
    [TemplateVisualState(GroupName = "Busy", Name = IsBusyStateName)]
    [TemplateVisualState(GroupName = "Busy", Name = NotBusyStateName)]
    public class BusyIndicator : Control
    {
        private const String IsBusyStateName = "IsBusy";
        private const String NotBusyStateName = "NotBusy";

        public bool IsBusy
        {
            private get { return (bool)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }

        public String BusyMessage
        {
            get { return (String)GetValue(BusyMessageProperty); }
            set { SetValue(BusyMessageProperty, value); }
        }

        public static readonly DependencyProperty BusyMessageProperty = DependencyProperty.Register("BusyMessage", typeof(String), typeof(BusyIndicator), new PropertyMetadata(null));
        public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register("IsBusy", typeof(bool), typeof(BusyIndicator), new PropertyMetadata(false, OnBusyIndicatorPropertyChanged));

        static BusyIndicator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BusyIndicator), new FrameworkPropertyMetadata(typeof(BusyIndicator)));
        }

        private static void OnBusyIndicatorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            BusyIndicator busyIndicator = (BusyIndicator)sender;
            busyIndicator.UpdateVisualState(true);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            UpdateVisualState(false);
        }

        private void UpdateVisualState(bool useTransitions)
        {
            if (IsBusy)
                VisualStateManager.GoToState(this, IsBusyStateName, useTransitions);
            else
                VisualStateManager.GoToState(this, NotBusyStateName, useTransitions);
        }
    }
}
