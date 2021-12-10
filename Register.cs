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
    public partial class Register : MaterialForm
    {
        public Register()
        {
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            InitializeComponent();
           
        }
        private static NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(@"Server=localhost;Port=5432;User Id=postgres;Password=kaluga4000;Database=Chat;");

        }
        private void button1_Click(object sender, EventArgs e)
        {
           
            using (NpgsqlConnection con = GetConnection())
            {
                int chek = 0;
                string newlogin = textBox1.Text;
                string newpassword = textBox2.Text;
                con.Open();
                string comText = "SELECT * FROM public.users WHERE login = '" + newlogin  + "'";
                //  string comText = "select from public.users where login, password, admin_rules) values ('" + newlogin + "','" + newpassword + "','" + "0" + "')";
                NpgsqlCommand cmd = new NpgsqlCommand(comText, con);
                int val;
                NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    val = Int32.Parse(reader[0].ToString());
                    if (val != 0)
                    {
                        if (chek==0)
                        {
                            MessageBox.Show("Логин занят!");
                            chek++;
                        }
                    }
                  
                }

                con.Close(); //close the current connection
                if (chek==0)
                {
                    using (NpgsqlConnection con1 = GetConnection())
                    {
                        
                        string comText2 = "insert into public.users (login, password, admin_rules) values ('" + newlogin + "','" + newpassword + "','" + "0" + "')";
                        NpgsqlCommand cmd1 = new NpgsqlCommand(comText2, con1);
                        con1.Open();
                        
                        cmd1.ExecuteNonQuery();
                    }
                    this.Visible = false;
                   Grid grid = new Grid(newlogin);
                    grid.Show();
                }
            }


        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
