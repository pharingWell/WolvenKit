using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ReactiveUI;
using WolvenKit.App.ViewModels.Shell;
using WolvenKit.RED4.Types;

namespace WolvenKit.Views.Editors
{
    /// <summary>
    /// Interaction logic for RedFloatEditor.xaml
    /// </summary>
    public partial class RedFloatEditor
    {
        public RedFloatEditor()
        {
            InitializeComponent();

            Observable.FromEventPattern<TextChangedEventHandler, TextChangedEventArgs>(
                handler => TextBox.TextChanged += handler,
                handler => TextBox.TextChanged -= handler)
                .Throttle(TimeSpan.FromSeconds(.5))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x =>
                {
                    SetRedValue(TextBox.Text);
                });
        }

        public CFloat RedNumber
        {
            get => (CFloat)GetValue(RedNumberProperty);
            set => SetValue(RedNumberProperty, value);
        }
        public static readonly DependencyProperty RedNumberProperty = DependencyProperty.Register(
            nameof(RedNumber), typeof(CFloat), typeof(RedFloatEditor), new PropertyMetadata(default(CFloat)));


        public string Text
        {
            get => GetValueFromRedValue();
            set => SetRedValue(value);
        }

        private void SetRedValue(string value)
        {
            try
            {
                ViewModel.DataObject = (CFloat)float.Parse(value);
            }
            catch (FormatException)
            {

            }
        }

        private string GetValueFromRedValue() => ((float)(CFloat)ViewModel.DataObject).ToString("G9");
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var tb = (TextBox)e.Source;
            e.Handled = !float.TryParse(tb.Text.Insert(tb.CaretIndex, e.Text), out _);
        }

    }
}
