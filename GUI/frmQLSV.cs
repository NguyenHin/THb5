using BUS;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThreeLayer
{
    public partial class frmQLSV : Form
    {
        private readonly StudentService studentService = new StudentService();
        private readonly FacultyService facultyService = new FacultyService();
       
        public frmQLSV()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                setGridViewStyle(dataGridView1);
                var listFaculties = facultyService.GetAll();
                var listStudents = studentService.GetAll();
                FillFacultyCombobox(listFaculties);
                BindGrid(listStudents);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FillFacultyCombobox(List<Faculty> listFaculties)
        {
            listFaculties.Insert(0, new Faculty());
            this.comboBox1.DataSource = listFaculties;
            this.comboBox1.DisplayMember = "FacultyName";
            this.comboBox1.ValueMember = "FacultyID";
        }

        private void BindGrid(List<Student> listStudents) {
            dataGridView1.Rows.Clear();
            foreach (var item in listStudents)
            {
                int index = dataGridView1.Rows.Add();
                dataGridView1.Rows[index].Cells[0].Value = item.StudentID;
                dataGridView1.Rows[index].Cells[1].Value = item.FullName;
                if (item.Faculty != null)
                    dataGridView1.Rows[index].Cells[2].Value = item.Faculty.FacultyName;
                dataGridView1.Rows[index].Cells[3].Value = item.AverageScore + "";
                if (item.MajorID != null)
                    dataGridView1.Rows[index].Cells[4].Value = item.Major.Name + "";
                ShowAvatar(item.Avatar);
            }
        }

        private void ShowAvatar(string ImageName) {
            if (string.IsNullOrEmpty(ImageName))
                picAvatar.Image = null;
            else
            {
                string parentDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                string imagePath = Path.Combine(parentDirectory, "Images", ImageName);
                picAvatar.Image = Image.FromFile(imagePath);
                picAvatar.Refresh();
            }
        }
        public void setGridViewStyle(DataGridView dgv)
        {
            dgv.BorderStyle = BorderStyle.None;
            dgv.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.BackgroundColor = Color.White;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            var listStudents = new List<Student>();
            if (this.checkBox1.Checked)
                listStudents = studentService.GetAllHasNoMajor();
            else
                listStudents = studentService.GetAll();
            BindGrid(listStudents);
        }

        private void btnPic_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All Files|*.*";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                picAvatar.Image = Image.FromFile(dlg.FileName);
                string studentID = txtMSSV.Text;
                string filePath = dlg.FileName;

                // Đường dẫn tới thư mục "Images"
                string parentDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                string imagesDirectory = Path.Combine(parentDirectory, "Images");

                // Lấy phần mở rộng của file
                string extension = Path.GetExtension(filePath);
                string imageName = $"{studentID}{extension}"; // Tên file mới
                string destinationPath = Path.Combine(imagesDirectory, imageName);

                // Lưu hình ảnh vào thư mục
                File.Copy(filePath, destinationPath, true);
            }
        }


        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Kiểm tra thông tin bắt buộc
            if (string.IsNullOrWhiteSpace(txtMSSV.Text) || string.IsNullOrWhiteSpace(txtTen.Text) || string.IsNullOrWhiteSpace(txtDTB.Text) || comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }

            // Kiểm tra mã số sinh viên
            if (txtMSSV.Text.Length != 10)
            {
                MessageBox.Show("Mã số sinh viên phải có 10 kí tự!");
                return;
            }

            // Kiểm tra và tạo đối tượng sinh viên
            Student newStudent = new Student
            {
                StudentID = txtMSSV.Text,
                FullName = txtTen.Text,
                AverageScore = float.Parse(txtDTB.Text),
                FacultyID = (int)comboBox1.SelectedValue
            };

            try
            {
                using (Model1 context = new Model1())
                {
                    // Thêm sinh viên vào CSDL
                    context.Students.Add(newStudent);
                    context.SaveChanges(); // Lưu thay đổi vào CSDL
                }

                // Tải lại dữ liệu
                LoadData();
                ResetForm(); // Đặt lại dữ liệu về giá trị ban đầu
                MessageBox.Show("Thêm mới dữ liệu thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm dữ liệu: " + ex.Message);
            }

        }

        private void ResetForm()
        {
            txtMSSV.Clear();
            txtTen.Clear();
            txtDTB.Clear();
            comboBox1.SelectedIndex = -1; //thiết lập lại về giá trị mặc định
        }

        private void LoadData()
        {
            try
            {
                Model1 context = new Model1();
                List<Faculty> listFaculties = context.Faculties.ToList(); //lay cac khoa
                List<Student> listStudents = context.Students.ToList(); //lay sv
                FillFacultyCombobox(listFaculties);
                BindGrid(listStudents);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            // Kiểm tra thông tin mã số sinh viên
            if (string.IsNullOrWhiteSpace(txtMSSV.Text))
            {
                MessageBox.Show("Vui lòng nhập mã số sinh viên cần xóa!");
                return;
            }

            using (Model1 context = new Model1())
            {
                // Tìm sinh viên theo mã số
                var studentToDelete = context.Students.FirstOrDefault(s => s.StudentID == txtMSSV.Text);
                if (studentToDelete == null)
                {
                    MessageBox.Show("Không tìm thấy MSSV cần xóa!");
                    return;
                }

                // Hiện cảnh báo xác nhận
                var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn xóa sinh viên này không?",
                                                     "Xác nhận xóa!",
                                                     MessageBoxButtons.YesNo,
                                                     MessageBoxIcon.Warning);

                if (confirmResult == DialogResult.Yes)
                {
                    try
                    {
                        // Xóa sinh viên
                        context.Students.Remove(studentToDelete);
                        context.SaveChanges(); // Lưu thay đổi vào CSDL

                        LoadData();
                        ResetForm();
                        MessageBox.Show("Xóa sinh viên thành công!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi xóa dữ liệu: " + ex.Message);
                    }
                }
            }
        }

        

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
            int selectrow = e.RowIndex;
            txtMSSV.Text = dataGridView1.Rows[selectrow].Cells["Column1"].Value.ToString();
            txtTen.Text = dataGridView1.Rows[selectrow].Cells["Column2"].Value.ToString();
            comboBox1.Text = dataGridView1.Rows[selectrow].Cells["Column3"].Value.ToString();
            txtDTB.Text = dataGridView1.Rows[selectrow].Cells["Column4"].Value.ToString();


            string studentId = txtMSSV.Text; // Hoặc tên cột khác chứa định danh
            string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" }; // Các định dạng hỗ trợ
            string imageName = studentId; // Bắt đầu với tên sinh viên

            // Kiểm tra các định dạng hình ảnh
            foreach (string extension in imageExtensions)
            {
                string imagePath = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName, "Images", imageName + extension);
                if (File.Exists(imagePath))
                {
                    ShowAvatar(imageName + extension);
                    return; // Nếu tìm thấy, thoát khỏi vòng lặp
                }
            }

            // Nếu không tìm thấy hình ảnh, gọi ShowAvatar với null
            ShowAvatar(null);
        }

        private void dangKyChuyenNganhToolStripMenuItem_Click(object sender, EventArgs e)
        {           
            frmRegister frmregister = new frmRegister();
            frmregister.ShowDialog();
        }
    }
}
