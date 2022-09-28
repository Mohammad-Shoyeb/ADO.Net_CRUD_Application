using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace R50_1268013
{
    public partial class EditSubjects : Form
    {
        string action = "Edit";
        Subject Subject;
        public EditSubjects()
        {
            InitializeComponent();
        }
        public int SubjectToEditDelete { get; set; }
        public ICrossDBDataSync FormLoaded { get; set; }
        private void EditSubjects_Load(object sender, EventArgs e)
        {
            ShowData();
        }
        private void ShowData()
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelp.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Subjects WHERE Subjectid =@i", con))
                {
                    cmd.Parameters.AddWithValue("@i", this.SubjectToEditDelete);
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        textBox1.Text = dr.GetInt32(0).ToString();
                        textBox2.Text = dr.GetString(1);
                        textBox3.Text = dr.GetInt32(2).ToString();
                        textBox4.Text = dr.GetInt32(3).ToString();

                    }
                    con.Close();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.action = "Edit";
            using (SqlConnection con = new SqlConnection(ConnectionHelp.ConnectionString))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {

                    using (SqlCommand cmd = new SqlCommand(@"UPDATE  Subjects  
                                            SET  Subjectname=@n, NumberOfStudents=@ns, StudentId=@s
                                            WHERE Subjectid=@i", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));
                        cmd.Parameters.AddWithValue("@n", textBox2.Text);
                        cmd.Parameters.AddWithValue("@ns", int.Parse(textBox3.Text));
                        cmd.Parameters.AddWithValue("@s", int.Parse(textBox3.Text));

                        try
                        {
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Data Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                Subject = new Subject
                                {
                                    SubjectId = int.Parse(textBox1.Text),
                                    SubjectName = textBox2.Text,
                                    NumberOfStudent = int.Parse(textBox3.Text),
                                    StudentId = int.Parse(textBox4.Text)

                                };
                                tran.Commit();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error: {ex.Message}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            tran.Rollback();
                        }
                        finally
                        {
                            if (con.State == ConnectionState.Open)
                            {
                                con.Close();
                            }
                        }

                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.action = "Delete";
            using (SqlConnection con = new SqlConnection(ConnectionHelp.ConnectionString))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {

                    using (SqlCommand cmd = new SqlCommand(@"DELETE  Subjects  
                                            WHERE Subjectid=@i", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));




                        try
                        {
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Data Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                tran.Commit();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error: {ex.Message}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            tran.Rollback();
                        }
                        finally
                        {
                            if (con.State == ConnectionState.Open)
                            {
                                con.Close();
                            }
                        }

                    }
                }

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ShowData();
        }

        private void EditSubjects_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.action == "edit")
                this.FormLoaded.UpdateSubject(Subject);
            else
                this.FormLoaded.RemoveSubject(Int32.Parse(this.textBox1.Text));
        }
    }
}
