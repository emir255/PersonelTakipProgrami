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
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }
        SqlConnection baglanti = new SqlConnection(@"Data Source=DESKTOP-FM4UKPC\SQLEXPRESS;Initial Catalog=PersonelTakip;Integrated Security=True;MultipleActiveResultSets=True");

        private void personellistele()
        {
            try
            {
                baglanti.Open();
                SqlDataAdapter adtr = new SqlDataAdapter("select tcno AS[TC KİMLİK NO], ad AS[ADI], soyad AS[SOYADI], cinsiyet AS[CİNSİYETİ], mezuniyet AS[MEZUNİYETİ], dogumtarihi AS[DOĞUM TARİHİ], gorevi AS[GÖREVİ], gorevyeri AS[GÖREVYERİ], maas AS[MAAŞI] from personeller Order by ad ASC ", baglanti);
                DataTable dt = new DataTable();
                adtr.Fill(dt);
                dataGridView1.DataSource = dt;
            }
            catch (Exception hatamsj)
            {
                MessageBox.Show(hatamsj.Message, "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (baglanti.State == ConnectionState.Open)
                {
                    baglanti.Close();
                }
            }
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            personellistele();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool KayitKontrol = false;

            if (maskedTextBox1.Text.Length == 11)
            {
                baglanti.Open();
                SqlCommand komut = new SqlCommand("select *from personeller where tcno = '" + maskedTextBox1.Text + "'", baglanti);
                SqlDataReader read = komut.ExecuteReader();
                while (read.Read())
                {
                    KayitKontrol = true;

                    try
                    {
                        pictureBox1.Image = Image.FromFile(Application.StartupPath + "\\personelresimler\\" + maskedTextBox1.Text + ".jpg");
                    }
                    catch
                    {
                        pictureBox1.Image = Image.FromFile(Application.StartupPath + "\\personelresimler\\resimyok.jpg");
                    }

                    label10.Text = read.GetValue(1).ToString();
                    label11.Text = read.GetValue(2).ToString();
                    label12.Text = read.GetValue(3).ToString();
                    label13.Text = read.GetValue(4).ToString();
                    label14.Text = read.GetValue(5).ToString();
                    label15.Text = read.GetValue(6).ToString();
                    label16.Text = read.GetValue(7).ToString();
                    label17.Text = read.GetValue(8).ToString();
                }
                if (KayitKontrol == false)
                {
                    MessageBox.Show("Aranan kayıt bulunamadı!", "Uyarı!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                baglanti.Close();
            }
            else
            {
                MessageBox.Show("Lütfen 11 haneli bir TC Kimlik No giriniz!", "Uyarı!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form3_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (MessageBox.Show("Çıkış yapılıyor ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                Application.Exit();
            }
            else
            {
                Form3 frm3 = new Form3();
                frm3.Show();
            }
        }
    }
}
