﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WPF_N_Tier_Test.Common.UI.UX
{
    public class SmoothScrollViewer : ScrollViewer
    {
        private double _totalVerticalOffset;

        private bool _isRunning;

        /// <summary>
        ///     CanMouseWheelProperty
        /// </summary>
        public static readonly DependencyProperty CanMouseWheelProperty = DependencyProperty.Register(
            nameof(CanMouseWheel), typeof(bool), typeof(SmoothScrollViewer), new PropertyMetadata(true));

        /// <summary>
        ///     CanMouseWheel
        /// </summary>
        public bool CanMouseWheel
        {
            get => (bool)GetValue(CanMouseWheelProperty);
            set => SetValue(CanMouseWheelProperty, value);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (!CanMouseWheel)
            {
                return;
            }

            e.Handled = true;
            if (!_isRunning)
            {
                _totalVerticalOffset = VerticalOffset;
                CurrentVerticalOffset = VerticalOffset;
            }
            _totalVerticalOffset = Math.Min(Math.Max(0, _totalVerticalOffset - e.Delta * 2), ScrollableHeight);
            ScrollToVerticalOffsetWithAnimation(_totalVerticalOffset);


        }

        internal void ScrollToTopInternal(double milliseconds = 300)
        {
            if (!_isRunning)
            {
                _totalVerticalOffset = VerticalOffset;
                CurrentVerticalOffset = VerticalOffset;
            }
            ScrollToVerticalOffsetWithAnimation(0, milliseconds);
        }

        public static DoubleAnimation CreateAnimation(double toValue, double milliseconds = 200)
        {
            return new(toValue, new Duration(TimeSpan.FromMilliseconds(milliseconds)))
            {
                EasingFunction = new CircleEase { EasingMode = EasingMode.EaseInOut }
            };
        }
        public void ScrollToVerticalOffsetWithAnimation(double offset, double milliseconds = 350)
        {

            var animation = CreateAnimation(offset, milliseconds);

            animation.EasingFunction = new CubicEase
            {
                EasingMode = EasingMode.EaseOut
            };
            animation.FillBehavior = FillBehavior.Stop;
            animation.Completed += (s, e1) =>
            {
                CurrentVerticalOffset = offset;
                _isRunning = false;
            };
            _isRunning = true;
            BeginAnimation(CurrentVerticalOffsetProperty, animation, HandoffBehavior.SnapshotAndReplace);
        }

        public void ScrollToHorizontalOffsetWithAnimation(double offset, double milliseconds = 300)
        {
            var animation = CreateAnimation(offset, milliseconds);
            animation.EasingFunction = new CubicEase
            {
                EasingMode = EasingMode.EaseOut
            };
            animation.FillBehavior = FillBehavior.Stop;
            animation.Completed += (s, e1) =>
            {
                CurrentHorizontalOffset = offset;
                _isRunning = false;
            };
            _isRunning = true;

            BeginAnimation(CurrentHorizontalOffsetProperty, animation, HandoffBehavior.Compose);
        }

        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters) =>
            IsPenetrating ? null : base.HitTestCore(hitTestParameters);


        public static readonly DependencyProperty IsInertiaEnabledProperty = DependencyProperty.RegisterAttached(
            "IsInertiaEnabled", typeof(bool), typeof(SmoothScrollViewer), new PropertyMetadata(false));

        public static void SetIsInertiaEnabled(DependencyObject element, bool value) => element.SetValue(IsInertiaEnabledProperty, value);

        public static bool GetIsInertiaEnabled(DependencyObject element) => (bool)element.GetValue(IsInertiaEnabledProperty);


        public bool IsInertiaEnabled
        {
            get => (bool)GetValue(IsInertiaEnabledProperty);
            set => SetValue(IsInertiaEnabledProperty, value);
        }

        public static readonly DependencyProperty IsPenetratingProperty = DependencyProperty.RegisterAttached(
            "IsPenetrating", typeof(bool), typeof(SmoothScrollViewer), new PropertyMetadata(false));


        public bool IsPenetrating
        {
            get => (bool)GetValue(IsPenetratingProperty);
            set => SetValue(IsPenetratingProperty, value);
        }

        public static void SetIsPenetrating(DependencyObject element, bool value) => element.SetValue(IsPenetratingProperty, value);

        public static bool GetIsPenetrating(DependencyObject element) => (bool)element.GetValue(IsPenetratingProperty);


        internal static readonly DependencyProperty CurrentVerticalOffsetProperty = DependencyProperty.Register(
            nameof(CurrentVerticalOffset), typeof(double), typeof(SmoothScrollViewer), new PropertyMetadata(0.0, OnCurrentVerticalOffsetChanged));

        private static void OnCurrentVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SmoothScrollViewer ctl && e.NewValue is double v)
            {
                ctl.ScrollToVerticalOffset(v);
            }
        }


        internal double CurrentVerticalOffset
        {
            // ReSharper disable once UnusedMember.Local
            get => (double)GetValue(CurrentVerticalOffsetProperty);
            set => SetValue(CurrentVerticalOffsetProperty, value);
        }

        internal static readonly DependencyProperty CurrentHorizontalOffsetProperty = DependencyProperty.Register(
            nameof(CurrentHorizontalOffset), typeof(double), typeof(SmoothScrollViewer), new PropertyMetadata(0.0, OnCurrentHorizontalOffsetChanged));

        private static void OnCurrentHorizontalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SmoothScrollViewer ctl && e.NewValue is double v)
            {
                ctl.ScrollToHorizontalOffset(v);
            }
        }


        internal double CurrentHorizontalOffset
        {
            get => (double)GetValue(CurrentHorizontalOffsetProperty);
            set => SetValue(CurrentHorizontalOffsetProperty, value);
        }


    }

}
