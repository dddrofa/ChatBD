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
    public partial class Create : MaterialForm
    {
        string Login;
        public Create(string login)
        {
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            InitializeComponent();
           Login = login;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private static NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(@"Server=localhost;Port=5432;User Id=postgres;Password=kaluga4000;Database=Chat;");

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
        private void button1_Click(object sender, EventArgs e)
        {
            using (NpgsqlConnection con = GetConnection())
            {
                long temp;
                using (NpgsqlConnection con1 = GetConnection())
                {
                   
                        con1.Open();
                        string comText1 = ("select count(id) from public.\"chats\"");
                        NpgsqlCommand cmd1 = new NpgsqlCommand(comText1, con1);
                        temp = (long)cmd1.ExecuteScalar();
                        cmd1.ExecuteNonQuery();
                        con1.Close();
                       


                    }
                    con.Open();
                temp = temp + 1;
                string comText = ("insert into  public.chats (chat_name, id, admin) values('" + textBox1.Text+ "','"+temp+ "','" + GetNameID(Login)+"')");
                NpgsqlCommand cmd = new NpgsqlCommand(comText, con);
              
                cmd.ExecuteNonQuery();
                con.Close();
            }
            Grid f = new Grid(Login);
            this.Visible = false;
            f.ShowDialog();
        }
    }
}
