using MyBookManager;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Web.Http;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace MyBookManager
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AddNewBookPage : Page
    {
        private bool imageEnable = false;
        private string currentImageBase64Str = "";

        private BookPageNameList books = new BookPageNameList();
        private BookCollectionClass bookCollection = null;
        private int booksCollectionListPreCount = 0;
        private bool forceNoSave_ChangeBookCollection = false;
        private bool forceNoSave_CreateNewBookCollection = false;

        private ArrayList addNewBookIdList = new ArrayList();

        private Brush saveBtnBG;
        private Brush saveBtnFontColor;

        public AddNewBookPage()
        {
            this.InitializeComponent();

            saveBtnBG = save_book_info.Background;
            saveBtnFontColor = save_book_info.Foreground;

            updateCollectionList();

            initLanguageComboBox();
            initCountryComboBox();
        }

        private async void updateCollectionList(bool useNewCollection = false)
        {
            //get book folder all file list
            var booksFolder = await ApplicationData.Current.RoamingFolder.GetFolderAsync(AppGloableData.appBooksFolderName);
            IReadOnlyList<StorageFile> fileList = await booksFolder.GetFilesAsync();
            var hasFile = fileList.Count > 0;
            if (!hasFile)
            {
                setAllInputEnable(false);
            }
            else
            {
                setAllInputEnable(true);
                foreach (var file in fileList)
                {
                    //name: book_id_name.json
                    string[] nameList = file.Name.Split('.', '_');
                    books.add(int.Parse(nameList[1]), nameList[2]);
                }
                var sou = books.getNameList();
                var size = sou.Length;
                combobox_book_collections.ItemsSource = sou;
                if (useNewCollection)
                {
                    combobox_book_collections.SelectedIndex = size - 1;
                }
                else
                {
                    combobox_book_collections.SelectedIndex = 0;
                }
            }
        }

        private void initLanguageComboBox()
        {
            inputLanguage.ItemsSource = AppGloableData.getLanguageDrawNameList();
        }

        private void initCountryComboBox()
        {
            inputCountry.ItemsSource = AppGloableData.getCountryDrawNameList();
        }

        private async void btn_Back_Click(object sender, RoutedEventArgs e)
        {
            bool canBack = true;
            if (isAddNewInfo())
            {
                string title = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_Check_Save_Title");
                string message = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_Check_Save_Content");
                string newMessage = message.Replace("XXXXXX", combobox_book_collections.SelectedItem.ToString());
                string btnYes = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_Check_Save_Btn_Yes");
                string btnNo = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_Check_Save_Btn_No");

                await Save(title, newMessage, btnYes, btnNo);
                
                canBack = true;
            }

            if (canBack)
            {
                Frame rootFrame = Window.Current.Content as Frame;
                if (rootFrame.CanGoBack)
                {
                    rootFrame.GoBack();
                }
            }
        }

        private async void btn_add_collection_Click(object sender, RoutedEventArgs e)
        {
            bool canCreateNew = true;
            if (isAddNewInfo())
            {
                if (!forceNoSave_CreateNewBookCollection && await showNoSaveAlarm())
                {
                    forceNoSave_CreateNewBookCollection = true;
                    canCreateNew = true;
                }
                else if (forceNoSave_CreateNewBookCollection)
                {
                    canCreateNew = true;
                }
                else
                {
                    canCreateNew = false;
                }
            }
            else
            {
                canCreateNew = true;
            }

            if (canCreateNew)
            {
                String labelText = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateNewCollectionLabelTxt_Book");
                CreateNewCollectionContentDialog createNewCollectionDialog = new CreateNewCollectionContentDialog(labelText, AppGloableData.Types.BOOK);
                ContentDialogResult res = await createNewCollectionDialog.ShowAsync();
                if (ContentDialogResult.Primary == res)
                {
                    //update collection list
                    updateCollectionList(true);
                    //change save btn color
                    setSaveBtnRed(false);
                }
            }
        }

        private void setAllInputEnable(bool enable = true)
        {
            inputISBN.IsEnabled = enable;
            findBookInfo.IsEnabled = enable;
            inputTitle.IsEnabled = enable;
            inputSubtitle.IsEnabled = enable;
            inputAuthor.IsEnabled = enable;
            imageEnable = enable;
            inputDescription.IsEnabled = enable;
            inputTranslator.IsEnabled = enable;
            inputPublisher.IsEnabled = enable;
            inputPublishDate.IsEnabled = enable;
            inputLanguage.IsEnabled = enable;
            inputCountry.IsEnabled = enable;
            inputPrice.IsEnabled = enable;
            inputCategorys.IsEnabled = enable;
            inputTags.IsEnabled = enable;
            addTag.IsEnabled = enable;
            removeLastTag.IsEnabled = enable;
            add_book_info.IsEnabled = enable;
            save_book_info.IsEnabled = enable;
        }

        private void clearAll()
        {
            inputISBN.Text = "";
            inputTitle.Text = "";
            inputSubtitle.Text = "";
            inputAuthor.Text = "";
            inputTranslator.Text = "";
            inputDescription.Text = "";
            inputPublisher.Text = "";
            inputPublishDate.SelectedDate = null;
            inputLanguage.SelectedIndex = -1;
            inputCountry.SelectedIndex = -1;
            inputPrice.Text = "";
            inputCategorys.Text = "";
            inputTags.Text = "";
            inputedTags.Text = "";

            coverImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/defultNoImage.png"));
            currentImageBase64Str = "";
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
            if(file != null)
            {
                currentImageBase64Str = await CommonFunction.ResizeImageAndChangeToBase64(file, ((int)imageBorder.Width) - 2, ((int)imageBorder.Height) - 2);
                if("" == currentImageBase64Str)
                {
                    byte[] bytes = (await FileIO.ReadBufferAsync(file)).ToArray();
                    currentImageBase64Str = Convert.ToBase64String(bytes);
                }

                reLoadImage();
            }
        }

        private async void findBookInfo_Click(object sender, RoutedEventArgs e)
        {
            if (inputISBN.Text == "") return;

            LoadingDialog loadingDialog = new LoadingDialog();
            var progressTask = loadingDialog.ShowAsync();

            var client = new HttpClient();
            Uri requestUri = new Uri("https://www.googleapis.com/books/v1/volumes?q=isbn:" + inputISBN.Text);
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            string httpResponseBody = "";
            try
            {
                httpResponse = await client.GetAsync(requestUri);
                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();

                JsonSerializer json = JsonSerializer.Create();
                GoogleBooksAPI.GoogleBooksAPIInfo bookInfo = json.Deserialize<GoogleBooksAPI.GoogleBooksAPIInfo>(new JsonTextReader(new StringReader(httpResponseBody)));

                if(bookInfo.totalItems > 0) 
                { 
                    Uri requestUriSelfLink = new Uri(bookInfo.items[0].selfLink);
                    try
                    {
                        httpResponse = await client.GetAsync(requestUriSelfLink);
                        httpResponse.EnsureSuccessStatusCode();
                        httpResponseBody = await httpResponse.Content.ReadAsStringAsync();

                        //close loading dialog
                        progressTask.Cancel();
                        loadingDialog.Hide();

                        GoogleBooksSelfLink.GoogleBooksSelfLinkInfo bookSelfLinkInfo = json.Deserialize<GoogleBooksSelfLink.GoogleBooksSelfLinkInfo>(new JsonTextReader(new StringReader(httpResponseBody)));

                        String message = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("ISBNSearchRes_Yes");
                        var dialog = new ContentDialog
                        {
                            Title = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("ISBNSearchRes_Yes_Title"),
                            Content = message,
                            PrimaryButtonText = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("ISBNSearchRes_Btn")
                        };
                        var bst = new Windows.UI.Xaml.Style(typeof(Button));
                        bst.Setters.Add(new Setter(Button.BackgroundProperty, Windows.UI.Colors.DodgerBlue));
                        dialog.PrimaryButtonStyle = bst;
                        var result = await dialog.ShowAsync();
                        if (result == ContentDialogResult.Primary)
                        {
                            //将搜索到的填到各个输入中
                            inputTitle.Text = bookSelfLinkInfo.volumeInfo.title != null ? bookSelfLinkInfo.volumeInfo.title : "";
                            inputSubtitle.Text = bookSelfLinkInfo.volumeInfo.subtitle != null ? bookSelfLinkInfo.volumeInfo.subtitle : "";

                            String anthorStr = "";
                            if(bookSelfLinkInfo.volumeInfo.authors != null)
                            {
                                foreach (string str in bookSelfLinkInfo.volumeInfo.authors)
                                {
                                    anthorStr += str + '\n';
                                }
                            }
                            inputAuthor.Text = anthorStr;

                            inputDescription.Text = bookSelfLinkInfo.volumeInfo.description != null ? bookSelfLinkInfo.volumeInfo.description : "";
                            inputPublisher.Text = bookSelfLinkInfo.volumeInfo.publisher != null ? bookSelfLinkInfo.volumeInfo.publisher : "";

                            if(bookSelfLinkInfo.volumeInfo.publishedDate != null)
                            {
                                string dateStr = bookSelfLinkInfo.volumeInfo.publishedDate;
                                string[] dateList = dateStr.Split('-');
                                inputPublishDate.SelectedDate = new DateTime(int.Parse(dateList[0]), 01, 01);
                            }

                            string category = "";
                            if(bookInfo.items[0].volumeInfo.categories != null)
                            {
                                foreach (var categoryStr in bookInfo.items[0].volumeInfo.categories)
                                {
                                    category += categoryStr + "-";
                                }
                                inputCategorys.Text = category.Substring(0, category.Length - 1);
                            }

                            if(bookInfo.items[0].volumeInfo.language != null)
                            {
                                setInputLanguageAndCountry(inputISBN.Text);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;

                        //close loading dialog
                        progressTask.Cancel();
                        loadingDialog.Hide();
                    }
                }
                else
                {
                    //close loading dialog
                    progressTask.Cancel();
                    loadingDialog.Hide();

                    String message = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("ISBNSearchRes_No");
                    String newMessage = message.Replace("XXXXXX", inputISBN.Text);
                    var dialog = new ContentDialog
                    {
                        Title = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("ISBNSearchRes_No_Title"),
                        Content = newMessage,
                        PrimaryButtonText = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("ISBNSearchRes_Btn")
                    };
                    var bst = new Windows.UI.Xaml.Style(typeof(Button));
                    bst.Setters.Add(new Setter(Button.BackgroundProperty, Windows.UI.Colors.Red));
                    dialog.PrimaryButtonStyle = bst;
                    await dialog.ShowAsync();
                }
            }
            catch (Exception ex)
            {
                //close loading dialog
                progressTask.Cancel();
                loadingDialog.Hide();

                httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
            }
        }

        private void setInputLanguageAndCountry(string isbnCode)
        {
            if (isbnCode.Substring(0, 4) == "9787") //中国大陆
            {
                inputLanguage.SelectedIndex = (int)AppGloableData.Language.CN;
                inputCountry.SelectedIndex = (int)AppGloableData.Country.CN;
            }
            else if (isbnCode.Substring(0, 6) == "978957" //中国台湾
                    || isbnCode.Substring(0, 6) == "978986" //中国台湾
                    || isbnCode.Substring(0, 6) == "978962" //中国香港
                    || isbnCode.Substring(0, 6) == "978988" //中国香港
                    || isbnCode.Substring(0, 8) == "97899937" //中国澳门
                    || isbnCode.Substring(0, 8) == "97899965") //中国澳门
            {
                inputLanguage.SelectedIndex = (int)AppGloableData.Language.TW;
                inputCountry.SelectedIndex = (int)AppGloableData.Country.CN;
            }
            else if (isbnCode.Substring(0, 4) == "9784") //日本
            {
                inputLanguage.SelectedIndex = (int)AppGloableData.Language.JP;
                inputCountry.SelectedIndex = (int)AppGloableData.Country.JP;
            }
            else if (isbnCode.Substring(0, 5) == "97889"
                     || isbnCode.Substring(0, 5) == "97911") //韩国
            {
                inputLanguage.SelectedIndex = (int)AppGloableData.Language.KR;
                inputCountry.SelectedIndex = (int)AppGloableData.Country.KR;
            }
            else if (isbnCode.Substring(0, 4) == "9780"
                     || isbnCode.Substring(0, 4) == "9781") //英美
            {
                inputLanguage.SelectedIndex = (int)AppGloableData.Language.EN;
                inputCountry.SelectedIndex = (int)AppGloableData.Country.US;
            }
            else if (isbnCode.Substring(0, 4) == "9782"
                     || isbnCode.Substring(0, 5) == "97910") //法国
            {
                inputLanguage.SelectedIndex = (int)AppGloableData.Language.FR;
                inputCountry.SelectedIndex = (int)AppGloableData.Country.FR;
            }
            else if (isbnCode.Substring(0, 5) == "97884") //西班牙
            {
                inputLanguage.SelectedIndex = (int)AppGloableData.Language.ES;
                inputCountry.SelectedIndex = (int)AppGloableData.Country.ES;
            }
            else if (isbnCode.Substring(0, 6) == "978972"
                     || isbnCode.Substring(0, 6) == "978989") //葡萄牙
            {
                inputLanguage.SelectedIndex = (int)AppGloableData.Language.PT;
                inputCountry.SelectedIndex = (int)AppGloableData.Country.PT;
            }
            else if (isbnCode.Substring(0, 4) == "9783") //德国
            {
                inputLanguage.SelectedIndex = (int)AppGloableData.Language.DE;
                inputCountry.SelectedIndex = (int)AppGloableData.Country.DE;
            }
            else if (isbnCode.Substring(0, 5) == "97888"
                     || isbnCode.Substring(0, 5) == "97912") //意大利
            {
                inputLanguage.SelectedIndex = (int)AppGloableData.Language.IT;
                inputCountry.SelectedIndex = (int)AppGloableData.Country.IT;
            }
            else if (isbnCode.Substring(0, 4) == "9785") //俄罗斯
            {
                inputLanguage.SelectedIndex = (int)AppGloableData.Language.RU;
                inputCountry.SelectedIndex = (int)AppGloableData.Country.RU;
            }
            else 
            {
                inputLanguage.SelectedIndex = (int)AppGloableData.Language.OTHER;
                inputCountry.SelectedIndex = (int)AppGloableData.Country.OTHER;
            }
        }

        private void inputISBN_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            if (textBox.Text != "")
            {
                if (!Regex.IsMatch(textBox.Text, "^[0-9]+$"))
                {
                    int pos = textBox.SelectionStart - 1;
                    textBox.Text = textBox.Text.Remove(pos, 1);
                    textBox.SelectionStart = pos;
                }
            }
        }

        private void inputPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            if(textBox.Text != "")
            {
                if(!Regex.IsMatch(textBox.Text, "^[0-9.]+$"))
                {
                    int pos = textBox.SelectionStart - 1;
                    textBox.Text = textBox.Text.Remove(pos, 1);
                    textBox.SelectionStart = pos;
                }
            }
        }

        private void addTag_Click(object sender, RoutedEventArgs e)
        {
            string tagText = inputTags.Text;
            if(0 == tagText.Length) return;

            if (0 != inputedTags.Text.Length) inputedTags.Text += " ";
            inputedTags.Text += "#" + tagText;

            inputTags.Text = "";
        }

        private void removeLastTag_Click(object sender, RoutedEventArgs e)
        {
            string tagText = inputedTags.Text;
            if (0 == tagText.Length) return;

            int lastIndex = tagText.LastIndexOf("#");
            string newTags = tagText.Substring(0, lastIndex);
            inputedTags.Text = newTags.TrimEnd();
        }

        private async void reLoadImage()
        {
            var bytes = Convert.FromBase64String(currentImageBase64Str);
            var stream = new MemoryStream(bytes);
            var bitmap = new BitmapImage();
            await bitmap.SetSourceAsync(stream.AsRandomAccessStream());

            coverImage.Source = bitmap;
        }

        private async void add_book_info_Click(object sender, RoutedEventArgs e)
        {
            string titleText = inputTitle.Text;
            string priceStr = inputPrice.Text;
            float price = 0.0F;
            //没有输入书名
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
            else if (priceStr != "" && !float.TryParse(inputPrice.Text, out price))
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
                bool bFindTheSameBook = false;
                foreach(var info in bookCollection.BookList)
                {
                    if (inputTitle.Text.Equals(info.Title))
                    {
                        bFindTheSameBook = true;
                        break;
                    }
                }

                bool bCanAdd = true;
                if (bFindTheSameBook)
                {
                    ContentDialog theSameBookDialog = new ContentDialog
                    {
                        Title = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_Find_Same_Book_Title"),
                        Content = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_Find_Same_Book_Content"),
                        PrimaryButtonText = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("ChangeLanguageDialog_Yes"),
                        CloseButtonText = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("ChangeLanguageDialog_No")
                    };

                    //修改按钮样式
                    var bst = new Windows.UI.Xaml.Style(typeof(Button));
                    bst.Setters.Add(new Setter(Button.BackgroundProperty, Windows.UI.Colors.Red));
                    bst.Setters.Add(new Setter(Button.ForegroundProperty, Windows.UI.Colors.White));
                    theSameBookDialog.PrimaryButtonStyle = bst;

                    ContentDialogResult result = await theSameBookDialog.ShowAsync();
                    if (result == ContentDialogResult.Primary)
                    {
                        bCanAdd = true;
                    }
                    else
                    {
                        bCanAdd = false;
                    }
                }

                if (bCanAdd)
                {
                    int newId = 0;
                    if (bookCollection.BookList.Count != 0)
                    {
                        newId = bookCollection.BookList[bookCollection.BookList.Count - 1].Id + 1;
                    }

                    BookClass newBook = new BookClass();
                    newBook.Id = newId;
                    newBook.Title = inputTitle.Text;
                    newBook.Subtitle = inputSubtitle.Text;
                    newBook.Description = inputDescription.Text;
                    newBook.ISBN = inputISBN.Text;
                    newBook.CoverImageBase64 = currentImageBase64Str;
                    newBook.Language = null == inputLanguage.SelectedItem ? "" : inputLanguage.SelectedItem.ToString();
                    newBook.Publisher = inputPublisher.Text;
                    newBook.PublishDate = null == inputPublishDate.SelectedDate ? "" : inputPublishDate.Date.Year.ToString();
                    newBook.Author = inputAuthor.Text;
                    newBook.Translator = inputTranslator.Text;
                    newBook.Country = null == inputCountry.SelectedItem ? "" : inputCountry.SelectedItem.ToString();
                    newBook.Categorys = inputCategorys.Text;
                    newBook.Price = price;

                    var tags = inputedTags.Text.Split(' ');
                    foreach (var tag in tags)
                    {
                        newBook.Tags.Add(tag);
                    }

                    bookCollection.BookList.Add(newBook);
                    addNewBookIdList.Add(newBook.Id);

                    refreshListView();

                    //change save btn color
                    setSaveBtnRed(true);
                    //reset force no save flag
                    forceNoSave_ChangeBookCollection = false;
                    forceNoSave_CreateNewBookCollection = false;
                }
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

        private async void save_book_info_Click(object sender, RoutedEventArgs e)
        {
            if(bookCollection.BookList.Count > booksCollectionListPreCount)
            {
                string title = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_Save_Dialog_Title");
                string content = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_Save_Dialog_Content");
                string btnYes = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("ChangeLanguageDialog_Yes");
                string btnNo = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("ChangeLanguageDialog_No");
                string newMessage = content.Replace("XXXXXX", combobox_book_collections.SelectedItem.ToString());
                await Save(title, newMessage, btnYes, btnNo);
            }
            else
            {
                //没有添加新东西警告
                ContentDialog dialog = new ContentDialog
                {
                    Title = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_Not_Add_Title"),
                    Content = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_Not_Add_Content"),
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

        private async Task<bool> Save(string dialogTitle, string dialogContent, string btnYes, string  btnNO)
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
                string json = JsonConvert.SerializeObject(bookCollection);
                var booksFolder = await ApplicationData.Current.RoamingFolder.GetFolderAsync(AppGloableData.appBooksFolderName);
                var filr = await booksFolder.GetFileAsync(books.getBookFullName(combobox_book_collections.SelectedIndex));
                await FileIO.WriteTextAsync(filr, json, UnicodeEncoding.Utf8);

                //change save btn color
                setSaveBtnRed(false);
                //update the count parameter
                booksCollectionListPreCount = bookCollection.BookList.Count;

                //new book list clear
                addNewBookIdList.Clear();

                refreshListView();

                return true;
            }
            return false;
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
            if(bookCollection != null && bookCollection.BookList != null) bookCollection.BookList.Clear();
            bookCollection = JsonConvert.DeserializeObject<BookCollectionClass>(bookInfoStr);

            booksCollectionListPreCount = bookCollection.BookList.Count;

            //new book list clear
            addNewBookIdList.Clear();

            refreshListView();

            //change save btn color
            setSaveBtnRed(false);
        }

        //刷新ListView
        private void refreshListView()
        {
            listview_book_list.Items.Clear();
            foreach (var info in bookCollection.BookList)
            {
                bool bNewBook = false;
                foreach (int newBookId in addNewBookIdList) 
                {
                    if (newBookId == info.Id) 
                    {
                        bNewBook = true;
                        break;
                    }
                }
                listview_book_list.Items.Add( (bNewBook ? " * " : "   ") 
                    + info.Id.ToString().PadLeft(5, '0') 
                    + " - " 
                    + info.Title);
            }

            clearAll();
        }

        private bool isAddNewInfo()
        {
            return null == bookCollection ? false : bookCollection.BookList.Count > booksCollectionListPreCount;
        }

        private async void combobox_book_collections_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if(!forceNoSave_ChangeBookCollection && isAddNewInfo() && await showNoSaveAlarm())
            {
                forceNoSave_ChangeBookCollection = true;
            }
        }

        private async Task<bool> showNoSaveAlarm()
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_No_Save_Title"),
                Content = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_No_Save_Content"),
                PrimaryButtonText = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_Check_Save_Btn_Yes"),
                CloseButtonText = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_Check_Save_Btn_No")
            };

            //修改按钮样式
            var bst = new Windows.UI.Xaml.Style(typeof(Button));
            bst.Setters.Add(new Setter(Button.BackgroundProperty, Windows.UI.Colors.Red));
            bst.Setters.Add(new Setter(Button.ForegroundProperty, Windows.UI.Colors.White));
            dialog.PrimaryButtonStyle = bst;

            ContentDialogResult result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary) return true;
            else return false;
        }

        private void inputTags_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            if (textBox.Text != "")
            {
                if (textBox.Text[textBox.Text.Length - 1] == '#')
                {
                    int pos = textBox.SelectionStart - 1;
                    textBox.Text = textBox.Text.Remove(pos, 1);
                    textBox.SelectionStart = pos;
                }
            }
        }
    }
}
