using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet;
using VkNet.Model.RequestParams;
using System.Threading;

namespace Bot.net4._7
{
    class SearchPostsGrp
    {
        public VkApi _api;
        public SearchPostsGrp(VkApi vkApi)
        {
            _api = vkApi;
        }
        public uint idWhoNeedCheck = 84289403;
        private int[] Score = new int[] { 0, 0, 0, 0, 0, 0 };
        public string Text { get; private set; } = "";
        private bool OneMoreHundred = true;
        private int offset = 0;
        DateTime TimeNow = DateTime.Now;
        public int[] groupsWhereNeedMakePosts { private set; get; } = new int[] { -34985835, -33764742, -8337923, -39130136, -32538224, -94946045 };
        
        

        public void Start(int grp)
        {
            OneMoreHundred = true;
            while (OneMoreHundred)
            {

                var WallSearch = _api.Wall.Search(new WallSearchParams
                {
                    OwnerId = groupsWhereNeedMakePosts[(grp)],
                    Query = "id84289403",
                    OwnersOnly = false,
                    Count = 100,
                    Offset = offset //смещение


                });
                for (int i = 0; i < 100; i++)
                {
                    if (WallSearch[i].Date.Value.Day != TimeNow.Day)
                    {
                        OneMoreHundred = false;
                        i = 100;
                    }
                    else if (Proverka(WallSearch[i].Date.Value) && WallSearch[i].FromId == idWhoNeedCheck)
                    {
                        Score[grp]++;
                    }
                    
                        
                }
                offset++;
            }
        }

        public string TextForMessage()
        {
            Text = "";
            for(int i = 0; i < groupsWhereNeedMakePosts.Length; i++)
            {
                Start(i);
            }
            var user = _api.Users.Get(idWhoNeedCheck);
            int ScoreAll = 0;
            Text += "[id" + idWhoNeedCheck + "|" + user.FirstName + "]";
            for (int i = 0; i < Score.Length; i++)
            {
                Text += "\n В группе vk.com/club" + groupsWhereNeedMakePosts[i].ToString().Remove(0,1) +" " + Score[i] + " постов";
                ScoreAll += Score[i];
            }
            Text += "\n Всего: " + ScoreAll;
            return Text;
        }
        

        private bool Proverka(DateTime time)
        {
            
            if (time.Day == TimeNow.Day)
            {
                if (time.Hour < TimeNow.Hour)
                    return true;
                else if (time.Hour == TimeNow.Hour && time.Minute <= TimeNow.Minute)
                    return true;
                else
                    return false;
            }
            else
                return false;
        } 
    }


}
