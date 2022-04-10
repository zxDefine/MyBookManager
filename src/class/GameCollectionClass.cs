using System;
using System.Collections.Generic;

namespace MyBookManager
{
    class GameCollectionClass
    {
        public int Id;                      //收藏ID
        public string Name;                 //收藏名字
        public string CreateDate;           //创建日期 2021-07-31 15:36:10
        public List<GameClass> GameList;    //游戏列表

        public GameCollectionClass()
        {
            GameList = new List<GameClass>();
        }
    }
}
