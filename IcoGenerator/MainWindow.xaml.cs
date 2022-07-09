using IcoGenerator.Source.MVVM.ViewModel;
using IcoGenerator.Source.Vendor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IcoGenerator
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        WindowResizer m_WindowResizer;

        // Constructor
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new WindowViewModel(this);

            m_WindowResizer = new WindowResizer(MinHeight, MinWidth);
            // Initializes the window resizer.
            SourceInitialized += (s, e) =>
            {
                m_WindowResizer.WindowCompositionTarget = PresentationSource.FromVisual(this).CompositionTarget;
                HwndSource.FromHwnd(new WindowInteropHelper(this).Handle).AddHook(m_WindowResizer.WindowProc);
            };
        }
    }
}
