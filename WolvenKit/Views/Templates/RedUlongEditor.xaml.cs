using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WolvenKit.RED4.Types;

namespace WolvenKit.Views.Editors
{
    /// <summary>
    /// Interaction logic for RedFloatEditor.xaml
    /// </summary>
    public partial class RedUlongEditor
    {
        public RedUlongEditor() => InitializeComponent();

        public IRedPrimitive<ulong> RedNumber
        {
            get => (IRedPrimitive<ulong>)GetValue(RedNumberProperty);
            set => SetValue(RedNumberProperty, value);
        }
        public static readonly DependencyProperty RedNumberProperty = DependencyProperty.Register(
            nameof(RedNumber), typeof(IRedPrimitive<ulong>), typeof(RedUlongEditor), new PropertyMetadata(default(IRedPrimitive<ulong>)));


        public string Text
        {
            get => GetValueFromRedValue();
            set => SetRedValue(value);
        }

        private void SetRedValue(string value)
        {
            switch (ViewModel.DataObject)
            {
                case CRUID:
                    ViewModel.DataObject = (CRUID)ulong.Parse(value);
                    break;
                case CUInt64:
                    ViewModel.DataObject = (CUInt64)ulong.Parse(value);
                    break;
                case TweakDBID:
                    ViewModel.DataObject = (TweakDBID)ulong.Parse(value);
                    break;
                default:
                    break;
            }
        }

        private string GetValueFromRedValue() => ViewModel.DataObject switch
        {
            CRUID cruid => ((ulong)cruid).ToString(),
            CUInt64 uint64 => ((ulong)uint64).ToString(),
            TweakDBID tdbid => ((ulong)tdbid).ToString(),
            _ => throw new ArgumentOutOfRangeException(nameof(ViewModel.DataObject)),
        };

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var tb = (TextBox)e.Source;
            e.Handled = !ulong.TryParse(tb.Text.Insert(tb.CaretIndex, e.Text), out _);
        }
    }
}
