using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Exception;
using VkNet.Categories;
using VkNet.Enums.SafetyEnums;
using VkNet.Utils;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;
using VkNet.Utils.AntiCaptcha;
using VkNet.Model;
using System.Collections;
using System.Collections.ObjectModel;
using VkNet.Enums;
using System.Windows.Forms;
using System.Data.Entity;
using System.Diagnostics;

namespace Bot.net4._7
{

    public partial class MainForm : System.Windows.Forms.Form
    {
        List<long> PRmembers = new List<long>(); //Чтоб создать пиар лист

        long myID = 84289403, botID = 425112130, AugustRimID = 400532107, ViktOdinID = 379514877;
        uint _peerId = 2000000004;

        public VkApi _api = new VkApi();

        public MainForm()
        {
            InitializeComponent();
            this.InitializeComponent();
            if (VkAuth())
            {
                Thread CheckMess = new Thread(new ThreadStart(CheckMessages));
                CheckMess.Start();

                PRmembers.Add(myID);//мой
                PRmembers.Add(AugustRimID);//Август Римский
                PRmembers.Add(ViktOdinID);//Виктория Одинцова


                //Проверка каждого 
                //IList<long> chatIds = new List<long>();
                //chatIds.Add(4);
                //var chat = _api.Messages.GetChatUsers(chatIds, UsersFields.Nickname, NameCase.Nom);
                //foreach (var users in chat)
                //{
                //    SearchPostsGrp srch = new SearchPostsGrp(_api);
                //    srch.idWhoNeedCheck = (uint)users.Id;
                //    Thread ThForCheck = new Thread(new ThreadStart(srch.TextForMessage));
                //    ThForCheck.Start();
                //    Thread.Sleep(5000);
                //}
            }
        }
        

        
        
        private bool VkAuth()
        {
            try
            {
                _api.Authorize(new ApiAuthParams
                {
                    ApplicationId = Convert.ToUInt64(System.IO.File.ReadAllText(@"C:\AppID.txt")),
                    Login = System.IO.File.ReadAllText(@"C:\login.txt"),
                    Password = System.IO.File.ReadAllText(@"C:\password.txt"),
                    Settings = Settings.All
                });
                return true;
            }
            catch (VkApiAuthorizationException) { MessageBox.Show("Пароль не верен"); return false; }
            catch (VkApiException) { MessageBox.Show("БОЛЬШЕ КОСТЫЛЕЙ или id не верен"); return false; }
        }

        private void CheckMessages()
        {
            bool Enable = true;
            while (Enable)
            {
                Thread.Sleep(1000);
                //try
                //{
                    var message = _api.Messages.GetHistory(new MessagesGetHistoryParams
                    {
                        Count = 1,
                        //UserId = myID // мой ИД
                        PeerId = _peerId
                    });
                    if (ReturnReques(message) != null)
                    {
                        //IncomMessEventCl(this, new IncomMessEvent(string.Format(message.Messages[0].Body))); евент
                        if (message.Messages[0].Body != "/stat")
                            SendMess(false, _peerId, ReturnReques(message));
                        else
                        {
                            SendMess(false, _peerId, ReturnReques(message));
                            SearchPostsGrp SrchPstGr = new SearchPostsGrp(_api);
                            SrchPstGr.idWhoNeedCheck = (uint)message.Messages[0].UserId;
                            Thread msg = new Thread(new ThreadStart(new ThreadStart(SrchPstGr.TextForMessage)));
                            msg.Start();
                        }

                    }
            }
        }


        private string ReturnReques(MessagesGetObject mess)
        {
            string forId = "";
            if (mess.Messages[0].Body.Contains("vk.com/id"))
            {
                forId = mess.Messages[0].Body.Substring(9);
                mess.Messages[0].Body = "Friends";
            }

            switch (mess.Messages[0].Body)
            {
                case "/groups":
                    string textRet = "👇Пиарить только здесь👇";
                    SearchPostsGrp SrchPstGr = new SearchPostsGrp(_api);
                    for (int i = 0; i < SrchPstGr.groupsWhereNeedMakePosts.Length; i++)
                    {
  
                        textRet += "\n ✖vk.com/club" + SrchPstGr.groupsWhereNeedMakePosts[i].ToString().Remove(0,1);
                    }
                    return textRet;
                case "/stat":
                    return "Подсчет...";
                case "/info":
                    return "информация";
                case "Friends":
                    return "Не сделано"; // TODO: доделать

                    int pars = 0;
                    if (Int32.TryParse(forId, out pars))
                    {
                        return CheckDidAddFriends(pars);
                    }
                    else
                    {
                        return "Указывать id в виде 'vk.com/xxxxx'";
                    }

                case "/list":
                    return List(mess.Messages[0].UserId.Value);
                default:
                    return null;
            }
        }
        

        private string CheckDidAddFriends(long id)
        {
            List<long> uIDmembers = new List<long>();
            IList<long> chatIds = new List<long>();
            chatIds.Add(4);
            var chat = _api.Messages.GetChatUsers(chatIds, UsersFields.Nickname, NameCase.Nom);
            string[] text = new string[] { "", "" };
            bool AllAdd = true;
            uIDmembers.Add(id);
            foreach (var users in chat)
            {
                Thread.Sleep(500);
                if (users.Id != botID && users.Id != id)
                {
                    uIDmembers.Add(users.Id);
                    var dict = _api.Friends.AreFriends(uIDmembers);
                    if(dict[uIDmembers[1]] == FriendStatus.OutputRequest || dict[uIDmembers[1]] == FriendStatus.Friend || dict[uIDmembers[1]] == FriendStatus.InputRequest)
                    {
                        text[0] += "\n vk.com/id" + id + " и vk.com/id" + users.Id + " возможные друзья";
                    }
                    else if(dict[uIDmembers[1]] == FriendStatus.NotFriend)
                    {
                        AllAdd = false;
                        text[1] += "\n vk.com/id" + id + " и vk.com/id" + users.Id + " не друзья";
                    }
                    uIDmembers.RemoveAt(1);
                }
            }

            SendMess(true, myID, text[0]);
            SendMess(true, myID, text[1]);
            
            string textForRet = (AllAdd) ? "Всех добавил" : "Добавил не всех";
            return textForRet;
            
        }

        public string List(long id)
        {
            
            if(PRmembers.Find(x => x == id) == 0)
            {
                PRmembers.Add(id);
            }


            string _list = "&#2972;&#9552;&#9552;&#9552;&#9552;&#9552;&#9552;&#9552;&#9552;&#2972;&#1769;&#1758;&#1769;&#2972;&#9552;&#9552;&#9552;&#9552;&#9552;&#9552;&#9552;&#2972; \n&#128150;&#128150;ХОЧЕШЬ ПО 200-300а ЗАЯВОК&#128150;&#128150; \n&#128293;&#128293; МЕСТА ОГРАНИЧЕНЫ &#128293;&#128293; \n&#128314;&#128160; ДОБАВЛЯЙСЯ КО ВСЕМ &#128160;&#128314; \n&#2972;&#9552;&#9552;&#9552;&#9552;&#9552;&#9552;&#9552;&#9552;&#2972;&#1769;&#1758;&#1769;&#2972;&#9552;&#9552;&#9552;&#9552;&#9552;&#9552;&#9552;&#2972; \n\n&#128081;..:Создатель:..&#128081; \n&#128041;[id84289403|Роман Бакакин] \n\n&#10145;&#128081; ...Администрация... &#128081; &#11013; \n&#10024; [id400532107|Август Римский] &#10024; \n&#10024;[id379514877|Виктория Одинцова]&#10024; \n\n&#10024;Пиaрщики&#10024;";
            foreach(int newIdUser in PRmembers)
            {
                if (newIdUser != myID && newIdUser != AugustRimID && newIdUser != ViktOdinID)
                {
                    string Name = _api.Users.Get(newIdUser).FirstName + " " + _api.Users.Get(newIdUser).LastName;
                    _list += "\n&#128029;[id" + newIdUser + "|" + Name + "] ";
                }
            }

            _list += "\n&#128029;Свободно \n&#128029;Свободно\n\n&#128312;&#128312;&#128312;&#128312;&#128312;&#128312;&#128312;&#128312;&#128312;&#128312;&#128312; \n&#128160;Всех добавил(а)? Хочешь в список&#10068;&#128527;&#128160; \n&#9999;Пиши [id400532107|МНЕ] или [id379514877|ЕЙ]&#9999; \n&#2972;&#9552;&#9552;&#9552;&#9552;&#9552;&#9552;&#9552;&#9552;&#2972;&#1769;&#1758;&#1769;&#2972;&#9552;&#9552;&#9552;&#9552;&#9552;&#9552;&#9552;&#2972;";

            return _list;
        }



        private void button4_Click(object sender, EventArgs e)
        {
            Thread Spam = new Thread(new ThreadStart(SpamMethod));
            Spam.Start();
        }

        private void SpamMethod()
        {
            string[] arrayText = new string[] { "буду", "спамить", "тебе", ",", "пока", "не", "скажишь", "секрет" };
            bool MakeSpam = true;
            int i = 0;
            while (MakeSpam)
            {
                try
                {
                    Thread.Sleep(500);
                    //410502291 аля
                    SendMess(true, 410502291, arrayText[i]);
                    i = (i < (arrayText.Length - 1)) ? i + 1 : i = 0;
                }
                catch (CaptchaNeededException Captcha)
                {
                    SendMess(true, myID, Captcha.Img.ToString());
                }
                catch (Exception)
                {
                    MakeSpam = false;
                    Thread.Sleep(1000);
                    SpamMethod();
                }

            }
        }

        private void SendMess(bool LS, long ToWhom, string text)
        {
            MessagesSendParams _params = new MessagesSendParams();

            if (LS) _params.UserId = ToWhom;
            else _params.PeerId = ToWhom;

            _params.Message = text;

            _api.Messages.Send(_params);
        }
    }



    public class DBUsers
    {
        public int BDid { get; set; }
        public int BDscore { get; set; }
    }
}