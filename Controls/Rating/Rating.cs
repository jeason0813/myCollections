using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace myCollections.Controls 
{

    /// <summary>
    /// Represents a Rating control
    /// </summary>
    public class Rating : Control
    {

        #region Declarations

        const String CATEGORY_NAME_BRUSHES = "Brushes";
        const String CATEGORY_NAME_CUSTOM = "Custom";
        private const Double MAXIMUM_VALUE = 5.0;
        private const Double STAR_WIDTH = 50.0;
        Double _saveValue;

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty EmptyStarBrushProperty =
            DependencyProperty.Register("EmptyStarBrush", typeof(Brush), typeof(Rating), new PropertyMetadata(new SolidColorBrush(Colors.WhiteSmoke)));

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(Boolean), typeof(Rating), new PropertyMetadata(false));

        public static readonly DependencyProperty ValueBrushProperty =
            DependencyProperty.Register("ValueBrush", typeof(Brush), typeof(Rating), new PropertyMetadata(new SolidColorBrush(Colors.Blue)));

        public static readonly DependencyProperty ValuePrecisionProperty =
            DependencyProperty.Register("ValuePrecision", typeof(Int32), typeof(Rating),
            new PropertyMetadata(1, null), ValuePrecisionValidateCallback);

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(Double), typeof(Rating),
            new PropertyMetadata(0.0, new PropertyChangedCallback(OnValueChanged)), ValueValidateCallback);

        public static readonly DependencyProperty ValueSelectorBackgroundProperty =
            DependencyProperty.Register("ValueSelectorBackground", typeof(Brush), typeof(Rating), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        internal static readonly DependencyProperty ValueWidthProperty =
            DependencyProperty.Register("ValueWidth", typeof(Double), typeof(Rating), new PropertyMetadata(0.0));
        
        static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            Rating r = d as Rating;

            if (r != null) {
                r.ValueWidth = ((Double)e.NewValue) * STAR_WIDTH;
            }
        }
        static Boolean ValuePrecisionValidateCallback(Object value) {
            Int32 v = (Int32)value;
            return v >= 0;
        }
        static Boolean ValueValidateCallback(Object value) {

            Double v = (Double)value;
            return v >= 0 && v <= MAXIMUM_VALUE;
        }

        /// <summary>
        /// Gets or sets the brush used to paint empty stars.
        /// </summary>
        /// <value>The empty star brush.</value>
        [Category(CATEGORY_NAME_BRUSHES)]
        [Description("Gets or sets the brush used to paint empty stars")]
        public Brush EmptyStarBrush {
            get { return (Brush)GetValue(EmptyStarBrushProperty); }
            set { SetValue(EmptyStarBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        [Category(CATEGORY_NAME_CUSTOM)]
        [Description("Gets or sets a value indicating whether this instance is read only")]
        public Boolean IsReadOnly {
            get { return (Boolean)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [Category(CATEGORY_NAME_CUSTOM)]
        [Description("Gets or sets the value")]
        public Double Value {
            get { return (Double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value brush.  This brush is used to paint the 5 stars as the value increases.
        /// </summary>
        /// <value>The value brush.</value>
        [Category(CATEGORY_NAME_BRUSHES)]
        [Description("Gets or sets the value brush.  This brush is used to paint the 5 stars as the value increases")]
        public Brush ValueBrush {
            get { return (Brush)GetValue(ValueBrushProperty); }
            set { SetValue(ValueBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value precision or how many digits to round the value to.  
        /// </summary>
        /// <value>The value precision.</value>
        [Category(CATEGORY_NAME_CUSTOM)]
        [Description("Gets or sets the value precision or how many digits to round the value to")]
        public Int32 ValuePrecision {
            get { return (Int32)GetValue(ValuePrecisionProperty); }
            set { SetValue(ValuePrecisionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value selector background.  The value selector background is the area inside the control but outside the 5 stars.
        /// </summary>
        /// <value>The value selector background.</value>
        [Category(CATEGORY_NAME_BRUSHES)]
        [Description("Gets or sets the value selector background.  The value selector background is the area inside the control but outside the 5 stars")]
        public Brush ValueSelectorBackground {
            get { return (Brush)GetValue(ValueSelectorBackgroundProperty); }
            set { SetValue(ValueSelectorBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the width of the value rectangle.
        /// </summary>
        /// <value>The width of the value.</value>
        /// <remarks>
        /// This needs to be available for TemplateBinding, but is not visible in the properties window or code editor.
        /// Additionally it has internal scope to limit visibility to the current assembly only.
        /// </remarks>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Double ValueWidth {
            get { return (Double)GetValue(ValueWidthProperty); }
            set { SetValue(ValueWidthProperty, value); }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the <see cref="Rating"/> class.
        /// </summary>
        static Rating() 
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Rating), new FrameworkPropertyMetadata(typeof(Rating)));
        }

        #endregion

        #region Methods

        public override void OnApplyTemplate() 
        {
            base.OnApplyTemplate();
            ValueWidth = Value * STAR_WIDTH;
        }

        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e) {
            base.OnMouseEnter(e);

            if (!IsReadOnly)
                _saveValue = Value;
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e) {
            base.OnMouseLeave(e);

            if (!IsReadOnly)
                Value = _saveValue;
        }

        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e) {
            base.OnMouseLeftButtonDown(e);

            if (!IsReadOnly)
                _saveValue = Value;
        }

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e) {
            base.OnMouseMove(e);

            if (!IsReadOnly) {

                Double ratingWidth = ActualWidth;

                Double mouseX = e.GetPosition(this).X;

                if (Math.Abs(mouseX - 0) < double.Epsilon) 
                    Value = 0;
                else if (mouseX >= ratingWidth) 
                    Value = MAXIMUM_VALUE;
                else 
                    Value = Math.Round((mouseX / ratingWidth) * MAXIMUM_VALUE, ValuePrecision);
            }
        }

        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e) {
            base.OnKeyDown(e);
            String key = e.Key.ToString();

            if (key.StartsWith("D")) {
                e.Handled = SetValueIfWithRange(key.Remove(0, 1));

            } else if (key.StartsWith("NumPad")) {
                e.Handled = SetValueIfWithRange(key.Remove(0, 6));

            }
        }

        Boolean SetValueIfWithRange(String key) {
            Int32 value;
            if (Int32.TryParse(key, out value) && value >= 0 && value <= MAXIMUM_VALUE) {
                Value = value;
                return true;
            }

            return false;
        }

        #endregion
    }
}
