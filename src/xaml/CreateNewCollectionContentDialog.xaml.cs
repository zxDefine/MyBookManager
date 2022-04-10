using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace MyBookManager
{
    public sealed partial class CreateNewCollectionContentDialog : ContentDialog
    {
        private readonly AppGloableData.Types mType;
        public CreateNewCollectionContentDialog(String labelText, AppGloableData.Types type)
        {
            this.InitializeComponent();

            labelTitle.Text = labelText;
            mType = type;
            IsPrimaryButtonEnabled = false;

            //修改按钮样式
            var bst = new Windows.UI.Xaml.Style(typeof(Button));
            bst.Setters.Add(new Setter(Button.BackgroundProperty, Windows.UI.Colors.DodgerBlue));
            bst.Setters.Add(new Setter(Button.ForegroundProperty, Windows.UI.Colors.White));
            this.PrimaryButtonStyle = bst;
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            switch (mType)
            {
                case AppGloableData.Types.BOOK:
                    var newCollection = new BookCollectionClass();
                    int newID = AppGloableData.getNewBookCollectionID() + 1;
                    newCollection.Id = newID;
                    newCollection.Name = inputName.Text;
                    newCollection.CreatedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                    string json = JsonConvert.SerializeObject(newCollection);
                    string fileName = AppGloableData.appBooksFolderName + "\\book_" + newID.ToString() + "_" + inputName.Text + ".json";
                    var file = await ApplicationData.Current.RoamingFolder.CreateFileAsync(fileName);
                    await FileIO.WriteTextAsync(file, json, Windows.Storage.Streams.UnicodeEncoding.Utf8);
                    AppGloableData.plusBookCollectionID();
                    break;
                case AppGloableData.Types.GAME: 
                    break;
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void inputName_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            IsPrimaryButtonEnabled = false;
            if (textBox.Text != "")
            {
                if(!Regex.IsMatch(textBox.Text, "^\\w+$"))
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
