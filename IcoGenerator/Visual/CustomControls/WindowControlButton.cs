using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace IcoGenerator.Visual.CustomControls
{
    /// <summary>
    /// A button with the ability to store two images.
    /// </summary>
    public class WindowControlButton : Button
    {

        /// <summary>
        /// The image that is normally shown.
        /// </summary>
        public Image PrimaryImage
        {
            get { return (Image)GetValue(PrimaryImageProperty); }
            set
            {
                SetValue(PrimaryImageProperty, value);
            }
        }

        /// <summary>
        /// The image that is normally shown.
        /// </summary>
        public static readonly DependencyProperty PrimaryImageProperty = 
            DependencyProperty.Register("PrimaryImage", typeof(Image), typeof(WindowControlButton));

        /// <summary>
        /// The image that is shown when the user hovers over the button.
        /// </summary>
        public Image HoveredImage
        {
            get { return (Image)GetValue(HoveredImageProperty); }
            set
            {
                SetValue(HoveredImageProperty, value);
            }
        }

        /// <summary>
        /// The image that is shown when the user hovers over the button.
        /// </summary>
        public static readonly DependencyProperty HoveredImageProperty =
            DependencyProperty.Register("HoveredImage", typeof(Image), typeof(WindowControlButton));

    }
}
