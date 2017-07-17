﻿using System;
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

namespace Bot.net4._7
{

    public partial class MainForm : System.Windows.Forms.Form
    {
        public MainForm()
        {
            InitializeComponent();
            this.InitializeComponent();
            if (VkAuth())
            {
                LoadChat();
            }
        }
        DateTime TimeNow = DateTime.Now;
        long myID = 84289403;
        long botID = 425112130;

        //public event EventHandler<IncomMessEvent> IncomMessEventCl;// евент в новом классе, подписка на входящие сообщения.

        List<List<string>> Members = new List<List<string>>(); //инициализация

        List<long> uIDmembers = new List<long>();

        //uint[] TOP = new uint[] { };


        string[] CommandsArray = new string[] { "/status", "/" };
        public VkApi _api = new VkApi();
        short countPeopleInChat;
        //List<string> _users = new List<string>(); //id + fName + lName

        uint _peerId = 2000000004;

        private bool VkAuth()
        {
            try
            {
                _api.Authorize(new ApiAuthParams
                {
                    ApplicationId = 6099938,
                    Login = "+79178687319",
                    Password = "9172357141Rom4ik",
                    Settings = Settings.All
                });

                return true;
            }
            catch (VkApiAuthorizationException) { MessageBox.Show("Пароль не верен"); return false; }
            catch (VkApiException) { MessageBox.Show("БОЛЬШЕ КОСТЫЛЕЙ или id не верен"); return false; }
        }




        private void LoadChat()
        {


            NameCase nom = NameCase.Nom;

            IList<long> chatIds = new List<long>();
            chatIds.Add(4);
            var chat = _api.Messages.GetChatUsers(chatIds, UsersFields.Nickname, nom);

            foreach (var users in chat)
            {
                int i = 0;
                if (users.Id != botID)
                {
                    uIDmembers.Add(users.Id);
                    listBox1.Items.Add(users.FirstName + " " + users.LastName);
                }



                Members.Add(new List<string>());//добавление новой строки
                Members[i].Add("asd");//добавление столбца в новую строку
                i++;
                //Members[0][0];  //обращение к первому столбцу первой строки


                //_users.Add(users.Id + " " + users.FirstName + " " + users.LastName);
            }

            //ПОЛУЧЕНИЕ IDшников
            //string text = "";
            //for (int i = 0; i < _users.Count; i++)
            //{
            //    text += _users[i].ToString() + ", ";
            //}
            //_api.Messages.Send(new MessagesSendParams { UserId = 84289403, Message = text });

            countPeopleInChat = (Int16)listBox1.Items.Count;
            textBox1.Text = countPeopleInChat.ToString();
            SendMess(true, myID, "bot Work");
        }


        private void button1_Click(object sender, EventArgs e)
        {
            var WallSearch = _api.Wall.Get(new WallGetParams
            {
                OwnerId = -62560719, //vk.com/club62560719
                Count = 100,
                Filter = WallFilter.Others
            });
            for (int i = 0; i < 100; i++)
            {
                if (WallSearch.WallPosts[0].Text.Contains("id84289403"))
                {
                    if (Proverka(WallSearch.WallPosts[i].Date.Value))
                    {
                        //if (Members.FindAll(new Action(delegate (string s) { return s.Contains(WallSearch.WallPosts[i].OwnerId.Value.ToString()));
                        //} Проверка есть ли этот id  в списке!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                        {

                        }
                    }
                    SendMess(true, myID, "da");
                }
                else
                {
                    //след группа
                }
            }
        }

        private bool Proverka(DateTime time)
        {

            if (time.Hour < TimeNow.Hour)
            {
                if (time.Minute < TimeNow.Minute)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else return false;
        }

        //Цикл с проверкой сообщений
        private void button3_Click(object sender, EventArgs e)
        {
            Thread CheckMess = new Thread(new ThreadStart(CheckMessages));
            CheckMess.Start();
        }

        private void CheckMessages()
        {
            bool Enable = true;
            while (Enable)
            {
                Thread.Sleep(1000);
                try
                {
                    var message = _api.Messages.GetHistory(new MessagesGetHistoryParams
                    {
                        Count = 1,
                        PeerId = _peerId,
                    });
                    if (ReturnReques(message.Messages[0].Body) != null)
                    {
                        //IncomMessEventCl(this, new IncomMessEvent(string.Format(message.Messages[0].Body))); евент
                        SendMess(false, _peerId, ReturnReques(message.Messages[0].Body));
                    }
                }
                catch (Exception ess)
                {
                    Enable = false;
                }
            }
        }


        private string ReturnReques(string mess)
        {
            string forId = "";
            if (mess.Contains("vk.com/id"))
            {
                forId = mess.Substring(9);
                mess = "Friends";

            }

            switch (mess)
            {
                case "/stat":
                    return "Чтение";
                case "/info":
                    return "информация";
                case "Friends":
                    return forId;
                default:
                    return null;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var dict = _api.Friends.AreFriends(uIDmembers);
            string messAddUser = "";
            bool Whiiile = true;

            while (Whiiile)
            {
                try
                {
                    for (int a = 0; a < uIDmembers.Count; a++)
                    {
                        if (dict[uIDmembers[a]] == FriendStatus.NotFriend && uIDmembers[a] != botID)
                        {
                            messAddUser += "\n" + "[id" + uIDmembers[a] + "|" + listBox1.Items[a].ToString() + "]" + " добавь всех в друзья";
                        }
                    }
                    Thread.Sleep(500);

                    //_api.Messages.Send(new MessagesSendParams { PeerId = myID, Message = messAddUser });
                    SendMess(false, _peerId, messAddUser);
                }
                catch (Exception someExpt)
                {
                    Whiiile = false;
                    Thread.Sleep(2000);
                    SendMess(true, myID, "Проверка на друзей УПАЛА");
                }
            }
        }

        public string List(VkApi api)
        {


            string _list = "ஜ════════ஜ۩۞۩ஜ═══════ஜ \n💖💖ХОЧЕШЬ ПО 200 - 300 ЗАЯВОК💖💖 \n🔥🔥 МЕСТА ОГРАНИЧЕНЫ 🔥🔥 \n🔺💠 ДОБАВЛЯЙСЯ КО ВСЕМ 💠🔺 \nஜ════════ஜ۩۞۩ஜ═══════ஜ \n➡👑 ...Администрация... 👑 ⬅\n 🌙✨ *greentech1256(Роман Бакакин) ✨🌙 \n🔥🔞 ТОП🔞 🔥 \n1. - Свободно";
            //2. - Свободно
            //3. - Свободно
            //4. - Свободно
            //5. - Свободно

            //💙👑..:V.I.P пиарщики:..👑💙 
            //1. 👻 *id433827056(Никита Фоминов) 👻 
            //2. 🌸 *id436004977(Дашуня Ковальская) 🌸 
            //3. 🈶 *id429927610(Дарья Жолобова) 🈶 
            //4. - Свободно
            //5. - Свободно

            //✨🐝Пиaрщики🐝✨ 
            //1. 😈 *id379514877(Виктория Одинцова) 😈 
            //2.❤ *id433486431(Максим Перов) ❤ 
            //3. 😎 *id400532107(Август Римский) 😎 
            //4. 😏 *id218124385(Андрей Куприянов) 😏 
            //5. 🌙 *id436085661(Даниэлла Баскервиль) 🌙 
            //6. 😊 *id399483163(Андрей Вершинин) 😊 
            //7. 👌*id314917106(Николай Юдин)👌 
            //8. 🐝 *id429588219(Тина Заяц) 🐝 
            //9. 💦 *id278673439(Анастасия Лесная)💦 
            //10. 🐠*id427487493(Марта Шаманенко) 🐠 
            //11. 🦊*id429855630(Лера Дидковская) 🦊 
            //12. - Свободно
            //13. - Свободно
            //14. - Свободно
            //15. - Свободно
            //16. - Свободно
            //17. - Свободно
            //18. - Свободно
            //19. - Свободно
            //20. - Свободно

            //🔸🔸🔸🔸🔸🔸🔸🔸🔸🔸🔸 
            //💠Всех добавил(а)? Хочешь в список❔😏💠 
            //📩Пиши* greentech1256(создателям) ✏ 
            //ஜ═══════ஜ۩۞۩ஜ══════ஜ⭐⭐";

            SendMess(true, botID, _list);
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


        //public ReadOnlyCollection<User> GetChatUsers(IEnumerable<long> chatIds, UsersFields fields, NameCase nameCase)
        //{
        //    return ;
        //}


    }



    public class DBUsers
    {
        public int BDid { get; set; }
        public int BDscore { get; set; }
    }
}