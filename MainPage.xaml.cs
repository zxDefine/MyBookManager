using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

            AppGloableData.initGameAndBookCollectionBaseID(bookBaseID);
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
                case "zh":
                    if(language.Substring(0, 7) == "zh-Hans")
                    {
                        radBtn_lang_cn.IsChecked = true;
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
            else if (true == radBtn_lang_ja.IsChecked)
            {
                currLanguage = "ja";
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
                case "mfItem_btn_ja":
                    if (false == radBtn_lang_ja.IsChecked)
                    {
                        displayChangeLanguageDialog(currLanguage, "ja");
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

        private void btn_new_book_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(AddNewBookPage));
        }

        private void btn_book_view_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(ViewBookCollectionPage));
        }

        private void btn_find_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(FindBookInfoPage));
        }

        private async Task<bool> importErrorDialog(string resStr) 
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("Import_Error_Title"),
                Content = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString(resStr),
                PrimaryButtonText = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("ISBNSearchRes_Btn")
            };

            //修改按钮样式
            var bst = new Windows.UI.Xaml.Style(typeof(Button));
            bst.Setters.Add(new Setter(Button.BackgroundProperty, Windows.UI.Colors.Red));
            bst.Setters.Add(new Setter(Button.ForegroundProperty, Windows.UI.Colors.White));
            dialog.PrimaryButtonStyle = bst;

            await dialog.ShowAsync();

            return true;
        }

        private async void btn_Import_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            picker.FileTypeFilter.Add(".json");

            var booksFolder = await ApplicationData.Current.RoamingFolder.GetFolderAsync(AppGloableData.appBooksFolderName);

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                //文件格式 book_0_bookName.json
                var nameList = file.Name.Split('_');
                int outID = 0;
                string fileName = nameList[2].Substring(0, nameList[2].Length - 5);
                if (nameList[0] != "book" || nameList.Length != 3 || !int.TryParse(nameList[1] , out outID)) 
                {
                    await importErrorDialog("Import_Error_Content_FileName_Error");
                    return;
                }

                //文件ID有不有重复
                IReadOnlyList<StorageFile> bookFileList = await booksFolder.GetFilesAsync();
                if (bookFileList.Count > 0) 
                {
                    List<int> bookIdList = new List<int>();
                    foreach (StorageFile bookFile in bookFileList)
                    {
                        var bookFileNameList = bookFile.Name.Split('_');
                        bookIdList.Add(int.Parse(bookFileNameList[1]));
                    }
                    if (bookIdList.Contains(outID)) 
                    {
                        await importErrorDialog("Import_Error_Content_Id_Exist");
                        return;
                    }
                }

                //是否可以转换成BookCollectionClass
                string importFileStr = await Windows.Storage.FileIO.ReadTextAsync(file);
                BookCollectionClass bookCollection = null;
                try
                {
                    bookCollection = JsonConvert.DeserializeObject<BookCollectionClass>(importFileStr);
                }
                catch (Exception ex) 
                {
                    await importErrorDialog("Import_Error_Content_Cannot_Cover");
                    return;
                }
                //文件ID跟BookName跟文件内容是否匹配
                bool isIdMatch = bookCollection.Id == outID;
                bool isNameMatch = bookCollection.Name.Equals(fileName);
                if (bookCollection != null && (bookCollection.Id != outID || !bookCollection.Name.Equals(fileName)))
                {
                    await importErrorDialog("Import_Error_Content_NameID_Diff");
                    return;
                }

                await file.CopyAsync(booksFolder, file.Name, NameCollisionOption.ReplaceExisting);
            }
            else 
            {
                // cancelled
            }
        }

        private async void btn_Export_Click(object sender, RoutedEventArgs e)
        {
            var folderPicker = new Windows.Storage.Pickers.FolderPicker();
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            folderPicker.FileTypeFilter.Add("*");

            Windows.Storage.StorageFolder targetFolder = await folderPicker.PickSingleFolderAsync();
            var booksFolder = await ApplicationData.Current.RoamingFolder.GetFolderAsync(AppGloableData.appBooksFolderName);
            if (targetFolder != null)
            {
                Windows.Storage.AccessCache.StorageApplicationPermissions.
                FutureAccessList.AddOrReplace("PickedFolderToken", targetFolder);

                StorageFolder newFolder = null;
                newFolder = await targetFolder.CreateFolderAsync(booksFolder.Name, CreationCollisionOption.ReplaceExisting);

                foreach (var file in await booksFolder.GetFilesAsync()) 
                {
                    await file.CopyAsync(newFolder, file.Name, NameCollisionOption.ReplaceExisting);
                }
            }
            else
            {
                // cancelled
            }
        }

        private async void btn_Help_Click(object sender, RoutedEventArgs e)
        {
            DeleteCollectionDialog dialog = new DeleteCollectionDialog();
            ContentDialogResult result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary) 
            {
                deleteCollectionExecute(dialog.Content.ToString());
            }
        }

        private async void deleteCollectionExecute(string collectionName)
        {
            string[] nameList = collectionName.Split('.', '_');

            string content = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("MainPage_Del_Collection_Alarm_Content");
            content = content.Replace("XXXXXX", nameList[2]+"("+ collectionName+")");
            ContentDialog dialog = new ContentDialog
            {
                Title = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("MainPage_Del_Collection_Alarm_Title"),
                Content = content,
                PrimaryButtonText = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("ViewBook_Check_Save_Btn_Yes"),
                CloseButtonText = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("ViewBook_Check_Save_Btn_No")
            };

            //修改按钮样式
            var bst = new Windows.UI.Xaml.Style(typeof(Button));
            bst.Setters.Add(new Setter(Button.BackgroundProperty, Windows.UI.Colors.Red));
            bst.Setters.Add(new Setter(Button.ForegroundProperty, Windows.UI.Colors.White));
            dialog.PrimaryButtonStyle = bst;

            ContentDialogResult result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var booksFolder = await ApplicationData.Current.RoamingFolder.GetFolderAsync(AppGloableData.appBooksFolderName);
                StorageFile delFile = await booksFolder.GetFileAsync(collectionName);
                await delFile.DeleteAsync();

            }
        }
    }
}
