using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
            ////////////////CREATION DE LA LIST DES APPLICATIONS ACTIVES & INSERTION EN BDD
            String connectionString = "Data Source=LAPTOP-QNN1SCH4;Initial Catalog=AppSmartLockerdb1;Integrated Security = True";
            DataTable dt = new DataTable();
            dt.Columns.Add("ProcessName");
            dt.Columns.Add("ProcessId");
            foreach (Process p in Process.GetProcesses("."))
            {
                try
                {
                    if (p.MainWindowTitle.Length > 0)
                    {
                        dt.Rows.Add();
                        dt.Rows[dt.Rows.Count - 1][0] = p.MainWindowTitle;
                        dt.Rows[dt.Rows.Count - 1][1] = p.Id.ToString();
                        InsertDataApp(connectionString, p.ProcessName, p.MainWindowTitle, p.Id, p.MainModule.FileName, 0, 24);
                        Console.WriteLine(p.ProcessName);
                    }
                }
                catch (Exception err) 
                { 
                    Console.WriteLine(err.Message+"Nous allons plutôt update le PID du processus deja présent en BDD");
                    UpdateDataAppPID(connectionString, p.ProcessName, p.Id);
                    Console.WriteLine("MISE A JOUR REUSSI !");
                }
            }

            listBox1.DataSource = dt;
            listBox1.DisplayMember = "ProcessName";
            listBox1.ValueMember = "ProcessId";
            Console.WriteLine("PUUUUUUUTE3");

            /////////////////////////////////////////////////////////
            

        }


        /////////////////////////////////////////LES REQUETES EN BDD///////////////////////////////////////////
        ///////////LES INSERTS///////
        ///
        private void InsertDataApp(string connectionString, string nameProcess, string libelle, int pid, string chemin, int isLocked, int authTime)
        {
            // define INSERT query with parameters
            string query = "INSERT INTO dbo.APP (NAMEPROCESS, LIBELLE, PID, CHEMIN, ISLOCKED, AUTHTIME) " +
                           "VALUES (@nameProcess, @libelle, @pid, @chemin, @isLocked, @authTime) ";

            // create connection and command
            using (SqlConnection cn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, cn))
            {
                // define parameters and their values
                cmd.Parameters.Add("@nameProcess", SqlDbType.VarChar, 50).Value = nameProcess;
                cmd.Parameters.Add("@libelle", SqlDbType.VarChar, 50).Value = libelle;
                cmd.Parameters.Add("@pid", SqlDbType.Int).Value = pid;
                cmd.Parameters.Add("@chemin", SqlDbType.VarChar, 50).Value = chemin;
                cmd.Parameters.Add("@isLocked", SqlDbType.Int).Value = isLocked;
                cmd.Parameters.Add("@authTime", SqlDbType.Int).Value = authTime;
                // open connection, execute INSERT, close connection
                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
            }
            Console.WriteLine("ET OUAI PUTE");
        }

        /////LES UPDATES
        private void UpdateDataAppPID(string connectionString, string nameProcess,int pid)
        {
            string query = "UPDATE dbo.APP SET PID = @pid WHERE NAMEPROCESS = @nameProcess";
            // create connection and command
            using (SqlConnection cn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, cn))
            {
                // define parameters and their values
                cmd.Parameters.Add("@pid", SqlDbType.Int).Value = pid;
                cmd.Parameters.Add("@nameProcess", SqlDbType.VarChar, 50).Value = nameProcess;
                // open connection, execute INSERT, close connection
                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
            }
        }

        private void UpdateDataAppTimeAuth(string connectionString, string nameProcess, int timeAuth)
        {
            string query = "UPDATE dbo.APP SET TIMEAUTH = @timeAuth WHERE NAMEPROCESS = @nameProcess";
            // create connection and command
            using (SqlConnection cn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, cn))
            {
                // define parameters and their values
                cmd.Parameters.Add("@timeAuth", SqlDbType.Int).Value = timeAuth;
                cmd.Parameters.Add("@nameProcess", SqlDbType.VarChar, 50).Value = nameProcess;
                // open connection, execute INSERT, close connection
                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
            }
        }

        private void UpdateDataAppIsLocked(string connectionString, string nameProcess, int isLocked)
        {
            string query = "UPDATE dbo.APP SET ISLOCKED = @isLocked WHERE NAMEPROCESS = @nameProcess";
            // create connection and command
            using (SqlConnection cn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, cn))
            {
                // define parameters and their values
                cmd.Parameters.Add("@isLocked", SqlDbType.Int).Value = isLocked;
                cmd.Parameters.Add("@nameProcess", SqlDbType.VarChar, 50).Value = nameProcess;
                // open connection, execute INSERT, close connection
                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
            }
        }

        ///////////////////////////////////////FIN DES REQUETES/////////////////////////////////////////////

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.ValueMember != "")
            {
                int pid = int.Parse(listBox1.SelectedValue.ToString());
                String procName = Process.GetProcessById(pid).ProcessName;
                textBox1.Text = procName;
            }
        }

        private void bindingSource1_CurrentChanged(object sender, EventArgs e)
        {

        }
    }
}
