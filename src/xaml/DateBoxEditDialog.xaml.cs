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
    public sealed partial class DateBoxEditDialog : ContentDialog
    {
        private readonly AppGloableData.CollectionParameter mType;
        string oldDate = "";
        public DateBoxEditDialog(AppGloableData.CollectionParameter type, string oldValue)
        {
            this.InitializeComponent();
            mType = type;
            oldDate = oldValue;
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
                    labelTitle.Text = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("ViewBook_Edit_Title_PublishDate");
                    break;
                default: break;
            }

            if(oldDate != "") 
            {
                inputValue.SelectedDate = new DateTime(int.Parse(oldDate), 01, 01);
            }
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.Content = inputValue.Date.Year.ToString();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void inputValue_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            var date = (DatePicker)sender;
            IsPrimaryButtonEnabled = false;
            string newDate = inputValue.Date.Year.ToString();
            if (oldDate != newDate) 
            {
                IsPrimaryButtonEnabled = true;
            }
        }
    }
}
