using Dna;
using SolidWorks.Interop.sldworks;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows.Controls;
using static SW_Utils.CommonResources;

namespace SW_Utils
{
    public abstract class DocumentAction : TreeViewItem //, IPathEditor
    {
        public static int MAX_PATH_L = 260;

        public event UIControlChanged ControlChanged;

        protected ModelDoc2 AssignedModel = null;
        public abstract bool IsFile { get; }
        public abstract bool IsActive { get; set; }
        public abstract string DocDir { get; set; }
        public abstract string DocNo { get; set; }
        public abstract string DocText { get; set; }
        public abstract string DocExt { get; set; }

        private List<DocumentAction> ChildActions = new List<DocumentAction>();

        public DocumentAction()
        {
            MinWidth = 200;
            IsExpanded = true;
        }
        public void RaiseControlChanged(object sender) 
        { 
            ControlChanged?.Invoke(sender);
            var fullPath = GetFullPath();
            if (fullPath.Length > MAX_PATH_L)
                Logger.Log(fullPath + " is too long for a Windows path!", MessageTypeEnum.Warning);

            UpdateChildren();
        }
        public ModelDoc2 GetModel() => AssignedModel;

        public string GetFullPath()
        {
            var fullPath = DocDir + Path.DirectorySeparatorChar + DocNo;

            if (!DocText.IsNullOrEmpty())
                fullPath += (IsFile ? AddInSettings.SEP_TEXT : AddInSettings.SEP_FOLDER_NUMERATION) + DocText;

            if (IsFile)
                fullPath += DocExt;

            return fullPath;
        }

        public string GetPointingPath()
        {
            return IsFile ? DocDir : GetFullPath();
        }

        public abstract void ExecuteAction();

        public void RegisterChild(DocumentAction child)
        {
            ChildActions.Add(child);
            UpdateChild(ChildActions.Count - 1);
        }

        #region METHODS TO UPDATE CHILDREN

        public void UpdateChild(int childIdx)
        {
            var pointingPath = GetPointingPath();

            var nextNo = IsFile ?
                DocNo + AddInSettings.SEP_FILE_NUMERATION + (AddInSettings.SEQUENCE_START + childIdx).ToString() :
                GetNextChildPrefix(pointingPath, ChildActions[childIdx].IsFile, ChildActions[childIdx].DocExt);

            ChildActions[childIdx].UpdateParameters(pointingPath, nextNo, IsActive);
        }

        public void UpdateChildren()
        {
            for (int i = 0; i < ChildActions.Count; i++)
            {
                UpdateChild(i);
            }
        }

        #endregion

        #region METHODS TO BE UPDATED BY PARENT
        public void UpdateParameters(string docDir, string docNo, bool isActive)
        {
            DocDir = docDir;
            DocNo = docNo;
            IsActive = isActive;
        }

        #endregion

        
    }
}
