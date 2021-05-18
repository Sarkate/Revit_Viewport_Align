using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Media.Imaging;
using System.Windows.Forms;
using System.Windows.Input;

namespace Viewport_Align
{

    public class DetailGrid
    {
        public int detailNum { get; set; }
        public XYZ point { get; set; }

    }

    [Transaction(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class AlignDetBor: IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
        {
            UIApplication uiApp = commandData.Application;
            Document doc = commandData.Application.ActiveUIDocument.Document;

            XYZ startPoint = new XYZ(0, 0, 0);
            XYZ endPoint = new XYZ(0, 0, 0);
            GetSheetOutLine(doc, out startPoint, out endPoint);

            List<DetailGrid> dgList = new List<DetailGrid>();
            GridGenerator(startPoint, endPoint, out dgList);

            List<Viewport> vpList = new List<Viewport>();
            GetAllViewsOnSheet(doc, out vpList);
            foreach (Viewport vp in vpList)
            {

                XYZ VpSp = new XYZ(0, 0, 0);
                XYZ VpEp = new XYZ(0, 0, 0);
                GetOutLine(doc, vp, out VpSp, out VpEp);

                //Get Detail Number
                Parameter dtNumPara = vp.get_Parameter(BuiltInParameter.VIEWER_DETAIL_NUMBER);
                int vpDtNum = Convert.ToInt32(dtNumPara.AsString());

                //Use Detail number to find detail location
                int detailNum = vpDtNum;
                DetailGrid dg = new DetailGrid();
                dg = dgList.ElementAt(detailNum - 1);

                //calculate new center point
                XYZ cenPoint = vp.GetBoxCenter();
                double xOffCenter = Math.Abs(cenPoint.X - VpSp.X);
                double yOffCenter = Math.Abs(cenPoint.Y - VpSp.Y);
                XYZ newCenter = new XYZ(dg.point.X - xOffCenter, dg.point.Y + yOffCenter, 0);

                //Set new detail location
                using (Transaction t = new Transaction(doc, "relocate View"))
                {
                    t.Start();
                    vp.SetBoxCenter(newCenter);
                    t.Commit();
                }
            }

                return Result.Succeeded;
        }

        private void GetSheetBbox(Document doc, out XYZ startPoint, out XYZ endPoint)
        {
            BoundingBoxXYZ bBox = doc.ActiveView.get_BoundingBox(doc.ActiveView);
            startPoint = new XYZ(bBox.Min.X, bBox.Min.Y, 0);
            endPoint = new XYZ(bBox.Max.X, bBox.Max.Y, 0);
            TaskDialog.Show("info", bBox.Max.ToString());
        }

        private void GetOutLine(Document doc, Viewport vp, out XYZ startPoint, out XYZ endPoint)
        {

            //0.12109375 OFFSET FROM DETAIL BOARDER
            double lowXShift = -0.12109375 / 12;
            double lowYShift = 0.12109375 / 12;

            double upXShift = 0.12109375 / 12;
            double upYShift = -0.12109375 / 12;

            XYZ lowRight = new XYZ(vp.GetBoxOutline().MaximumPoint.X, vp.GetBoxOutline().MinimumPoint.Y, 0);
            XYZ upLeft = new XYZ(vp.GetBoxOutline().MinimumPoint.X, vp.GetBoxOutline().MaximumPoint.Y, 0);

            startPoint = new XYZ((lowRight.X + lowXShift), (lowRight.Y + lowYShift), 0);
            endPoint = new XYZ((upLeft.X + upXShift), (upLeft.Y + upYShift), 0);
        }

        private void GetSheetOutLine(Document doc, out XYZ startPoint, out XYZ endPoint)
        {

            double lowXShift = -5.25 / 12;
            double lowYShift = 0.5 / 12;
            double upXShift = 0.5 / 12;
            double upYShift = -0.5 / 12;
            XYZ lowRight = new XYZ(doc.ActiveView.Outline.Max.U, doc.ActiveView.Outline.Min.V, 0);
            XYZ upLeft = new XYZ(doc.ActiveView.Outline.Min.U, doc.ActiveView.Outline.Max.V, 0)
                ;
            startPoint = new XYZ((lowRight.X + lowXShift), (lowRight.Y + lowYShift), 0);
            endPoint = new XYZ((upLeft.X + upXShift), (upLeft.Y + upYShift), 0);

            //up right to low left
            //startPoint =  new XYZ(doc.ActiveView.Outline.Max.U, doc.ActiveView.Outline.Max.V,0);
            //endPoint = new XYZ(doc.ActiveView.Outline.Min.U,doc.ActiveView.Outline.Min.V,0);

            // get the view's origin point's
            // coordinates in current view sheet.

            //UV ptSourceViewOriginInSheet = new UV(xyzLocation.Max.X - ptMaxOutline.X,xyzLocation.Max.Y - ptMaxOutline.Y );
        }

        private void GridGenerator(XYZ end0, XYZ end1, out List<DetailGrid> dgList)
        {
            dgList = new List<DetailGrid>();

            List<double> xList = new List<double>();
            #region xList (Columns)
            xList.Add(end0.X);
            double colNum = 5;
            double colSpace = Math.Abs(end0.X - end1.X) / colNum;

            double colInc = end0.X;

            for (int i = 1; i <= colNum - 1; i++)
            {
                colInc -= colSpace;
                xList.Add(colInc);
            }
            #endregion

            List<double> yList = new List<double>();
            #region yList (Rows)
            yList.Add(end0.Y);
            double rowNum = 4;
            double rowSpace = Math.Abs(end0.Y - end1.Y) / rowNum;

            double rowInc = end0.Y;

            for (int i = 1; i <= rowNum - 1; i++)
            {
                rowInc += rowSpace;
                yList.Add(rowInc);
            }
            #endregion

            int pointIndex = 0;
            int yIndex = 0;
            double y = 0;
            foreach (double x in xList)
            {
                for (int i = 0; i < 4; i++)
                {
                    y = yList.ElementAt(i);
                    DetailGrid dg = new DetailGrid();
                    XYZ np = new XYZ(x, y, 0);
                    pointIndex++;
                    dg.detailNum = pointIndex;
                    dg.point = np;
                    dgList.Add(dg);
                }
            }
        }

        private void GetAllViewsOnSheet(Document doc, out List<Viewport> vpListvpList)
        {

            vpListvpList = new List<Viewport>();
            ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Viewports);
            FilteredElementCollector collector = new FilteredElementCollector(doc, doc.ActiveView.Id);
            IList<Element> allViews = collector.WherePasses(filter).ToElements();
            foreach (Element e in allViews)
            {
                Viewport vp = e as Viewport;
                vpListvpList.Add(vp);
                //Parameter p = vp.get_Parameter(BuiltInParameter.VIEWPORT_DETAIL_NUMBER);
            }

        }
    }

}

