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
    public partial class AddSubjects : Form
    {
        List<Subject> Subject = new List<Subject>();
        public AddSubjects()
        {
            InitializeComponent();
        }
        public ICrossDBDataSync FormToLoaded { get; set; }

        private void AddSubjects_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = this.GetNewSubjectId().ToString();
        }
            
        private int GetNewSubjectId()
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelp.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(SubjectId), 0) FROM Subjects", con))
                {
                    con.Open();
                    int id = (int)cmd.ExecuteScalar();
                    con.Close();
                    return id + 1;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelp.ConnectionString))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {

                    using (SqlCommand cmd = new SqlCommand(@"INSERT INTO Subjects
                                            (Subjectid, Subjectname, NumberOfStudents,StudentId) VALUES
                                            (@i, @n, @ns,@s)", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));
                        cmd.Parameters.AddWithValue("@n", textBox2.Text);
                        cmd.Parameters.AddWithValue("@ns", int.Parse(textBox3.Text));
                        cmd.Parameters.AddWithValue("@s", int.Parse(textBox4.Text));

                        try
                        {
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Data Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                Subject.Add(new Subject
                                {
                                    SubjectId = int.Parse(textBox1.Text),
                                    SubjectName = textBox2.Text,
                                    NumberOfStudent = int.Parse(textBox3.Text),
                                    StudentId = int.Parse(textBox4.Text),

                                }); ;
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

        private void AddSubjects_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.FormToLoaded.ReloadSubject(this.Subject);
        }
    }
}
