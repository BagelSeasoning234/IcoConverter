using System;
using IcoGenerator.Source.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace IcoGenerator.Source.MVVM.ViewModel
{
    /// <summary>
    /// The view model for the window.
    /// </summary>
    class WindowViewModel : ObservableObject
    {

        #region Commands

        /// <summary>
        /// The command to minimize the window.
        /// </summary>
        public RelayCommand MinimizeCommand { get; set; }
        /// <summary>
        /// The command to maximize the window.
        /// </summary>
        public RelayCommand MaximizeCommand { get; set; }
        /// <summary>
        /// The command to close the window.
        /// </summary>
        public RelayCommand CloseCommand { get; set; }
        /// <summary>
        /// The command to open the menu when the icon is clicked.
        /// </summary>
        public RelayCommand MenuCommand { get; set; }

        #endregion

        #region Private Members

        /// <summary>
        /// The window this view model controls.
        /// </summary>
        private Window m_Window;

        /// <summary>
        /// The internal object for the view that is currently shown onscreen.
        /// </summary>
        private object m_CurrentView;

        /// <summary>
        /// The internal int for the minimum height of the window.
        /// </summary>
        private int m_WindowMinimumHeight = 400;

        /// <summary>
        /// The margin around the window to allow for a drop shadow.
        /// </summary>
        private int m_OuterMarginSize = 1;

        #endregion

        #region Public Members

        /// <summary>
        /// The height of the titlebar / the draggable area at the top.
        /// </summary>
        public int TitleHeight { get; set; } = 32;

        /// <summary>
        /// The minimum height of the window, compensating for the title height.
        /// </summary>
        public int WindowMinimumHeight { get { return m_WindowMinimumHeight + TitleHeight; } set { m_WindowMinimumHeight = value; } }

        /// <summary>
        /// The minimum width of the window.
        /// </summary>
        public int WindowMinimumWidth { get; set; } = 700;

        /// <summary>
        /// The size of the resize border around the window, taking into account the outer margin.
        /// </summary>
        public Thickness ResizeBorderThickness { get { return new Thickness(ResizeBorder + OuterMarginSize); } }

        /// <summary>
        /// The size of the resizable border around the window.
        /// </summary>
        public int ResizeBorder { get { return m_Window.WindowState == WindowState.Maximized ? 0 : 5; } }

        /// <summary>
        /// The margin around the window to allow for a drop shadow.
        /// </summary>
        public int OuterMarginSize
        {
            get
            {
                // No border if the window is maximized.
                return m_Window.WindowState == WindowState.Maximized ? 0 : m_OuterMarginSize;
            }
            set
            {
                m_OuterMarginSize = value;
            }
        }

        /// <summary>
        /// The margin thickness around the window to allow for a drop shadow.
        /// </summary>
        public Thickness OuterMarginSizeThickness { get { return new Thickness(OuterMarginSize); } }

        #endregion

        #region Views

        public MainViewModel MainVm { get; set; }

        // The view that is currently shown onscreen.
        public object CurrentView
        {
            get { return m_CurrentView; }
            set 
            { 
                m_CurrentView = value;
                OnPropertyChanged();
            }
        }

        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="window">The window for this view model to control.</param>
        public WindowViewModel( Window window)
        {
            m_Window = window;

            MainVm = new MainViewModel();

            CurrentView = MainVm;

            // Instantiate commands
            MinimizeCommand = new RelayCommand(o => { m_Window.WindowState = WindowState.Minimized; });
            MaximizeCommand = new RelayCommand(o => { m_Window.WindowState ^= WindowState.Maximized; });
            CloseCommand = new RelayCommand(o => { m_Window.Close(); });
            MenuCommand = new RelayCommand(o => { SystemCommands.ShowSystemMenu(m_Window, GetMouseLocation(window)); });

            // Call initial functions
            SetWindowMinimums();
        }

        #region Private Helpers

        /// <summary>
        /// Sets the minimum size of the window.
        /// </summary>
        private void SetWindowMinimums()
        {
            m_Window.MinHeight = WindowMinimumHeight;
            m_Window.MinWidth = WindowMinimumWidth;
        }

        /// <summary>
        /// Gets the current mouse position on the screen
        /// </summary>
        private Point GetMouseLocation(Window window)
        {
            // Position of the mouse relative to the window.
            var position = Mouse.GetPosition(m_Window);

            if (window.WindowState == WindowState.Normal)
            {
                // Adds the window position so the final position is located on the window.
                return new Point(position.X + m_Window.Left, position.Y + m_Window.Top);
            }
            else
            {
                // When maximized, returns the absolute position of the cursor.
                return new Point(position.X, position.Y);
            }

        }

        #endregion
    }
}
