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
    public partial class FormChangeOrders : Form
    {

        DataBase dataBase = new DataBase();
        private readonly CheckUser _user;

        enum RowState
        {
            Exited,
            New,
            Modifided,
            ModifidedNew,
            Deleted
        }

        int selectedRow;

        public FormChangeOrders(CheckUser user)
        {
            _user = user;
            InitializeComponent();
        }

        private void FormChangeOrders_Load(object sender, EventArgs e)
        {
            CreateColumns();
            RefreshDataGrid(dataGridView1);
            label2.Text = $"{_user.Login}:{_user.Status}";
        }

        private void CreateColumns()
        {
            dataGridView1.Columns.Add("id_orders", "ID заказа"); //0
            dataGridView1.Columns.Add("date_orders", "Дата заказа"); //1
            dataGridView1.Columns.Add("number_order", "Номер заказа"); //2
            dataGridView1.Columns.Add("name_product", "Название продукта"); //3
            dataGridView1.Columns.Add("summa_orders", "Сумма заказа"); //4
            dataGridView1.Columns.Add("summa_discount", "Скидка"); //5
            dataGridView1.Columns.Add("pickupllocation", "Пункт выдачи"); //6
            dataGridView1.Columns.Add("code_orders", "Код получения заказа"); //7
            dataGridView1.Columns.Add("isNew", String.Empty);
            dataGridView1.Columns["isNew"].Visible = false;
        }

        private void ReadSingleRow(DataGridView dgw, IDataRecord record)
        {
            dgw.Rows.Add(record.GetInt32(0), record.GetDateTime(1), record.GetInt64(2),record.GetString(3), record.GetInt64(4), record.GetInt64(5), record.GetString(6), record.GetInt64(7), RowState.ModifidedNew);
        }

        private void RefreshDataGrid(DataGridView dgw)
        {
            dgw.Rows.Clear();

            string querryString = $"select * from orders_tbl";

            NpgsqlCommand comm = new NpgsqlCommand(querryString, dataBase.GetConnection());

            dataBase.OpenConnecting();

            NpgsqlDataReader reader = comm.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow(dgw, reader);
            }
            reader.Close();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRow = e.RowIndex;

            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[selectedRow];

                textBox3.Text = row.Cells[0].Value.ToString();
                textBox1.Text = row.Cells[1].Value.ToString();
                textBox2.Text = row.Cells[2].Value.ToString();
                textBox4.Text = row.Cells[3].Value.ToString();
                textBox6.Text = row.Cells[4].Value.ToString();
                textBox5.Text = row.Cells[5].Value.ToString();
                textBox8.Text = row.Cells[6].Value.ToString();
                textBox7.Text = row.Cells[7].Value.ToString();
            }
        }

        private void Edit()
        {
            var selectedRowIndex = dataGridView1.CurrentCell.RowIndex;

            var id = textBox3.Text;
            var date_orders = textBox1.Text;
            var number_order = textBox2.Text;
            var nameProducts = textBox4.Text;
            var summa_orders = textBox6.Text;
            var summa_discount = textBox5.Text;
            var pickupllocation = textBox8.Text;
            var code_orders = textBox7.Text;

            if (dataGridView1.Rows[selectedRowIndex].Cells[7].Value.ToString() != string.Empty)
            {
                dataGridView1.Rows[selectedRowIndex].SetValues(id, date_orders, number_order,nameProducts, summa_orders, summa_discount, pickupllocation, code_orders);
                dataGridView1.Rows[selectedRowIndex].Cells[8].Value = RowState.Modifided;
            }

        }

        private void Update()
        {
            dataBase.OpenConnecting();

            for (int index = 0; index < dataGridView1.Rows.Count; index++)
            {
                var rowState = (RowState)dataGridView1.Rows[index].Cells[7].Value;

                if (rowState == RowState.Exited)
                    continue;
                if (rowState == RowState.Modifided)
                {
                    var id = dataGridView1.Rows[index].Cells[0].Value.ToString();
                    var nameProducts = dataGridView1.Rows[index].Cells[3].Value.ToString();
                    var pickupllocation = dataGridView1.Rows[index].Cells[6].Value.ToString();

                    var changeQuery = $"update orders_tbl set name_product = '{nameProducts}', pickupllocation = '{pickupllocation}' where id_orders = '{id}'";

                    var comm = new NpgsqlCommand(changeQuery, dataBase.GetConnection());
                    comm.ExecuteNonQuery();
                }
            }
            dataBase.CloseConnecting();
        }
                private void changeBT_Click(object sender, EventArgs e)
                {
                Edit();
                }

       

        private void button1_Click_1(object sender, EventArgs e)
        {
            FormProductsAdmin formProductsAdmin = new FormProductsAdmin(_user);
            formProductsAdmin.Show();
            Hide();
        }
    }
}
