using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R50_1268013
{
    public interface ICrossDBDataSync
    {
        void ReloadData(List<Student> students);
        void UpdateStudent(Student student);
        void RemoveStudent(int id);

        void ReloadSubject(List<Subject> students);
        void UpdateSubject(Subject student);
        void RemoveSubject(int id);
    }
}
