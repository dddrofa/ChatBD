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
    public partial class Grid : MaterialForm
    {
        public string choose;
        private DataSet ds = new DataSet();
        private DataTable dt = new DataTable();
        private static NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(@"Server=localhost;Port=5432;User Id=postgres;Password=kaluga4000;Database=Chat;");

        }
        private void NumberOfUsers()
        {
            long count;
            string strange;
            NpgsqlConnection con = GetConnection();
            
                con.Open();
                string comText = ("SELECT COUNT(*) FROM public.chats");
                NpgsqlCommand cmd = new NpgsqlCommand(comText, con);
            count = (long)cmd.ExecuteScalar();
          //  count = Int32.Parse(strange);
                cmd.ExecuteNonQuery();
                con.Close();
            long temp;
            using (NpgsqlConnection con1 = GetConnection())
            {
                for (int i = 1; i < count+1; i++)
                {
                    con1.Open();
                    string comText1 = ("select count(*) from (select id_user  from public.\"Messages\" where id_chat="+ i+" group by id_user ) foo ");
                    NpgsqlCommand cmd1 = new NpgsqlCommand(comText1, con1);
                    temp = (long)cmd1.ExecuteScalar();
                    cmd1.ExecuteNonQuery();
                    con1.Close();
                    NpgsqlConnection con2 = GetConnection();

                    con2.Open();
                    string comText2 = ("update  public.chats set number_of_members ="+ temp + "where chats.id=" + i);
                    NpgsqlCommand cmd2 = new NpgsqlCommand(comText2, con2);
                  //  count = (long)cmd2.ExecuteScalar();
                    cmd2.ExecuteNonQuery();
                    con2.Close();
                }
               
            }

        }
        public string Login;
        public Grid(string login)
        {
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            InitializeComponent();
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.ColumnHeadersVisible = false;
            NumberOfUsers();
            NpgsqlConnection con = GetConnection();
            Login = login;
            con.Open();
            string sql = ("SELECT chat_name, number_of_members FROM public.chats");

            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, con);
            ds.Reset();
            da.Fill(ds);
            dt = ds.Tables[0];
            dataGridView1.DataSource = dt;
            con.Close();


        }
        
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
            {
                 choose = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                int user_id;

                using (NpgsqlConnection con1 = GetConnection())
                {


                    con1.Open();
                    string comText1 = ("select id from public.\"users\" where login = '" + Login + "'");
                    NpgsqlCommand cmd1 = new NpgsqlCommand(comText1, con1);
                    user_id = (int)cmd1.ExecuteScalar();
                    cmd1.ExecuteNonQuery();
                    con1.Close();

                }
                int chat_id;
                using (NpgsqlConnection con1 = GetConnection())
                {

                    con1.Open();
                    string comText1 = ("select id from public.\"chats\" where chat_name = '" + choose + "'");
                    NpgsqlCommand cmd1 = new NpgsqlCommand(comText1, con1);
                    chat_id = (int)cmd1.ExecuteScalar();
                    cmd1.ExecuteNonQuery();
                    con1.Close();

                }
                long temp;
                NpgsqlConnection con2 = GetConnection();
               
                    con2.Open();
                    string comText2 = ("select count(id) from public.\"bans\" where id_user=" + user_id + "and id_chat = "+ chat_id);
                    NpgsqlCommand cmd2 = new NpgsqlCommand(comText2, con2);
                    temp = (long)cmd2.ExecuteScalar();
                    cmd2.ExecuteNonQuery();
                    con2.Close();

                if (temp>0)
                {
                    MessageBox.Show("Вы забанены в этом чате");
                }
                else
                {
                    ChatForm f = new ChatForm(Login, choose);
                    this.Visible = false;
                    f.ShowDialog();
                }
                
               

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Create f = new Create(Login);
            this.Visible = false;
            f.ShowDialog();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
