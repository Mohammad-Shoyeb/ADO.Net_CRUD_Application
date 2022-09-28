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
    
    public partial class EditStudents : Form
    {
        string filePath, oldFile, fileName;
        string action = "Edit";
        Student student;
        public EditStudents()
        {
            InitializeComponent();
        }
        public int StudentsEditDelete { get; set; }
        public ICrossDBDataSync FormLoaded { get; set; }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.filePath = this.openFileDialog1.FileName;
                this.label8.Text = Path.GetFileName(this.filePath);
                this.pictureBox1.Image = Image.FromFile(this.filePath);
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

                    using (SqlCommand cmd = new SqlCommand(@"UPDATE  Students  
                                            SET  StudentName=@n, DateofBirth=@d, IsContinuing= @c, Phone=@p, Email=@e, Picture=@pi 
                                            WHERE StudentId=@i", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));
                        cmd.Parameters.AddWithValue("@n", textBox2.Text);
                        cmd.Parameters.AddWithValue("@d", dateTimePicker1.Value);
                        cmd.Parameters.AddWithValue("@c", checkBox1.Checked);
                        cmd.Parameters.AddWithValue("@p", textBox3.Text);
                        cmd.Parameters.AddWithValue("@e", textBox4.Text);
                        if (!string.IsNullOrEmpty(this.filePath))
                        {
                            string ext = Path.GetExtension(this.filePath);
                            fileName = $"{Guid.NewGuid()}{ext}";
                            string savePath = Path.Combine(Path.GetFullPath(@"..\..\Pictures"), fileName);
                            File.Copy(filePath, savePath, true);
                            cmd.Parameters.AddWithValue("@pi", fileName);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@pi", oldFile);
                        }


                        try
                        {
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Data Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                student = new Student
                                {
                                    StudentId = int.Parse(textBox1.Text),
                                    StudentName = textBox2.Text,
                                    DOB = dateTimePicker1.Value,
                                    IsContinuning = checkBox1.Checked,
                                    Phone = textBox3.Text,
                                    Email = textBox4.Text,
                                    Picture = filePath == "" ? oldFile : fileName
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

        private void button3_Click(object sender, EventArgs e)
        {
            ShowData();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.action = "Delete";
            using (SqlConnection con = new SqlConnection(ConnectionHelp.ConnectionString))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {

                    using (SqlCommand cmd = new SqlCommand(@"DELETE  Students  
                                            WHERE StudentId=@i", con, tran))
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

        private void EditStudents_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.action == "edit")
                this.FormLoaded.UpdateStudent(student);
            else
                this.FormLoaded.RemoveStudent(Int32.Parse(this.textBox1.Text));
        }

        private void EditStudents_Load_1(object sender, EventArgs e)
        {
            ShowData();
        }
        private void ShowData()
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelp.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Students WHERE StudentId =@i", con))
                {
                    cmd.Parameters.AddWithValue("@i", this.StudentsEditDelete);
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        textBox1.Text = dr.GetInt32(0).ToString();
                        textBox2.Text = dr.GetString(1);
                        dateTimePicker1.Value = dr.GetDateTime(2);
                        checkBox1.Checked = dr.GetBoolean(3);
                        textBox3.Text = dr.GetString(4);
                        textBox4.Text = dr.GetString(5);                
                        oldFile = dr.GetString(6).ToString();
                        pictureBox1.Image = Image.FromFile(Path.Combine(@"..\..\Pictures", dr.GetString(6).ToString()));
                    }
                    con.Close();
                }
            }

        }
    }
}
