using Lumeer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Lumeer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NullableDatePicker : ContentView
    {
        public static readonly BindableProperty NullableDateTimeProperty =
            BindableProperty.Create(nameof(NullableDate),
                typeof(DateTime?),
                typeof(NullableDatePicker),
                defaultValue: null,
                defaultBindingMode: BindingMode.TwoWay,
                propertyChanged: NullableDateTimePropertyChanged);

        public static void NullableDateTimePropertyChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            var nullableDatePicker = (NullableDatePicker)bindableObject;
            var newDate = (DateTime?)newValue;
            nullableDatePicker.dateLabel.Text = newDate?.ToString(nullableDatePicker.Format);
        }

        public DateTime? NullableDate 
        {
            get => (DateTime?)GetValue(NullableDateTimeProperty);
            set => SetValue(NullableDateTimeProperty, value);
        }
        
        public string Format
        {
            get => (string)GetValue(DatePicker.FormatProperty);
            set => SetValue(DatePicker.FormatProperty, value);
        }

        public NullableDatePicker(DateTime? nullableDate, string format)
        {
            InitializeComponent();
            BindingContext = this;

            Format = format;
            NullableDate = nullableDate;

            if (nullableDate.HasValue)
            {
                datePicker.Date = nullableDate.Value;
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            datePicker.Focus(); // Displays DatePicker
        }

        private void datePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            NullableDate = e.NewDate;
        }
    }
}