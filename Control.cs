using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bingofun
{
    public partial class Control : Form
    {
        string[][] user = { new string[] { "ad", "0000" } ,
                        new string[] { "admin", "01234" }};
        bool Islogin = false;
        int[] number = new int[7];
        int selected = 0;
        List<Button> number_buttons = new List<Button>();
        public string Lotto
        {
            set { open.Text = value; }
            get { return open.Text; }
        }
        public Control()
        {
            InitializeComponent();
        }
        private void Control_Mouseclick(Object sender, MouseEventArgs e)
        {
            if(sender is Button)
            {
                Button btn = (Button)sender;

                if (btn.Name.Contains("button") && open.Enabled)
                    Control_自選號(btn);
                if (btn.Name.Contains("clear"))
                    Control_重選();
            }
          
        }
        public void Control_自選號(Button btn)
        {
            string s = "";
            s = btn.Text;
            btncolorchange(btn);
            Number = s;
            open.Text = "開獎號: " + number轉字串(Getnumber());
        }
        public void Control_重選()
        {
            foreach (var i in Getnumber())
                if (i > 0 && i < 50)
                    Number = i.ToString();

            foreach(var i in number_buttons)
                i.BackColor =  Color.SkyBlue;
            open.Text = "開獎號: ";
        }
        public string Number
        {
            get
            {
                return number轉字串(number);
            }
            set
            {
                bool isnumber = int.TryParse(value, out int parsemart);

                if (isnumber && is是否號碼被取過(parsemart, number))
                {
                    for (int i = 0; i < 7; i++)
                    {
                        if (parsemart == number[i])
                        {
                            number[i] = 0;
                            selected--;
                            isnumber = false;
                            break;
                        }
                    }
                }
                if (isnumber)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        if (number[i] == 0)
                        {
                            number[i] = parsemart;
                            selected++;
                            break;
                        }
                    }
                }
            }
        }
        public int[] Getnumber()
        {
            return number;
        }
        public void btncolorchange(Button btn)
        {
            if (selected < 7)
            {
                if (btn.BackColor != Color.Yellow)
                {
                    number_buttons.Add(btn);
                    btn.BackColor = Color.Yellow;
                }
                else
                    btn.BackColor = Color.SkyBlue;
            }
            else
                btn.BackColor = Color.SkyBlue;
        }
        public string number轉字串(int[] n)
        {
            string s = "";
            for (int i = 0; i < n.Length; i++)
            {
                if (n[i] > 9) s += n[i] + "  ";
                else if (n[i] > 0) s += "0" + n[i] + "  ";
                else if(n[i] == 0) s += "00" + "  ";
            }
            return s;
        }
        public bool is是否號碼被取過(int p, int[] NOs)
        {
            if (p <= 0)
                return false;
            foreach (int i in NOs)
            {
                if (p == i)
                    return true;
            }
            return false;
        }
        private void text_boxclick(Object sender, MouseEventArgs e)
        {
            (sender as TextBox).Text = "";
        }
        private void login_Click(object sender, EventArgs e)
        {
            string[] ap = { textBox2.Text, textBox1.Text };
            
            foreach (var n in user)
                Islogin = n.All(u => ap.Any(a => a.Equals(u)));
            if(Islogin)
            {
                richTextBox1.Hide();
                groupBox1.Visible = true;
            }
        }
        private void cancel_Click(object sender, EventArgs e)
        {
            string str = number轉字串(Getnumber());
            Console.WriteLine(str);
            Customer s = (Customer)this.Owner;
            s.C_lotto = str;
            this.Close();
        }
    }
}
