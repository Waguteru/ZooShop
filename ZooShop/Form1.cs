using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ZooShop
{
    public partial class FormProducts : Form
    {
        DataBase dataBase = new DataBase();

        enum RowState
        {
            Existed,
            New,
            Modfied,
            ModfiedNew,
            Deleted
        }

        public FormProducts()
        {
            InitializeComponent();
            addOrders.Visible = false;
        }

        private void CreateColumns()
        {
            dataGridView1.Columns.Add("name_product", "название товара"); //0
            dataGridView1.Columns.Add("description", "описание товара"); //1
            dataGridView1.Columns.Add("manufacturer", "производитель"); //2
            dataGridView1.Columns.Add("price", "цена");                //3
            dataGridView1.Columns.Add("diccount", "скидка");          //4
            dataGridView1.Columns.Add("photo_products", "путь изображения"); //5

        }

        private void RefreshDataGrid(DataGridView dataGrid)
        {
            dataGrid.Rows.Clear();
            string queryString = $"select name_product,description,manufacturer,price,diccount,photo_products from product_tbl";

            NpgsqlCommand command = new NpgsqlCommand(queryString,dataBase.GetConnection());

            dataBase.OpenConnecting();

            NpgsqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow(dataGrid, reader);
            }
            reader.Close();
        }

        private void ReadSingleRow(DataGridView gridView,IDataRecord record)
        {
            gridView.Rows.Add(record.GetString(0), record.GetString(1), record.GetString(2), record.GetInt64(3), record.GetInt64(4), record.GetString(5),RowState.ModfiedNew);
        }

        private void FormProducts_Load(object sender, EventArgs e)
        {
            CreateColumns();
            RefreshDataGrid(dataGridView1);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                string imagePath = row.Cells[5].Value.ToString();

                if(!string.IsNullOrEmpty(imagePath) )
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

        private int clickedRowIndex = -1;

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

                FormOrders formOrders = new FormOrders(description, name, manufacturer, price, discount, npgsqlConnection);
                formOrders.Show();
                this.Close();
              
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

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
