using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R50_1268013
{
    public static class ConnectionHelp
    {
        public static string ConnectionString
        {
            get
            {
                string databPath = Path.Combine(Path.GetFullPath(@"..\..\"), "StudentSubject.mdf");
                return $@"Data Source=(localdb)\mssqllocaldb;AttachDbFilename={databPath};Initial Catalog=StudentSubject;Trusted_Connection=True";
            }
        }
    }
}
