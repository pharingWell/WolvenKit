using System;
using System.Windows;
using System.Windows.Controls;
using ReactiveUI;
using WolvenKit.RED4.Types;

namespace WolvenKit.Views.Editors
{
    /// <summary>
    /// Interaction logic for RedIntegerEditor.xaml
    /// </summary>
    public partial class RedIntegerEditor
    {
        public RedIntegerEditor()
        {
            InitializeComponent();
        }

        #region properties

        public IRedInteger RedInteger
        {
            get => (IRedInteger)GetValue(RedIntegerProperty);
            set => SetValue(RedIntegerProperty, value);
        }
        public static readonly DependencyProperty RedIntegerProperty = DependencyProperty.Register(
            nameof(RedInteger), typeof(IRedInteger), typeof(RedIntegerEditor), new PropertyMetadata(default(IRedInteger)));

        public int NumberDecimalDigits => GetNumberDecimalDigits();

        public double MinValue => GetMinValue();

        public double MaxValue => GetMaxValue();

        public double Value
        {
            get => GetValueFromRedValue();
            set => SetRedValue(value);
        }

        #endregion

        #region methods

        private int GetNumberDecimalDigits() => ViewModel.DataObject switch
        {
            CDouble => 17,
            CUInt8 => 0,
            CInt8 => 0,
            CInt16 => 0,
            CUInt16 => 0,
            CInt32 => 0,
            CUInt32 => 0,
            CInt64 => 0,
            CFloat  => 9,
            _ => throw new ArgumentOutOfRangeException(nameof(ViewModel.DataObject)),
        };

        private double GetMinValue()
        {
            if (ViewModel != null)
            {
                return ViewModel.DataObject switch
                {
                    CDouble => double.MinValue,
                    CUInt8 => byte.MinValue,
                    CInt8 => sbyte.MinValue,
                    CInt16 => short.MinValue,
                    CUInt16 => ushort.MinValue,
                    CInt32 => int.MinValue,
                    CUInt32 => uint.MinValue,
                    CInt64 => long.MinValue,
                    CFloat => float.MinValue,
                    _ => throw new ArgumentOutOfRangeException(nameof(ViewModel.DataObject))
                };
            }
            return double.MinValue;
        }

        private double GetMaxValue()
        {
            if (ViewModel != null)
            {
                return ViewModel.DataObject switch
                {
                    CDouble => double.MaxValue,

                    CUInt8 => byte.MaxValue,
                    CInt8 => sbyte.MaxValue,
                    CInt16 => short.MaxValue,
                    CUInt16 => ushort.MaxValue,
                    CInt32 => int.MaxValue,
                    CUInt32 => uint.MaxValue,
                    CInt64 => long.MaxValue,
                    CFloat => float.MaxValue,
                    _ => throw new ArgumentOutOfRangeException(nameof(ViewModel.DataObject))
                };
            }
            return double.MinValue;
        }

        private void SetRedValue(double value)
        {
            switch (ViewModel.DataObject)
            {
                case CDouble:
                    ViewModel.DataObject = (CDouble)value;
                    break;
                case CFloat:
                    ViewModel.DataObject = (CFloat)value;
                    break;
                case CUInt8:
                    ViewModel.DataObject = (CUInt8)value;
                    break;
                case CInt8:
                    ViewModel.DataObject = (CInt8)value;
                    break;
                case CInt16:
                    ViewModel.DataObject = (CInt16)value;
                    break;
                case CUInt16:
                    ViewModel.DataObject = (CUInt16)value;
                    break;
                case CInt32:
                    ViewModel.DataObject = (CInt32)value;
                    break;
                case CUInt32:
                    ViewModel.DataObject = (CUInt32)value;
                    break;
                case CInt64:
                    ViewModel.DataObject = (CInt64)value;
                    break;
                default:
                    break;
            }

        }

        private double GetValueFromRedValue() => ViewModel.DataObject switch
        {
            CDouble cruid => (double)cruid,
            CUInt8 uint64 => uint64,
            CInt8 uint64 => uint64,
            CInt16 uint64 => uint64,
            CUInt16 uint64 => uint64,
            CInt32 uint64 => uint64,
            CUInt32 uint64 => uint64,
            CInt64 uint64 => uint64,
            CFloat uint64 => uint64,
            _ => throw new ArgumentOutOfRangeException(nameof(ViewModel.DataObject)),
        };

        #endregion
    }
}
