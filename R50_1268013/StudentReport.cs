using R50_1268013.Reports;
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
    public partial class StudentReport : Form
    {
        public StudentReport()
        {
            InitializeComponent();
        }

        private void StudentReport_Load(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(ConnectionHelp.ConnectionString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Students", con))
                {
                    da.Fill(ds, "Studentsi");
                    ds.Tables["Studentsi"].Columns.Add(new DataColumn("image", typeof(System.Byte[])));
                    for (var i = 0; i < ds.Tables["Studentsi"].Rows.Count; i++)
                    {
                        ds.Tables["Studentsi"].Rows[i]["image"] = File.ReadAllBytes(Path.Combine(Path.GetFullPath(@"..\..\Pictures"), ds.Tables["Studentsi"].Rows[i]["Picture"].ToString()));
                    }
                    StudentsRpt rpt = new StudentsRpt();
                    rpt.SetDataSource(ds);
                    crystalReportViewer1.ReportSource = rpt;
                    rpt.Refresh();
                    crystalReportViewer1.Refresh();
                }
            }

        }
    }
}
