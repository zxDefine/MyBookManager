using System;
using System.Collections.Generic;

namespace MyBookManager
{
    class GameClass
    {
        public int Id;
        public String Title;                    //游戏名字
        public String Subtitle;                 //副标题
        public String Description;              //说明
        public string CoverImageBase64;         //封面
        public string Language;                 //语言
        public String Developer;                //开发商
        public String Publisher;                //发行商
        public string PublishDate;              //发售时间
        public string Platform;                 //平台
        public String Categorys;                //分类
        public List<String> Tags;               //标签

        public GameClass()
        {
            Tags = new List<String>();
        }
    }
}
