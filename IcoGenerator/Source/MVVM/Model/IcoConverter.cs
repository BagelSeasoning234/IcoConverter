using IcoGenerator.Source.Core;
using IcoGenerator.Source.Vendor;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace IcoGenerator.Source.MVVM.Model
{

    /// <summary>
    /// A class for converting between png and ico files.
    /// </summary>
    class IcoConverter : ObservableObject
    {

        #region Private Members

        /// <summary>
        /// The list of resolution options.
        /// </summary>
        private ObservableCollection<string> m_ResolutionOptions;

        /// <summary>
        /// The target resolution for the converted image.
        /// </summary>
        private string m_TargetResolution;

        /// <summary>
        /// The path to the original file to be converted.
        /// </summary>
        private string m_ImportPath;

        /// <summary>
        /// The path to the folder to export the new file.
        /// </summary>
        private string m_ExportPath;

        /// <summary>
        /// Determines which direction to convert. 
        /// True for png to ico, false for ico to png.
        /// </summary>
        private bool ConvertMode = true;

        /// <summary>
        /// Determines whether the aspect ratio should be preserved.
        /// </summary>
        private bool m_PreserveAspectRatio { get; set; } = false;

        /// <summary>
        /// The status of the current conversion.
        /// </summary>
        private string m_Status;

        #endregion

        #region Public Members

        /// <summary>
        /// The list of resolution options
        /// </summary>
        public ObservableCollection<string> ResolutionOptions 
        {
            get { return m_ResolutionOptions; } 
            set 
            { 
                m_ResolutionOptions = value;
                OnPropertyChanged();
            } 
        }

        /// <summary>
        /// The target resolution for the converted image.
        /// </summary>
        public string TargetResolution
        {
            get { return m_TargetResolution; }
            set
            {
                m_TargetResolution = value; 
                OnPropertyChanged(); 
            }
        }

        /// <summary>
        /// The path to the original file to be converted.
        /// </summary>
        public string ImportPath
        {
            get { return m_ImportPath; }
            set 
            { 
                m_ImportPath = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The path to the folder to export the new file.
        /// </summary>
        public string ExportPath
        {
            get { return m_ExportPath; }
            set 
            { 
                m_ExportPath = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Determines whether the aspect ratio should be preserved.
        /// </summary>
        public bool PreserveAspectRatio
        {
            get { return m_PreserveAspectRatio; }
            set
            {
                m_PreserveAspectRatio = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The status of the current conversion.
        /// </summary>
        public string Status
        {
            get { return m_Status; }
            set 
            { 
                m_Status = value;
                OnPropertyChanged();
            }
        }

        #endregion

        // Constructor
        public IcoConverter()
        {
            InitializeResolutionOptions();

        }

        #region Private Methods

        /// <summary>
        /// Converts an ico image to a png.
        /// </summary>
        /// <returns>The png object.</returns>
        private Bitmap PngFromIcon()
        {
            Icon icon = new Icon(ImportPath);
            Bitmap png = null;

            // Creates a memory stream for the icon, and then decodes it.
            var iconStream = new System.IO.MemoryStream();

            icon.Save(iconStream);
            var decoder = new IconBitmapDecoder(iconStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);

            // Creates a memory stream for the png, and then encodes it, and adding the decoded icon frame.
            var pngStream = new System.IO.MemoryStream();

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(decoder.Frames[0]);
            encoder.Save(pngStream);

            // Creates a bitmap from the memory stream and returns it.
            png = (Bitmap)Bitmap.FromStream(pngStream);

            int width = GetTargetResolution();
            if (width == -1)
                return null;

            int height = width; //Already validated the size when checking width.

            Bitmap resizedImage = (Bitmap)ResizeImage(png, new System.Drawing.Size(width, height));

            return resizedImage;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the conversion mode for the file.
        /// True for png to ico, false for ico to png.
        /// </summary>
        public void SetConvertMode(bool convertMode)
        {
            ConvertMode = convertMode;

            // Clear the current path (so the user doesn't try to convert the wrong file type).
            ImportPath = string.Empty;
        }

        /// <summary>
        /// Opens the file explorer so the user can browse to the file to import.
        /// </summary>
        public void BrowseImportPath()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            string filter;

            if (ConvertMode)
                filter = "Image Files (*.png)|*.png";
            else
                filter = "Ico Files (*.ico)|*.ico";

            dialog.Filter = filter;

            // Fires once the user has selected the file.
            if (dialog.ShowDialog() == true)
            {
                ImportPath = dialog.FileName;
            }
        }

        /// <summary>
        /// Opens the file explorer so the user can browse to the folder to export the converted file.
        /// </summary>
        public void BrowseExportPath()
        {
            // We call the vista dialog from Ookii
            // because for some reason wpf doesn't support folders by default. WHY.
            var dialog = new VistaFolderBrowserDialog();

            // Fires once the user has selected the folder.
            if ((bool)dialog.ShowDialog())
            {
                ExportPath = dialog.SelectedPath;
            }
        }

        /// <summary>
        /// Converts the imported file to the type specified earlier.
        /// </summary>
        public void Convert()
        {
            if (!ValidateFile())
            {
                Status = "Error.";
                return;
            }

            if (ConvertMode)
            {
                // Png to ico
                string export = ExportPath + "\\" + Path.GetFileNameWithoutExtension(ImportPath) + "_ICO" + ".ico";
                int size = GetTargetResolution();
                if (size == -1)
                {
                    Status = "Error.";
                    return;
                }

                if (ImagingHelper.ConvertToIcon(ImportPath, export, size, PreserveAspectRatio))
                    Status = "Success.";
                else
                    Status = "Error.";

            }
            else
            {
                // Ico to png
                Image image = PngFromIcon();
                string export = ExportPath + "\\" + Path.GetFileNameWithoutExtension(ImportPath) + "_PNG" + ".png";
                image.Save(export);
                Status = "Success.";
            }

        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Initizalizes the resolution options, 
        /// and sets the default value to "Select a Resolution".
        /// </summary>
        private void InitializeResolutionOptions()
        {
            ResolutionOptions = new ObservableCollection<string>
            {
                "Select a Resolution",
                "16 x 16",
                "32 x 32",
                "48 x 48",
                "64 x 64",
                "128 x 128",
                "256 x 256",
                "512 x 512",
            };

            TargetResolution = ResolutionOptions[0];
        }

        /// <summary>
        /// Checks to see if the file is the right type for this conversion.
        /// </summary>
        /// <returns>True if the file was successfully validated. False otherwise.</returns>
        private bool ValidateFile()
        {
            if (string.IsNullOrEmpty(ImportPath))
                return false;

            string[] parts = ImportPath.Split(".");
            string extension = parts[1];

            if (ConvertMode)
                return extension == "png";
            else
                return extension == "ico";
        }

        /// <summary>
        /// Gets the target resolution specified earlier. 
        /// Returns -1 if the resolution could not be determined.
        /// </summary>
        private int GetTargetResolution()
        {
            if (TargetResolution == "16 x 16")
                return 16;
            if (TargetResolution == "32 x 32")
                return 32;
            if (TargetResolution == "48 x 48")
                return 48;
            if (TargetResolution == "64 x 64")
                return 64;
            if (TargetResolution == "128 x 128")
                return 128;
            if (TargetResolution == "256 x 256")
                return 256;
            if (TargetResolution == "512 x 512")
                return 512;
            // If the result couldn't be determined, return -1.
            return -1;
        }

        
        private Image ResizeImage(Image image, System.Drawing.Size size)
        {
            return new Bitmap(image, size);
        }

        #endregion

    }
}
