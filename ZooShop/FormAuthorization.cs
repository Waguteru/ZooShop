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
    public partial class FormAuthorization : Form
    {

        DataBase dataBase = new DataBase();

        public FormAuthorization()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FormProducts formProducts = new FormProducts();
            formProducts.Show();
            Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataBase.OpenConnecting();

            var login = textBox1.Text;
            var password = textBox2.Text;

            NpgsqlDataAdapter adapter = new NpgsqlDataAdapter();
            DataTable table = new DataTable();

            var query = $"select id_user, login_roles, password_roles, id_role from roles_tbl where login_roles = '{login}' and password_roles = '{password}'";

            NpgsqlCommand npgsqlCommand = new NpgsqlCommand(query, dataBase.GetConnection());

            adapter.SelectCommand = npgsqlCommand;
            adapter.Fill(table);

            if(table.Rows.Count == 1)
            {
                var user = new CheckUser(table.Rows[0].ItemArray[1].ToString(), Convert.ToBoolean(table.Rows[0].ItemArray[3]));

                MessageBox.Show("Вы успешно вошли!", "Успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FormProductsAdmin productsAdmin = new FormProductsAdmin(user);
                this.Hide();
                productsAdmin.ShowDialog();

            }
            else
            {
                MessageBox.Show("Такого аккаунта не существует!", "Аккаунта не существует!", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            dataBase.CloseConnecting();
        }
        
    }
}
