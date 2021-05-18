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

//print trigger event to set detail boarder color to white
namespace Viewport_Align
{
    [Transaction(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class HideDetBor: IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
        {
            UIApplication uiApp = commandData.Application;
            Document doc = commandData.Application.ActiveUIDocument.Document;

            // myPrintingEvent will run just BEFORE Revit prints
            doc.DocumentPrinting += new EventHandler<DocumentPrintingEventArgs>(myPrintingEvent);

            // myPrintedEvent will run just AFTER Revit prints
            doc.DocumentPrinted += new EventHandler<DocumentPrintedEventArgs>(myPrintedEvent);

            void myPrintingEvent(object sender, DocumentPrintingEventArgs args)
            {
                // set color to white (255, 255, 255)
                CategoryLineColor(new Color(Byte.MaxValue, Byte.MaxValue, Byte.MaxValue));
            }

            void myPrintedEvent(object sender, DocumentPrintedEventArgs args)
            {
                // set color to black (0, 0, 0)
                CategoryLineColor(new Color(Byte.MinValue, Byte.MinValue, Byte.MinValue));
            }

            void CategoryLineColor(Color newColor)
            {
                // Get all categories in the document
                Categories categories = doc.Settings.Categories;

                // The "Generic Annotations" category
                Category genericAnnotations = categories.get_Item("Generic Annotations");

                // The ""Do Not Print" subcategory of the "Generic Annotations" category
                Category doNotPrint = genericAnnotations.SubCategories.get_Item("Detail Border");

                // Create transaction, with the value of the color (0 or 255) in the transaction name
                using (Transaction t = new Transaction(doc, "Do Not Print color = " + newColor.Blue))
                {
                    t.Start();
                    // set the style's line color to the "newColor"
                    doNotPrint.LineColor = newColor;
                    t.Commit();
                }
            }

            return Result.Succeeded;
        }
    }

}

