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
    public sealed partial class DropdownBoxEditDialog : ContentDialog
    {
        private readonly AppGloableData.CollectionParameter mType;
        private readonly int oldSelectIndex = -1;
        public DropdownBoxEditDialog(AppGloableData.CollectionParameter type, int selectIndex)
        {
            this.InitializeComponent();

            mType = type;
            oldSelectIndex = selectIndex;
            IsPrimaryButtonEnabled = false;

            //修改按钮样式
            var bst = new Windows.UI.Xaml.Style(typeof(Button));
            bst.Setters.Add(new Setter(Button.BackgroundProperty, Windows.UI.Colors.DodgerBlue));
            bst.Setters.Add(new Setter(Button.ForegroundProperty, Windows.UI.Colors.White));
            this.PrimaryButtonStyle = bst;

            //set title
            switch (mType)
            {
                case AppGloableData.CollectionParameter.BOOK_LANGUAGES:
                    labelTitle.Text = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("ViewBook_Edit_Title_Languages");
                    inputValue.ItemsSource = AppGloableData.getLanguageDrawNameList();
                    if(selectIndex != -1)
                    {
                        inputValue.SelectedIndex = selectIndex;
                    }
                    break;
                case AppGloableData.CollectionParameter.BOOK_COUNTRY:
                    labelTitle.Text = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("ViewBook_Edit_Title_Country");
                    inputValue.ItemsSource = AppGloableData.getCountryDrawNameList();
                    if (selectIndex != -1)
                    {
                        inputValue.SelectedIndex = selectIndex;
                    }
                    break;
                default: break;
            }
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.Content = inputValue.SelectedItem.ToString();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void inputValue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox == null) return;
            IsPrimaryButtonEnabled = false;
            if (comboBox.SelectedIndex != oldSelectIndex) 
            {
                IsPrimaryButtonEnabled = true;
            }
        }
    }
}
