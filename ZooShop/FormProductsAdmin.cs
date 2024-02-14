using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZooShop
{
    public partial class FormProductsAdmin : Form
    {
        private readonly CheckUser _user;
        DataBase dataBase = new DataBase();

        enum RowState
        {
            Existed,
            New,
            Modfied,
            ModifidedNew,
            Deleted,
            Exited
        }
        public FormProductsAdmin(CheckUser user)
        {
            InitializeComponent();
            addOrders.Visible = false;
            _user = user;
            StartPosition = FormStartPosition.CenterScreen;
        }


        private void IsAdmin()
        {
            addProducts.Enabled = _user.IsAdmin;
            button1.Enabled = _user.IsAdmin;
        }

        private void FormProductsAdmin_Load(object sender, EventArgs e)
        {
            label2.Text = $"{_user.Login}:{_user.Status}";
            CreateColumns();
            RefreshDataGrid(dataGridView1);
            IsAdmin();


        }

        private int clickedRowIndex = -1;

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                string imagePath = row.Cells[5].Value.ToString();

                if (!string.IsNullOrEmpty(imagePath))
                {

                    pictureBox1.ImageLocation = imagePath;
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                }
                else
                {
                    MessageBox.Show("Изображение не найдено");
                }
            }
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                // Сохраняем индекс строки, на которую нажал пользователь правой кнопкой мыши
                clickedRowIndex = e.RowIndex;

                // Показываем panel1
                addOrders.Visible = true;

            }
        }

        private void addOrders_Click(object sender, EventArgs e)
        {
            if (clickedRowIndex >= 0 && clickedRowIndex < dataGridView1.Rows.Count)
            {
                string name = dataGridView1.Rows[clickedRowIndex].Cells[0].Value.ToString();
                string description = dataGridView1.Rows[clickedRowIndex].Cells[1].Value.ToString();
                string manufacturer = dataGridView1.Rows[clickedRowIndex].Cells[2].Value.ToString();
                decimal price = Convert.ToDecimal(dataGridView1.Rows[clickedRowIndex].Cells[3].Value);
                decimal discount = Convert.ToDecimal(dataGridView1.Rows[clickedRowIndex].Cells[4].Value);


                NpgsqlConnection npgsqlConnection = new NpgsqlConnection("Server = localhost; port = 5432;Database = zooShop; User Id=postgres; Password = 123");

                FormOrdersAdmin formOrdersAdmin = new FormOrdersAdmin(description, name, manufacturer, price, discount, npgsqlConnection, _user);
                formOrdersAdmin.Show();
                this.Close();

            }
        }

        private void CreateColumns()
        {
          //  dataGridView1.Columns.Add("ID_products", "ID заказа"); //0
            dataGridView1.Columns.Add("name_product", "название товара"); //1
            dataGridView1.Columns.Add("description", "описание товара"); //2
            dataGridView1.Columns.Add("manufacturer", "производитель"); //3
            dataGridView1.Columns.Add("price", "цена");                //4
            dataGridView1.Columns.Add("diccount", "скидка");          //5
            dataGridView1.Columns.Add("photo_products", "путь изображения"); //6
            dataGridView1.Columns.Add("isNew", String.Empty); //7
            dataGridView1.Columns["isNew"].Visible = false;
        }

        private void RefreshDataGrid(DataGridView dataGrid)
        {
            dataGrid.Rows.Clear();
            string queryString = $"select name_product,description,manufacturer,price,diccount,photo_products from product_tbl";

            NpgsqlCommand command = new NpgsqlCommand(queryString, dataBase.GetConnection());

            dataBase.OpenConnecting();

            NpgsqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow(dataGrid, reader);
            }
            reader.Close();
        }

        private void ReadSingleRow(DataGridView gridView, IDataRecord record)
        {
            gridView.Rows.Add(record.GetString(0), record.GetString(1), record.GetString(2), record.GetInt64(3), record.GetInt64(4), record.GetString(5), RowState.ModifidedNew);
        }

        private void changeOrders_Click(object sender, EventArgs e)
        {
            FormChangeOrders formChangeOrders = new FormChangeOrders(_user);
            formChangeOrders.Show();
            this.Close();
        }

        private void addProducts_Click(object sender, EventArgs e)
        {
            FormAddProducts formAddProducts = new FormAddProducts(_user);
            formAddProducts.Show();
            this.Close();
        }

        private void deleteRow()
        {
            int index = dataGridView1.CurrentCell.RowIndex;

            dataGridView1.Rows[index].Visible = false;

            if (dataGridView1.Rows[index].Cells[0].Value.ToString() == string.Empty)
            {
                dataGridView1.Rows[index].Cells[6].Value = RowState.Deleted;
                return;
            }
            dataGridView1.Rows[index].Cells[6].Value = RowState.Deleted;
        }

        //private void Update()
        //{
        //    dataBase.OpenConnecting();

        //    for (int index = 0; index < dataGridView1.Rows.Count; index++)
        //    {
        //        var rowState = (RowState)dataGridView1.Rows[index].Cells[7].Value;

        //        //if (rowState == RowState.Exited)
        //        //    continue;

        //        if (rowState == RowState.Deleted)
        //        {
        //            var id = Convert.ToInt32(dataGridView1.Rows[index].Cells[0].Value);
        //            var deleteQuery = $"DELETE FROM product_tbl WHERE ID_products = {id}";

        //            NpgsqlCommand command = new NpgsqlCommand(deleteQuery, dataBase.GetConnection());
        //            command.ExecuteNonQuery();

        //        }
        //    }
        //    dataBase.CloseConnecting();
        //}

        private void button1_Click(object sender, EventArgs e)
        {
            deleteRow();
        }


    }
}
