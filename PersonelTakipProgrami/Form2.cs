using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;// Güveli Parola Oluşturmak İçin
using System.IO;// Klasör İşlermleri İçin

namespace PersonelTakipProgrami
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        //Veritabanı Bağlantısı
        SqlConnection baglanti = new SqlConnection(@"Data Source=DESKTOP-FM4UKPC\SQLEXPRESS;Initial Catalog=PersonelTakip;Integrated Security=True;MultipleActiveResultSets=True");

        private void kullanicilistele()
        {
            try
            {
                baglanti.Open();
                SqlDataAdapter adtr = new SqlDataAdapter("select tcno AS[TC KİMLİK NO], ad AS[ADI], soyad AS[SOYADI], yetki AS[YETKİ], kullaniciadi AS[KULLANICI ADI], parola AS[PAROLA] from kullanicilar Order by ad ASC", baglanti);
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
        private void personellistele()
        {
            try
            {
                baglanti.Open();
                SqlDataAdapter adtr = new SqlDataAdapter("select tcno AS[TC KİMLİK NO], ad AS[ADI], soyad AS[SOYADI], cinsiyet AS[CİNSİYETİ], mezuniyet AS[MEZUNİYETİ], dogumtarihi AS[DOĞUM TARİHİ], gorevi AS[GÖREVİ], gorevyeri AS[GÖREVYERİ], maas AS[MAAŞI] from personeller Order by ad ASC ", baglanti);
                DataTable dt = new DataTable();
                adtr.Fill(dt);
                dataGridView2.DataSource = dt;
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

        private void toppage1temizle()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
        }

        private void toppage2temizle()
        {
            pictureBox2.Image = null;
            maskedTextBox1.Clear();
            maskedTextBox2.Clear();
            maskedTextBox3.Clear();
            maskedTextBox4.Clear();
            comboBox1.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            kullanicilistele();
            personellistele();

            try
            {
                pictureBox1.Image = Image.FromFile(Application.StartupPath + "\\kullaniciresimler\\" + Form1.tcno + ".jpg");
            }
            catch
            {
                pictureBox1.Image = Image.FromFile(Application.StartupPath + "\\kullaniciresimler\\" + "resimyok.jpg");
            }

            label12.Text = Form1.adi + " " + Form1.soyadi;

            //Personel İşlemleri Ayarları
            DateTime zaman = DateTime.Now;
            int yil = int.Parse(zaman.ToString("yyyy"));
            int ay = int.Parse(zaman.ToString("MM"));
            int gun = int.Parse(zaman.ToString("dd "));

            dateTimePicker1.MinDate = new DateTime(yil - 60, ay, gun);
            dateTimePicker1.MaxDate = new DateTime(yil - 18, ay, gun);

            dateTimePicker1.Format = DateTimePickerFormat.Short;

        }

        // TextChanged özelliği textBox' a harf basıldıktan sonra çalışır.
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == "0")
            {
                textBox1.Text = "";
            }
            if (textBox1.Text.Length < 11)
            {
                errorProvider1.SetError(textBox1, "TC Kimlik No 11 karakter olmalı!");
            }
            else
            {
                errorProvider1.Clear();
            }
        }

        // KeyPress özelliği textBox' a harf basılmadan önce çalışır.
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Sadece rakam ve backspace' ye izin verdik.
            if (((int)e.KeyChar >= 48 && (int)e.KeyChar <= 57) || (int)e.KeyChar == 8)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Sadece harf, backspace ve boşluk tuşlarına basılabilir.
            if (char.IsLetter(e.KeyChar) == true || char.IsControl(e.KeyChar) == true || char.IsSeparator(e.KeyChar) == true)
            {
                e.Handled = false;
                e.KeyChar = Char.ToUpper(e.KeyChar);
            }
            else
            {
                e.Handled = true;
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Sadece harf, backspace ve boşluk tuşlarına basılabilir.
            if (char.IsLetter(e.KeyChar) == true || char.IsControl(e.KeyChar) == true || char.IsSeparator(e.KeyChar) == true)
            {
                e.Handled = false;
                e.KeyChar = Char.ToUpper(e.KeyChar);
            }
            else
            {
                e.Handled = true;
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (textBox4.Text.Length != 8)
            {
                errorProvider2.SetError(textBox4, "Kullanıcı Adı 8 karakter olmalı!");
            }
            if (textBox4.Text.Length == 8 || textBox4.Text == "")
            {
                errorProvider2.Clear();
            }

        }

        int ParolaSkoru = 0;
        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            #region Eski kodlar
            //int ParolaSkoru = 0;
            //string ParolaSeviyesi = "";
            //int KucukHarfSkoru = 0, BuyukHarfSkoru = 0, RakamSkoru = 0, SembolSkoru = 0;
            //string Sifre = textBox5.Text;
            //string DuzeltilmisSifre;
            //DuzeltilmisSifre = Sifre;
            //DuzeltilmisSifre = DuzeltilmisSifre.Replace("İ", "I");
            //DuzeltilmisSifre = DuzeltilmisSifre.Replace("ı", "i");
            //DuzeltilmisSifre = DuzeltilmisSifre.Replace("Ç", "C");
            //DuzeltilmisSifre = DuzeltilmisSifre.Replace("ç", "c");
            //DuzeltilmisSifre = DuzeltilmisSifre.Replace("Ş", "S");
            //DuzeltilmisSifre = DuzeltilmisSifre.Replace("ş", "s");
            //DuzeltilmisSifre = DuzeltilmisSifre.Replace("Ğ", "G");
            //DuzeltilmisSifre = DuzeltilmisSifre.Replace("ğ", "g");
            //DuzeltilmisSifre = DuzeltilmisSifre.Replace("Ü", "U");
            //DuzeltilmisSifre = DuzeltilmisSifre.Replace("ü", "u");
            //DuzeltilmisSifre = DuzeltilmisSifre.Replace("Ö", "O");
            //DuzeltilmisSifre = DuzeltilmisSifre.Replace("ö", "o");
            //if (Sifre != DuzeltilmisSifre)
            //{
            //    Sifre = DuzeltilmisSifre;
            //    textBox5.Text = Sifre;
            //    MessageBox.Show("Paroladaki Türkçe karakterler İngilizce karakterlere dönüştürülmüştür!","",MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
            //// 1 Küçük harf 10 puan, 2 ve üzeri 20 puan.
            //int KucukHarfSayisi = Sifre.Length - Regex.Replace(Sifre, "[a-z]", "").Length;
            //KucukHarfSkoru = Math.Min(2, KucukHarfSayisi) * 10;
            //// 1 Büyük harf 10 puan, 2 ve üzeri 20 puan.
            //int BuyukHarfSayisi = Sifre.Length - Regex.Replace(Sifre, "[A-Z]", "").Length;
            //BuyukHarfSkoru = Math.Min(2, BuyukHarfSayisi) * 10;
            //// 1 Rakam 10 puan, 2 ve üzeri 20 puan.
            //int RakamSayisi = Sifre.Length - Regex.Replace(Sifre, "[0-9]", "").Length;
            //RakamSkoru = Math.Min(2, RakamSayisi) * 10;
            //// 1 Sembol 10 puan, 2 ve üzeri 20 puan.
            //int SembolSayisi = Sifre.Length - (KucukHarfSayisi + BuyukHarfSayisi + RakamSayisi);
            //SembolSkoru = Math.Min(2, SembolSayisi) * 10;

            //ParolaSkoru = KucukHarfSkoru + BuyukHarfSkoru + RakamSkoru + SembolSkoru;
            //if (Sifre.Length == 9)
            //{
            //    ParolaSkoru += 10;
            //}
            //else if (Sifre.Length == 10)
            //{
            //    ParolaSkoru += 20;
            //}

            //if (KucukHarfSkoru == 0 || BuyukHarfSkoru == 0 || RakamSkoru == 0 || SembolSkoru == 0)
            //{
            //    label22.Text = "Büyük harf, küçük harf, rakam ve sembol mutlaka kullanmalısın!";
            //}
            //else if (KucukHarfSkoru != 0 && BuyukHarfSkoru != 0 && RakamSkoru != 0 && SembolSkoru != 0)
            //{
            //    label22.Text = "";
            //}

            //if (ParolaSkoru < 70)
            //{
            //    ParolaSeviyesi = "Kabul edilemez";
            //}
            //else if (ParolaSkoru == 70 || ParolaSkoru == 80)
            //{
            //    ParolaSeviyesi = "Güçlü";
            //}
            //else if (ParolaSkoru == 90 || ParolaSkoru == 100)
            //{
            //    ParolaSeviyesi = "Çok Güçlü";
            //}

            //label9.Text = "%" + Convert.ToString(ParolaSkoru);
            //label10.Text = ParolaSeviyesi;
            //progressBar1.Value = ParolaSkoru;

            #endregion

            string ParolaSeviyesi = "";
            int KucukHarfSkoru = 0, BuyukHarfSkoru = 0, RakamSkoru = 0, SembolSkoru = 0;
            string Sifre = textBox5.Text;

            Sifre = Sifre.Replace("İ", "I");
            Sifre = Sifre.Replace("ı", "i");
            Sifre = Sifre.Replace("Ç", "C");
            Sifre = Sifre.Replace("ç", "c");
            Sifre = Sifre.Replace("Ş", "S");
            Sifre = Sifre.Replace("ş", "s");
            Sifre = Sifre.Replace("Ğ", "G");
            Sifre = Sifre.Replace("ğ", "g");
            Sifre = Sifre.Replace("Ü", "U");
            Sifre = Sifre.Replace("ü", "u");
            Sifre = Sifre.Replace("Ö", "O");
            Sifre = Sifre.Replace("ö", "o");

            // 1 Küçük harf 10 puan, 2 ve üzeri 20 puan.
            int KucukHarfSayisi = Sifre.Length - Regex.Replace(Sifre, "[a-z]", "").Length;
            KucukHarfSkoru = Math.Min(2, KucukHarfSayisi) * 10;
            // 1 Büyük harf 10 puan, 2 ve üzeri 20 puan.
            int BuyukHarfSayisi = Sifre.Length - Regex.Replace(Sifre, "[A-Z]", "").Length;
            BuyukHarfSkoru = Math.Min(2, BuyukHarfSayisi) * 10;
            // 1 Rakam 10 puan, 2 ve üzeri 20 puan.
            int RakamSayisi = Sifre.Length - Regex.Replace(Sifre, "[0-9]", "").Length;
            RakamSkoru = Math.Min(2, RakamSayisi) * 10;
            // 1 Sembol 10 puan, 2 ve üzeri 20 puan.
            int SembolSayisi = Sifre.Length - (KucukHarfSayisi + BuyukHarfSayisi + RakamSayisi);
            SembolSkoru = Math.Min(2, SembolSayisi) * 10;

            ParolaSkoru = KucukHarfSkoru + BuyukHarfSkoru + RakamSkoru + SembolSkoru;

            if (Sifre.Length == 9)
            {
                ParolaSkoru += 10;
            }
            else if (Sifre.Length == 10)
            {
                ParolaSkoru += 20;
            }

            if (KucukHarfSkoru == 0 || BuyukHarfSkoru == 0 || RakamSkoru == 0 || SembolSkoru == 0)
            {
                label22.Text = "Büyük harf, küçük harf, rakam ve sembol kullanmalısın!";
            }
            else if (KucukHarfSkoru != 0 && BuyukHarfSkoru != 0 && RakamSkoru != 0 && SembolSkoru != 0)
            {
                label22.Text = "";
            }

            if (ParolaSkoru < 70)
            {
                ParolaSeviyesi = "Kabul edilemez";
            }
            else if (ParolaSkoru == 70 || ParolaSkoru == 80)
            {
                ParolaSeviyesi = "Güçlü";
            }
            else if (ParolaSkoru == 90 || ParolaSkoru == 100)
            {
                ParolaSeviyesi = "Çok Güçlü";
            }

            label9.Text = "%" + Convert.ToString(ParolaSkoru);
            label10.Text = ParolaSeviyesi;
            progressBar1.Value = ParolaSkoru;

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            if (textBox5.Text != textBox6.Text)
            {
                errorProvider1.SetError(textBox6, "Parola tekrarı eşleşmiyor!");
            }
            else
            {
                errorProvider1.Clear();
            }
        }

        private void maskedTextBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = (e.KeyChar.ToString()).ToUpper().ToCharArray()[0];
        }

        private void maskedTextBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = (e.KeyChar.ToString()).ToUpper().ToCharArray()[0];
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string yetki = "";
            bool KayitKontrol = false;

            #region Kayıtın veri tabanında olup olmadığını tcno ya göre kontrol ediliyor.

            baglanti.Open();
            SqlCommand komut = new SqlCommand("select *from kullanicilar where tcno= '" + textBox1.Text + "'", baglanti);
            SqlDataReader read = komut.ExecuteReader();
            while (read.Read())
            {
                KayitKontrol = true;
            }
            baglanti.Close();

            #endregion

            if (KayitKontrol == false)
            {
                #region Textboxlara veri girişi kontrolü

                // TC Kimlik No Kontrolü
                if (textBox1.Text.Length < 11 || textBox1.Text == "")
                {
                    label1.ForeColor = Color.Red;
                }
                else
                {
                    label1.ForeColor = Color.Black;
                }
                // Ad Kontrolü
                if (textBox2.Text.Length < 2 || textBox2.Text == "")
                {
                    label2.ForeColor = Color.Red;
                }
                else
                {
                    label2.ForeColor = Color.Black;
                }
                // Soyad Kontrolü
                if (textBox3.Text.Length < 2 || textBox3.Text == "")
                {
                    label3.ForeColor = Color.Red;
                }
                else
                {
                    label3.ForeColor = Color.Black;
                }
                // Kullanıcı Adı Kontrolü
                if (textBox4.Text.Length != 8 || textBox4.Text == "")
                {
                    label5.ForeColor = Color.Red;
                }
                else
                {
                    label5.ForeColor = Color.Black;
                }
                // Parola Kontrolü
                if (ParolaSkoru < 70 || textBox5.Text == "")
                {
                    label6.ForeColor = Color.Red;
                }
                else
                {
                    label6.ForeColor = Color.Black;
                }
                // Parola Tekrar Kontrolü
                if (textBox5.Text != textBox6.Text || textBox6.Text == "")
                {
                    label7.ForeColor = Color.Red;
                }
                else
                {
                    label7.ForeColor = Color.Black;
                }
                #endregion

                if (textBox1.Text.Length == 11 && textBox2.Text.Length > 1 && textBox3.Text.Length > 1 && textBox4.Text.Length == 8 && textBox5.Text == textBox6.Text && ParolaSkoru >= 70)
                {
                    if (radioButton1.Checked == true)
                    {
                        yetki = "Yönetici";
                    }
                    else if (radioButton2.Checked == true)
                    {
                        yetki = "Kullanıcı";
                    }

                    try
                    {
                        baglanti.Open();
                        SqlCommand komut2 = new SqlCommand("insert into kullanicilar values(@tcno, @ad, @soyad, @yetki, @kullaniciadi, @parola)", baglanti);
                        komut2.Parameters.AddWithValue("@tcno", textBox1.Text);
                        komut2.Parameters.AddWithValue("@ad", textBox2.Text);
                        komut2.Parameters.AddWithValue("@soyad", textBox3.Text);
                        komut2.Parameters.AddWithValue("@yetki", yetki);
                        komut2.Parameters.AddWithValue("@kullaniciadi", textBox4.Text);
                        komut2.Parameters.AddWithValue("@parola", textBox5.Text);
                        komut2.ExecuteNonQuery();
                        baglanti.Close();
                        MessageBox.Show("Yeni kullanıcı kaydı oluşturuldu.", "Kayıt Eklendi!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        //label1.ForeColor = Color.Black;
                        //label5.ForeColor = Color.Black;
                        toppage1temizle();
                    }
                    catch (Exception hatamsj)
                    {
                        MessageBox.Show(hatamsj.Message, "Personel Takip Programı", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        if (baglanti.State == ConnectionState.Open)
                        {
                            baglanti.Close();
                        }

                    }
                }
                else
                {
                    MessageBox.Show("Yazı rengi kırmızı olan alanları yeniden gözden geçiriniz!", "Kayıt Yapılamadı!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Girilen TC Kimlik Numarası daha önceden kayıtlıdır.", "UYARI!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool KayitKontrol = false;

            if (textBox1.Text.Length == 11)
            {
                baglanti.Open();
                SqlCommand komut = new SqlCommand("select *from kullanicilar where tcno = '" + textBox1.Text + "'", baglanti);
                SqlDataReader read = komut.ExecuteReader();
                while (read.Read())
                {
                    KayitKontrol = true;

                    textBox2.Text = read.GetValue(1).ToString();
                    textBox3.Text = read.GetValue(2).ToString();
                    if (read.GetValue(3).ToString() == "Yönetici")
                    {
                        radioButton1.Checked = true;
                        radioButton2.Checked = false;
                    }
                    else
                    {
                        radioButton1.Checked = false;
                        radioButton2.Checked = true;
                    }
                    textBox4.Text = read.GetValue(4).ToString();
                    textBox5.Text = read.GetValue(5).ToString();
                    textBox6.Text = read.GetValue(5).ToString();
                }
                if (KayitKontrol == false)
                {
                    MessageBox.Show("Aranan kayıt bulunamadı!", "Uyarı!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    toppage1temizle();
                }
                baglanti.Close();
            }
            else
            {
                MessageBox.Show("Lütfen 11 haneli bir TC Kimlik No giriniz!", "Uyarı!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                toppage1temizle();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string yetki = "";
            #region textboxlara veri girişi kontrolü

            // TC Kimlik No Kontrolü
            if (textBox1.Text.Length < 11 || textBox1.Text == "")
            {
                label1.ForeColor = Color.Red;
            }
            else
            {
                label1.ForeColor = Color.Black;
            }
            // Ad Kontrolü
            if (textBox2.Text.Length < 2 || textBox2.Text == "")
            {
                label2.ForeColor = Color.Red;
            }
            else
            {
                label2.ForeColor = Color.Black;
            }
            // Soyad Kontrolü
            if (textBox3.Text.Length < 2 || textBox3.Text == "")
            {
                label3.ForeColor = Color.Red;
            }
            else
            {
                label3.ForeColor = Color.Black;
            }
            // Kullanıcı Adı Kontrolü
            if (textBox4.Text.Length != 8 || textBox4.Text == "")
            {
                label5.ForeColor = Color.Red;
            }
            else
            {
                label5.ForeColor = Color.Black;
            }
            // Parola Kontrolü
            if (ParolaSkoru < 70 || textBox5.Text == "")
            {
                label6.ForeColor = Color.Red;
            }
            else
            {
                label6.ForeColor = Color.Black;
            }
            // Parola Tekrar Kontrolü
            if (textBox5.Text != textBox6.Text || textBox6.Text == "")
            {
                label7.ForeColor = Color.Red;
            }
            else
            {
                label7.ForeColor = Color.Black;
            }
            #endregion

            if (textBox1.Text.Length == 11 && textBox2.Text.Length > 1 && textBox3.Text.Length > 1 && textBox4.Text.Length == 8 && textBox5.Text == textBox6.Text && ParolaSkoru >= 70)
            {
                if (radioButton1.Checked == true)
                {
                    yetki = "Yönetici";
                }
                else if (radioButton2.Checked == true)
                {
                    yetki = "Kullanıcı";
                }

                try
                {
                    baglanti.Open();
                    SqlCommand komut2 = new SqlCommand("update kullanicilar set ad=@ad, soyad=@soyad, yetki=@yetki, kullaniciadi=@kullaniciadi, parola=@parola where tcno = @tcno", baglanti);
                    komut2.Parameters.AddWithValue("@tcno", textBox1.Text);
                    komut2.Parameters.AddWithValue("@ad", textBox2.Text);
                    komut2.Parameters.AddWithValue("@soyad", textBox3.Text);
                    komut2.Parameters.AddWithValue("@yetki", yetki);
                    komut2.Parameters.AddWithValue("@kullaniciadi", textBox4.Text);
                    komut2.Parameters.AddWithValue("@parola", textBox5.Text);
                    komut2.ExecuteNonQuery();
                    baglanti.Close();
                    MessageBox.Show("Kullanıcı bilgileri güncellendi!", "Personel Takip Programı", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //label1.ForeColor = Color.Black;
                    //label5.ForeColor = Color.Black;
                    toppage1temizle();
                    kullanicilistele();
                }
                catch (Exception hatamsj)
                {
                    MessageBox.Show(hatamsj.Message);

                    if (baglanti.State == ConnectionState.Open)
                    {
                        baglanti.Close();
                    }

                }
            }
            else
            {
                MessageBox.Show("Yazı rengi kırmızı olan alanları yeniden gözden geçiriniz!", "Kayıt Yapılamadı!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 11)
            {
                bool KayitVarmi = false;

                baglanti.Open();
                SqlCommand komut = new SqlCommand("select *from kullanicilar where tcno = '"+textBox1.Text+"'", baglanti);
                SqlDataReader read = komut.ExecuteReader();
                while (read.Read())
                {
                    KayitVarmi = true;
                    SqlCommand komut4 = new SqlCommand("delete from kullanicilar where tcno = '"+textBox1.Text+"'", baglanti);
                    komut4.ExecuteNonQuery();
                    MessageBox.Show("Kullanıcı Kaydı Silindi!", "Personel Takip Programı", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    baglanti.Close();
                    kullanicilistele();
                    break;
                }
                if (KayitVarmi == false)
                {
                    MessageBox.Show("Silinecek kayıt bulunamadı!", "Personel Takip Programı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (baglanti.State == ConnectionState.Open)
                {
                    baglanti.Close();
                }
                toppage1temizle();
            }
            else
            {
                MessageBox.Show("Lütfen 11 karakterden oluşan bir TC Kimlik No giriniz!", "Personel Takip Programı", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            toppage1temizle();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            OpenFileDialog resimsec = new OpenFileDialog();
            resimsec.Title = "Personel Resmi Seçiniz...";
            resimsec.Filter = "JPG Dosyalar (*.jgp) | *.jpg";
            if (resimsec.ShowDialog() == DialogResult.OK)
            {
                pictureBox2.Image = new Bitmap(resimsec.OpenFile());
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string Cinsiyet = "";
            bool KayitKontrol = false;

            #region Kayıtın veri tabanında olup olmadığını tcno ya göre kontrol ediliyor.

            baglanti.Open();
            SqlCommand komut = new SqlCommand("select *from personeller where tcno= '" + maskedTextBox1.Text + "'", baglanti);
            SqlDataReader read = komut.ExecuteReader();
            while (read.Read())
            {
                KayitKontrol = true;
            }
            baglanti.Close();

            #endregion

            if (KayitKontrol == false)
            {
                #region Alanlara veri girişi kontrolü

                if (pictureBox2.Image == null)
                {
                    button6.ForeColor = Color.Red;
                }
                else
                {
                    button6.ForeColor = Color.Black;
                }

                if (maskedTextBox1.MaskCompleted == false)
                {
                    label13.ForeColor = Color.Red;
                }
                else
                {
                    label13.ForeColor = Color.Black;
                }

                if (maskedTextBox2.MaskCompleted == false)
                {
                    label14.ForeColor = Color.Red;
                }
                else
                {
                    label14.ForeColor = Color.Black;
                }

                if (maskedTextBox3.MaskCompleted == false)
                {
                    label15.ForeColor = Color.Red;
                }
                else
                {
                    label15.ForeColor = Color.Black;
                }

                if (comboBox1.Text == "")
                {
                    label17.ForeColor = Color.Red;
                }
                else
                {
                    label17.ForeColor = Color.Black;
                }

                if (comboBox2.Text == "")
                {
                    label19.ForeColor = Color.Red;
                }
                else
                {
                    label19.ForeColor = Color.Black;
                }

                if (comboBox3.Text == "")
                {
                    label20.ForeColor = Color.Red;
                }
                else
                {
                    label20.ForeColor = Color.Black;
                }

                if (maskedTextBox3.Text == "")
                {
                    label21.ForeColor = Color.Red;
                }
                else
                {
                    label21.ForeColor = Color.Black;
                }

                if (maskedTextBox4.Text == "")
                {
                    label21.ForeColor = Color.Red;
                }
                else
                {
                    label21.ForeColor = Color.Black;
                }

                #endregion

                if (pictureBox2.Image != null && maskedTextBox1.MaskCompleted == true && maskedTextBox2.MaskCompleted == true && maskedTextBox3.MaskCompleted == true && comboBox1.Text != "" && comboBox2.Text != "" && comboBox3.Text != "" && maskedTextBox4.Text != "")
                {
                    if (radioButton3.Checked == true)
                    {
                        Cinsiyet = "Bay";
                    }
                    else if (radioButton4.Checked == true)
                    {
                        Cinsiyet = "Bayan";
                    }

                    try
                    {
                        baglanti.Open();
                        SqlCommand komut2 = new SqlCommand("insert into personeller values(@tcno, @ad, @soyad, @cinsiyet, @mezuniyet, @dogumtarihi, @gorevi, @gorevyeri, @maas)", baglanti);
                        komut2.Parameters.AddWithValue("@tcno", maskedTextBox1.Text);
                        komut2.Parameters.AddWithValue("@ad", maskedTextBox2.Text);
                        komut2.Parameters.AddWithValue("@soyad", maskedTextBox3.Text);
                        komut2.Parameters.AddWithValue("@cinsiyet", Cinsiyet);
                        komut2.Parameters.AddWithValue("@mezuniyet", comboBox1.Text);
                        komut2.Parameters.AddWithValue("@dogumtarihi", dateTimePicker1.Value);
                        komut2.Parameters.AddWithValue("@gorevi", comboBox2.Text);
                        komut2.Parameters.AddWithValue("@gorevyeri", comboBox3.Text);
                        komut2.Parameters.AddWithValue("@maas", maskedTextBox4.Text);
                        komut2.ExecuteNonQuery();
                        baglanti.Close();
                        if (!Directory.Exists(Application.StartupPath + "\\personelresimler"))
                        {
                            Directory.CreateDirectory(Application.StartupPath + "\\personelresimler");
                        }
                        pictureBox2.Image.Save(Application.StartupPath + "\\personelresimler\\" + maskedTextBox1.Text + ".jpg");
                        
                        MessageBox.Show("Personel kaydı oluşturuldu.", "Kayıt Eklendi!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        toppage2temizle();
                        personellistele();

                    }
                    catch (Exception hatamsj)
                    {
                        MessageBox.Show(hatamsj.Message, "Personel Takip Programı", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        if (baglanti.State == ConnectionState.Open)
                        {
                            baglanti.Close();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Yazı rengi kırmızı olan alanları yeniden gözden geçiriniz!", "Kayıt Yapılamadı!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Girilen TC Kimlik Numarası daha önceden kayıtlıdır.", "UYARI!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button7_Click(object sender, EventArgs e)
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
                        pictureBox2.Image = Image.FromFile(Application.StartupPath + "\\personelresimler\\" + maskedTextBox1.Text + ".jpg");
                    }
                    catch
                    {
                        pictureBox2.Image = Image.FromFile(Application.StartupPath + "\\personelresimler\\resimyok.jpg");
                    }

                    maskedTextBox2.Text = read.GetValue(1).ToString();
                    maskedTextBox3.Text = read.GetValue(2).ToString();
                    if (read.GetValue(3).ToString() == "Bay")
                    {
                        radioButton3.Checked = true;
                        radioButton4.Checked = false;
                    }
                    else
                    {
                        radioButton3.Checked = false;
                        radioButton4.Checked = true;
                    }
                    comboBox1.Text = read.GetValue(4).ToString();
                    dateTimePicker1.Value = Convert.ToDateTime(read.GetValue(5));
                    comboBox2.Text = read.GetValue(6).ToString();
                    comboBox3.Text = read.GetValue(7).ToString();
                    maskedTextBox4.Text = read.GetValue(8).ToString();
                }
                if (KayitKontrol == false)
                {
                    MessageBox.Show("Aranan kayıt bulunamadı!", "Uyarı!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    toppage1temizle();
                }
                baglanti.Close();
            }
            else
            {
                MessageBox.Show("Lütfen 11 haneli bir TC Kimlik No giriniz!", "Uyarı!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                toppage1temizle();
            }
        }
    }
}
