using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace MyBookManager
{
    public sealed partial class CustomTagEditDialog : ContentDialog
    {
        private readonly AppGloableData.CollectionParameter mType;
        public CustomTagEditDialog(AppGloableData.CollectionParameter type, String oldValue)
        {
            this.InitializeComponent();
            mType = type;
            IsPrimaryButtonEnabled = false;

            //修改按钮样式
            var bst = new Windows.UI.Xaml.Style(typeof(Button));
            bst.Setters.Add(new Setter(Button.BackgroundProperty, Windows.UI.Colors.DodgerBlue));
            bst.Setters.Add(new Setter(Button.ForegroundProperty, Windows.UI.Colors.White));
            this.PrimaryButtonStyle = bst;

            //set title
            switch (mType)
            {
                case AppGloableData.CollectionParameter.BOOK_TAGS:
                    labelTitle.Text = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("ViewBook_Edit_Title_Tags");
                    break;
                default: break;
            }

            inputedTags.Text = oldValue;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.Content = inputedTags.Text;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void addTag_Click(object sender, RoutedEventArgs e)
        {
            string tagText = inputTags.Text;
            if (0 == tagText.Length) return;

            if (0 != inputedTags.Text.Length) inputedTags.Text += " ";
            inputedTags.Text += "#" + tagText;
            IsPrimaryButtonEnabled = true;

            inputTags.Text = "";
        }

        private void removeLastTag_Click(object sender, RoutedEventArgs e)
        {
            string tagText = inputedTags.Text;
            if (0 == tagText.Length) return;

            int lastIndex = tagText.LastIndexOf("#");
            string newTags = tagText.Substring(0, lastIndex);
            inputedTags.Text = newTags.TrimEnd();

            IsPrimaryButtonEnabled = true;
        }

        private void inputTags_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            if (textBox.Text != "")
            {
                if (textBox.Text[textBox.Text.Length - 1] == '#')
                {
                    int pos = textBox.SelectionStart - 1;
                    textBox.Text = textBox.Text.Remove(pos, 1);
                    textBox.SelectionStart = pos;
                }
            }
        }
    }
}
