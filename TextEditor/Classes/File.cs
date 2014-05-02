using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TextEditor
{
    public class File
    {
        private string fileName;
        private string path;
        private bool isChanged;
        private bool isSaved;
        private TabItem tab;
        private RichTextBox rtb;
        private ScrollViewer scroll;

        public string FileName { get { return fileName; } set { fileName = value; } }
        public string Path { get { return path; } set { path = value; } }
        public bool IsChanged { get { return isChanged; } set { isChanged = value; } }
        public bool IsSaved { get { return isSaved; } set { isSaved = value; } }
        public TabItem Tab { get { return tab; } set { tab = value; } }
        public RichTextBox RTB { get { return rtb; } set { rtb = value; } }
        public ScrollViewer Scroll { get { return scroll; } set { scroll = value; } }

        public File()
        {
            fileName = "";
            path = "";
            tab = new TabItem();
            rtb = new RichTextBox();
            scroll = new ScrollViewer();
            isChanged = false;
            isSaved = false;
        }

    }//end of Class
}
