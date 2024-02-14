using Npgsql;
using NpgsqlTypes;
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
    public partial class FormOrders : Form
    {
        DataBase dataBase = new DataBase();

        private decimal price;
        private decimal discount;
        private Random random = new Random();

        private string description;
        private string name;
        private string manufacturer;

        public FormOrders(string description, string name, string manufacturer, decimal price, decimal discount, Npgsql.NpgsqlConnection npgsqlConnection)
        {
            InitializeComponent();

            this.description = description;
            this.name = name;
            this.manufacturer = manufacturer;
            conn = npgsqlConnection;

            richTextBox1.Text =
                                $"Наименование товара: {this.name}\n\n" +
                                $"Описание товара: {this.description}\n\n" +
                                $"Производитель: {this.manufacturer}\n\n" +
                                $"Цена: {price:C}\n\n" +
                                $"Размер скидки: {discount}\n\n" +
                                $"Дата заказа: {DateTime.Now:dd/MM/yyyy} \n\n" +
                                $"Номер заказа: {GenerateOrderNumber()}\n\n" +
                                $"Пункт выдачи:  {comboBox1.SelectedItem} \n\n" +
                                $"Код получение: {GenerateRandomCode()}\n\n";

            numericUpDown1.Value = 1;
            this.price = price;
            this.discount = discount;

            comboBox1.Items.Add("Минская 2В");
            comboBox1.Items.Add("Ленинский проспект 150");
            comboBox1.Items.Add("Депутатская д.11");
            comboBox1.Items.Add("Димитрова 31");
            comboBox1.Items.Add("9 Января д.31");
        }


        private string GenerateOrderNumber()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssfff");
        }

        private string GenerateRandomCode()
        {
            // Генерация трех случайных цифр
            return random.Next(100, 999).ToString();
        }

        private void UpdateRichTextBox()
        {
            decimal quantity = numericUpDown1.Value;

            if (quantity <= 0)
            {
                MessageBox.Show("Заказ не может быть пустым.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                FormProducts formoOpen = new FormProducts();
                formoOpen.Show();
                this.Close();
            }
            else
            {
                decimal totalAmount = (this.price * quantity) - this.discount;

                // Обновляем содержимое richTextBox1 на основе сохраненных данных
                richTextBox1.Text = 
                                    $"Наименование товара: {this.name}\n\n" +
                                    $"Описание товара: {this.description}\n\n" +
                                    $"Производитель: {this.manufacturer}\n\n" +
                                    $"Цена: {totalAmount:C}\n\n" +
                                    $"Размер скидки: {this.discount}\n\n" +
                                    $"Дата заказа: {DateTime.Now:dd/MM/yyyy} \n\n" +
                                    $"Номер заказа: {GenerateOrderNumber()}\n\n" +
                                    $"Пункт выдачи:  {comboBox1.SelectedItem} \n\n" +
                                    $"Код получение: {GenerateRandomCode()}\n\n";
                textBox1.Text = totalAmount.ToString();
            }
        }



        private NpgsqlConnection conn;

        private void FormOrders_Load(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            decimal quantity = numericUpDown1.Value;

            UpdateRichTextBox();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UpdateRichTextBox();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Получаем значения из richTextBox1
            string names = this.name;
            decimal discount = this.discount;
            decimal totalAmount = decimal.Parse(textBox1.Text); // Общая сумма заказа
            string orderNumber = GenerateOrderNumber();
            string pickupLocation = comboBox1.SelectedItem.ToString();
            string retrievalCode = GenerateRandomCode();

            // Сохраняем данные в базу данных
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                conn.Open();
                cmd.Connection = conn;

                // Запрос для вставки данных в таблицу orderuser
                string sql = "INSERT INTO orders_tbl (date_orders,number_order, name_product,summa_orders,summa_discount,pickupllocation,code_orders) " +
                             "VALUES (@OrderDate, @OrderNumber, @names, @TotalAmount, @discount, @PickupLocation, @RetrievalCode)";

                cmd.CommandText = sql;

                // Параметры запроса
                cmd.Parameters.Add(new NpgsqlParameter("@OrderDate", NpgsqlDbType.Date)).Value = DateTime.Now.Date;
                cmd.Parameters.AddWithValue("@OrderNumber", long.Parse(orderNumber));
                cmd.Parameters.AddWithValue("@names", names); // Предполагается, что название товара берется из поля name
                cmd.Parameters.AddWithValue("@TotalAmount", totalAmount);
                cmd.Parameters.AddWithValue("@discount", discount);
                cmd.Parameters.AddWithValue("@PickupLocation", pickupLocation);
                cmd.Parameters.AddWithValue("@RetrievalCode", int.Parse(retrievalCode));

                // Выполняем запрос
                cmd.ExecuteNonQuery();

                MessageBox.Show("Заказ успешно оформлен и сохранен в базе данных.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                FormProducts product = new FormProducts();
                product.Show();
                this.Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            printPreviewDialog1.Document = printDocument1;
            printPreviewDialog1.ShowDialog();
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            e.Graphics.DrawString(richTextBox1.Text, new Font("Times New Roman", 16, FontStyle.Regular), Brushes.Black, new Point(10, 10));

        }
    }
}
