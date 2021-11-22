using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace WorkRequest
{
    public partial class Form1 : Form
    {
        string connectionString = @"Data Source=HSPHMLTSQL2;Initial Catalog=HSPMelt;Persist Security Info=False;User ID=HSPMelt_rw_datauser;Password=HSPMelt_rw_datauser";
        string querySelect = null;
        string queryDropDownSelect = null;
        string queryInsert = null;
        string queryUpdate = null;
        DataTable data = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            GetData();
        }
        private void GetData()
        {
            querySelect = $"SELECT * FROM WorkRequests order by DateTime desc";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(querySelect, conn);

                data = new DataTable();
                adapter.Fill(data);

                dataGridView1.DataSource = data;
            }

            queryDropDownSelect = "SELECT DISTINCT Status FROM WorkRequests ORDER BY Status asc ";
            cbStatus.Items.Clear();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(queryDropDownSelect, conn);
                SqlDataReader DR = cmd.ExecuteReader();

                while (DR.Read())
                { cbStatus.Items.Add(DR[0]); }
            }

            queryDropDownSelect = "SELECT DISTINCT [Area] FROM WorkRequests ORDER BY Area asc ";
            cbArea.Items.Clear();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(queryDropDownSelect, conn);
                SqlDataReader DR = cmd.ExecuteReader();

                while (DR.Read())
                { cbArea.Items.Add(DR[0]); }
            }

            queryDropDownSelect = "SELECT DISTINCT [SubArea] FROM WorkRequests ORDER BY SubArea asc ";
            cbSubArea.Items.Clear();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(queryDropDownSelect, conn);
                SqlDataReader DR = cmd.ExecuteReader();

                while (DR.Read())
                { cbSubArea.Items.Add(DR[0]); }
            }

            queryDropDownSelect = "SELECT DISTINCT [Type] FROM WorkRequests ORDER BY Type asc ";
            cbType.Items.Clear();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(queryDropDownSelect, conn);
                SqlDataReader DR = cmd.ExecuteReader();

                while (DR.Read())
                { cbType.Items.Add(DR[0]); }
            }
        }

        private void Submit_Click(object sender, EventArgs e)
        {
            queryInsert = $"INSERT INTO WorkRequests (DateTime, Status, Area, SubArea, Type, Description)  " +
                   $"VALUES ('{DateTime.Now}', '{cbStatus.Text}', '{cbArea.Text}', '{cbSubArea.Text}', '{cbType.Text}', '{tbDescription.Text}');";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(queryInsert, conn);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

                GetData();
            }

        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            GetData();
        }

        private void Update_Click(object sender, EventArgs e)
        {
            queryUpdate = $"UPDATE WorkRequests " +
                $"SET Status = '{cbStatus.Text}', Area = '{cbArea.Text}', SubArea = '{cbSubArea.Text}', Type = '{cbType.Text}', Description = '{tbDescription.Text}'" +
                $"WHERE ID = {dataGridView1.SelectedRows[0].Cells[0].Value}";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(queryUpdate, conn);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

                GetData();
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                cbStatus.Text = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
                cbArea.Text = dataGridView1.SelectedRows[0].Cells[3].Value.ToString();
                cbSubArea.Text = dataGridView1.SelectedRows[0].Cells[4].Value.ToString();
                cbType.Text = dataGridView1.SelectedRows[0].Cells[5].Value.ToString();
                tbDescription.Text = dataGridView1.SelectedRows[0].Cells[6].Value.ToString();
            }
        }

        public void Email(string message, string ToRecipient)
        {
            System.Net.Mail.MailMessage Email = new System.Net.Mail.MailMessage();
            Email.From = new System.Net.Mail.MailAddress("barnesaa", "List update");
            Email.To.Add(ToRecipient);
            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient("Steelrelay.steel.net");
            Email.BodyEncoding = Encoding.ASCII;
            Email.Body = "test text.";//message;
            client.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
            client.Send(Email);
        }

        private void EmailSend_Click(object sender, EventArgs e)
        {
            Email($"{data}", $"{cbEmailTo.Text}");
        }
    }
}
