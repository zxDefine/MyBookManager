using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
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
    public sealed partial class SingleTextEditDialog : ContentDialog
    {
        private readonly AppGloableData.CollectionParameter mType;
        public SingleTextEditDialog(AppGloableData.CollectionParameter type, String oldValue)
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
                case AppGloableData.CollectionParameter.BOOK_ISBN:
                    labelTitle.Text = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("ViewBook_Edit_Title_ISBN");
                    break;
                case AppGloableData.CollectionParameter.BOOK_TITLE:
                    labelTitle.Text = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("ViewBook_Edit_Title_Title");
                    break;
                case AppGloableData.CollectionParameter.BOOK_SUBTITLE:
                    labelTitle.Text = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("ViewBook_Edit_Title_Subtitle");
                    break;
                case AppGloableData.CollectionParameter.BOOK_PUBLISHERS:
                    labelTitle.Text = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("ViewBook_Edit_Title_Publishers"); 
                    break;
                case AppGloableData.CollectionParameter.BOOK_PRICE:
                    labelTitle.Text = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("ViewBook_Edit_Title_Price"); 
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
            bool isNotValid = false;
            if(textBox.Text != "") 
            {
                switch (mType)
                {
                    case AppGloableData.CollectionParameter.BOOK_ISBN:
                        if(!Regex.IsMatch(textBox.Text, "[0-9]+$")) isNotValid = true;
                        break;
                    case AppGloableData.CollectionParameter.BOOK_TITLE:break;
                    case AppGloableData.CollectionParameter.BOOK_SUBTITLE:break;
                    case AppGloableData.CollectionParameter.BOOK_PUBLISHERS:break;
                    case AppGloableData.CollectionParameter.BOOK_PRICE:
                        string priceStr = textBox.Text;
                        float price = -1.0F;
                        if (!Regex.IsMatch(textBox.Text, "^[0-9.]+$"))
                        {
                            isNotValid = true;
                        }
                        else if(priceStr != "" && !float.TryParse(priceStr, out price)) 
                        {
                            isNotValid = true;
                        }
                        break;
                    default:break;
                }

                if (isNotValid)
                {
                    int pos = textBox.SelectionStart - 1;
                    textBox.Text = textBox.Text.Remove(pos, 1);
                    textBox.SelectionStart = pos;
                }

                IsPrimaryButtonEnabled = true;
            }
        }
    }
}
