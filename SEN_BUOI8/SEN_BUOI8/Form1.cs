using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SEN_BUOI8
{
    public partial class Form1 : Form
    {
        private SchoolDBEntities context;
        private BindingSource studentBindingSource = new BindingSource();
        private BindingSource majorBindingSource = new BindingSource();
        private int currentIndex = 0;
        private bool isUpdating = false;
        public Form1()
        {
            InitializeComponent();
            context = new SchoolDBEntities();
            LoadData();
            LoadMajors();
            //UpdateControls();

        }
        private void LoadMajors()
        {
            // Lấy các giá trị ngành học duy nhất từ cột Major trong bảng Students
            var majors = context.Students
                .Select(s => s.Major)
                .Distinct()
                .ToList();

            // Gán danh sách ngành học vào ComboBox
            majorBindingSource.DataSource = majors;
            cmbMajor.DataSource = majorBindingSource;
        }
        private void LoadData()
        {
            var students = context.Students.ToList();
            studentBindingSource.DataSource = students;
            dataGridView1.DataSource = studentBindingSource;
            currentIndex = 0;
            UpdateControls();
        }
        private void UpdateControls()
        {
            if (studentBindingSource.Count > 0)
            {
                var student = (Students)studentBindingSource[currentIndex];
                txtFullName.Text = student.FullName;
                txtAge.Text = student.Age.ToString();
                cmbMajor.SelectedItem = student.Major;

                // Cập nhật trạng thái nút Previous và Next
                btnPrevious.Enabled = currentIndex > 0;
                btnNext.Enabled = currentIndex < studentBindingSource.Count - 1;

                // Cập nhật dòng được chọn trong DataGridView
                isUpdating = true; // Bắt đầu cập nhật
                dataGridView1.ClearSelection();
                dataGridView1.Rows[currentIndex].Selected = true;
                dataGridView1.CurrentCell = dataGridView1.Rows[currentIndex].Cells[0];
                isUpdating = false; // Kết thúc cập nhật
            }
        }
        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên sinh viên.");
                return false;
            }

            if (!int.TryParse(txtAge.Text, out int age) || age <= 0)
            {
                MessageBox.Show("Vui lòng nhập tuổi hợp lệ.");
                return false;
            }

            if (cmbMajor.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn ngành học.");
                return false;
            }

            return true;
        }
        private void btnThem_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                var student = new Students
                {
                    FullName = txtFullName.Text,
                    Age = int.Parse(txtAge.Text),
                    Major = cmbMajor.SelectedItem.ToString()
                };

                context.Students.Add(student);
                context.SaveChanges();

                // Cập nhật lại DataGridView
                LoadData();
                LoadMajors();
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                var student = (Students)dataGridView1.CurrentRow.DataBoundItem;
                context.Students.Remove(student);
                context.SaveChanges();

                // Cập nhật lại DataGridView
                LoadData();
                LoadMajors();
            }
        }

        private void btnSửa_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null && ValidateInput())
            {
                var student = (Students)dataGridView1.CurrentRow.DataBoundItem;
                student.FullName = txtFullName.Text;
                student.Age = int.Parse(txtAge.Text);
                student.Major = cmbMajor.SelectedItem.ToString();

                context.SaveChanges();

                // Cập nhật lại DataGridView
                LoadData();
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (!isUpdating) // Kiểm tra nếu không đang cập nhật
            {
                if (dataGridView1.CurrentRow != null)
                {
                    currentIndex = dataGridView1.CurrentRow.Index; // Cập nhật chỉ số
                    UpdateControls(); // Cập nhật các control
                }
            }
            
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (currentIndex > 0)
            {
                currentIndex--; // Giảm chỉ số
                UpdateControls(); // Cập nhật control và DataGridView
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (currentIndex < studentBindingSource.Count - 1)
            {
                currentIndex++; // Tăng chỉ số
                UpdateControls(); // Cập nhật control và DataGridView
            }
        }
    }
}
