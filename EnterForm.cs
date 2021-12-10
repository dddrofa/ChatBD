using System;
using MaterialSkin;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin.Controls;
using Npgsql;
namespace Client
{

    
    public partial class EnterForm : MaterialForm
    {

      
        public EnterForm()
        {
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
           // materialSkinManager.ColorScheme = new ColorScheme(Primary.Blue400, Primary.Blue400, Primary.Blue400, Accent.LightBlue200, TextShade.WHITE);

        }

        private void EnterForm_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

            Register registerform = new Register();
            registerform.Show();

            this.Visible = false;

        }
        private static NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(@"Server=localhost;Port=5432;User Id=postgres;Password=kaluga4000;Database=Chat;");

        }
        private void button1_Click(object sender, EventArgs e)
        {
            using (NpgsqlConnection con = GetConnection())
            {
                string login = textBox1.Text;
                string password = textBox2.Text;
                con.Open();
                string comText = "SELECT * FROM public.users WHERE login = '" + login + "' and password= '" + password + "'";
              //  string comText = "select from public.users where login, password, admin_rules) values ('" + newlogin + "','" + newpassword + "','" + "0" + "')";
                NpgsqlCommand cmd = new NpgsqlCommand(comText, con);
                int val;
                NpgsqlDataReader reader = cmd.ExecuteReader();
                int chek = 0;
                while (reader.Read())
                {
                    val = Int32.Parse(reader[0].ToString());
                    if (val != 0)
                    {
                        
                       Grid grid = new Grid(login);
                        chek++;
                          grid.Show();
                        this.Visible = false;
                    }
                   
                }
                if (chek == 0)
                {
                    MessageBox.Show("Неверный логин или пароль!");
                }
               
                con.Close(); //close the current connection
            }
           
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }
    }
}
