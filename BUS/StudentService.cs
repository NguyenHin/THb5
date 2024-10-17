using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BUS
{
    public class StudentService
    {
        public List<Student> GetAll()
        {
            Model1 context = new Model1();
            return context.Students.ToList();
        }
        public List<Student> GetAllHasNoMajor()
        {
            Model1 context = new Model1();
            return context.Students.Where(p => p.MajorID == null).ToList();
        }
        public List<Student> GetAllHasNoMajor(int facultyID)
        {
            Model1 context = new Model1();
            return context.Students.Where(p=>p.MajorID == null && p.FacultyID == facultyID).ToList();
        }
        public Student FindByID(string studentId)
        {
            Model1 context = new Model1();
            return context.Students.FirstOrDefault(p=>p.StudentID == studentId);
        }
        public void InsertUpdate(Student s)
        {
            Model1 context = new Model1();
            context.Students.AddOrUpdate(s);
            context.SaveChanges();
        }

        public void RegisterMajor(string studentId, int majorId)
        {
            using (Model1 context = new Model1())
            {
                var student = context.Students.FirstOrDefault(s => s.StudentID == studentId);
                if (student != null)
                {
                    student.MajorID = majorId; // Cập nhật chuyên ngành
                    context.SaveChanges(); // Lưu thay đổi vào CSDL
                }
                else
                {
                    throw new Exception("Không tìm thấy sinh viên.");
                }
            }
        }


    }
}
