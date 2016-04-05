using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HostManager.Models
{
    public class InfoLabelModel
    {
        public class Warning
        {
            BrushConverter _brushConverter = new BrushConverter();

            public Brush BackgroundColor()
            {
                return (Brush)_brushConverter.ConvertFromString("#F2DEDE");
            }

            public Brush ForegroundColor()
            {
                return (Brush)_brushConverter.ConvertFromString("#A94442");
            }
        }

        public class Success
        {
            BrushConverter _brushConverter = new BrushConverter();

            public Brush BackgroundColor()
            {
                return (Brush)_brushConverter.ConvertFromString("#DFF0D8");
            }

            public Brush ForegroundColor()
            {
                return (Brush)_brushConverter.ConvertFromString("#3C763D");
            }
        }

        public class Info
        {
            BrushConverter _brushConverter = new BrushConverter();

            public Brush BackgroundColor()
            {
                return (Brush)_brushConverter.ConvertFromString("#D9EDF7");
            }

            public Brush ForegroundColor()
            {
                return (Brush)_brushConverter.ConvertFromString("#31708F");
            }
        }

        public class None
        {
            BrushConverter _brushConverter = new BrushConverter();

            public Brush BackgroundColor()
            {
                return (Brush)_brushConverter.ConvertFromString("#00FFFFFF");
            }

            public Brush ForegroundColor()
            {
                return (Brush)_brushConverter.ConvertFromString("#00FFFFFF");
            }
        }
    }
}
