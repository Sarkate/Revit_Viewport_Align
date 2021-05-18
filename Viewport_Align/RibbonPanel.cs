using Autodesk.Revit.UI;
using System;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace Viewport_Align
{
    public class MainPanel : IExternalApplication
    {
        //Create Tool Tab
        public Result OnStartup(UIControlledApplication application)
        {
            //Create a custom tab
            string tabName = "Viewport Align";
            application.CreateRibbonTab(tabName);

            //Creat Push Buttons
            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;
            PushButtonData butnHideBorData = new PushButtonData("Views0", "Hide\r\nDetail Boarders", thisAssemblyPath, "Viewport_Align.HideDetBor");
            PushButtonData butnAlignBorData = new PushButtonData("Views1", "Align\r\nDetail Boarders", thisAssemblyPath, "Viewport_Align.AlignDetBor");

            //Creat Ribbon Panel
            RibbonPanel viewPanel = application.CreateRibbonPanel(tabName, "View");
            
            // List<RibbonItem> projectButton = new List<RibbonItem>();
            //Add Button to View panel
            PushButton butnHideBor = viewPanel.AddItem(butnHideBorData) as PushButton;
            PushButton butnAlignBor = viewPanel.AddItem(butnAlignBorData) as PushButton;

            //Get Image
            BitmapImage imagePrint = new BitmapImage();
            imagePrint.BeginInit();
            imagePrint.UriSource = new Uri(@"C:\Revit Add-in\Images\hide.png");
            imagePrint.EndInit();

            BitmapImage imageAlign = new BitmapImage();
            imageAlign.BeginInit();
            imageAlign.UriSource = new Uri(@"C:\Revit Add-in\Images\align-icon.png");
            imageAlign.EndInit();

            //Assign Image to Button
            butnHideBor.LargeImage = imagePrint;
            butnAlignBor.LargeImage = imageAlign;

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            // nothing to clean up in this simple case
            return Result.Succeeded;
        }
    }
}