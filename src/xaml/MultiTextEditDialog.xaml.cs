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
    public sealed partial class MultiTextEditDialog : ContentDialog
    {
        private readonly AppGloableData.CollectionParameter mType;
        public MultiTextEditDialog(AppGloableData.CollectionParameter type, String oldValue)
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
                case AppGloableData.CollectionParameter.BOOK_AUTHOR:
                    labelTitle.Text = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("ViewBook_Edit_Title_Author");
                    break;
                case AppGloableData.CollectionParameter.BOOK_TRANSLATOR:
                    labelTitle.Text = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("ViewBook_Edit_Title_Translator");
                    break;
                case AppGloableData.CollectionParameter.BOOK_DESCRIPTION:
                    labelTitle.Text = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("ViewBook_Edit_Title_Description");
                    break;
                case AppGloableData.CollectionParameter.BOOK_CATEGORYS:
                    labelTitle.Text = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("ViewBook_Edit_Title_Categorys");
                    break;
                default: break;
            }

            inputValue.Text = oldValue;
            inputValue.Select(inputValue.Text.Length, 0);
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.Content = inputValue.Text;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void inputValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            IsPrimaryButtonEnabled = false;
            if (textBox.Text != "") 
            {
                IsPrimaryButtonEnabled = true;
            }
        }
    }
}
