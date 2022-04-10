using System;
using System.Collections.Generic;

namespace MyBookManager
{
    class BookCollectionClass
    {
        /// <summary>
        /// 名称可以由用户定义，ID由程序自己分配。
        /// 在创建文件的时候，为了避免重名创建失败，使用[Book_ID_Name]的命名规则
        /// </summary>
        public int Id;                    //收藏ID
        public string Name;               //收藏名字
        public string CreatedDate;        //创建日期 2021-07-31 15:36:10
        public List<BookClass> BookList;  //图书列表

        public BookCollectionClass()
        {
            BookList = new List<BookClass>();
        }
    }
}
