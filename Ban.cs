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
using Npgsql;
namespace Client
{
    public partial class Ban : MaterialForm
    {
      

        private static NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(@"Server=localhost;Port=5432;User Id=postgres;Password=kaluga4000;Database=Chat;");

        }
        public string ChatName;
        public string ChatLogin;
        public Ban(string chatName,string chatLogin)
        {
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            InitializeComponent();
            ChatName = chatName;
            ChatLogin = chatLogin;
            int chat_id;
            using (NpgsqlConnection con1 = GetConnection())
            {

                con1.Open();
                string comText1 = ("select id from public.\"chats\" where chat_name = '" + ChatName + "'");
                NpgsqlCommand cmd1 = new NpgsqlCommand(comText1, con1);
                chat_id = (int)cmd1.ExecuteScalar();
                cmd1.ExecuteNonQuery();
                con1.Close();

            }
            NpgsqlConnection conn = GetConnection();
       
           
            conn.Open();
            DataTable dt = new DataTable();
            NpgsqlDataAdapter adap = new NpgsqlDataAdapter("SELECT login FROM public.\"users\", public.\"Messages\" where id_chat ="+ chat_id + " and id_user = users.id group by login", conn);
            adap.Fill(dt);
            comboBox1.DataSource = dt;  
            comboBox1.DisplayMember = "login";
          //  comboBox1.ValueMember = "id_sotrud";
            conn.Close();
        }

        public void button1_Click(object sender, EventArgs e)
        {
            string user_name = comboBox1.Text;
            int user_id;

            using (NpgsqlConnection con1 = GetConnection())
            {
          

                con1.Open();
                string comText1 = ("select id from public.\"users\" where login = '"+ user_name + "'");
                NpgsqlCommand cmd1 = new NpgsqlCommand(comText1, con1);
                user_id = (int)cmd1.ExecuteScalar();
                cmd1.ExecuteNonQuery();
                con1.Close();

            }
            int chat_id;
            using (NpgsqlConnection con1 = GetConnection())
            {

                con1.Open();
                string comText1 = ("select id from public.\"chats\" where chat_name = '" + ChatName + "'");
                NpgsqlCommand cmd1 = new NpgsqlCommand(comText1, con1);
                chat_id = (int)cmd1.ExecuteScalar();
                cmd1.ExecuteNonQuery();
                con1.Close();

            }
            NpgsqlConnection con2 = GetConnection();

            con2.Open();
            string comText2 = ("insert into public.\"bans\" (id_user, id_chat) values ('" + user_id + "','" + chat_id +  "')");
            NpgsqlCommand cmd2 = new NpgsqlCommand(comText2, con2);
            //  count = (long)cmd2.ExecuteScalar();
            cmd2.ExecuteNonQuery();
            con2.Close();
            using (NpgsqlConnection con1 = GetConnection())
            {

                con1.Open();
                string comText1 = ("delete  from public.\"Messages\" where id_user = " + user_id + "and id_chat ="+ chat_id);
                NpgsqlCommand cmd1 = new NpgsqlCommand(comText1, con1);
               
                cmd1.ExecuteNonQuery();
                con1.Close();

            }
            ChatForm f = new ChatForm(ChatLogin, ChatName);
            this.Visible = false;
            f.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ChatForm f = new ChatForm(ChatLogin, ChatName);
            this.Visible = false;
            f.ShowDialog();
        }
    }
}
