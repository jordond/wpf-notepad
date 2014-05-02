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

namespace TextEditor
{
    /// <summary>
    /// Interaction logic for TextEditorToolBar.xaml
    /// </summary>
    public partial class TextEditorToolBar : UserControl
    {
        RichTextBox activeBox = new RichTextBox();

        public TextEditorToolBar()
        {
            InitializeComponent();            
        }
        
        //I did it this way for a reason, Can't remember that reason but i know i had a purpose.
        public void setFontSize()
        {
            //Create an array of doubles for the sizes of the fonts
            double[] sizes = new double[]             
              { 6.0,6.5,7.0,7.5,8.0,8.5,9.0,9.5,10.0,10.5,11.0,11.5,12.0,12.5,13.0,13.5,14.0,15.0,
                16.0,17.0,18.0,19.0,20.0,22.0,24.0,26.0,28.0,30.0,32.0,34.0,36.0,38.0,40.0,44.0,48.0,
                52.0,56.0,60.0,64.0,68.0,72.0,76.0,80.0,88.0,96.0,104.0,112.0,120.0,128.0,136.0,144.0 };

            //Assign the sizes to the combobox
            comboSize.ItemsSource = sizes;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            setFontSize();
        }

        public void Synchronize(RichTextBox rtb)
        {
            activeBox = rtb;
            updateFont();
            updateSizeFont();
            updateColor();
            updateToggleButtons();
        }

        //Apply the font of the text 
        private void comboFont_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboFont.SelectedItem != null)
            {
                FontFamily editValue = (FontFamily)e.AddedItems[0];
                applyToSelectedText(TextElement.FontFamilyProperty, editValue);
            }
        }

        //Apply the size change to the selected text
        private void comboSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //get the font size, then send apply to text
            if (comboSize.SelectedItem != null)
            {
                double fontSize = (double)comboSize.SelectedItem;
                applyToSelectedText(TextElement.FontSizeProperty, fontSize);
            }
        }

        private void colorPicked(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            if (colorPicker.SelectedColor != null)
            {
                //Get text color and set to the selected text
                SolidColorBrush brush = (SolidColorBrush)new BrushConverter().ConvertFromString(colorPicker.SelectedColorText);
                applyToSelectedText(TextElement.ForegroundProperty, brush);

                //Fix problems with pasted text with different background color
                brush = Brushes.White;
                applyToSelectedText(TextElement.BackgroundProperty, brush);
            }
        }

        //Straight forward..
        public void applyToSelectedText(DependencyProperty formattingProperty, object value)
        {
            if (value == null)
                return;

            activeBox.Selection.ApplyPropertyValue(formattingProperty, value);
        }

        //Check whether or not a toggle button should be selected or not
        private void updateToggleButtons()
        {
            itemChecked(tBold, TextElement.FontWeightProperty, FontWeights.Bold);
            itemChecked(tItalic, TextElement.FontStyleProperty, FontStyles.Italic);
            itemChecked(tUnderline, Inline.TextDecorationsProperty, TextDecorations.Underline);
            itemChecked(tLeft, Paragraph.TextAlignmentProperty, TextAlignment.Left);
            itemChecked(tJustify, Paragraph.TextAlignmentProperty, TextAlignment.Right);
            itemChecked(tCenter, Paragraph.TextAlignmentProperty, TextAlignment.Center);
            itemChecked(tRight, Paragraph.TextAlignmentProperty, TextAlignment.Right);
        }//end updatetoggles

        //Update the value for the font of the selected text then change combo box
        private void updateFont()
        {
            object value = activeBox.Selection.GetPropertyValue(TextElement.FontFamilyProperty);
            FontFamily currentFontFamily = (FontFamily)((value == DependencyProperty.UnsetValue) ? null : value);
            if (currentFontFamily != null)
                comboFont.SelectedItem = currentFontFamily;
        }

        //Update the color of the selected font
        private void updateColor()
        {
            object value = activeBox.Selection.GetPropertyValue(TextElement.ForegroundProperty);
            SolidColorBrush currentColor = (SolidColorBrush)((value == DependencyProperty.UnsetValue) ? null : value);
            if (currentColor != null)
                colorPicker.SelectedColor = currentColor.Color;
        }

        //Update the value for the size of the font of the selected text then change combo box
        private void updateSizeFont()
        {
            object value = activeBox.Selection.GetPropertyValue(TextElement.FontSizeProperty);
            comboSize.SelectedValue = (value == DependencyProperty.UnsetValue) ? null : value;
        }

        //Get the current value of the selected text, to match if a button should be enabled
        void itemChecked(ToggleButton button, DependencyProperty formattingProperty, object expectedValue)
        {
            object currentValue = activeBox.Selection.GetPropertyValue(formattingProperty);
            button.IsChecked = (currentValue == DependencyProperty.UnsetValue) ? false : currentValue != null && currentValue.Equals(expectedValue);
        }

        private void ScaleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //Create transform object, and binding to slider value
            ScaleTransform scale = new ScaleTransform();
            Binding bindingX = new Binding("Value");
            Binding bindingY = new Binding("Value");
            bindingX.Source = ScaleSlider;
            bindingY.Source = ScaleSlider;

            //Set the bindings
            BindingOperations.SetBinding(scale, ScaleTransform.ScaleXProperty, bindingX);
            BindingOperations.SetBinding(scale, ScaleTransform.ScaleYProperty, bindingY);

            //Perform the zoom
            activeBox.LayoutTransform = scale;
        }

    }
}
