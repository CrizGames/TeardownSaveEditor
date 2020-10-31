using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TeardownSaveEditor.Controls
{
    /// <summary>
    /// Interaktionslogik für TitleTextBox.xaml
    /// </summary>
    public partial class TextField : UserControl
    {
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(TextField));


        public bool NumericOnly
        {
            get { return (bool)GetValue(NumericProperty); }
            set { SetValue(NumericProperty, value); }
        }

        public static DependencyProperty NumericProperty = DependencyProperty.Register("NumericOnly", typeof(bool), typeof(TextField));


        public event EventHandler<string> OnTextEntered;


        private string currentNumericText = "0";


        public TextField()
        {
            InitializeComponent();

            DataContext = this;

            if (NumericOnly)
                UpdateTextNumeric("");
        }

        public void SetText(string txt)
        {
            text.Text = txt;

            if (NumericOnly)
                UpdateTextNumeric(text.Text);

            OnTextEntered?.Invoke(this, text.Text);
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            if (text.Text.Length > 1)
            {
                if (NumericOnly)
                    UpdateTextNumeric(text.Text);
            }
            else
            {
                if (!text.Text.StartsWith("+") && !text.Text.StartsWith("-"))
                    text.Text = "";
            }
        }

        private void UpdateTextNumeric(string newTxt)
        {
            if (!NumericOnly) return;

            if (string.IsNullOrWhiteSpace(newTxt))
            {
                currentNumericText = text.Text = "0";
                return;
            }

            // IsNumeric with parsing greater than int.MaxValue
            if (float.TryParse(newTxt, out float f) && !newTxt.Contains(".") && !newTxt.Contains(","))
            {
                if (f > int.MaxValue)
                    text.Text = int.MaxValue.ToString();
                else if (f < int.MinValue)
                    text.Text = int.MinValue.ToString();

                currentNumericText = text.Text;
            }
            else
            {
                text.Text = currentNumericText;
                text.CaretIndex = text.Text.Length;
            }
        }

        private void textLostFocus(object sender, RoutedEventArgs e)
        {
            if (NumericOnly)
                UpdateTextNumeric(text.Text);

            OnTextEntered?.Invoke(this, text.Text);
        }

        private void textKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (NumericOnly)
                    UpdateTextNumeric(text.Text);

                OnTextEntered?.Invoke(this, text.Text);
            }
        }
    }
}
