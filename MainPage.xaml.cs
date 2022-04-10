using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace MyBookManager
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();
            tryCreateAppFolder();
        }

        private async void tryCreateAppFolder()
        {
            StorageFolder folder = ApplicationData.Current.RoamingFolder;
            int bookBaseID = AppGloableData.appBeginId;
            int gameBaseID = AppGloableData.appBeginId;
            try
            {
                _ = await folder.CreateFolderAsync(AppGloableData.appGamesFolderName);
            }
            catch(Exception)
            {
                /// 已经创建
                var gamesFolder = await ApplicationData.Current.RoamingFolder.GetFolderAsync(AppGloableData.appGamesFolderName);
                IReadOnlyList<StorageFile> fileList = await gamesFolder.GetFilesAsync();
                gameBaseID = findBookAndGameFileBaseID(fileList);
            }

            try
            {
                _ = await folder.CreateFolderAsync(AppGloableData.appBooksFolderName);
            }
            catch(Exception)
            {
                /// 已经创建
                var booksFolder = await ApplicationData.Current.RoamingFolder.GetFolderAsync(AppGloableData.appBooksFolderName);
                IReadOnlyList<StorageFile> fileList = await booksFolder.GetFilesAsync();
                bookBaseID = findBookAndGameFileBaseID(fileList);
            }

            AppGloableData.initGameAndBookCollectionBaseID(gameBaseID, bookBaseID);
        }

        /// <summary>
        /// 根据收藏文件的命名规则找到文件名里面的最大ID，作为BaseID返回.
        /// fileList的size为0时，返回0.
        /// </summary>
        /// <param name="fileList"></param>
        /// <returns>base id</returns>
        private int findBookAndGameFileBaseID(IReadOnlyList<StorageFile> fileList)
        {
            int res = AppGloableData.appBeginId;
            foreach(var file in fileList)
            {
                string[] nameList = file.Name.ToString().Split('_');
                int idNum = int.Parse(nameList[1]);
                if (idNum > res)
                {
                    res = idNum;
                }
            }
            return res;
        }

        private void btn_Language_Click(object sender, RoutedEventArgs e)
        {
            string language = Windows.Globalization.ApplicationLanguages.Languages[0];
            string subStr = language.Substring(0, 2);
            switch (subStr)
            {
                case "en":
                    radBtn_lang_en.IsChecked = true;
                    break;
                case "ja":
                    radBtn_lang_ja.IsChecked = true;
                    break;
                case "ko":
                    radBtn_lang_ko.IsChecked = true;
                    break;
                case "zh":
                    if(language.Substring(0, 7) == "zh-Hans")
                    {
                        radBtn_lang_cn.IsChecked = true;
                    }
                    else
                    {
                        radBtn_lang_tw.IsChecked = true;
                    }
                    break;
            }
        }

        private void list_Language_ItemClick(object sender, ItemClickEventArgs e)
        {
            flyout_Language.Hide();
            var listView = sender as ListView;
            var clickedItem = e.ClickedItem as RelativePanel;

            String currLanguage = "";
            if (true == radBtn_lang_cn.IsChecked)
            {
                currLanguage = "zh-Hans";
            }
            else if(true == radBtn_lang_tw.IsChecked)
            {
                currLanguage = "zh-Hant";
            }
            else if (true == radBtn_lang_ja.IsChecked)
            {
                currLanguage = "ja";
            }
            else if (true == radBtn_lang_ko.IsChecked)
            {
                currLanguage = "ko";
            }
            else if (true == radBtn_lang_en.IsChecked)
            {
                currLanguage = "en-US";
            }

            switch (clickedItem.Name)
            {
                case "mfItem_btn_cn":
                    if (false == radBtn_lang_cn.IsChecked)
                    {
                        displayChangeLanguageDialog(currLanguage, "zh-Hans");
                    }
                    break;
                case "mfItem_btn_tw":
                    if (false == radBtn_lang_tw.IsChecked)
                    {
                        displayChangeLanguageDialog(currLanguage, "zh-Hant");
                    }
                    break;
                case "mfItem_btn_ja":
                    if (false == radBtn_lang_ja.IsChecked)
                    {
                        displayChangeLanguageDialog(currLanguage, "ja");
                    }
                    break;
                case "mfItem_btn_ko":
                    if (false == radBtn_lang_ko.IsChecked)
                    {
                        displayChangeLanguageDialog(currLanguage, "ko");
                    }
                    break;
                case "mfItem_btn_en":
                    if (false == radBtn_lang_en.IsChecked)
                    {
                        displayChangeLanguageDialog(currLanguage, "en-US");
                    }
                    break;
            }
        }

        private async void displayChangeLanguageDialog(String oldLanguage, String newLanguage)
        {
            ContentDialog changeLanguageDialog = new ContentDialog
            {
                Title = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("ChangeLanguageDialogTitle"),
                Content = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("ChangeLanguageDialogTxt"),
                PrimaryButtonText = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("ChangeLanguageDialog_Yes"),
                CloseButtonText = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("ChangeLanguageDialog_No")
            };

            //修改按钮样式
            var bst = new Windows.UI.Xaml.Style(typeof(Button));
            bst.Setters.Add(new Setter(Button.BackgroundProperty, Windows.UI.Colors.Red));
            bst.Setters.Add(new Setter(Button.ForegroundProperty, Windows.UI.Colors.White));
            changeLanguageDialog.PrimaryButtonStyle = bst;

            ContentDialogResult result = await changeLanguageDialog.ShowAsync();

            if(result == ContentDialogResult.Primary)
            {
                //设置新的语言
                Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = newLanguage;

                //重新启动程序
                await Windows.ApplicationModel.Core.CoreApplication.RequestRestartAsync("");
            }
        }

        private void list_Language_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //取消ListView里面的选中效果
            var value = sender as ListView;
            value.SelectedItem = null;
        }

        private void btn_new_game_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(AddNewGamePage));
        }

        private void btn_new_book_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(AddNewBookPage));
        }

        private void btn_game_view_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(ViewGameCollectionPage));
        }

        private void btn_book_view_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(ViewBookCollectionPage));
        }
    }
}
