using ReactiveUI;
using WolvenKit.App.ViewModels.Shell;

namespace WolvenKit.Views.Editors
{
    /// <summary>
    /// Interaction logic for RedTypeView.xaml
    /// </summary>
    public partial class RedTypeView : ReactiveUserControl<PropertyViewModel>
    {
        public RedTypeView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                if (DataContext is PropertyViewModel vm)
                {
                    SetCurrentValue(ViewModelProperty, vm);
                }
            });
        }
    }
}
