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

namespace TextEditor
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class TextEditorMenu : UserControl
    {
        public TextEditorMenu()
        {
            InitializeComponent();
        }
        
        private void MenuHelp_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Texty Edit a simple Notepad++ clone.\nHowever if I were you I'd stick to Notepad++"
                    + "\nCreated by Jordon de Hoog 2013.", "About: Texty Edit++", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
        }

    }
}
