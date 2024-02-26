using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Drawing;
using System.Windows.Forms;

using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Windows.Data;
using EO.WebBrowser;

using AcAp = Autodesk.AutoCAD.ApplicationServices.Application;
using System.Json;

namespace ClassLibrary2
{
    public partial class PaletteTab : UserControl
    {
        // instance fields
        double radius;
        private ComboBox cbxLayer;
        private Button btnOk;
        private TextBox txtRadius;
        private DevComponents.DotNetBar.SuperTabControl superTabControl1;
        private DevComponents.DotNetBar.SuperTabControlPanel superTabControlPanel1;
        private DevComponents.DotNetBar.SuperTabItem superTabItem1;
        private EO.WebBrowser.WebView webView1;
        private EO.WinForm.WebControl webControl1;
        private WebView webView2;
        DataItemCollection layers; // layer data collection

        /// <summary>
        /// Creates a new instance of PaletteTab.
        /// Defines the data bindings for the ComboBox control.
        /// </summary>
        public PaletteTab()
        {
            EO.WebBrowser.Runtime.AddLicense(
            "3a5rp7PD27FrmaQHEPGs4PP/6KFrqKax2r1GgaSxy5916u34GeCt7Pb26bSG" +
            "prT6AO5p3Nfh5LRw4rrqHut659XO6Lto6u34GeCt7Pb26YxDs7P9FOKe5ff2" +
            "6YxDdePt9BDtrNzCnrWfWZekzRfonNzyBBDInbW1yQKzbam2xvGvcau0weKv" +
            "fLOz/RTinuX39vTjd4SOscufWbPw+g7kp+rp9unMmuX59hefi9jx+h3ks7Oz" +
            "/RTinuX39hC9RoGkscufddjw/Rr2d4SOscufWZekscu7mtvosR/4qdzBs/DO" +
            "Z7rsAxrsnpmkBxDxrODz/+iha6iywc2faLWRm8ufWZfAwAzrpeb7z7iJWZek" +
            "sefuq9vpA/Ttn+ak9QzznrSmyNqxaaa2wd2wW5f3Bg3EseftAxDyeuvBs+I=");

            InitializeComponent();

            webView2.JSExtInvoke += new JSExtInvokeHandler(WebView_JSExtInvoke);


            layers = layers = AcAp.UIBindings.Collections.Layers; ;

            // binds the layer data to the ComboBox control
            BindData();

            // updates the ComboBox control when the layer data collection changes
            layers.CollectionChanged += (s, e) => BindData();

            // radius default value
            txtRadius.Text = "10";
        }

        /// <summary>
        /// Handles the 'TextChanged' event of the 'Radius' TextBox.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event data.</param>
        private void txtRadius_TextChanged(object sender, EventArgs e)
        {
            // OK button is 'disable' if the text does not represent a valid number
            // the radius field is updated accordingly
            btnOk.Enabled = double.TryParse(txtRadius.Text, out radius);
        }

        // <summary>
        /// Handles the 'Click' event of the 'Radius' button.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event data.</param>
        private void btnRadius_Click(object sender, EventArgs e)
        {
            // set the focus to AutoCAD editor
            // before AutoCAD 2015, use : Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();
            AcAp.MainWindow.Focus();

            // prompt the user to specify a distance
            var ed = AcAp.DocumentManager.MdiActiveDocument.Editor;
            var opts = new PromptDistanceOptions("\nSpecify the radius: ");
            opts.AllowNegative = false;
            opts.AllowZero = false;
            var pdr = ed.GetDistance(opts);
            if (pdr.Status == PromptStatus.OK)
                txtRadius.Text = pdr.Value.ToString();
        }

        /// <summary>
        /// Handles the 'Click' event of the 'OK' button.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event data.</param>
        private void btnOk_Click(object sender, EventArgs e)
        {
            // calling the 'CMD_CIRCLE' command with the layer and radius
            AcAp.DocumentManager.MdiActiveDocument?.SendStringToExecute(
               $"CMD_CIRCLE_PALETTE \"{((INamedValue)cbxLayer.SelectedItem).Name}\" {radius} ", false, false, false);
        }

        /// <summary>
        /// Defines the data bindings of the Combobox control.
        /// </summary>
        private void BindData()
        {
            // binding to data source
            cbxLayer.DataSource = new BindingSource(layers, null);

            // definition of the drop down lsit items appearance
            cbxLayer.DrawMode = DrawMode.OwnerDrawFixed;
            cbxLayer.DrawItem += (_, e) =>
            {
                if (e.Index > -1)
                {
                    // get the item and its properties
                    var item = layers[e.Index];
                    var properties = item.GetProperties();

                    // draw a square with the layer color
                    var color = (Autodesk.AutoCAD.Colors.Color)properties["Color"].GetValue(item);
                    var bounds = e.Bounds;
                    int height = bounds.Height;
                    Graphics graphics = e.Graphics;
                    e.DrawBackground();
                    var rect = new Rectangle(bounds.Left + 4, bounds.Top + 4, height - 8, height - 8);
                    graphics.FillRectangle(new SolidBrush(color.ColorValue), rect);
                    graphics.DrawRectangle(new Pen(Color.Black), rect);

                    // write the layer name
                    graphics.DrawString(
                        (string)properties["Name"].GetValue(item),
                        e.Font,
                        new SolidBrush(e.ForeColor), bounds.Left + height, bounds.Top + 1);
                    e.DrawFocusRectangle();
                }
            };

            // select the current layer
            cbxLayer.SelectedItem = layers.CurrentItem;
        }

        private void InitializeComponent()
        {
            this.cbxLayer = new System.Windows.Forms.ComboBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.txtRadius = new System.Windows.Forms.TextBox();
            this.superTabControl1 = new DevComponents.DotNetBar.SuperTabControl();
            this.superTabControlPanel1 = new DevComponents.DotNetBar.SuperTabControlPanel();
            this.superTabItem1 = new DevComponents.DotNetBar.SuperTabItem();
            this.webView1 = new EO.WebBrowser.WebView();
            this.webControl1 = new EO.WinForm.WebControl();
            this.webView2 = new EO.WebBrowser.WebView();
            ((System.ComponentModel.ISupportInitialize)(this.superTabControl1)).BeginInit();
            this.superTabControl1.SuspendLayout();
            this.superTabControlPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbxLayer
            // 
            this.cbxLayer.FormattingEnabled = true;
            this.cbxLayer.Location = new System.Drawing.Point(76, 33);
            this.cbxLayer.Name = "cbxLayer";
            this.cbxLayer.Size = new System.Drawing.Size(108, 21);
            this.cbxLayer.TabIndex = 5;
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(0, 7);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(70, 32);
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "button1";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click_1);
            // 
            // txtRadius
            // 
            this.txtRadius.Location = new System.Drawing.Point(76, 7);
            this.txtRadius.Name = "txtRadius";
            this.txtRadius.Size = new System.Drawing.Size(188, 20);
            this.txtRadius.TabIndex = 3;
            // 
            // superTabControl1
            // 
            // 
            // 
            // 
            // 
            // 
            // 
            this.superTabControl1.ControlBox.CloseBox.Name = "";
            // 
            // 
            // 
            this.superTabControl1.ControlBox.MenuBox.Name = "";
            this.superTabControl1.ControlBox.Name = "";
            this.superTabControl1.ControlBox.SubItems.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.superTabControl1.ControlBox.MenuBox,
            this.superTabControl1.ControlBox.CloseBox});
            this.superTabControl1.Controls.Add(this.superTabControlPanel1);
            this.superTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.superTabControl1.Location = new System.Drawing.Point(0, 0);
            this.superTabControl1.Name = "superTabControl1";
            this.superTabControl1.ReorderTabsEnabled = true;
            this.superTabControl1.SelectedTabFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.superTabControl1.SelectedTabIndex = 0;
            this.superTabControl1.Size = new System.Drawing.Size(427, 482);
            this.superTabControl1.TabFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.superTabControl1.TabIndex = 6;
            this.superTabControl1.Tabs.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.superTabItem1});
            this.superTabControl1.Text = "superTabControl1";
            this.superTabControl1.SelectedTabChanged += new System.EventHandler<DevComponents.DotNetBar.SuperTabStripSelectedTabChangedEventArgs>(this.superTabControl1_SelectedTabChanged);
            // 
            // superTabControlPanel1
            // 
            this.superTabControlPanel1.Controls.Add(this.webControl1);
            this.superTabControlPanel1.Controls.Add(this.cbxLayer);
            this.superTabControlPanel1.Controls.Add(this.btnOk);
            this.superTabControlPanel1.Controls.Add(this.txtRadius);
            this.superTabControlPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.superTabControlPanel1.Location = new System.Drawing.Point(0, 25);
            this.superTabControlPanel1.Name = "superTabControlPanel1";
            this.superTabControlPanel1.Size = new System.Drawing.Size(427, 457);
            this.superTabControlPanel1.TabIndex = 1;
            this.superTabControlPanel1.TabItem = this.superTabItem1;
            this.superTabControlPanel1.Click += new System.EventHandler(this.superTabControlPanel1_Click);
            // 
            // superTabItem1
            // 
            this.superTabItem1.AttachedControl = this.superTabControlPanel1;
            this.superTabItem1.GlobalItem = false;
            this.superTabItem1.Name = "superTabItem1";
            this.superTabItem1.Text = "superTabItem1";
            // 
            // webView1
            // 
            this.webView1.InputMsgFilter = null;
            this.webView1.ObjectForScripting = null;
            this.webView1.Title = null;
            // 
            // webControl1
            // 
            this.webControl1.BackColor = System.Drawing.Color.White;
            this.webControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.webControl1.Location = new System.Drawing.Point(0, 60);
            this.webControl1.Name = "webControl1";
            this.webControl1.Size = new System.Drawing.Size(427, 397);
            this.webControl1.TabIndex = 6;
            this.webControl1.Text = "webControl1";
            this.webControl1.WebView = this.webView2;
            // 
            // webView2
            // 
            this.webView2.InputMsgFilter = null;
            this.webView2.ObjectForScripting = null;
            this.webView2.Title = null;
            // 
            // PaletteTab
            // 
            this.Controls.Add(this.superTabControl1);
            this.Name = "PaletteTab";
            this.Size = new System.Drawing.Size(427, 482);
            this.Load += new System.EventHandler(this.PaletteTab_Load);
            ((System.ComponentModel.ISupportInitialize)(this.superTabControl1)).EndInit();
            this.superTabControl1.ResumeLayout(false);
            this.superTabControlPanel1.ResumeLayout(false);
            this.superTabControlPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        private void PaletteTab_Load(object sender, EventArgs e)
        {
            webControl1.WebView.LoadUrl(@"file:///c:/HTML5/Viewer.html?URN=dXJuOmFkc2sud2lwcHJvZDpmcy5maWxlOnZmLjRZbHhxTjVwVFc2U0ZEdTBzTUNzX2c/dmVyc2lvbj0x&Token=eyJhbGciOiJSUzI1NiIsImtpZCI6IlU3c0dGRldUTzlBekNhSzBqZURRM2dQZXBURVdWN2VhIn0.eyJzY29wZSI6WyJkYXRhOnJlYWQiLCJkYXRhOndyaXRlIiwiZGF0YTpjcmVhdGUiLCJkYXRhOnNlYXJjaCIsImJ1Y2tldDpjcmVhdGUiLCJidWNrZXQ6cmVhZCIsImJ1Y2tldDp1cGRhdGUiLCJidWNrZXQ6ZGVsZXRlIl0sImNsaWVudF9pZCI6IkxybjZvcUxud3BDQmQ4R1MwTHVpbUd4NVNIT05ZdzRiIiwiYXVkIjoiaHR0cHM6Ly9hdXRvZGVzay5jb20vYXVkL2Fqd3RleHA2MCIsImp0aSI6Im1ZdGhtbE1yTVdtNDlJNTZQSUEzcmZqOFIwMDRKRnJLeWExU01abE5RUVI0ZGZ2bWM1SXlpcVVGQVg5UTJ3VGQiLCJleHAiOjE2NDE1MDI3MzZ9.BAMt8NOHaTBp9yKbrYv3T7PZr_3FuRvgYH5rgKbLr9IBZmxtiflCGUAtnoW4CI-wu-m2P7Sm5ndY7Jo4qKK_X4s6VOX7POCbjGbPtKsLWUK7-liIzaVsvz7p4Fr1dLyzbJRIp6EddwtaK237iWEw4RPYPcL6-DpqyVCTr0siTtrheXDSMqA4VODdBc8XM85WUmRG7_rcD1njZKnQYOgSL-VXDvZSeY9Es8Ro-3vjmeJeHadyV8Zu10IaASSXjbA6LezurSHGSZ96GQdkAjvaQsgzcJVunYXCeETqZS3AVHj2SjOgdj8nMtYtp4XHmZNNLXaH6SaOWT6GB1F0lRTybg");
            EO.WebBrowser.Runtime.AddLicense(
            "3a5rp7PD27FrmaQHEPGs4PP/6KFrqKax2r1GgaSxy5916u34GeCt7Pb26bSG" +
            "prT6AO5p3Nfh5LRw4rrqHut659XO6Lto6u34GeCt7Pb26YxDs7P9FOKe5ff2" +
            "6YxDdePt9BDtrNzCnrWfWZekzRfonNzyBBDInbW1yQKzbam2xvGvcau0weKv" +
            "fLOz/RTinuX39vTjd4SOscufWbPw+g7kp+rp9unMmuX59hefi9jx+h3ks7Oz" +
            "/RTinuX39hC9RoGkscufddjw/Rr2d4SOscufWZekscu7mtvosR/4qdzBs/DO" +
            "Z7rsAxrsnpmkBxDxrODz/+iha6iywc2faLWRm8ufWZfAwAzrpeb7z7iJWZek" +
            "sefuq9vpA/Ttn+ak9QzznrSmyNqxaaa2wd2wW5f3Bg3EseftAxDyeuvBs+I=");

        }


        void WebView_JSExtInvoke(object sender, JSExtInvokeArgs e)
        {
            switch (e.FunctionName)
            {
                case "demoAbout":
                    /*string browserEngine = e.Arguments[0] as string;
                    string url = e.Arguments[1] as string;
                    MessageBox.Show("Browser Engine: " + browserEngine + ", Url:" + url);*/

                    /*Excel.Workbook libro = Globals.ThisAddIn.Application.ActiveWorkbook;
                    Excel.Worksheet hoja = libro.Worksheets[1];*/
                    //hoja.Cells[2, 1] = e.Arguments[2];
                    string ID = e.Arguments[0] as string;
                    JsonValue data = JsonObject.Parse(e.Arguments[1] as string);


                    //hoja.Cells[2, 1] = (string)ID;
                    string texto= (string)ID;

                    for (int X = 0; X < data.Count; X++)
                    {
                        texto += (string)data[X]["attributeName"];
                        texto += (string)data[X]["displayName"];
                        texto += data[X]["displayValue"].ToString();
                    }
                    //string name = (string)data["name"];
                    //string version = (string)data["version"];

                    AcAp.ShowAlertDialog(texto);
                    break;
            }
        }

        private void btnOk_Click_1(object sender, EventArgs e)
        {

        }

        private void superTabControl1_SelectedTabChanged(object sender, DevComponents.DotNetBar.SuperTabStripSelectedTabChangedEventArgs e)
        {

        }

        private void superTabControlPanel1_Click(object sender, EventArgs e)
        {

        }
    }
}
