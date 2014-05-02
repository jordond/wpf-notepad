/*
 * Name:            Project Three - Text Editor
 * FileName:        MainWindow.xaml.cs
 * Author:          Jordon de Hoog
 * Student Number:  0460685
 * Create Date:     June 8th 2013
 * Due Date:        June 21st 2013
 * Description:     Create a text editor
 *                  
 * TODO: 
 *       Create document functions in new class or in File.cs                     --in progress
 *       Create editing modes for rtf, txt, and cs                                  
 *       Change tab names to match saved file name
 *       
 *       If time permits:                       --update june 18th - unlikely
 *                           Create "greyscale" version of buttom images to indicate not appliciable
 *                           Add "unsaved" features to the TabItem Header - like notepad++
 *                           Add close buttons to the tabitem headers
 *                            
 * NOTE:
 *          I implemented the Extended WPF Toolkit in my project for the use of its ColorPicker control.
 *          so Xceed.Wpf.Toolkit - is needed to run
 *          http://wpftoolkit.codeplex.com/                           
 *                      
 * Revisions:
 *              June 8th - Create project add GUI elements
 *              June 9th - Finished most of GUI started working on C# backend
 *              June 9th/10th - Got most of program working with single tab and richtextbox
 *              June 12th - Version 1 of multitabbed richtextboxes
 *              June 13th - Cleaned up multitabbed editor, created todo list
 *              June 14th - Custom controls for menu and toolbar, created TaskManager class
 *              June 14th - Broke the check for opening duplicates added to todo - broke a lot more trying to fix, reverted back old style so it was functional for checkpoint.
 *              June 16th - fixed some issues with opening files
 *              June 17th - Added save functionality, can only save rtf currently - and now text
 *              June 19th - Fixed up saving, adding confirmation before closing documents
 *              June 21st - Fixed zoom slider
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.IO;
namespace TextEditor
{
    public partial class MainWindow : Window
    {
        public static List<File> g_files = new List<File>();
        private TabManager g_TabManager;
        
        public MainWindow()
        {
            InitializeComponent();                    
        }

        private void main_Loaded(object sender, RoutedEventArgs e)
        {
            g_TabManager = new TabManager();
            createBlankTab("default");
            g_TabManager = new TabManager(g_files[0]);
            GetActiveBox().AddHandler(RichTextBox.SelectionChangedEvent, new RoutedEventHandler(RichTextBox_SelectionChanged)); //register event handler to that new document   
        }
        
        private void OpenExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            List<string> fileNames = g_TabManager.readFileNames();

            if (fileNames != null)
            {
                //Check to see if the first tab is a new blank
                if ((string)g_files[0].Tab.Header == "new 1" && g_files[0].IsChanged == false && g_files.Count < 2)
                {
                    g_files.RemoveAt(0);
                    mainTabControl.Items.RemoveAt(0);
                }                
                foreach (string name in fileNames)
                {
                    //Check if file is already open, then change tab to that item
                    File result = g_files.FirstOrDefault(t => t.Path == name);
                    if (result != null)
                    {
                        setActiveTabObject(result.Tab);
                        continue;
                    }

                    File temp = g_TabManager.createNewTab(name);
                    mainTabControl.Items.Add( temp.Tab );
                    mainTabControl.SelectedItem = temp.Tab;

                    //Add the temporary file object to the list
                    g_files.Add(temp);
                }
                //Set focus to newly opened tab, change title                
                mainWindow.Title = GetActiveTabObject().Path + " - TextyEdit++";
            }            
        }//end openexecute

        private void NewTabExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            createBlankTab(null);

            textStatus.Text = "Status: Created new tab!";
        }

        private void createBlankTab(string name)
        {
            //When opened create new blank tab
            File blank = g_TabManager.blankTab();
            if ( name != null )
                blank.Tab.Name = name;     //default first blank tab
            mainTabControl.Items.Add(blank.Tab);
            mainTabControl.SelectedIndex = g_files.Count;
            g_files.Add(blank);
            mainWindow.Title = blank.Tab.Header + " - TextyEdit++";
        }

        private void CanExecute(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = true; }

        private void PrintExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (g_TabManager.PrintTab())
                textStatus.Text = "Status: Print Successful!";
        }

        private void SaveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if ((string)e.Parameter == "all")
            {
                g_TabManager.SaveAllTabs(mainTabControl.Items.Count);
                textStatus.Text = "Document Saved";
                
            }
            else if ((string)e.Parameter == "as")
            {
                if (g_TabManager.SaveDocumentAs())
                    textStatus.Text = "Document Saved";
            }
            else
            {
                g_TabManager.SaveDocument();
                textStatus.Text = "Document Saved";
            }
        }

        private void CloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Environment.Exit(0);
        }
        
        //Check status of the selected text
        private void RichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            maintoolbar.Synchronize(GetActiveBox());
            GetActiveTabObject().IsChanged = true;
        }

        private void CloseTab(object sender, ExecutedRoutedEventArgs e)
        {
            //Pull the command paramater from the event
            string command = e.Parameter as string;

            //Check what tabs to close
            if (command == "current" || command == null)    //If close current or Ctrl-W was clicked/pressed
            {
                ////Create and initilize a file object
                File temp = GetActiveTabObject();

                if (temp.IsChanged == true )
                {
                    MessageBoxResult result = MessageBox.Show("Would you like to save " + temp.Tab.Header + " before closing?", "Save before Closing", MessageBoxButton.YesNoCancel, MessageBoxImage.Asterisk);
                    if (result == MessageBoxResult.Yes)
                    {
                        if (!g_TabManager.SaveDocument())
                            return;
                    }
                    else if (result == MessageBoxResult.Cancel)
                        return;                            
                }
                //Remove the tab from the collection and the list
                mainTabControl.Items.Remove(temp.Tab);
                g_files.Remove(temp);

                //If that was the last tab open a new blank tab
                if (g_files.Count == 0)
                    createBlankTab("default");

                textStatus.Text = "Status: Closed current file";
            }
            else if (command == "all")
            {
                for (int i = 0; i < g_files.Count; i++)
                {
                    if (g_files[i].IsChanged == true)
                    {
                        MessageBoxResult result = MessageBox.Show("Would you like to save " + g_files[i].Tab.Header + " before closing?", "Save before Closing", MessageBoxButton.YesNoCancel, MessageBoxImage.Asterisk);
                        if (result == MessageBoxResult.Yes)
                            if (!g_TabManager.SaveDocument())
                                continue; //next loop
                        else if (result == MessageBoxResult.Cancel)
                            return; //exit loop
                    }
                }
                //Close all the current tabs then create a new blank one
                g_files.Clear();
                mainTabControl.Items.Clear();
                g_TabManager.blankTab();

                createBlankTab("default");
                
                textStatus.Text = "Status: Closed all files";
            }
            else if (command == "allbut")
            {
                File temp = GetActiveTabObject();
                for (int i = 0; i < g_files.Count; i++)
                {
                    if (g_files[i].Tab.Header == temp.Tab.Header)
                        continue;
                    if (g_files[i].IsChanged == true)
                    {
                        MessageBoxResult result = MessageBox.Show("Would you like to save " + g_files[i].Tab.Header + " before closing?", "Save before Closing", MessageBoxButton.YesNoCancel, MessageBoxImage.Asterisk);
                        if (result == MessageBoxResult.Yes)
                            if (!g_TabManager.SaveDocument())
                                continue; //next loop
                            else if (result == MessageBoxResult.Cancel)
                                return; //exit loop
                    }
                }
                //Get the current tab then remove all but it from list and tab control                
                if (temp != null)
                {
                    mainTabControl.Items.Clear();
                    g_files.Clear();
                    mainTabControl.Items.Add(temp.Tab);
                    mainTabControl.SelectedItem = temp.Tab;
                    g_files.Add(temp);
                }
            }//end close all but
        }

        private void ToggleEditorMode(object sender, ExecutedRoutedEventArgs e)
        {
            string command = e.Parameter as string;
            switch (command)
            {
                case "plain": mainMenu.menuPlain.IsChecked = true; mainMenu.menuRich.IsChecked = false; mainMenu.menuCode.IsChecked = false; textMode.Text = "Plain Text"; break;
                case "rich": mainMenu.menuPlain.IsChecked = false; mainMenu.menuRich.IsChecked = true; mainMenu.menuCode.IsChecked = false; textMode.Text = "Rich Text"; break;
                case "code": mainMenu.menuPlain.IsChecked = false; mainMenu.menuRich.IsChecked = false; mainMenu.menuCode.IsChecked = true; textMode.Text = "C# Source"; break;
            }            
        }

        public void setActiveTabObject(TabItem tab)
        {
            mainWindow.mainTabControl.SelectedItem = tab;
        }

        public RichTextBox GetActiveBox()
        {
            TabItem tab = (TabItem)mainTabControl.SelectedItem;
            ScrollViewer current = new ScrollViewer();
            if (tab != null)                
                current = (ScrollViewer)tab.Content;
            RichTextBox activeBox = null;

            if (current != null)
                activeBox = current.Content as RichTextBox;
            return activeBox;
        }

        public File GetActiveTabObject()
        {
            //Search for the selected item then return it
            File result = g_files.FirstOrDefault(t => t.Tab == (TabItem)mainTabControl.SelectedItem);
            if (result != null)
                return result;

            return null;
        }//end getActiveTab

        private void TabControl_ChangeTab(object sender, SelectionChangedEventArgs e)
        {     
            //Create dynamic event handler for the newly selected richtext box
            RichTextBox active = GetActiveBox();
            if(active != null)
                GetActiveBox().AddHandler(RichTextBox.SelectionChangedEvent, new RoutedEventHandler(RichTextBox_SelectionChanged));
            
            //Set the title
            File temp = GetActiveTabObject();
            if (temp != null)
            {
                //Assign the newly selected tab to tabmanager
                g_TabManager = new TabManager(g_files.ElementAt(mainTabControl.SelectedIndex));
                                
                if (temp.Path == "")
                    mainWindow.Title = temp.Tab.Header + " - TextyEdit++";
                else
                    mainWindow.Title = temp.Path + " - TextyEdit++";
            }
        }//end of Tab change

        private void mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void CanSave(object sender, CanExecuteRoutedEventArgs e)
        {
            if (g_TabManager != null)
                e.CanExecute = g_TabManager.CanSaveDocument();
        }

        private void ToggleOnTop(object sender, ExecutedRoutedEventArgs e)
        {
            Window top = Window.GetWindow(mainWindow);
            if (mainMenu.menuTop.IsChecked == true)
                top.Topmost = true;
            else
                top.Topmost = false;
            
        }
    }//end of main window
        
    public static class Command
    {
        public static RoutedUICommand NewTab = new RoutedUICommand("NewTab", "NewTab", typeof(MainWindow));
        public static RoutedUICommand CloseTab = new RoutedUICommand("CloseTab", "CloseTab", typeof(MainWindow));
        public static RoutedUICommand ToggleMode = new RoutedUICommand("ToggleMode", "ToggleMode", typeof(MainWindow));
        public static RoutedUICommand ToggleOnTop = new RoutedUICommand("ToogleOnTop", "ToggleOnTop", typeof(MainWindow));
        
    }
}