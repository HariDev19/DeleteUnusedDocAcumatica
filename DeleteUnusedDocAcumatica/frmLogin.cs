using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;

namespace DeleteUnusedDocAcumatica
{
    public partial class frmLogin : KryptonForm
    {
        string usernameEncrypt, passEncrypt, usernameDecrypt, passDecrypt;
        public frmLogin()
        {
            InitializeComponent();
            usernameEncrypt = Properties.Settings.Default.username;
            passEncrypt = Properties.Settings.Default.password;

            usernameDecrypt = funDecrypt(usernameEncrypt);
            passDecrypt = funDecrypt(passEncrypt);
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            
        }

        private void btnEnterN_Click(object sender, EventArgs e)
        {
            if (!funCheckValidation())
                return;
            else
            {
                this.Hide();

                Cursor = Cursors.WaitCursor;
                frmMain frmMain = new frmMain();
                frmMain.Show();
                Cursor = Cursors.Default;
            }
        }

        private void btnCancelN_Click(object sender, EventArgs e)
        {
            Application.ExitThread();
        }

        private bool funCheckValidation()
        {
            bool ret = true;
            if (txtUsernameN.Text.Trim() == "")
            {
                ret = false;
                MessageBox.Show("username must be filled in !");
                txtUsernameN.Select();
            }
            else if (txtPasswordN.Text.Trim() == "")
            {
                ret = false;
                MessageBox.Show("password must be filled in !");
                txtPasswordN.Select();
            }
            else if (txtUsernameN.Text.Trim() != "" && txtPasswordN.Text.Trim() != "")
            {
                if (usernameDecrypt != txtUsernameN.Text.Trim())
                {
                    ret = false;
                    MessageBox.Show("username was incorrect !");
                    txtUsernameN.Select();
                }
                else if (passDecrypt != txtPasswordN.Text.Trim())
                {
                    ret = false;
                    MessageBox.Show("invalid password ! ");
                    txtPasswordN.Select();
                }
            }
            else
            {
                return ret;
            }

            return ret;
        }

        public static string funDecrypt(string inputStr)
        {
            var key = "b14ca5898a4e4133bbce2ea2315a1916";
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(inputStr);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        public static string funEncrypt(string inputStr)
        {
            var key = "b14ca5898a4e4133bbce2ea2315a1916";
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(inputStr);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(array);
        }
    }
}
