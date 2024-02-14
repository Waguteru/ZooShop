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
    public partial class FormAddProducts : Form
    {

        DataBase dataBase = new DataBase();

        private readonly CheckUser _user;

        public FormAddProducts(CheckUser user)
        {
            _user = user;
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FormProductsAdmin formProductsAdmin = new FormProductsAdmin(_user);
            formProductsAdmin.Show();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        { 
            dataBase.OpenConnecting();

            var name_product = name_products_BT.Text;
            var description = description_BT.Text;
            var manufacturer = manufacturer_BT.Text;
            var price = price_BT.Text;
            var diccount = diccount_BT.Text;
            var photo_products = photo_products_BT.Text;

            string query = $"insert into product_tbl (name_product,description,manufacturer,price,diccount,photo_products) values ('{name_product}','{description}','{manufacturer}','{price}','{diccount}','{photo_products}')";
        
            NpgsqlCommand npgsqlCommand = new NpgsqlCommand(query, dataBase.GetConnection());
            npgsqlCommand.ExecuteNonQuery();

            MessageBox.Show("данные успешно добавлены!", "Успех!", MessageBoxButtons.OK, MessageBoxIcon.Information);

            dataBase.CloseConnecting();

            FormProductsAdmin formProducts = new FormProductsAdmin(_user);
            formProducts.Show();
            this.Close();
        }

        private void FormAddProducts_Load(object sender, EventArgs e)
        {
            label7.Text = $"{_user.Login}:{_user.Status}";
        }
    }
}
