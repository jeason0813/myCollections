using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace myCollections.Utils
{
    internal class SwitchBindingExtension : Binding
    {
        public SwitchBindingExtension()
        {
            Initialize();
        }

        public SwitchBindingExtension(string path): base(path)
        {
            Initialize();
        }

        public SwitchBindingExtension(string path, object valueIfTrue, object valueIfFalse): base(path)
        {
            Initialize();

            if (valueIfTrue.GetType().Name == "ResourceReferenceExpression")
            {
                string True = (valueIfTrue.GetType().GetProperty("ResourceKey").GetValue(valueIfTrue,null)).ToString();
                string False = (valueIfFalse.GetType().GetProperty("ResourceKey").GetValue(valueIfFalse, null)).ToString();

                if (string.IsNullOrWhiteSpace(True)==false)
                    ValueIfTrue = ((App) Application.Current).LoadedLanguageResourceDictionary[True];

                if (string.IsNullOrWhiteSpace(False)==false)
                    ValueIfFalse = ((App)Application.Current).LoadedLanguageResourceDictionary[False];
            }

        }

        public SwitchBindingExtension(string path, string valueIfTrue, string valueIfFalse): base(path)
        {
            Initialize();
            this.ValueIfTrue = valueIfTrue;
            this.ValueIfFalse = valueIfFalse;
        }

        private void Initialize()
        {
          //  this.ValueIfTrue = Binding.DoNothing;
           // this.ValueIfFalse = Binding.DoNothing;
            this.Converter = new SwitchConverter(this);
            this.Mode=BindingMode.OneTime;
        }

        [ConstructorArgument("valueIfTrue")]
        public object ValueIfTrue { get; set; }

        [ConstructorArgument("valueIfFalse")]
        public object ValueIfFalse { get; set; }

        private class SwitchConverter : IValueConverter
        {
            public SwitchConverter(SwitchBindingExtension switchExtension)
            {
                _switch = switchExtension;
            }

            private readonly SwitchBindingExtension _switch;

            #region IValueConverter Members

            public object Convert(object value, Type targetType, object parameter,
                                  System.Globalization.CultureInfo culture)
            {
                try
                {
                    bool b = System.Convert.ToBoolean(value);
                    return b ? _switch.ValueIfTrue : _switch.ValueIfFalse;
                }
                catch
                {
                    return DependencyProperty.UnsetValue;
                }
            }

            public object ConvertBack(object value, Type targetType, object parameter,
                                      System.Globalization.CultureInfo culture)
            {
                return Binding.DoNothing;
            }

            #endregion
        }
    }
}