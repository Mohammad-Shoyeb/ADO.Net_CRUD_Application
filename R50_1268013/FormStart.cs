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
    public partial class FormStart : Form,ICrossDBDataSync
    {
        DataSet ds;
        BindingSource tsStudents = new BindingSource();
        BindingSource tsSubjects = new BindingSource();

        public object Sub { get; private set; }

        public FormStart()
        {
            InitializeComponent();
        }

        private void FormStart_Load(object sender, EventArgs e)
        {
            this.dataGridView1.AutoGenerateColumns = false;
            LoadData();
            BindData();
        }
        public void LoadData()
        {
            ds = new DataSet();
            using (SqlConnection con = new SqlConnection(ConnectionHelp.ConnectionString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Students", con))
                {
                    da.Fill(ds, "Students");
                    ds.Tables["Students"].Columns.Add(new DataColumn("image", typeof(System.Byte[])));
                    for (var i = 0; i < ds.Tables["Students"].Rows.Count; i++)
                    {
                        ds.Tables["Students"].Rows[i]["image"] = File.ReadAllBytes(Path.Combine(Path.GetFullPath(@"..\..\Pictures"), ds.Tables["Students"].Rows[i]["Picture"].ToString()));
                    }
                    da.SelectCommand.CommandText = "SELECT * FROM Subjects";
                    da.Fill(ds, "Subjects");
                    DataRelation rel = new DataRelation("FK_BOOK_TOC",
                        ds.Tables["Students"].Columns["StudentId"],
                        ds.Tables["Subjects"].Columns["StudentId"]);
                    ds.Relations.Add(rel);
                    ds.AcceptChanges();
                }
            }
        }
        private void BindData()
        {
            tsStudents.DataSource = ds;
            tsStudents.DataMember = "Students";
            tsSubjects.DataSource = tsStudents;
            tsSubjects.DataMember = "FK_BOOK_TOC";
            this.dataGridView1.DataSource = tsSubjects;
            lblName.DataBindings.Add(new Binding("Text", tsStudents, "StudentName"));
            lblDOB.DataBindings.Add(new Binding("Text", tsStudents, "DateofBirth"));
            lblPhone.DataBindings.Add(new Binding("Text", tsStudents, "Phone"));
            checkBox1.DataBindings.Add(new Binding("Checked", tsStudents, "IsContinuing"));
            lblEmail.DataBindings.Add(new Binding("Text", tsStudents, "Email"));
            pictureBox1.DataBindings.Add(new Binding("Image", tsStudents, "image", true));
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AddStudents() { FormToLoaded = this }.ShowDialog();
        }

        public void ReloadData(List<Student> students)
        {
            foreach (var s in students)
            {
                DataRow dr = ds.Tables["Students"].NewRow();
                dr[0] = s.StudentId;
                dr["StudentName"] = s.StudentName;
                dr["DateofBirth"] = s.DOB;
                dr["IsContinuing"] = s.IsContinuning;
                dr["Phone"] = s.Phone;
                dr["Email"] = s.Email;
                dr["Picture"] = s.Picture;
                dr["image"] = File.ReadAllBytes(Path.Combine(Path.GetFullPath(@"..\..\Pictures"), s.Picture));
                ds.Tables["Students"].Rows.Add(dr);

            }
            ds.AcceptChanges();
            tsStudents.MoveLast();
        }

        public void UpdateStudent(Student student)
        {
            for (var i = 0; i < ds.Tables["Students"].Rows.Count; i++)
            {
                if ((int)ds.Tables["Students"].Rows[i]["StudentId"] == student.StudentId)
                {
                    ds.Tables["Students"].Rows[i]["StudentName"] = student.StudentName;
                    ds.Tables["Students"].Rows[i]["DateofBirth"] = student.DOB;
                    ds.Tables["Students"].Rows[i]["IsContinuing"] = student.IsContinuning;
                    ds.Tables["Students"].Rows[i]["Phone"] = student.Phone;
                    ds.Tables["Students"].Rows[i]["Email"] = student.Email;
                    ds.Tables["Students"].Rows[i]["image"] = File.ReadAllBytes(Path.Combine(Path.GetFullPath(@"..\..\Pictures"), student.Picture));
                    break;
                }
            }
            ds.AcceptChanges();
        }

        public void RemoveStudent(int id)
        {
            for (var i = 0; i < ds.Tables["Students"].Rows.Count; i++)
            {
                if ((int)ds.Tables["Students"].Rows[i]["StudentId"] == id)
                {
                    ds.Tables["Students"].Rows.RemoveAt(i);
                    break;
                }
            }
            ds.AcceptChanges();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (tsStudents.Position < tsStudents.Count - 1)
            {
                tsStudents.MoveNext();
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (tsStudents.Position > 0)
            {
                tsStudents.MovePrevious();
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            tsStudents.MoveLast();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            tsStudents.MoveFirst();
        }

        private void editDeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int id = (int)(this.tsStudents.Current as DataRowView).Row[0];
            new EditStudents { StudentsEditDelete = id, FormLoaded = this }.ShowDialog();
        }

        public void ReloadSubject(List<Subject> Subject)
        {
            foreach(var s in Subject)
            {
                DataRow dr = ds.Tables["Subjects"].NewRow();
                dr[0] = s.SubjectId;
                dr["SubjectName"] = s.SubjectName;
                dr["NumberOfStudents"] = s.NumberOfStudent;
                dr["StudentId"] = s.StudentId;
                ds.Tables["Subjects"].Rows.Add(dr);
            }
            ds.AcceptChanges();
            tsSubjects.MoveLast();
        }

        public void UpdateSubject(Subject subject)
        {
            for (var i = 0; i < ds.Tables["Subjects"].Rows.Count; i++)
            {
                if ((int)ds.Tables["Subjects"].Rows[i]["SubjectId"] ==subject.SubjectId )
                {
                    ds.Tables["Subjects"].Rows[i]["SubjectName"] = subject.SubjectName;
                    ds.Tables["Subjects"].Rows[i]["NumberOfStudents"] = subject.NumberOfStudent;
                    break;
                }
            }
            ds.AcceptChanges();
        }

        public void RemoveSubject(int id)
        {
            for (var i = 0; i < ds.Tables["Subjects"].Rows.Count; i++)
            {
                if ((int)ds.Tables["Subjects"].Rows[i]["SubjectId"] == id)
                {
                    ds.Tables["Subjects"].Rows.RemoveAt(i);
                    break;
                }
            }
            ds.AcceptChanges();
        }

        private void addToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new AddSubjects() { FormToLoaded = this }.ShowDialog();
        }

        private void editDeleteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            int id = (int)(this.tsSubjects.Current as DataRowView).Row[0];
            new EditSubjects { SubjectToEditDelete = id, FormLoaded = this }.ShowDialog();
        }

        private void studentsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new StudentReport().ShowDialog();
        }

        private void subjectsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new StudentGroupReport().ShowDialog();
        }
    }
}
