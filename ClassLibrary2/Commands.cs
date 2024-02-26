using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

[assembly: CommandClass(typeof(ClassLibrary2.Commands))]


namespace ClassLibrary2
{
    public class Commands
    {
        // static field
        static CustomPaletteSet palette;

        // instance fields (default values)
        double radius = 10.0;
        string layer;

        /// <summary>
        /// Command to show the palette.
        /// </summary>
        [CommandMethod("CMD_PALETTE")]
        public void ShowPaletteSet()
        {
            // creation of the palette at the first call of the command
            if (palette == null)
                palette = new CustomPaletteSet();
            palette.Visible = true;
        }

        /// <summary>
        /// Command to draw the circle
        /// </summary>
        [CommandMethod("CMD_CIRCLE_PALETTE", CommandFlags.Modal)]
        public void DrawCircleCmd()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;

            // choosethe layer
            if (string.IsNullOrEmpty(layer))
                layer = (string)Application.GetSystemVariable("clayer");
            var strOptions = new PromptStringOptions("\nLayer name: ");
            strOptions.DefaultValue = layer;
            strOptions.UseDefaultValue = true;
            var strResult = ed.GetString(strOptions);
            if (strResult.Status != PromptStatus.OK)
                return;
            layer = strResult.StringResult;

            // specify the radius
            var distOptions = new PromptDistanceOptions("\nSpecify the radius: ");
            distOptions.DefaultValue = radius;
            distOptions.UseDefaultValue = true;
            var distResult = ed.GetDistance(distOptions);
            if (distResult.Status != PromptStatus.OK)
                return;
            radius = distResult.Value;

            // specify the center
            var ppr = ed.GetPoint("\nSpecify the center: ");
            if (ppr.Status == PromptStatus.OK)
            {
                // drawing the circle in the current space
                using (var tr = db.TransactionManager.StartTransaction())
                {
                    var curSpace =
                        (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                    var ucs = ed.CurrentUserCoordinateSystem;
                    using (var circle = new Circle(ppr.Value, Vector3d.ZAxis, distResult.Value))
                    {
                        circle.TransformBy(ed.CurrentUserCoordinateSystem);
                        circle.Layer = strResult.StringResult;
                        curSpace.AppendEntity(circle);
                        tr.AddNewlyCreatedDBObject(circle, true);
                    }
                    tr.Commit();
                }
            }
        }
    }
}
