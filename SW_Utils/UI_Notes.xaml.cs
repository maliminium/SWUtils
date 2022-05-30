using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using AngelSix.SolidDna;

namespace SW_Utils
{
    /// <summary>
    /// Interaction logic for NotesUI.xaml
    /// </summary>
    public partial class UI_Notes : UserControl
    {
        private const string STR_TRUE = "T";    //UI_Properties'de de var
        private const string STR_FALSE = "F";    //UI_Properties'de de var
        private const string STR_SPLIT = ";;";
        private const string CP_IsDone = "swuIsDone";
        private const string CP_Note = "swuNote";
        private const string CP_ToDoList = "swuToDoList";

        private readonly System.Windows.GridLength GRD_LEN_EDIT_ON = new System.Windows.GridLength(55);
        private readonly System.Windows.GridLength GRD_LEN_EDIT_OFF = new System.Windows.GridLength(30);

        public UI_Notes()
        {
            InitializeComponent();
        }

        
        private void LoadDocumentInfo()
        {
            ThreadHelpers.RunOnUIThread(() =>
            {
                var model = SolidWorksEnvironment.Application.ActiveModel;
                if (model == null)
                {
                    grdMain.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    grdMain.Visibility = System.Windows.Visibility.Visible;



                    cbDone.IsChecked = model.GetCustomProperty(CP_IsDone) == STR_TRUE ? true : false;

                    var noteText = model.GetCustomProperty(CP_Note);

                    rtbNote.Document.Blocks.Clear();

                    rtbNote.AppendText(noteText);

                    for (int i = 0; i < stpList.Children.Count; i++)
                    {
                        RemoveItem(stpList.Children[0]);
                        i--;
                    }
                    var listInfo = model.GetCustomProperty(CP_ToDoList);
                    if (!string.IsNullOrWhiteSpace(listInfo))
                    {
                        foreach (var item in listInfo.Split(STR_SPLIT.ToCharArray()))
                        {
                            if (item.Length > 2)
                            {
                                var itemContent = item.Substring(1);
                                var isChecked = item.StartsWith(CheckListItem.STR_CHECKED);
                                AddItem(itemContent, isChecked);
                            }
                        }
                    }
                }
            });
        }
        private void SaveDocumentInfo()
        {
            var model = SolidWorksEnvironment.Application.ActiveModel;

            var infoList = new List<string>();
            for (int i = 0; i < stpList.Children.Count; i++)
            {
                infoList.Add(((CheckListItem)stpList.Children[i]).GetItemString());
            }
            model.SetCustomProperty(CP_ToDoList, string.Join(STR_SPLIT, infoList));
            model.UnsafeObject.SetSaveFlag();
        }



        #region Events
        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            try //One reason is  not to get error during development of MainAddInUI.xaml
            {
                SolidWorksEnvironment.Application.ActiveModelInformationChanged += Application_ActiveModelInformationChanged;
                LoadDocumentInfo();                
            }
            catch
            {

            }
            finally
            {
                SetEditMode(false);
            }
            
        }
        private void Application_ActiveModelInformationChanged(Model obj) => LoadDocumentInfo();
        #endregion        

        #region CheckListMethods
        private void AddItem(string itemContent, bool isChecked)
        {
            var item = new CheckListItem(itemContent, isChecked);
            item.ValueChanged += item_ValueChanged;
            item.DeleteRequested += item_DeleteRequested;
            stpList.Children.Add(item);
        }
        private void RemoveItem(object itemToRemove)
        {
            var item = (CheckListItem)itemToRemove;
            item.ValueChanged -= item_ValueChanged;
            item.DeleteRequested -= item_DeleteRequested;
            stpList.Children.Remove(item);
        }
        private void AddNewItem()
        {
            if (!string.IsNullOrWhiteSpace(txtNewContent.Text))
            {
                txtNewContent.Text = txtNewContent.Text.Trim();
                AddItem(txtNewContent.Text, false);
                txtNewContent.Text = string.Empty;
                SaveDocumentInfo();
            }
        }
        private void SetEditMode(bool isOn)
        {
            if (isOn)
            {
                colButtons.Width = GRD_LEN_EDIT_ON;
                btnSave.Visibility = System.Windows.Visibility.Visible;
                btnCancel.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                colButtons.Width = GRD_LEN_EDIT_OFF;
                btnSave.Visibility = System.Windows.Visibility.Hidden;
                btnCancel.Visibility = System.Windows.Visibility.Hidden;
            }
        }
        private void txtNewContent_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddNewItem();
            }
            else if (e.Key == Key.Escape)
            {
                txtNewContent.Text = string.Empty;
            }
        }
        private void txtNewContent_LostFocus(object sender, System.Windows.RoutedEventArgs e) => SetEditMode(false);
        private void txtNewContent_GotFocus(object sender, System.Windows.RoutedEventArgs e) => SetEditMode(true);
        private void btnSave_Click(object sender, System.Windows.RoutedEventArgs e) => AddNewItem();
        private void btnCancel_Click(object sender, System.Windows.RoutedEventArgs e) => txtNewContent.Text = string.Empty;
        private void item_DeleteRequested(object sender, System.EventArgs e) => RemoveItem(sender);
        private void item_ValueChanged(object sender, System.EventArgs e) => SaveDocumentInfo();
        #endregion



        private void rtbNote_TextChanged(object sender, TextChangedEventArgs e)
        {
            var model = SolidWorksEnvironment.Application.ActiveModel;
            model.SetCustomProperty(CP_Note, new TextRange(rtbNote.Document.ContentStart, rtbNote.Document.ContentEnd).Text);
            model.UnsafeObject.SetSaveFlag();
        }

        private void cbDone_Changed(object sender, System.Windows.RoutedEventArgs e)
        {
            var model = SolidWorksEnvironment.Application.ActiveModel;
            var value = cbDone.IsChecked == true ? STR_TRUE : STR_FALSE;
            model.SetCustomProperty(CP_IsDone, value);
            model.UnsafeObject.SetSaveFlag();
        }

        #region MARKUP TRIALS


        //private static string GetPreceedingWordInParagraph(TextPointer position, out TextPointer wordStartPosition)

        //{

        //    wordStartPosition = null;

        //    string word = String.Empty;

        //    Paragraph paragraph = position.Paragraph;

        //    if (paragraph != null)

        //    {

        //        TextPointer navigator = position;

        //        while (navigator.CompareTo(paragraph.ContentStart) > 0)

        //        {

        //            string runText = navigator.GetTextInRun(LogicalDirection.Backward);

        //            if (runText.Contains(" ")) // Any globalized application would need more sophisticated word break testing.

        //            {

        //                int index = runText.LastIndexOf(" ");

        //                word = runText.Substring(index + 1, runText.Length - index - 1) + word;

        //                wordStartPosition = navigator.GetPositionAtOffset(-1 * (runText.Length - index - 1));

        //                break;

        //            }

        //            else

        //            {

        //                wordStartPosition = navigator;

        //                word = runText + word;

        //            }

        //            navigator = navigator.GetNextContextPosition(LogicalDirection.Backward);

        //        }

        //    }

        //    return word;

        //}

        //private void rtbNote_PreviewKeyDown(object sender, KeyEventArgs e)
        //{
        //    //TextPointer caretPosition = rtbNote.Selection.Start;

        //    //if (e.Key == Key.Space || e.Key == Key.Enter)
        //    //{
        //    //    try
        //    //    {
        //    //        TextPointer wordStartPosition;

        //    //        string word = GetPreceedingWordInParagraph(caretPosition, out wordStartPosition);

        //    //        Uri uri;


        //    //        if (Uri.TryCreate(word, UriKind.Absolute, out uri)) // A real app would need a more sophisticated RegEx match expression for hyperlinks.

        //    //        {
        //    //            Logger.Log(uri.AbsoluteUri);

        //    //            // Insert hyperlink element at word boundaries.

        //    //            var link = new Hyperlink(

        //    //                wordStartPosition.GetPositionAtOffset(0, LogicalDirection.Backward),

        //    //                caretPosition.GetPositionAtOffset(0, LogicalDirection.Forward));
        //    //            link.Click += Link_Click;
        //    //        }
        //    //    }
        //    //    catch (Exception ex)
        //    //    {
        //    //        Logger.Log(ex.Message);
        //    //    }
        //    //}
        //}

        //private void Link_Click(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    var link = (Hyperlink)sender;
        //    Logger.Log("navigate to " + link.NavigateUri);
        //}
        #endregion
    }
}
