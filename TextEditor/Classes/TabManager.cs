using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.IO;
using Microsoft.Win32;
namespace TextEditor
{
    class TabManager : MainWindow
    {
        private File _currentTab;

        public TabManager()
        {         
        }
        public TabManager(File file)
        {
            _currentTab = file;
        }

        public File blankTab()
        {
            //Create a temporary object to hold info for blank tab
            File temp = new File();
            //Assign header and tab name
            temp.Tab.Name = "tab" + (MainWindow.g_files.Count + 1);
            temp.Tab.Header = "new " + (MainWindow.g_files.Count + 1);

            //Set up the scroll and RTB properties
            temp.Scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            temp.Scroll.CanContentScroll = true;
            Style removeSpace = new Style(typeof(Paragraph));
            removeSpace.Setters.Add(new Setter(Paragraph.MarginProperty, new Thickness(0)));
            temp.RTB.Resources.Add(typeof(Paragraph), removeSpace);
            temp.RTB.AcceptsTab = true;
            
            //Add the richtext box to scrollviewer, and scroll to tab
            temp.Scroll.Content = temp.RTB;
            temp.Tab.Content = temp.Scroll;
            
            return temp;
        }
        
        public List<string> readFileNames()
        {
            //Create the file open dialog
            Microsoft.Win32.OpenFileDialog openDialog = new Microsoft.Win32.OpenFileDialog();

            //Set the filter options
            openDialog.Title = "Select the files you want to open.";
            openDialog.Filter = "All files |*.txt; *.rtf; *.cs;|Text Files (.txt)|*.txt|Rich Text Files (.rtf)|*.rtf|C# Code File (.cs)|*.cs";
            openDialog.FilterIndex = 1;
            openDialog.Multiselect = true;

            //Show the dialog box to the user
            Nullable<bool> result = openDialog.ShowDialog();

            //Get the filename(s)
            if (result == true)
            {
                List<String> opened = new List<String>();

                //Add the file names to the list of all filenames
                foreach (string t in openDialog.FileNames)
                    opened.Add(t);
                return opened;
            }
            return null;
        }//end openexecute

        public File createNewTab(string path)
        {
            //Create new object to hold all the details about the tab
            File file = new File();

            //Assign access name to the new tab
            file.Tab.Name = "tab" + (g_files.Count + 1);

            //file variables
            file.Path = path;
            file.FileName = System.IO.Path.GetFileName(path);
            file.Tab.Header = file.FileName;

            //Set properties for the scroll bar
            file.Scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            file.Scroll.CanContentScroll = true;


            //Fix the added extra line in the richtext box
            Style removeSpace = new Style(typeof(Paragraph));
            removeSpace.Setters.Add(new Setter(Paragraph.MarginProperty, new Thickness(0)));
            file.RTB.Resources.Add(typeof(Paragraph), removeSpace);
            file.RTB.AcceptsTab = true;

            //Load the rich text box with data from the file            
            TextRange range = new TextRange(file.RTB.Document.ContentStart, file.RTB.Document.ContentEnd);
            FileStream input = new FileStream(file.Path, FileMode.OpenOrCreate);
            if (System.IO.Path.GetExtension(path) == ".txt")
                range.Load(input, DataFormats.Text);
            else if (System.IO.Path.GetExtension(path) == ".rtf")
                range.Load(input, DataFormats.Rtf);
            else
                range.Load(input, DataFormats.Text);
            input.Close();
           
            //Add the richtext box control to the scrollviewer
            file.Scroll.Content = file.RTB;

            //add the scrollviewer control to the tab item
            file.Tab.Content = file.Scroll;
                        
            return file;
        }

        public bool PrintTab()
        {
            //Create the print dialog object
            PrintDialog plg = new PrintDialog();
            plg.PageRangeSelection = PageRangeSelection.AllPages;

            //Show the file dialog
            Nullable<bool> result = plg.ShowDialog();

            if (result == true)
            {
                plg.PrintVisual(_currentTab.RTB as Visual, "TextyEdit");
                return true;
            }
            return false;
        }
        
        public bool CanSaveDocument()
        {
            if(_currentTab != null)
                return _currentTab.IsChanged;
            return false;
        }

        public bool SaveDocument()
        {
            if (string.IsNullOrEmpty(_currentTab.Path))
            {
                return SaveDocumentAs();
            }
            else
            {
                using (Stream stream = new FileStream(_currentTab.Path, FileMode.Create))
                {
                    TextRange range = new TextRange
                    (
                        _currentTab.RTB.Document.ContentStart,
                        _currentTab.RTB.Document.ContentEnd
                    );

                    if (System.IO.Path.GetExtension(_currentTab.Path) == ".rtf")
                        range.Save(stream, DataFormats.Rtf);
                    else
                        range.Save(stream, DataFormats.Text);
                }
                return true;
            }
        }
        public bool SaveDocumentAs()
        {
            SaveFileDialog dig = new SaveFileDialog();
            dig.Filter = "Text File|*.txt|RichText Files|*.rtf";
            if (dig.ShowDialog() == true)
            {
                _currentTab.Path = dig.FileName;
                _currentTab.FileName = dig.SafeFileName;
                return SaveDocument();
            }
            return false;
        }
        public void SaveAllTabs(int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (g_files[i].IsChanged == false )
                    continue;
                if (string.IsNullOrEmpty(g_files[i].Path))
                {
                    SaveFileDialog dig = new SaveFileDialog();
                    dig.Filter = "Text File|*.txt|RichText Files|*.rtf";
                    if (dig.ShowDialog() == true)
                    {
                        g_files[i].Path = dig.FileName;
                        g_files[i].FileName = dig.SafeFileName;
                        using (Stream stream = new FileStream(g_files[i].Path, FileMode.Create))
                        {
                            TextRange range = new TextRange
                            (
                                g_files[i].RTB.Document.ContentStart,
                                g_files[i].RTB.Document.ContentEnd
                            );

                            if (System.IO.Path.GetExtension(_currentTab.Path) == ".rtf")
                                range.Save(stream, DataFormats.Rtf);
                            else
                                range.Save(stream, DataFormats.Text);
                        }
                    }
                }
                else
                {
                    using (Stream stream = new FileStream(g_files[i].Path, FileMode.Create))
                    {
                        TextRange range = new TextRange
                        (
                            g_files[i].RTB.Document.ContentStart,
                            g_files[i].RTB.Document.ContentEnd
                        );

                        if (System.IO.Path.GetExtension(_currentTab.Path) == ".rtf")
                            range.Save(stream, DataFormats.Rtf);
                        else
                            range.Save(stream, DataFormats.Text);
                    }
                }
            }                
        }
        
    }//end of tab manager
}
