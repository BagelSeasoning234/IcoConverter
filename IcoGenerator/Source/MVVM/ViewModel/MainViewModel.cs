using System;
using IcoGenerator.Source.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IcoGenerator.Source.MVVM.Model;

namespace IcoGenerator.Source.MVVM.ViewModel
{
    /// <summary>
    /// The view model for the main view.
    /// </summary>
    class MainViewModel : ObservableObject
    {

        #region Commands

        /// <summary>
        /// Lets the ico converter know to convert from a png to an ico.
        /// </summary>
        public RelayCommand PngToIcoCommand { get; set; }
        /// <summary>
        /// Lets the ico converter know to convert from an ico to a png.
        /// </summary>
        public RelayCommand IcoToPngCommand { get; set; }

        /// <summary>
        /// Lets the user browse for the file to import.
        /// </summary>
        public RelayCommand BrowseImportCommand { get; set; }
        /// <summary>
        /// Lets the user browse for the folder to export the converted file.
        /// </summary>
        public RelayCommand BrowseExportCommand { get; set; }

        /// <summary>
        /// Converts the chosen file to the specified file type.
        /// </summary>
        public RelayCommand ConvertCommand { get; set; }

        #endregion

        #region Public Members

        /// <summary>
        /// The ico converter class to handle operations.
        /// </summary>
        public IcoConverter icoConverter { get; set; }

        #endregion

        public MainViewModel()
        {
            icoConverter = new IcoConverter();

            // Initialize commands
            PngToIcoCommand = new RelayCommand(o => { icoConverter.SetConvertMode(true); });
            IcoToPngCommand = new RelayCommand(o => { icoConverter.SetConvertMode(false); });

            BrowseImportCommand = new RelayCommand(o => { icoConverter.BrowseImportPath(); });
            BrowseExportCommand = new RelayCommand(o => { icoConverter.BrowseExportPath(); });

            ConvertCommand = new RelayCommand(o => { icoConverter.Convert(); });
        }
    }
}
