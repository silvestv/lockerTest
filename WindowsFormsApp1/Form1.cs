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
        private String connectionString = "Data Source=LAPTOP-QNN1SCH4;Initial Catalog=AppSmartLockerdb1;Integrated Security = True";
        private String currentSelectedProc = "";
        private const string column_IsLocked = "ISLOCKED";
        private const string column_TimeAuth = "AUTHTIME";
        private const string column_TimeLock = "LOCKTIME";
        private const string column_NameProcess = "NAMEPROCESS";
        private const int second_by_day = 86400;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
            ////////////////CREATION DE LA LIST DES APPLICATIONS ACTIVES & INSERTION EN BDD
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
                        InsertDataApp(connectionString, p.ProcessName, p.MainWindowTitle, p.Id, p.MainModule.FileName, 0, second_by_day, 0);
                        Console.WriteLine("INSERTION REUSSI : " + p.ProcessName);
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

            /////////////////////////////////////////////////////////
            

        }


        /////////////////////////////////////////LES REQUETES EN BDD///////////////////////////////////////////
        ///////////LES INSERTS///////
        ///
        private void InsertDataApp(string connectionString, string nameProcess, string libelle, int pid, string chemin, int isLocked, int authTime, int lockTime)
        {
            // define INSERT query with parameters
            string query = "INSERT INTO dbo.APP (NAMEPROCESS, LIBELLE, PID, CHEMIN, ISLOCKED, AUTHTIME, LOCKTIME) " +
                           "VALUES (@nameProcess, @libelle, @pid, @chemin, @isLocked, @authTime, @lockTime) ";

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
                cmd.Parameters.Add("@lockTime", SqlDbType.Int).Value = lockTime;
                // open connection, execute INSERT, close connection
                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
            }
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
                // open connection, execute UPDATE, close connection
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
                // open connection, execute UPDATE, close connection
                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
            }
        }

        private void UpdateDataAppOnSaveRestriction(string connectionString, string nameProcess, int isLocked, int authTime, int lockTime)
        {
            string query = "UPDATE dbo.APP SET ISLOCKED = @isLocked, AUTHTIME = @authTime, LOCKTIME = @lockTime WHERE NAMEPROCESS = @nameProcess";
            // create connection and command
            using (SqlConnection cn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, cn))
            {
                // define parameters and their values
                cmd.Parameters.Add("@nameProcess", SqlDbType.VarChar, 50).Value = nameProcess;
                cmd.Parameters.Add("@isLocked", SqlDbType.Int).Value = isLocked;
                cmd.Parameters.Add("@authTime", SqlDbType.Int).Value = authTime;
                cmd.Parameters.Add("@lockTime", SqlDbType.Int).Value = lockTime;
                // open connection, execute UPDATE, close connection
                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
            }
            Console.WriteLine("Restriction Enregistrée");
        }

        /////LES SELECTS
        private int  SelectDataAppColumnInt(string connectionString, string nameProcess, string column)
        {
            int result = -1;
            string query = "SELECT "+column+" FROM dbo.APP WHERE NAMEPROCESS = @nameProcess";
            // create connection and command
            using (SqlConnection cn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, cn))
            {
                // define parameters and their values
                cmd.Parameters.Add("@nameProcess", SqlDbType.VarChar, 50).Value = nameProcess;
                // open connection, execute SELECT, close connection
                cn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                result = reader.GetInt32(0);
                cn.Close();
            }
            return result;
        }



        ///////////////////////////////////////FIN DES REQUETES/////////////////////////////////////////////

        //////////////////////////////////////FONCTIONS UTILES////////////////////////////////////////////
        private int HeureMinConvertInSeconde(int heure, int min)
        {
            return heure * 60 * 60 + min * 60;

        }

        private String SecondConvertInHeureMinSecond(int second)
        {
            return TimeSpan.FromSeconds(second).ToString();
        }


        //////////////////////////////////////LES EVENTS///////////////////////////////////////////////////
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.ValueMember != "")
            {
                int pid = int.Parse(listBox1.SelectedValue.ToString());
                String procName = Process.GetProcessById(pid).ProcessName;
                currentSelectedProc = procName;
                textBox1.Text = procName;
                textBox5.Text = pid.ToString();
                
                if(SelectDataAppColumnInt(connectionString, procName, column_IsLocked) == 0)
                {
                    textBox2.Text = "Unlocked process";
                    radioButtonUnlock.Checked = true;
                } else if (SelectDataAppColumnInt(connectionString, procName, column_IsLocked) == 1)
                {
                    textBox2.Text = "Lock Active on this process, there are a restriction on it !";
                    radioButtonLock.Checked = true;
                } else
                {
                    textBox2.Text = "Problem on locker status";
                }
                
                textBox3.Text = SecondConvertInHeureMinSecond(SelectDataAppColumnInt(connectionString, procName, column_TimeAuth));
                textBox4.Text = SecondConvertInHeureMinSecond(SelectDataAppColumnInt(connectionString, procName, column_TimeLock));

            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if(currentSelectedProc != "")
            {
                String statusWantedstr = "";
                int statusWanted = -1;
                int authTimeWanted = -1;
                int lockTimeWanted = -1;
                foreach (RadioButton rb in groupBox1.Controls)
                {
                    if (rb.Checked == true)
                    {
                        statusWantedstr += rb.Name;
                        if (statusWantedstr == "radioButtonUnlock")
                        {
                            statusWanted = 0;
                        }
                        else if (statusWantedstr == "radioButtonLock")
                        {
                            statusWanted = 1;
                        }
                    }
                }
                authTimeWanted = HeureMinConvertInSeconde((int)numericUpDown1.Value, (int)numericUpDown3.Value);
                lockTimeWanted = HeureMinConvertInSeconde((int)numericUpDown2.Value, (int)numericUpDown4.Value);
                Console.WriteLine(statusWanted);
                Console.WriteLine(authTimeWanted);
                Console.WriteLine(lockTimeWanted);
                UpdateDataAppOnSaveRestriction(connectionString, currentSelectedProc, statusWanted, authTimeWanted, lockTimeWanted);
                MessageBox.Show("Restriction enregistrée en BDD");
            } else
            {
                MessageBox.Show("Vous devez selectionner un processus pour pouvoir le bloquer !");
            }
            
        }

        private void bindingSource1_CurrentChanged(object sender, EventArgs e)
        {

        }

    }
}
