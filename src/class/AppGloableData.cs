using System;

namespace MyBookManager
{
    public static class AppGloableData
    {
        public static readonly string appBooksFolderName = "BookCollectionLib";
        public static readonly int appBeginId = -1;

        private static int appBookCollectionBaseID = 0;

        public enum Types { BOOK, GAME }

        public enum CollectionParameter {
            BOOK_ISBN,                          //ISBN码
            BOOK_TITLE,                         //图书标题    
            BOOK_SUBTITLE,                      //图书副标题
            BOOK_IMAGE,                         //图书图片
            BOOK_AUTHOR,                        //图书作者
            BOOK_TRANSLATOR,                    //图书翻译者
            BOOK_DESCRIPTION,                   //图书描述
            BOOK_PUBLISHERS,                    //图书出版社
            BOOK_PUBLISHDATE,                   //图书出版时间
            BOOK_LANGUAGES,                     //图书语言
            BOOK_COUNTRY,                       //图书国家
            BOOK_PRICE,                         //图书价格
            BOOK_TAGS,                          //图书标签
            BOOK_CATEGORYS,                     //图书类型
            //-------------------------
            GAME_TITLE,                         //游戏标题
            GAME_SUBTITLE,                      //游戏副标题
            GAME_IMAGE,                         //游戏图片
            GAME_DEVELOPER,                     //游戏开发商
            GAME_PUBLISHER,                     //游戏发行商
            GAME_DESCRIPTION,                   //游戏描述
            GAME_PUBLISHDATE,                   //游戏发售时间
            GAME_PLATFORM,                      //游戏发售平台
            GAME_LANGUAGES,                     //游戏语言
            GAME_CATEGORYS,                     //游戏类型
            GAME_TAGS                           //游戏标签
        }

        //顺序以下
        //中文简体,中文繁体,日语,韩语,英语,法语,西班牙语,葡萄牙语,德语,意大利语,俄罗斯语,其他
        public enum Language { CN, TW, JP, KR, EN, FR, ES, PT, DE, IT, RU, OTHER }
        //中,日,韩,美,英,法,西班牙,葡萄牙,德国,意大利,俄罗斯,其他
        public enum Country { CN, JP, KR, US, UK, FR, ES, PT, DE, IT, RU, OTHER }

        public static void initGameAndBookCollectionBaseID(int bcID)
        {
            appBookCollectionBaseID = bcID;
        }

        public static int getNewBookCollectionID()
        {
            return appBookCollectionBaseID;
        }

        public static void plusBookCollectionID()
        {
            appBookCollectionBaseID++;
        }

        public static string[] getLanguageDrawNameList() 
        {
            int listSize = Enum.GetNames(typeof(AppGloableData.Language)).Length;
            string[] lanDrawNameList = new string[listSize];

            for (int i = 0; i < listSize; i++)
            {
                switch ((AppGloableData.Language)i)
                {
                    case AppGloableData.Language.CN:
                        lanDrawNameList[i] = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_Language_CN");
                        break;
                    case AppGloableData.Language.TW:
                        lanDrawNameList[i] = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_Language_TW");
                        break;
                    case AppGloableData.Language.JP:
                        lanDrawNameList[i] = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_Language_JP");
                        break;
                    case AppGloableData.Language.KR:
                        lanDrawNameList[i] = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_Language_KR");
                        break;
                    case AppGloableData.Language.EN:
                        lanDrawNameList[i] = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_Language_EN");
                        break;

                    case AppGloableData.Language.OTHER:
                        lanDrawNameList[i] = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_Language_Other");
                        break;
                    case AppGloableData.Language.FR:
                        lanDrawNameList[i] = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_Language_FR");
                        break;
                    case AppGloableData.Language.ES:
                        lanDrawNameList[i] = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_Language_ES");
                        break;
                    case AppGloableData.Language.PT:
                        lanDrawNameList[i] = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_Language_PT");
                        break;
                    case AppGloableData.Language.DE:
                        lanDrawNameList[i] = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_Language_DE");
                        break;

                    case AppGloableData.Language.IT:
                        lanDrawNameList[i] = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_Language_IT");
                        break;
                    case AppGloableData.Language.RU:
                        lanDrawNameList[i] = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_Language_RU");
                        break;
                }
            }

            return lanDrawNameList;
        }

        public static int getLanguageEnumIndexByDrawName(string drawName) 
        {
            string[] drawNameList = getLanguageDrawNameList();
            int index = -1;
            for(int i = 0; i < drawNameList.Length; i++)
            {
                if (drawNameList[i] == drawName) 
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        public static string[] getCountryDrawNameList() 
        {
            int listSize = Enum.GetNames(typeof(AppGloableData.Country)).Length;
            string[] couDrawNameList = new string[listSize];
            for (int i = 0; i < listSize; i++)
            {
                switch ((AppGloableData.Country)i)
                {
                    case AppGloableData.Country.CN:
                        couDrawNameList[i] = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_Country_CN");
                        break;
                    case AppGloableData.Country.JP:
                        couDrawNameList[i] = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_Country_JP");
                        break;
                    case AppGloableData.Country.KR:
                        couDrawNameList[i] = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_Country_KR");
                        break;
                    case AppGloableData.Country.US:
                        couDrawNameList[i] = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_Country_US");
                        break;
                    case AppGloableData.Country.UK:
                        couDrawNameList[i] = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_Country_UK");
                        break;

                    case AppGloableData.Country.FR:
                        couDrawNameList[i] = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_Country_FR");
                        break;
                    case AppGloableData.Country.ES:
                        couDrawNameList[i] = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_Country_ES");
                        break;
                    case AppGloableData.Country.PT:
                        couDrawNameList[i] = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_Country_PT");
                        break;
                    case AppGloableData.Country.DE:
                        couDrawNameList[i] = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_Country_DE");
                        break;
                    case AppGloableData.Country.IT:
                        couDrawNameList[i] = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_Country_IT");
                        break;

                    case AppGloableData.Country.RU:
                        couDrawNameList[i] = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_Country_RU");
                        break;
                    case AppGloableData.Country.OTHER:
                        couDrawNameList[i] = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("CreateBook_Country_OTHER");
                        break;
                }
            }

            return couDrawNameList;
        }

        public static int getCountryEnumIndexByDrawName(string drawName)
        {
            string[] drawNameList = getCountryDrawNameList();
            int index = -1;
            for (int i = 0; i < drawNameList.Length; i++)
            {
                if (drawNameList[i] == drawName)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }
    }
}
