using System.Runtime.InteropServices;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;

namespace NoteApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SqlConnection baglanti = new SqlConnection(@"Data Source=DESKTOP-RPEDS99\SQLEXPRESS;Initial Catalog=NoteDb;Integrated Security=True");
        private void Form1_Load(object sender, EventArgs e)
        {
            panel4.Visible = false;
            panel8.Visible = false;
        }
        #region Navigation
        private void btnMinus_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void btnMaximize_Click(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                WindowState = FormWindowState.Normal;
            }
        }

        private void pnlNavi_Paint(object sender, PaintEventArgs e)
        {

        }
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPT�ON = 0x2;
        [DllImport("User32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        private void pnlNavi_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPT�ON, 0);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        #endregion

        #region Login_Logout
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username, password;
            username = txtUserName.Text;
            password = txtPassword.Text;
            baglanti.Open();
            try
            {
                string sqlKomut = "SELECT * FROM Users WHERE UserName = '" + txtUserName.Text + "' AND Password = '" + txtPassword.Text + "'";
                SqlDataAdapter sqlDA = new SqlDataAdapter(sqlKomut, baglanti);
                DataTable dtable = new DataTable();
                sqlDA.Fill(dtable);
                if (dtable.Rows.Count > 0)
                {
                    username = txtUserName.Text;
                    password = txtPassword.Text;
                    MessageBox.Show("Giri� Ba�ar�l�");
                    panel2.Visible = false;
                    panel4.Visible = true;
                    panel8.Visible = true;
                }
                else
                {
                    MessageBox.Show("Kullan�c� Bulunamad�", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtUserName.Clear();
                    txtPassword.Clear();
                    txtUserName.Focus();
                }
            }
            catch
            {
                MessageBox.Show("SqlDatabase Hatas�");
            }
            finally
            {
                baglanti.Close();
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("��k�� yapmak istiyor musunuz?", "��k��", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                panel4.Visible = false;
                panel8.Visible = false;
                panel2.Visible = true;
                txtUserName.Clear();
                txtPassword.Clear();
            }
        }
        #endregion

        #region Tools
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtNote.Text != "")
                {
                    lstNote.Items.Add(txtNote.Text);
                    baglanti.Open();
                    SqlCommand sqlKomut = new SqlCommand($"INSERT INTO Notes (Note, CreatedDate) VALUES(@p1, @p2)", baglanti);
                    sqlKomut.Parameters.AddWithValue("@p1", txtNote.Text);
                    sqlKomut.Parameters.AddWithValue("@p2", DateTime.Now);
                    sqlKomut.ExecuteNonQuery();
                    //baglanti.Close();
                    MessageBox.Show("Not Eklendi", "Ba�ar�l�", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                else MessageBox.Show("Eklenecek Notu Yaz�n�z", "Bo� Text!!", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch
            {
                MessageBox.Show("SqlDatabase Hatas�");
            }
            finally
            {
                baglanti.Close();
            }
        }

        private void btnList_Click(object sender, EventArgs e)
        {
            lstNote.Items.Clear();
            try
            {
                baglanti.Open();
                SqlCommand sqlKomut = new SqlCommand("SELECT * FROM Notes", baglanti);
                SqlDataReader sqlDR = sqlKomut.ExecuteReader();
                while (sqlDR.Read())
                {
                    string id = sqlDR[0].ToString();
                    string note = sqlDR[1].ToString();
                    string date = sqlDR[2].ToString();
                    lstNote.Items.Add(note);
                }
            }
            catch
            {
                MessageBox.Show("SqlDatabase Hatas�");
            }
            finally
            {
                baglanti.Close();
            }
        }

        private void btnLstClear_Click(object sender, EventArgs e)
        {
            lstNote.Items.Clear();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Se�ilen Not Veritaban�ndan Silinecek\nDevam Edilsin mi?", "Not Silme", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                baglanti.Open();
                SqlCommand sqlDelete = new SqlCommand("Delete From Notes Where Note=@k1", baglanti);
                sqlDelete.Parameters.AddWithValue("@k1", lstNote.SelectedItem.ToString());
                sqlDelete.ExecuteNonQuery();
                baglanti.Close();
                MessageBox.Show("Se�ilen Not Veritaban�ndan Silindi");
            }
        }
        #endregion

        #region Links
        private void btnGithub_Click(object sender, EventArgs e)
        {
            string githubUrl = "https://github.com/eren5854";
            Process.Start(new ProcessStartInfo
            {
                FileName = githubUrl,
                UseShellExecute = true
            });
        }

        private void btnLinkedin_Click(object sender, EventArgs e)
        {
            string linkedinUrl = "https://www.linkedin.com/in/ihsan-eren-deliba%C5%9F-208581159/";
            Process.Start(new ProcessStartInfo
            {
                FileName = linkedinUrl,
                UseShellExecute = true
            });
        }
        #endregion

        #region TxtKay�t
        private void btnExport_Click(object sender, EventArgs e)
        {
            SaveTxt(lstNote, "F:\\Eren\\YMYP\\NoteApp\\Notlar.txt");
        }
        private void SaveTxt(ListBox listBox, string filePath)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (var item in lstNote.Items)
                    {
                        writer.WriteLine(item.ToString());
                    }
                }
                MessageBox.Show("Veriler dosyaya ba�ar�yla kaydedildi.", "Ba�ar�l�", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veriler dosyaya kaydedilirken bir hata olu�tu:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}