using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DropDownMenuControl
{
    public class DropDownMenu : ContentControl
    {
        static DropDownMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownMenu), new FrameworkPropertyMetadata(typeof(DropDownMenu)));
        }

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(DropDownMenu), new PropertyMetadata(false));

        public string DropDownTextPart
        {
            get { return (string)GetValue(DropDownTextPartProperty); }
            set { SetValue(DropDownTextPartProperty, value); }
        }

        public static readonly DependencyProperty DropDownTextPartProperty =
            DependencyProperty.Register("DropDownTextPart", typeof(string), typeof(DropDownMenu), new PropertyMetadata("Text_Part"));


        public Brush IconFill
        {
            get { return (Brush)GetValue(IconFillProperty); }
            set { SetValue(IconFillProperty, value); }
        }

        public static readonly DependencyProperty IconFillProperty =
            DependencyProperty.Register("IconFill", typeof(Brush), typeof(DropDownMenu), new PropertyMetadata(Brushes.Black));


    }
}