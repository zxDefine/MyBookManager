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

        public ViewBookCollectionPage()
        {
            this.InitializeComponent();

            borderBG = editISBNBorder.Background;

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
        }

        private void coverImage_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (!imageEnable) return;

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

            refreshListView();
            resetListTotalNum();
        }

        //刷新ListView
        private void refreshListView()
        {
            listview_book_list.Items.Clear();
            foreach (var info in bookCollection.BookList)
            {
                listview_book_list.Items.Add(info.Id.ToString().PadLeft(5, '0') + " - " + info.Title);
            }

            clearAll();
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
    }
}
