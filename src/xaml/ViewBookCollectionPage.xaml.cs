using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage.Streams;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace MyBookManager
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ViewBookCollectionPage : Page
    {
        private bool imageEnable = false;
        private string currentImageBase64Str = "";
        private BookPageNameList books = new BookPageNameList();
        private BookCollectionClass bookCollection = null;

        private Brush borderBG;
        private bool isEditMode = false;
        private int currSelectBookId = -1;

        private Brush saveBtnBG;
        private Brush saveBtnFontColor;

        public ViewBookCollectionPage()
        {
            this.InitializeComponent();

            borderBG = editISBNBorder.Background;
            saveBtnBG = save_book_info.Background;
            saveBtnFontColor = save_book_info.Foreground;

            updateCollectionList();
        }

        private async void updateCollectionList()
        {
            //get book folder all file list
            var booksFolder = await ApplicationData.Current.RoamingFolder.GetFolderAsync(AppGloableData.appBooksFolderName);
            IReadOnlyList<StorageFile> fileList = await booksFolder.GetFilesAsync();
            setAllInputEnable(false);
            var hasFile = fileList.Count > 0;
            if (hasFile)
            {
                foreach (var file in fileList)
                {
                    //name: book_id_name.json
                    string[] nameList = file.Name.Split('.', '_');
                    books.add(int.Parse(nameList[1]), nameList[2]);
                }
                var sou = books.getNameList();
                var size = sou.Length;
                combobox_book_collections.ItemsSource = sou;
            }
        }

        private void btn_Back_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }
        }

        private void setAllInputEnable(bool enable = true)
        {
            editTitleBtn.IsEnabled = enable;
            editSubtitleBtn.IsEnabled = enable;
            editAuthorBtn.IsEnabled = enable;
            editTranslatorBtn.IsEnabled = enable;
            editDescriptionBtn.IsEnabled = enable;
            imageEnable = enable;
            editPublisherBtn.IsEnabled = enable;
            editLanguageBtn.IsEnabled = enable;
            editCountryBtn.IsEnabled = enable;
            editPublishDateBtn.IsEnabled = enable;
            editPriceBtn.IsEnabled = enable;
            editCategorysBtn.IsEnabled = enable;
            editTagsBtn.IsEnabled = enable;
            editISBNBtn.IsEnabled = enable;
        }

        private void clearAll()
        {
            editISBN.Text = "";
            editTitle.Text = "";
            editSubtitle.Text = "";
            editAuthor.Text = "";
            editTranslator.Text = "";
            editDescription.Text = "";
            editPublisher.Text = "";
            editPublishDate.Text = "";
            editLanguage.Text = "";
            editCountry.Text = "";
            editPrice.Text = "";
            editCategorys.Text = "";
            editTags.Text = "";
            itemTotalNums.Text = "";

            coverImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/defultNoImage.png"));
            currentImageBase64Str = "";

            currSelectBookId = -1;
        }

        private async void coverImage_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (!imageEnable) return;

            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".bmp");
            picker.FileTypeFilter.Add(".png");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                currentImageBase64Str = await CommonFunction.ResizeImageAndChangeToBase64(file, ((int)imageBorder.Width) - 2, ((int)imageBorder.Height) - 2);
                if ("" == currentImageBase64Str)
                {
                    byte[] bytes = (await FileIO.ReadBufferAsync(file)).ToArray();
                    currentImageBase64Str = Convert.ToBase64String(bytes);
                }

                reLoadImage();

                setTheEditStatus(true, AppGloableData.CollectionParameter.BOOK_IMAGE);
            }
        }

        private async void combobox_book_collections_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox == null) return;

            var fileName = books.getBookFullName(comboBox.SelectedIndex);
            if ("" == fileName) return;

            var booksFolder = await ApplicationData.Current.RoamingFolder.GetFolderAsync(AppGloableData.appBooksFolderName);
            var bookFile = await booksFolder.GetFileAsync(fileName);
            string bookInfoStr = await Windows.Storage.FileIO.ReadTextAsync(bookFile);
            if (bookCollection != null && bookCollection.BookList != null) bookCollection.BookList.Clear();
            bookCollection = JsonConvert.DeserializeObject<BookCollectionClass>(bookInfoStr);

            refreshListView(false);
            resetListTotalNum();
        }

        //刷新ListView
        private void refreshListView(bool bNoClear)
        {
            listview_book_list.Items.Clear();
            foreach (var info in bookCollection.BookList)
            {
                listview_book_list.Items.Add( ((currSelectBookId != -1 && currSelectBookId == info.Id) ? " * " : "   ")
                    + info.Id.ToString().PadLeft(5, '0') 
                    + " - " 
                    + info.Title);
            }

            if (!bNoClear)
            {
                clearAll();
            }
        }

        private void listview_book_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListView;
            if (listBox == null) return;

            if (listBox.SelectedItem == null) return;

            var selectedItemStr = listBox.SelectedItem.ToString();
            if (selectedItemStr == "") return;

            setAllInputEnable(true);

            string[] itemInfo = selectedItemStr.Split('-');
            int itemId = int.Parse(itemInfo[0]);

            foreach(var bookInfo in bookCollection.BookList)
            {
                if (bookInfo != null && bookInfo.Id == itemId)
                {
                    editISBN.Text = bookInfo.ISBN;
                    editTitle.Text = bookInfo.Title;
                    editSubtitle.Text = bookInfo.Subtitle;
                    editAuthor.Text = bookInfo.Author;
                    editTranslator.Text = bookInfo.Translator;
                    editDescription.Text = bookInfo.Description;
                    editPublisher.Text = bookInfo.Publisher;
                    editPublishDate.Text = bookInfo.PublishDate;
                    editLanguage.Text = bookInfo.Language;
                    editCountry.Text = bookInfo.Country;
                    editPrice.Text = bookInfo.Price.ToString();
                    editCategorys.Text = bookInfo.Categorys;
                    string tagsStr = "";
                    foreach(var tag in bookInfo.Tags)
                    {
                        tagsStr += tag + ' ';
                    }
                    editTags.Text = tagsStr.Substring(0, tagsStr.Length - 1);
                    currentImageBase64Str = bookInfo.CoverImageBase64;

                    reLoadImage();

                    currSelectBookId = bookInfo.Id;

                    break;
                }
            }
        }

        private async void reLoadImage()
        {
            if ("" == currentImageBase64Str)
            {
                coverImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/defultNoImage.png"));
            }
            else
            {
                var bytes = Convert.FromBase64String(currentImageBase64Str);
                var stream = new MemoryStream(bytes);
                var bitmap = new BitmapImage();
                await bitmap.SetSourceAsync(stream.AsRandomAccessStream());

                coverImage.Source = bitmap;
            }
        }

        private void resetListTotalNum()
        {
            itemTotalNums.Text = bookCollection.BookList.Count.ToString();
        }

        private async void editTitleBtn_Click(object sender, RoutedEventArgs e)
        {
            SingleTextEditDialog dialog = new SingleTextEditDialog(AppGloableData.CollectionParameter.BOOK_TITLE, editTitle.Text);
            ContentDialogResult result = await dialog.ShowAsync();
            if(ContentDialogResult.Primary == result)
            {
                editTitle.Text = dialog.Content.ToString();
                setTheEditStatus(true, AppGloableData.CollectionParameter.BOOK_TITLE);
            }
        }

        private async void editSubtitleBtn_Click(object sender, RoutedEventArgs e)
        {
            SingleTextEditDialog dialog = new SingleTextEditDialog(AppGloableData.CollectionParameter.BOOK_SUBTITLE, editSubtitle.Text);
            ContentDialogResult result = await dialog.ShowAsync();
            if (ContentDialogResult.Primary == result)
            {
                editSubtitle.Text = dialog.Content.ToString();
                setTheEditStatus(true, AppGloableData.CollectionParameter.BOOK_SUBTITLE);
            }
        }

        private async void editISBNBtn_Click(object sender, RoutedEventArgs e)
        {
            SingleTextEditDialog dialog = new SingleTextEditDialog(AppGloableData.CollectionParameter.BOOK_ISBN, editISBN.Text);
            ContentDialogResult result = await dialog.ShowAsync();
            if (ContentDialogResult.Primary == result)
            {
                editISBN.Text = dialog.Content.ToString();
                setTheEditStatus(true, AppGloableData.CollectionParameter.BOOK_ISBN);
            }
        }

        private async void editPriceBtn_Click(object sender, RoutedEventArgs e)
        {
            SingleTextEditDialog dialog = new SingleTextEditDialog(AppGloableData.CollectionParameter.BOOK_PRICE, editPrice.Text);
            ContentDialogResult result = await dialog.ShowAsync();
            if (ContentDialogResult.Primary == result)
            {
                editPrice.Text = dialog.Content.ToString();
                setTheEditStatus(true, AppGloableData.CollectionParameter.BOOK_PRICE);
            }
        }

        private void setTheEditStatus(bool flg, AppGloableData.CollectionParameter type)
        {
            setBorderBGRed(flg, type);
            if (flg)
            {
                isEditMode = true;
                setSaveBtnRed(true);

                refreshListView(true);
            }
        }

        private void setBorderBGRed(bool flg, AppGloableData.CollectionParameter type)
        {
            switch (type)
            {
                case AppGloableData.CollectionParameter.BOOK_ISBN: editISBNBorder.Background = flg ? new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)) : borderBG; break;
                case AppGloableData.CollectionParameter.BOOK_TITLE: editTitleBorder.Background = flg ? new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)) : borderBG; break;
                case AppGloableData.CollectionParameter.BOOK_SUBTITLE: editSubtitleBorder.Background = flg ? new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)) : borderBG; break;
                case AppGloableData.CollectionParameter.BOOK_AUTHOR: editAuthorBorder.Background = flg ? new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)) : borderBG; break;
                case AppGloableData.CollectionParameter.BOOK_TRANSLATOR: editTranslatorBorder.Background = flg ? new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)) : borderBG; break;
                case AppGloableData.CollectionParameter.BOOK_DESCRIPTION: editDescriptionBorder.Background = flg ? new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)) : borderBG; break;
                case AppGloableData.CollectionParameter.BOOK_PUBLISHERS: editPublisherBorder.Background = flg ? new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)) : borderBG; break;
                case AppGloableData.CollectionParameter.BOOK_PUBLISHDATE: editPublishDateBorder.Background = flg ? new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)) : borderBG; break;
                case AppGloableData.CollectionParameter.BOOK_LANGUAGES: editLanguageBorder.Background = flg ? new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)) : borderBG; break;
                case AppGloableData.CollectionParameter.BOOK_COUNTRY: editCountryBorder.Background = flg ? new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)) : borderBG; break;
                case AppGloableData.CollectionParameter.BOOK_PRICE: editPriceBorder.Background = flg ? new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)) : borderBG; break;
                case AppGloableData.CollectionParameter.BOOK_TAGS: editTagsBorder.Background = flg ? new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)) : borderBG; break;
                case AppGloableData.CollectionParameter.BOOK_CATEGORYS: editCategorysBorder.Background = flg ? new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)) : borderBG; break;
                case AppGloableData.CollectionParameter.BOOK_IMAGE: break;
            }
        }

        private async void editPublisherBtn_Click(object sender, RoutedEventArgs e)
        {
            SingleTextEditDialog dialog = new SingleTextEditDialog(AppGloableData.CollectionParameter.BOOK_PUBLISHERS, editPublisher.Text);
            ContentDialogResult result = await dialog.ShowAsync();
            if (ContentDialogResult.Primary == result)
            {
                editPublisher.Text = dialog.Content.ToString();
                setTheEditStatus(true, AppGloableData.CollectionParameter.BOOK_PUBLISHERS);
            }
        }

        private async void editAuthorBtn_Click(object sender, RoutedEventArgs e)
        {
            MultiTextEditDialog dialog = new MultiTextEditDialog(AppGloableData.CollectionParameter.BOOK_AUTHOR, editAuthor.Text);
            ContentDialogResult result = await dialog.ShowAsync();
            if (ContentDialogResult.Primary == result)
            {
                editAuthor.Text = dialog.Content.ToString();
                setTheEditStatus(true, AppGloableData.CollectionParameter.BOOK_AUTHOR);
            }
        }

        private async void editTranslatorBtn_Click(object sender, RoutedEventArgs e)
        {
            MultiTextEditDialog dialog = new MultiTextEditDialog(AppGloableData.CollectionParameter.BOOK_TRANSLATOR, editTranslator.Text);
            ContentDialogResult result = await dialog.ShowAsync();
            if (ContentDialogResult.Primary == result)
            {
                editTranslator.Text = dialog.Content.ToString();
                setTheEditStatus(true, AppGloableData.CollectionParameter.BOOK_TRANSLATOR);
            }
        }

        private async void editCategorysBtn_Click(object sender, RoutedEventArgs e)
        {
            MultiTextEditDialog dialog = new MultiTextEditDialog(AppGloableData.CollectionParameter.BOOK_CATEGORYS, editCategorys.Text);
            ContentDialogResult result = await dialog.ShowAsync();
            if (ContentDialogResult.Primary == result)
            {
                editCategorys.Text = dialog.Content.ToString();
                setTheEditStatus(true, AppGloableData.CollectionParameter.BOOK_CATEGORYS);
            }
        }

        private async void editDescriptionBtn_Click(object sender, RoutedEventArgs e)
        {
            MultiTextEditDialog dialog = new MultiTextEditDialog(AppGloableData.CollectionParameter.BOOK_DESCRIPTION, editDescription.Text);
            ContentDialogResult result = await dialog.ShowAsync();
            if (ContentDialogResult.Primary == result)
            {
                editDescription.Text = dialog.Content.ToString();
                setTheEditStatus(true, AppGloableData.CollectionParameter.BOOK_DESCRIPTION);
            }
        }

        private async void editLanguageBtn_Click(object sender, RoutedEventArgs e)
        {
            DropdownBoxEditDialog dialog = new DropdownBoxEditDialog(
                AppGloableData.CollectionParameter.BOOK_LANGUAGES,
                AppGloableData.getLanguageEnumIndexByDrawName(editLanguage.Text));

            ContentDialogResult result = await dialog.ShowAsync();
            if (ContentDialogResult.Primary == result)
            {
                editLanguage.Text = dialog.Content.ToString();
                setTheEditStatus(true, AppGloableData.CollectionParameter.BOOK_LANGUAGES);
            }
        }

        private async void editCountryBtn_Click(object sender, RoutedEventArgs e)
        {
            DropdownBoxEditDialog dialog = new DropdownBoxEditDialog(
                AppGloableData.CollectionParameter.BOOK_COUNTRY,
                AppGloableData.getCountryEnumIndexByDrawName(editCountry.Text));

            ContentDialogResult result = await dialog.ShowAsync();
            if(ContentDialogResult.Primary == result)
            {
                editCountry.Text = dialog.Content.ToString();
                setTheEditStatus(true, AppGloableData.CollectionParameter.BOOK_COUNTRY);
            }
        }

        private async void editPublishDateBtn_Click(object sender, RoutedEventArgs e)
        {
            DateBoxEditDialog dialog = new DateBoxEditDialog(
                AppGloableData.CollectionParameter.BOOK_PUBLISHDATE, 
                editPublishDate.Text);

            ContentDialogResult result = await dialog.ShowAsync();
            if (ContentDialogResult.Primary == result)
            { 
                editPublishDate.Text = dialog.Content.ToString();
                setTheEditStatus(true, AppGloableData.CollectionParameter.BOOK_PUBLISHDATE);
            }
        }

        private async void editTagsBtn_Click(object sender, RoutedEventArgs e)
        {
            CustomTagEditDialog dialog = new CustomTagEditDialog(
                AppGloableData.CollectionParameter.BOOK_TAGS, 
                editTags.Text);

            ContentDialogResult result = await dialog.ShowAsync();
            if (ContentDialogResult.Primary == result) 
            {
                editTags.Text = dialog.Content.ToString();
                setTheEditStatus(true, AppGloableData.CollectionParameter.BOOK_TAGS);
            }
        }

        private async void save_book_info_Click(object sender, RoutedEventArgs e)
        {
            if (isEditMode)
            {
                string titleText = editTitle.Text;
                string priceStr = editPrice.Text;
                float price = 0.0F;
                if ("" == titleText) 
                {
                    ContentDialog saveDialog = new ContentDialog
                    {
                        Title = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_Save_Dialog_Title_Error"),
                        Content = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_Save_Dialog_Content_Error"),
                        PrimaryButtonText = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_Save_Dialog_Error_Btn")
                    };

                    //修改按钮样式
                    var bst = new Windows.UI.Xaml.Style(typeof(Button));
                    bst.Setters.Add(new Setter(Button.BackgroundProperty, Windows.UI.Colors.Red));
                    saveDialog.PrimaryButtonStyle = bst;

                    await saveDialog.ShowAsync();
                }
                else if (priceStr != "" && !float.TryParse(editPrice.Text, out price))
                {
                    ContentDialog saveDialog = new ContentDialog
                    {
                        Title = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_Save_Dialog_Price_Error"),
                        Content = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_Save_Dialog_Content_Price_Error"),
                        PrimaryButtonText = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_Save_Dialog_Error_Btn")
                    };

                    //修改按钮样式
                    var bst = new Windows.UI.Xaml.Style(typeof(Button));
                    bst.Setters.Add(new Setter(Button.BackgroundProperty, Windows.UI.Colors.Red));
                    saveDialog.PrimaryButtonStyle = bst;

                    await saveDialog.ShowAsync();
                }
                else
                {
                    await SaveWithNoDialog(price);
                }
                    
            }
            else 
            {
                //没有任何修改不处理
                ContentDialog dialog = new ContentDialog
                {
                    Title = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("ViewBook_No_Book_Update_Title"),
                    Content = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("ViewBook_No_Book_Update_Content"),
                    PrimaryButtonText = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("ISBNSearchRes_Btn")
                };

                //修改按钮样式
                var bst = new Windows.UI.Xaml.Style(typeof(Button));
                bst.Setters.Add(new Setter(Button.BackgroundProperty, Windows.UI.Colors.Red));
                bst.Setters.Add(new Setter(Button.ForegroundProperty, Windows.UI.Colors.White));
                dialog.PrimaryButtonStyle = bst;

                await dialog.ShowAsync();
            }
        }

        private void setSaveBtnRed(bool flg)
        {
            if (flg)
            {
                save_book_info.Background = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                save_book_info.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            }
            else
            {
                save_book_info.Background = saveBtnBG;
                save_book_info.Foreground = saveBtnFontColor;
            }
        }

        private void saveToCollection(float price) 
        { 
            foreach(var bookInfo in bookCollection.BookList) 
            {
                if(bookInfo.Id == currSelectBookId) 
                {
                    bookInfo.Title = editTitle.Text;
                    bookInfo.Subtitle = editSubtitle.Text;
                    bookInfo.Description = editDescription.Text;
                    bookInfo.ISBN = editISBN.Text;
                    bookInfo.CoverImageBase64 = currentImageBase64Str;
                    bookInfo.Language = editLanguage.Text;
                    bookInfo.Publisher = editPublisher.Text;
                    bookInfo.PublishDate = editPublishDate.Text;
                    bookInfo.Author = editAuthor.Text;
                    bookInfo.Translator = editTranslator.Text;
                    bookInfo.Country = editCountry.Text;
                    bookInfo.Categorys = editCategorys.Text;
                    bookInfo.Price = price;

                    var tags = editTags.Text.Split(' ');
                    bookInfo.Tags.Clear();
                    foreach (var tag in tags)
                    {
                        bookInfo.Tags.Add(tag);
                    }
                    break;
                }
            }
        }

        private void resetEditStatus() 
        {
            isEditMode = false;

            setTheEditStatus(false, AppGloableData.CollectionParameter.BOOK_ISBN);
            setTheEditStatus(false, AppGloableData.CollectionParameter.BOOK_TITLE);
            setTheEditStatus(false, AppGloableData.CollectionParameter.BOOK_SUBTITLE);
            setTheEditStatus(false, AppGloableData.CollectionParameter.BOOK_IMAGE);
            setTheEditStatus(false, AppGloableData.CollectionParameter.BOOK_AUTHOR);
            setTheEditStatus(false, AppGloableData.CollectionParameter.BOOK_TRANSLATOR);
            setTheEditStatus(false, AppGloableData.CollectionParameter.BOOK_DESCRIPTION);
            setTheEditStatus(false, AppGloableData.CollectionParameter.BOOK_PUBLISHERS);
            setTheEditStatus(false, AppGloableData.CollectionParameter.BOOK_PUBLISHDATE);
            setTheEditStatus(false, AppGloableData.CollectionParameter.BOOK_LANGUAGES);
            setTheEditStatus(false, AppGloableData.CollectionParameter.BOOK_COUNTRY);
            setTheEditStatus(false, AppGloableData.CollectionParameter.BOOK_PRICE);
            setTheEditStatus(false, AppGloableData.CollectionParameter.BOOK_TAGS);
            setTheEditStatus(false, AppGloableData.CollectionParameter.BOOK_CATEGORYS);
        }

        private async Task<bool> SaveWithNoDialog(float price) 
        {
            saveToCollection(price);

            string json = JsonConvert.SerializeObject(bookCollection);
            var booksFolder = await ApplicationData.Current.RoamingFolder.GetFolderAsync(AppGloableData.appBooksFolderName);
            var filr = await booksFolder.GetFileAsync(books.getBookFullName(combobox_book_collections.SelectedIndex));
            await FileIO.WriteTextAsync(filr, json, UnicodeEncoding.Utf8);

            //change save btn color
            setSaveBtnRed(false);

            resetEditStatus();

            //init curr select item index
            int tmpId = currSelectBookId;
            currSelectBookId = -1;
            refreshListView(true);
            currSelectBookId = tmpId;

            return true;
        }

        /*
        private async Task<bool> Save(string dialogTitle, string dialogContent, string btnYes, string btnNO, float price)
        {

            ContentDialog saveDialog = new ContentDialog
            {
                Title = dialogTitle,
                Content = dialogContent,
                PrimaryButtonText = btnYes,
                CloseButtonText = btnNO
            };

            //修改按钮样式
            var bst = new Windows.UI.Xaml.Style(typeof(Button));
            bst.Setters.Add(new Setter(Button.BackgroundProperty, Windows.UI.Colors.DodgerBlue));
            bst.Setters.Add(new Setter(Button.ForegroundProperty, Windows.UI.Colors.White));
            saveDialog.PrimaryButtonStyle = bst;

            ContentDialogResult result = await saveDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                saveToCollection(price);

                string json = JsonConvert.SerializeObject(bookCollection);
                var booksFolder = await ApplicationData.Current.RoamingFolder.GetFolderAsync(AppGloableData.appBooksFolderName);
                var filr = await booksFolder.GetFileAsync(books.getBookFullName(combobox_book_collections.SelectedIndex));
                await FileIO.WriteTextAsync(filr, json, UnicodeEncoding.Utf8);

                //change save btn color
                setSaveBtnRed(false);
                //init curr select item index
                currSelectBookId = -1;

                refreshListView(false);

                return true;
            }
            return false;
        }
        */
    }
}
