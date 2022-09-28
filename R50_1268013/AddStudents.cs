using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace R50_1268013
{
    public partial class AddStudents : Form
    {
        string filePath = "";
        List<Student> students = new List<Student>();
        public AddStudents()
        {
            InitializeComponent();
        }
        public ICrossDBDataSync FormToLoaded { get; set; }
       
        private int GetNewStudentId()
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelp.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(StudentId), 0) FROM Students", con))
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

                    using (SqlCommand cmd = new SqlCommand(@"INSERT INTO Students 
                                            (StudentId, StudentName, DateofBirth, IsContinuing, Phone,Email, Picture) VALUES
                                            (@i, @n, @d, @c, @p,@e, @pi)", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));
                        cmd.Parameters.AddWithValue("@n", textBox2.Text);
                        cmd.Parameters.AddWithValue("@d", dateTimePicker1.Value);
                        cmd.Parameters.AddWithValue("@c", checkBox1.Checked);
                        cmd.Parameters.AddWithValue("@p", textBox3.Text);
                        cmd.Parameters.AddWithValue("@e", textBox4.Text);                                    
                        string ext = Path.GetExtension(this.filePath);
                        string fileName = $"{Guid.NewGuid()}{ext}";
                        string savePath = Path.Combine(Path.GetFullPath(@"..\..\Pictures"), fileName);
                        File.Copy(filePath, savePath, true);
                        cmd.Parameters.AddWithValue("@pi", fileName);

                        try
                        {
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Data Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                students.Add(new Student
                                {
                                    StudentId = int.Parse(textBox1.Text),
                                    StudentName = textBox2.Text,
                                    DOB = dateTimePicker1.Value,
                                    IsContinuning = checkBox1.Checked,
                                    Phone = textBox3.Text,
                                    Email=textBox4.Text,
                                    Picture = fileName
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

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.filePath = this.openFileDialog1.FileName;
                this.label8.Text = Path.GetFileName(this.filePath);
                this.pictureBox1.Image = Image.FromFile(this.filePath);
            }
        }

        private void AddStudents_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.FormToLoaded.ReloadData(this.students);
        }

        private void AddStudents_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = this.GetNewStudentId().ToString();
        }
    }
    
}
