using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.Windows;

namespace ClassLibrary2
{
    internal class CustomPaletteSet : PaletteSet
    {
        // static field
        static bool wasVisible;

        /// <summary>
        /// Creates a new instance of CustomPaletteSet.
        /// </summary>
        public CustomPaletteSet()
            : base("Palette", "CMD_PALETTE", new Guid("{1836C7AC-C70E-4CF7-AA05-F6298D275046}"))
        {
            Style =
                PaletteSetStyles.ShowAutoHideButton |
                PaletteSetStyles.ShowCloseButton |
                PaletteSetStyles.ShowPropertiesMenu;
            MinimumSize = new System.Drawing.Size(250, 150);
            Add("Circle", new PaletteTab());

            // automatically hide the palette while none document is active (no document state)
            var docs = Application.DocumentManager;
            docs.DocumentBecameCurrent += (s, e) => Visible = e.Document == null ? false : wasVisible;
            docs.DocumentCreated += (s, e) => Visible = wasVisible;
            docs.DocumentToBeDeactivated += (s, e) => wasVisible = Visible;
            docs.DocumentToBeDestroyed += (s, e) =>
            {
                wasVisible = Visible;
                if (docs.Count == 1)
                    Visible = false;
            };
        }
    }
}
