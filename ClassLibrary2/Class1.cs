using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
//using Autodesk.AutoCAD.Interop.Common;
using Autodesk.AutoCAD.Ribbon;


using Autodesk.Windows;
using System.Windows.Input;

[assembly: ExtensionApplication(typeof(ClassLibrary2.Class1))]

namespace ClassLibrary2
{    
    public class Class1 : IExtensionApplication
    {

        public void Initialize()
        {
            try
            {
                /// When the ribbon is available, do
                /// initialization for it:
                RibbonHelper.OnRibbonFound(this.SetupRibbon);

                /// TODO: Initialize your app
            }
            catch (System.Exception ex)
            {
                Document doc = Application.DocumentManager.MdiActiveDocument;
                if (doc != null)
                    doc.Editor.WriteMessage(ex.ToString());
            }
        }

        public void Terminate()
        {
        }

        /// A method that performs one-time setup
        /// of Ribbon components:
        void SetupRibbon(RibbonControl ribbon)
        {
            /// TODO: Place ribbon initialization code here

            /// Example only: Verify our method was called
            Application.ShowAlertDialog("SetupRibbon() called");

            RibbonTab ribTab = new RibbonTab();


            Autodesk.Windows.RibbonControl rbnCtrl = Autodesk.AutoCAD.Ribbon.RibbonServices.RibbonPaletteSet.RibbonControl;

            ribTab.Title = "BIM";
            ribTab.Id = "TAB_ID";
            rbnCtrl.Tabs.Add(ribTab);

            RibbonPanelSource ribSourcePanel = new RibbonPanelSource();
            ribSourcePanel.Title = "Opciones BIM";

            RibbonPanel ribPanel = new RibbonPanel();
            ribPanel.Source = ribSourcePanel;
            ribTab.Panels.Add(ribPanel);

            RibbonButton ribbonbutton4 = new RibbonButton();
            ribbonbutton4.Text = "BIM 1";
            ribbonbutton4.ShowText = true;

            ribbonbutton4.CommandParameter = "PRESU ";
            ribbonbutton4.ShowImage = true;
            ribbonbutton4.CommandHandler = new LegalDraftingRibbonCommandHandler();

            RibbonButton ribbonbutton5 = new RibbonButton();
            ribbonbutton5.Text = "BIM 2";
            ribbonbutton5.ShowText = true;

            ribbonbutton5.CommandParameter = "CMD_PALETTE ";
            ribbonbutton5.ShowImage = true;
            ribbonbutton5.CommandHandler = new LegalDraftingRibbonCommandHandler();


            ribSourcePanel.DialogLauncher = ribbonbutton4;
            ribSourcePanel.Items.Add(ribbonbutton4);
            ribSourcePanel.Items.Add(new RibbonSeparator());

            ribSourcePanel.Items.Add(ribbonbutton5);
            ribSourcePanel.Items.Add(new RibbonSeparator());


        }

        [CommandMethod("Proyecto")]
        public static void proy() {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurvDb = acDoc.Database;
            using (Transaction acTrasn = acCurvDb.TransactionManager.StartTransaction()) {
                BlockTable acBlkTlb = acTrasn.GetObject(acCurvDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord acBlkTlbRec = acTrasn.GetObject(acBlkTlb[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Editor e = Application.DocumentManager.MdiActiveDocument.Editor;
                Document d = Application.DocumentManager.MdiActiveDocument;

                PromptResult r = e.GetString("ESCRIBA SU NOMBRE ");

                e.WriteMessage("hOLA " + r.StringResult);

                Line l = new Line(new Point3d(0, 0, 0), new Point3d(50, 0, 0));
                Line l1 = new Line(new Point3d(50, 0, 0), new Point3d(0, 50, 0));
                Line l2 = new Line(new Point3d(0, 50, 0), new Point3d(0, 0, 0));

                acBlkTlbRec.AppendEntity(l);
                acTrasn.AddNewlyCreatedDBObject(l, true);
                acBlkTlbRec.AppendEntity(l1);
                acTrasn.AddNewlyCreatedDBObject(l1, true);
                acBlkTlbRec.AppendEntity(l2);
                acTrasn.AddNewlyCreatedDBObject(l2, true);

                DBObjectCollection Poligono = new DBObjectCollection();
                Poligono.Add(l);
                Poligono.Add(l1);
                Poligono.Add(l2);
                DBObjectCollection region = Region.CreateFromCurves(Poligono);
                Region reg = (Region)region[0];
                Solid3d solid = new Solid3d();
                solid.Extrude(reg, 50, 0);

                solid.SetDatabaseDefaults();
                acBlkTlbRec.AppendEntity(solid);
                acTrasn.AddNewlyCreatedDBObject(solid, true);

                Circle cir = new Circle();
                cir.Radius = 5;
                cir.Center = new Point3d(50, 50, 50);

                acBlkTlbRec.AppendEntity(cir);
                acTrasn.AddNewlyCreatedDBObject(cir, true);

                acTrasn.Commit();
            }
        }


        [CommandMethod("PRESU")]
        public static void PRESU()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurvDb = acDoc.Database;
            using (Transaction acTrasn = acCurvDb.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTlb = acTrasn.GetObject(acCurvDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord acBlkTlbRec = acTrasn.GetObject(acBlkTlb[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Editor e = Application.DocumentManager.MdiActiveDocument.Editor;
                Document d = Application.DocumentManager.MdiActiveDocument;

                PromptResult r = e.GetString("ESCRIBA SU NOMBRE ");

                e.WriteMessage("hOLA " + r.StringResult);

                Line l = new Line(new Point3d(0, 0, 0), new Point3d(50, 0, 0));
                Line l1 = new Line(new Point3d(50, 0, 0), new Point3d(0, 50, 0));
                Line l2 = new Line(new Point3d(0, 50, 0), new Point3d(0, 0, 0));

                acBlkTlbRec.AppendEntity(l);
                acTrasn.AddNewlyCreatedDBObject(l, true);
                acBlkTlbRec.AppendEntity(l1);
                acTrasn.AddNewlyCreatedDBObject(l1, true);
                acBlkTlbRec.AppendEntity(l2);
                acTrasn.AddNewlyCreatedDBObject(l2, true);

                DBObjectCollection Poligono = new DBObjectCollection();
                Poligono.Add(l);
                Poligono.Add(l1);
                Poligono.Add(l2);
                DBObjectCollection region = Region.CreateFromCurves(Poligono);
                Region reg = (Region)region[0];
                Solid3d solid = new Solid3d();
                solid.Extrude(reg, 50, 0);

                solid.SetDatabaseDefaults();
                acBlkTlbRec.AppendEntity(solid);
                acTrasn.AddNewlyCreatedDBObject(solid, true);

                Circle cir = new Circle();
                cir.Radius = 5;
                cir.Center = new Point3d(50, 50, 50);

                acBlkTlbRec.AppendEntity(cir);
                acTrasn.AddNewlyCreatedDBObject(cir, true);

                acTrasn.Commit();
            }
        }





        public class LegalDraftingRibbonCommandHandler : System.Windows.Input.ICommand
        {

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                RibbonButton btn = parameter as RibbonButton;
                if (btn != null)
                {
                    Document dwg = Autodesk.AutoCAD.ApplicationServices.Application.
                        DocumentManager.MdiActiveDocument;

                    dwg.SendStringToExecute((string)btn.CommandParameter, true, false, true);
                }
            }
        }


    }
}








namespace Autodesk.AutoCAD.Ribbon
{
    public class RibbonHelper
    {
        Action<RibbonControl> action = null;
        bool idleHandled = false;
        bool created = false;

        RibbonHelper(Action<RibbonControl> action)
        {
            if (action == null)
                throw new ArgumentNullException("initializer");
            this.action = action;
            SetIdle(true);
        }

        /// <summary>
        /// 
        /// Pass a delegate that takes the RibbonControl
        /// as its only argument, and it will be invoked
        /// when the RibbonControl is available. 
        /// 
        /// If the RibbonControl exists when the constructor
        /// is called, the delegate will be invoked on the
        /// next idle event. Otherwise, the delegate will be
        /// invoked on the next idle event following the 
        /// creation of the RibbonControl.
        /// 
        /// </summary>

        public static void OnRibbonFound(Action<RibbonControl> action)
        {
            new RibbonHelper(action);
        }

        void SetIdle(bool value)
        {
            if (value ^ idleHandled)
            {
                if (value)
                    Application.Idle += idle;
                else
                    Application.Idle -= idle;
                idleHandled = value;
            }
        }

        void idle(object sender, EventArgs e)
        {
            SetIdle(false);
            if (action != null)
            {
                var ps = RibbonServices.RibbonPaletteSet;
                if (ps != null)
                {
                    var ribbon = ps.RibbonControl;
                    //if (ribbon != null)
                    //{
                        action(ribbon);
                        action = null;
                    //}
                }
                else if (!created)
                {
                    created = true;
                    RibbonServices.RibbonPaletteSetCreated +=
                       ribbonPaletteSetCreated;
                }
            }
        }

        void ribbonPaletteSetCreated(object sender, EventArgs e)
        {
            RibbonServices.RibbonPaletteSetCreated
               -= ribbonPaletteSetCreated;
            SetIdle(true);
        }
    }
}
