using System;
using System.Collections.Generic;

namespace MyBookManager
{
    class BookClass
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id;                                               //图书ID
        public String Title;                                         //图书名字
        public String Subtitle;                                      //副标题
        public String Description;                                   //说明
        public string ISBN;                                          //ISBN
        public string CoverImageBase64;                              //封面
        public string Language;                                      //语言
        public String Publisher;                                     //出版社
        public string PublishDate;                                   //出版日期 2020-6
        public String Author;                                        //作者
        public String Translator;                                    //翻译者
        public string Country;                                       //出版国家
        public String Categorys;                                     //分类
        public float Price;                                          //价格
        public List<String> Tags;                                    //标签

        public BookClass()
        {
            Tags = new List<String>();
        }
    }

    class BookPageNameListUnit
    {
        public string name;
        public int Id;
    }

    class BookPageNameList
    {
        public List<BookPageNameListUnit> nameList = new List<BookPageNameListUnit>();

        public void add(int id, string name)
        {
            BookPageNameListUnit tmp = new BookPageNameListUnit();
            tmp.name = name;
            tmp.Id = id;
            bool isExist = false;
            foreach(var book in nameList)
            {
                if(book.Id == tmp.Id)
                {
                    isExist = true;
                }
            }

            if (!isExist)
            {
                nameList.Add(tmp);
            }
        }

        public string[] getNameList()
        {
            string[] tmpList = new string[nameList.Count];
            for (int i = 0; i < tmpList.Length; i++)
            {
                tmpList[i] = nameList[i].name;
            }
            return tmpList;
        }

        public string getBookFullName(int index)
        {
            if(index >= 0 && index < nameList.Count)
            {
                return "book_" + nameList[index].Id.ToString() + "_" + nameList[index].name +".json";
            }
            return "";
        }
    }
}
