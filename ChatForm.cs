using System;
using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;
using Npgsql;

namespace Client
{
    public partial class ChatForm : MaterialForm
    {
        private delegate void ChatEvent(string content, string clr);
        private ChatEvent _addMessage;
        private Socket _serverSocket;
        private Thread listenThread;
        private string _host = "127.0.0.1";
        private int _port = 2222;
        public string LoginName;
        public string ChatName;
       
   
       
        public ChatForm(string Login, string Chatname)
        {

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            InitializeComponent();
           
            messageData.Focus();
            //или
            messageData.Select();
            LoginName = Login;
            ChatName = Chatname;
            GetNameID(LoginName);
            label2.Text = "Чат:" + Chatname;
            _addMessage = new ChatEvent(AddMessage);
            userMenu = new ContextMenuStrip();
            ToolStripMenuItem PrivateMessageItem = new ToolStripMenuItem();
            PrivateMessageItem.Text = "Личное сообщение";
            PrivateMessageItem.Click += delegate
            {
                if (userList.SelectedItems.Count > 0)
                {
                    messageData.Text = $"\"{userList.SelectedItem} ";
                }
            };
            userMenu.Items.Add(PrivateMessageItem);

        }

        private void AddMessage(string Content, string Color = "Black")
        {
            if (InvokeRequired)
            {
                Invoke(_addMessage, Content, Color);
                return;
            }

            chatBox.SelectionStart = chatBox.TextLength;
            chatBox.SelectionLength = Content.Length;
            
            chatBox.AppendText(Content + Environment.NewLine);
        }

       

        private void ChatForm_Load(object sender, EventArgs e)
        {
            IPAddress temp = IPAddress.Parse(_host);
            _serverSocket = new Socket(temp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _serverSocket.Connect(new IPEndPoint(temp, _port));
            if (_serverSocket.Connected)
            {

                AddMessage("Связь с сервером установлена.");
                listenThread = new Thread(listner);
                listenThread.IsBackground = true;
                listenThread.Start();

            }
            else
                AddMessage("Связь с сервером не установлена.");
            string nickName = LoginName;
            int chatID = GetChatID(ChatName);
            int nameID = GetNameID(LoginName);
            int last = LastMessID(chatID);
            if (true)
            {
                using (NpgsqlConnection con = GetConnection())
                {

                    con.Open();
                    string comText = ("SELECT admin FROM public.\"chats\"  where id =" + chatID );
                    NpgsqlCommand cmd = new NpgsqlCommand(comText, con);
                    string mess = (string)cmd.ExecuteScalar();
                    if (Convert.ToInt32(mess) == nameID)
                    {
                        botton2.Visible = true;
                        label1.Text = "Вы админ этого чата";
                    }
                    else
                    {
                        botton2.Visible = false;
                        label1.Text = "";
                    }
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            if (last != 0)
            {


                for (int i = last - 100; i <= last; i++)
                {
                    using (NpgsqlConnection con = GetConnection())
                    {

                        con.Open();
                        string comText = ("SELECT login FROM public.\"Messages\", public.\"users\", public.\"chats\" where chats.id = id_chat and users.id = id_user and id_chat =" + chatID + "and id_mess =" + i);
                        NpgsqlCommand cmd = new NpgsqlCommand(comText, con);
                        string mess = (string)cmd.ExecuteScalar();
                        if (mess != null)
                        {
                            chatBox.AppendText(mess + ":");
                        }
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    using (NpgsqlConnection con = GetConnection())
                    {

                        con.Open();
                        string comText = ("SELECT  message  FROM public.\"Messages\",public.\"chats\" where id_mess =" + i + " and id_chat = " + chatID);
                        NpgsqlCommand cmd = new NpgsqlCommand(comText, con);
                        string mess = (string)cmd.ExecuteScalar();
                        if (mess != null)
                        {
                            chatBox.AppendText(mess + Environment.NewLine);
                        }

                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }

            }
            Send($"#setname|{nickName}");
            Send($"#setchat|{ChatName}");
        }
        private static NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(@"Server=localhost;Port=5432;User Id=postgres;Password=kaluga4000;Database=Chat;");

        }
        public int LastMessID(int IDchat)
        {
            int lastmess;
            NpgsqlConnection con1 = GetConnection();
            con1.Open();

            string comText1 = ("SELECT  count(*)  FROM public.\"Messages\" where id_chat = " + IDchat );
            NpgsqlCommand cmd1 = new NpgsqlCommand(comText1, con1);
            long count = (long)cmd1.ExecuteScalar();
            cmd1.ExecuteNonQuery();
            if (count!=0)
            {


                NpgsqlConnection con = GetConnection();
                con.Open();

                string comText = ("SELECT  id_mess  FROM public.\"Messages\" where id_chat = " + IDchat + " order by id_mess desc limit 1");
                NpgsqlCommand cmd = new NpgsqlCommand(comText, con);
                lastmess = (int)cmd.ExecuteScalar();
                cmd.ExecuteNonQuery();
            }
            else
            {
                lastmess = 0;
            }
            return lastmess;
        }
        public int GetChatID(string chatName)
        {
            NpgsqlConnection con = GetConnection();
            con.Open();
            string comText = ("SELECT id FROM public.chats where chat_name = '"+chatName+"'");
            NpgsqlCommand cmd = new NpgsqlCommand(comText, con);
            int count = (int)cmd.ExecuteScalar();
            return count;
        }
       
        public int GetNameID(string userName)
        {
            NpgsqlConnection con = GetConnection();
            con.Open();
            string comText = ("SELECT id FROM public.users where login = '" + userName + "'");
            NpgsqlCommand cmd = new NpgsqlCommand(comText, con);
            int count = (int)cmd.ExecuteScalar();
            return count;
        }
        public void Send(byte[] buffer)
        {
            try
            {
                _serverSocket.Send(buffer);

            }
            catch { }
        }
        public void Send(string Buffer)
        {
            try
            {
                _serverSocket.Send(Encoding.Unicode.GetBytes(Buffer));


            }
            catch { }
        }


        public void handleCommand(string cmd)
        {

            string[] commands = cmd.Split('#');
            int countCommands = commands.Length;
            for (int i = 0; i < countCommands; i++)
            {
                try
                {
                    string currentCommand = commands[i];
                    if (string.IsNullOrEmpty(currentCommand))
                        continue;
                    if (currentCommand.Contains("setnamesuccess"))
                    {


                        //Из-за того что программа пыталась получить доступ к контролам из другого потока вылетал эксепщен и поля не разблокировались

                        Invoke((MethodInvoker)delegate
                        {
                            AddMessage($"Добро пожаловать, {LoginName}");
                            
                            //chatBox.Enabled = true;
                            messageData.Enabled = true;
                            userList.Enabled = true;
                         
                        });
                        continue;
                    }
                    if (currentCommand.Contains("setnamefailed"))
                    {
                        AddMessage("Неверный ник!");
                        continue;
                    }
                    if (currentCommand.Contains("msg"))
                    {

                        string[] Arguments = currentCommand.Split('|');
                        AddMessage(Arguments[1], Arguments[2]);
                        continue;
                    }

                    if (currentCommand.Contains("userlist"))
                    {
                        string[] Users = currentCommand.Split('|')[1].Split(',');
                        int countUsers = Users.Length;
                        userList.Invoke((MethodInvoker)delegate { userList.Items.Clear(); });
                        for (int j = 0; j < countUsers; j++)
                        {
                            userList.Invoke((MethodInvoker)delegate { userList.Items.Add(Users[j]); });
                        }
                        continue;

                    }


                }
                catch (Exception exp) { Console.WriteLine("Error with handleCommand: " + exp.Message); }

            }


        }
        public void listner()
        {
            try
            {
                while (_serverSocket.Connected)
                {
                    byte[] buffer = new byte[2048];
                    int bytesReceive = _serverSocket.Receive(buffer);
                    handleCommand(Encoding.Unicode.GetString(buffer, 0, bytesReceive));
                }
            }
            catch
            {
                MessageBox.Show("Связь с сервером прервана");
                Application.Exit();
            }
        }

        private void enterChat_Click(object sender, EventArgs e)
        {
          
        }

        private void ChatForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_serverSocket.Connected)
                Send("#endsession");
        }

        private void messageData_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                
                string msgData = messageData.Text;
                if (string.IsNullOrEmpty(msgData))
                    return;
                
                Send($"#message|{msgData}");

                using (NpgsqlConnection con = GetConnection())
                {
                   string comtext = "INSERT INTO public.\"Messages\" (message, id_user, id_chat)values('" + msgData + "','" + GetNameID(LoginName) + "','" + GetChatID(ChatName) + "')";
                    //string comtext = "INSERT INTO public.\"Messages\" (message, id_user, id_chat) values('asd', '1','2')";

                    NpgsqlCommand cmd = new NpgsqlCommand(comtext, con);
                    con.Open();
                    cmd.ExecuteNonQuery();

                }
                messageData.Text = string.Empty;
            }
        }


private void chatBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void ChatForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Close();
            EnterForm enterform = (EnterForm)Application.OpenForms["EnterForm"];
            if (enterform != null)
            {
                enterform.Close();
            }
            Register register = (Register)Application.OpenForms["Register"];
            if (register != null)
            {
                register.Close();
            }
            Grid grid = (Grid)Application.OpenForms["Register"];
            if (grid != null)
            {
                grid.Close();
            }
        }

        private void messageData_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_serverSocket.Connected)
                Send("#endsession");
            Grid f = new Grid(LoginName);
            this.Visible = false;
            f.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (_serverSocket.Connected)
                Send("#endsession");
            Ban f = new Ban(ChatName, LoginName);
            this.Visible = false;
            f.ShowDialog();
        }
    }
}
