using Newtonsoft.Json;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace MyBookManager
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class FindBookInfoPage : Page
    {
        private BookPageNameList books = new BookPageNameList();
        private List<BookCollectionClass> bookCollectionList = new List<BookCollectionClass>();

        public FindBookInfoPage()
        {
            this.InitializeComponent();

            initBaseBookInfo();
            initFindCondition();
        }

        private void btn_Back_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }
        }

        private async void initBaseBookInfo() 
        {
            //init books
            var booksFolder = await ApplicationData.Current.RoamingFolder.GetFolderAsync(AppGloableData.appBooksFolderName);
            IReadOnlyList<StorageFile> fileList = await booksFolder.GetFilesAsync();
            var hasFile = fileList.Count > 0;
            if (hasFile)
            {
                foreach (var file in fileList)
                {
                    //name: book_id_name.json
                    string[] nameList = file.Name.Split('.', '_');
                    books.add(int.Parse(nameList[1]), nameList[2]);
                }
            }
        }

        private async void initFindCondition() 
        {
            //get book folder all file list
            var booksFolder = await ApplicationData.Current.RoamingFolder.GetFolderAsync(AppGloableData.appBooksFolderName);
            IReadOnlyList<StorageFile> fileList = await booksFolder.GetFilesAsync();
            var hasFile = fileList.Count > 0;

            if(hasFile)
            {
                foreach (var file in fileList)
                {
                    //name: book_id_name.json
                    string[] nameList = file.Name.Split('.', '_');
                    books.add(int.Parse(nameList[1]), nameList[2]);
                }
                string[] sou = new string[books.getNameList().Length + 1];
                sou[0] = "ALL";
                var list = books.getNameList();
                for (int i = 0; i < list.Length; ++i) 
                {
                    sou[i + 1] = list[i];
                }
                combobox_book_collection_target.ItemsSource = sou;
                combobox_book_collection_target.SelectedIndex = 0;
            }
        }

        private void btn_find_Click(object sender, RoutedEventArgs e)
        {
            list_find_result.ItemsSource = null;

            List<FindBookDrawClass> resList = new List<FindBookDrawClass>();

            //get key word
            List<string> keyWordList = new List<string>();
            foreach (string keyWord in textbox_input_keyword.Text.ToString().Split(' ', '\r'))
            {
                if(keyWord != "")
                {
                    keyWordList.Add(keyWord);
                }
            }
            int size = keyWordList.Count;

            foreach (var bookCollection in bookCollectionList)
            {
                foreach(var bookInfo in bookCollection.BookList)
                {
                    bool bCanAdd = false;
                    if (size != 0) 
                    {
                        foreach(string keyWord in keyWordList)
                        {
                            if(true == cb_from_title.IsChecked && bookInfo.Title.IndexOf(keyWord, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                bCanAdd = true;
                                continue;
                            }

                            if (true == cb_from_subtitle.IsChecked && bookInfo.Subtitle.IndexOf(keyWord, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                bCanAdd = true;
                                continue;
                            }

                            if (true == cb_from_description.IsChecked && bookInfo.Description.IndexOf(keyWord, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                bCanAdd = true;
                                continue;
                            }

                            if (true == cb_from_publisher.IsChecked && bookInfo.Publisher.IndexOf(keyWord, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                bCanAdd = true;
                                continue;
                            }

                            if (true == cb_from_author.IsChecked && bookInfo.Author.IndexOf(keyWord, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                bCanAdd = true;
                                continue;
                            }

                            if (true == cb_from_translator.IsChecked && bookInfo.Translator.IndexOf(keyWord, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                bCanAdd = true;
                                continue;
                            }

                            if (true == cb_from_categorys.IsChecked && bookInfo.Categorys.IndexOf(keyWord, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                bCanAdd = true;
                                continue;
                            }

                            if (true == cb_from_tags.IsChecked && bookInfo.getTagsString().IndexOf(keyWord, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                bCanAdd = true;
                                continue;
                            }

                            if (true == cb_from_language.IsChecked && bookInfo.Language.IndexOf(keyWord, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                bCanAdd = true;
                                continue;
                            }

                            if (true == cb_from_country.IsChecked && bookInfo.Country.IndexOf(keyWord, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                bCanAdd = true;
                                continue;
                            }

                            if (true == cb_from_publishDate.IsChecked && bookInfo.PublishDate.IndexOf(keyWord, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                bCanAdd = true;
                                continue;
                            }
                        }
                    }
                    else
                    {
                        bCanAdd = true;
                    }

                    if (bCanAdd)
                    {
                        FindBookDrawClass bookDrawInfo = new FindBookDrawClass();
                        bookDrawInfo.BookID = bookInfo.Id.ToString().PadLeft(5, '0');
                        bookDrawInfo.BookTitle = bookInfo.Title == "" ? "-" : bookInfo.Title;
                        bookDrawInfo.BookSubtitle = bookInfo.Subtitle == "" ? "-" : bookInfo.Subtitle;
                        bookDrawInfo.BookISBN = bookInfo.ISBN == "" ? "-" : bookInfo.ISBN;
                        bookDrawInfo.BookAuthor = bookInfo.Author == "" ? "-" : bookInfo.Author;
                        bookDrawInfo.BookTranslator = bookInfo.Translator == "" ? "-" : bookInfo.Translator;
                        bookDrawInfo.BookLanguage = bookInfo.Language == "" ? "-" : bookInfo.Language;
                        bookDrawInfo.BookCountry = bookInfo.Country == "" ? "-" : bookInfo.Country;
                        bookDrawInfo.BookPublisher = bookInfo.Publisher == "" ? "-" : bookInfo.Publisher;
                        bookDrawInfo.BookPublishDate = bookInfo.PublishDate == "" ? "-" : bookInfo.PublishDate;
                        bookDrawInfo.BookPrice = bookInfo.Price.ToString();
                        bookDrawInfo.BookCategory = bookInfo.Categorys == "" ? "-" : bookInfo.Categorys;
                        bookDrawInfo.BookTag = bookInfo.getTagsString();
                        bookDrawInfo.BookDescription = bookInfo.Description == "" ? "-" : bookInfo.Description;
                        bookDrawInfo.BookFromCollection = bookCollection.Name;

                        var bytes = Convert.FromBase64String(bookInfo.CoverImageBase64);
                        var stream = new MemoryStream(bytes);
                        var bitmap = new BitmapImage();
                        var source = stream.AsRandomAccessStream();
                        bitmap.SetSourceAsync(source);
                        bookDrawInfo.BookImage = bitmap;

                        resList.Add(bookDrawInfo);
                    }
                }
            }

            text_find_res_num.Text = resList.Count.ToString();

            list_find_result.ItemsSource = resList;
        }

        private async void combobox_book_collection_target_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combobox = sender as ComboBox;
            int selsectIndex = combobox.SelectedIndex;
            List<int> bookIndexList = new List<int>();
            if(0 == selsectIndex) 
            {
                for(int i = 0; i < books.getNameList().Length; ++i) 
                {
                    bookIndexList.Add(i);
                }
            }
            else
            {
                bookIndexList.Add(combobox.SelectedIndex - 1);
            }

            bookCollectionList.Clear();
            foreach (var index in bookIndexList) 
            {
                var fileName = books.getBookFullName(index);
                var booksFolder = await ApplicationData.Current.RoamingFolder.GetFolderAsync(AppGloableData.appBooksFolderName);
                var bookFile = await booksFolder.GetFileAsync(fileName);
                string bookInfoStr = await Windows.Storage.FileIO.ReadTextAsync(bookFile);
                BookCollectionClass targetBookCollection = new BookCollectionClass();
                targetBookCollection = JsonConvert.DeserializeObject<BookCollectionClass>(bookInfoStr);
                bookCollectionList.Add(targetBookCollection);
            }
        }
    }
}
