using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace PersonelTakipProgrami
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //Veritabanı Bağlantısı
        SqlConnection baglanti = new SqlConnection(@"Data Source=DESKTOP-FM4UKPC\SQLEXPRESS;Initial Catalog=PersonelTakip;Integrated Security=True");

        //Formlar Arası Değişkenler
        public static string tcno, adi, soyadi, yetki;

        //Yerel Değşkenler
        int hak = 3;
        bool durum = false;

        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Çıkış yapılıyor ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }        

        private void Form1_Load(object sender, EventArgs e)
        {
            label5.Text = Convert.ToString(hak);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (hak != 0)
            {
                baglanti.Open();
                SqlCommand komut = new SqlCommand("select *from kullanicilar", baglanti);
                SqlDataReader read = komut.ExecuteReader();
                while (read.Read())
                {
                    if (radioButton1.Checked == true)
                    {
                        if (read["kullaniciadi"].ToString() == textBox1.Text && read["parola"].ToString() == textBox2.Text && read["yetki"].ToString() == "Yönetici")
                        {
                            durum = true;
                            tcno = (read["tcno"]).ToString();
                            adi = (read["ad"]).ToString();
                            soyadi = (read["soyad"]).ToString();
                            yetki = (read["yetki"]).ToString();
                            this.Hide();
                            Form2 frm2 = new Form2();
                            frm2.Show();
                            break;
                        }
                    }
                    else if (radioButton2.Checked == true)
                    {
                        if (textBox1.Text == read["kullaniciadi"].ToString() && textBox2.Text == read["parola"].ToString() && "Kullanıcı" == read["yetki"].ToString())
                        {
                            durum = true;
                            tcno = (read["tcno"]).ToString();
                            adi = (read["ad"]).ToString();
                            soyadi = (read["soyad"]).ToString();
                            yetki = (read["yetki"]).ToString();
                            this.Hide();
                            Form3 frm3 = new Form3();
                            frm3.Show();
                            break;
                        }
                    }


                }
                if (durum == false)
                {
                    hak--;
                }
                baglanti.Close();
                label5.Text = Convert.ToString(hak);
            }
            if (hak == 0)
            {
                MessageBox.Show("Giriş hakkı kalmadı!", "Giriş Hakkı Uyarısı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }
    }

}
