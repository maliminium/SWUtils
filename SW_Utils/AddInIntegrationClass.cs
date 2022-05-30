using AngelSix.SolidDna;
using Dna;
using System.IO;

namespace SW_Utils
{
    public class AddInIntegrationClass : AddInIntegration
    {
        public override void ApplicationStartup()
        {

        }

        public override void ConfigureServices(FrameworkConstruction construction)
        {

        }

        public override void PreConnectToSolidWorks()
        {

        }

        public override void PreLoadPlugIns()
        {

        }
    }

    public class ToDoListPlugIn : SolidPlugIn
    {
        private TaskpaneIntegration<MainTaskPaneUI> mTaskpane;
        public override string AddInTitle => "SW Utilities";
        public override string AddInDescription => "Utilities for SolidWorks CAD Environment";

        public override void ConnectedToSolidWorks()
        {
            mTaskpane = new TaskpaneIntegration<MainTaskPaneUI>()
            {
                Icon = Path.Combine(this.AssemblyPath(), "todo_icon16.png"),
                WpfControl = new MainAddInUI()
            };

            mTaskpane.AddToTaskpaneAsync();
        }

        public override void DisconnectedFromSolidWorks()
        {

        }
    }
}
