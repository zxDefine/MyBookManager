using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace MyBookManager
{
    class FindBookDrawClass 
    {
        public BitmapImage BookImage { get; set; }
        public string BookTitle { get; set; }
        public string BookSubtitle { get; set; }
        public string BookISBN { get; set; }
        public string BookAuthor { get; set; }
        public string BookTranslator { get; set; }
        public string BookLanguage { get; set; }
        public string BookCountry { get; set; }
        public string BookPublisher { get; set; }
        public string BookPublishDate { get; set; }
        public string BookPrice { get; set; }
        public string BookCategory { get; set; }
        public string BookTag { get; set; }
        public string BookDescription { get; set; }

        public FindBookDrawClass()
        {
            BookImage = new BitmapImage(new Uri("ms-appx:///Assets/defultNoImage.png")); ;
            BookTitle = "-";
            BookSubtitle = "-";
            BookISBN = "-";
            BookAuthor = "-";
            BookTranslator = "-";
            BookLanguage = "-";
            BookCountry = "-";
            BookPublisher = "-";
            BookPublishDate = "-";
            BookPrice = "-";
            BookCategory = "-";
            BookTag = "-";
            BookDescription = "-";
        }
    }
}
