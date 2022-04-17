using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class DeleteCollectionDialog : ContentDialog
    {
        private BookPageNameList books = new BookPageNameList();

        public DeleteCollectionDialog()
        {
            this.InitializeComponent();

            IsPrimaryButtonEnabled = false;

            //修改按钮样式
            var bst = new Windows.UI.Xaml.Style(typeof(Button));
            bst.Setters.Add(new Setter(Button.BackgroundProperty, Windows.UI.Colors.DodgerBlue));
            bst.Setters.Add(new Setter(Button.ForegroundProperty, Windows.UI.Colors.White));
            this.PrimaryButtonStyle = bst;

            updateCollectionList();
        }

        private async void updateCollectionList()
        {
            var booksFolder = await ApplicationData.Current.RoamingFolder.GetFolderAsync(AppGloableData.appBooksFolderName);
            IReadOnlyList<StorageFile> fileList = await booksFolder.GetFilesAsync();
            foreach (var file in fileList)
            {
                //name: book_id_name.json
                string[] nameList = file.Name.Split('.', '_');
                books.add(int.Parse(nameList[1]), nameList[2]);
            }

            inputValue.ItemsSource = books.getNameList();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.Content = books.getBookFullName(inputValue.SelectedIndex);
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void inputValue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IsPrimaryButtonEnabled = true;
        }
    }
}
