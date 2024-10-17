using BUS;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThreeLayer
{
    public partial class frmRegister : Form
    {
        private readonly StudentService studentService = new StudentService();
        private readonly FacultyService facultyService = new FacultyService();
        private readonly MajorService majorService = new MajorService();
        private List<Student> students;
        public frmRegister()
        {
            InitializeComponent();
        }

        private void frmRegister_Load(object sender, EventArgs e)
        {
            try
            {
                var listFacuties = facultyService.GetAll();
                FillFacultyCombobox(listFacuties);
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void FillFacultyCombobox(List<Faculty> listFaculties)
        {
            this.cmbKhoa.DataSource = listFaculties;
            this.cmbKhoa.DisplayMember = "FacultyName";
            this.cmbKhoa.ValueMember = "FacultyID";
        }

        private void cmbKhoa_SelectedIndexChanged(object sender, EventArgs e)
        {
            Faculty selectedFaculty = cmbKhoa.SelectedItem as Faculty;
            if (selectedFaculty != null)
            {
                var listMajor = majorService.GetAllByFaculty(selectedFaculty.FacultyID);
                FillMajorCombobox(listMajor);
                var listStudents = studentService.GetAllHasNoMajor(selectedFaculty.FacultyID);
                
                BindGrid(listStudents);
            }
        }
        private void BindGrid(List<Student> listStudents)
        {
            dataGridView1.Rows.Clear();
            foreach (var item in listStudents)
            {
                int index = dataGridView1.Rows.Add();
                dataGridView1.Rows[index].Cells[0].Value = item.StudentID;
                dataGridView1.Rows[index].Cells[1].Value = item.FullName;
                if (item.Faculty != null)
                {
                    dataGridView1.Rows[index].Cells[2].Value = item.Faculty.FacultyName;
                }
                dataGridView1.Rows[index].Cells[3].Value = item.AverageScore + "";
                if (item.Major != null)
                    dataGridView1.Rows[index].Cells[4].Value = item.Major.Name + "";
            }
        }

        private void FillMajorCombobox(List<Major> listMajor)
        {
            this.cmbCN.DataSource = listMajor;
            this.cmbCN.DisplayMember = "Name";
            this.cmbCN.ValueMember = "MajorID";
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có sinh viên nào được chọn không
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một sinh viên để đăng ký chuyên ngành.");
                return;
            }

            // Lấy thông tin sinh viên đã chọn
            var selectedRow = dataGridView1.SelectedRows[0];
            string studentId = selectedRow.Cells[0].Value.ToString(); 
            int majorId = (int)cmbCN.SelectedValue;
            // Kiểm tra xem chuyên ngành đã được chọn chưa
            if (majorId == 0)
            {
                MessageBox.Show("Vui lòng chọn chuyên ngành.");
                return;
            }

            try
            {
                // Gọi để cập nhật chuyên ngành cho sinh viên
                studentService.RegisterMajor(studentId, majorId);
                MessageBox.Show("Đăng ký chuyên ngành thành công!");

                Faculty selectedFaculty = cmbKhoa.SelectedItem as Faculty;
                // cập nhật lại giao diện hoặc dữ liệu
                BindGrid(studentService.GetAllHasNoMajor(selectedFaculty.FacultyID)); // Cập nhật danh sách sinh viên chưa có chuyên ngành
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi đăng ký chuyên ngành: " + ex.Message);
            }
        }


    }
}
