using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Text.RegularExpressions;
using System.Resources;

namespace Bingofun
{
    public partial class lotto : Form
    {
        public static Random R = new Random();
        int selected = 0;
        int[] number = new int[6];
        int[] lotto = new int[7];
        List<int[]> auto_numbers = new List<int[]>();
        List<int[]> bingo_number = new List<int[]>();
        Dictionary<string, Button> allbtn = new Dictionary<string, Button>();
        long T_獎金 = 10000000;
        long W_獎金 = 0;
        
        int string_run=0;   //跑馬燈左右控制
        
        public string c_lotto;  //第二個視窗回傳值
        public string C_lotto 
        {
            set
            {
                c_lotto = value;
                descrip.Enabled = true;
            }
            get
            {
                return c_lotto;
            }

        }
        public Customer()
        {
            InitializeComponent();
            Closing += new CancelEventHandler(this.Customer_Closing);
            
            for (int i=0;i<Controls.Count;i++)
                if(Controls[i] is Button)
                    allbtn.Add(Controls[i].Text, Controls[i] as Button);

            
        }
        private void Customer_Closing(Object sender, CancelEventArgs e)
        {
            DialogResult dr = MessageBox.Show("確定要關閉程式嗎?",                "關閉程式", MessageBoxButtons.YesNo);
            if (dr == DialogResult.No)
                e.Cancel = true;//取消離開
            else
                e.Cancel = false;
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

                if (isnumber && is是否號碼被取過(parsemart,number))
                {
                    for (int i = 0; i < 6; i++)
                    {
                        if(parsemart == number[i])
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
                    for(int i=0;i<6;i++)
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
        private void Control_Mouseclick(Object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;
            
            if (btn.Name.Contains("button") && open.Enabled)
                Control_自選號(btn);
            if (btn.Name.Contains("next") && open.Enabled)
                Control_下一個();
            if (btn.Name.Contains("auto_select") && open.Enabled)
                Control_電腦選號();
            if (btn.Name.Contains("open"))
                Control_開獎();
            if (btn.Name.Contains("clear"))
                Control_清除();
            if(btn.Name.Contains("descrip"))
            {
                Control C = new Control();
                C.Show(this);
                C_lotto = C.Lotto;
                btn.Enabled = false;
            }
            
            bouns.Text = "總獎金:" + T_獎金;
            timer1.Enabled = open.Enabled;
        }
        public void Control_自選號(Button btn)
        {
            string s = "";
            s = btn.Text;
            btncolorchange(btn);
            Number = s;
            label1.Text = "自選號: " + number轉字串(Getnumber());
        }
        public void Control_下一個()
        {
            var n = new int[6]; //必須配置記憶體否則List會一直參考number
            Getnumber().CopyTo(n, 0);
            
            if (this.selected < 6)
            {
                fn亂數取號(this.selected, n);
            }
            sort(n);
            this.auto_numbers.Add(n);
            var s = auto_numbers.Count + ": " + number轉字串(n);
            this.listBox1.Items.Add(s);
            
            foreach (var i in Getnumber())
            {
                if (i != 0)
                {
                    allbtn[i.ToString()].BackColor = Color.SkyBlue;
                    Number = i.ToString();
                } 
            }
            
            label1.Text = "自選號: "+Number;
            T_獎金 += 50;
        }
        public void Control_開獎()
        {
            this.open.Enabled = false;
            this.next.Enabled = false;
            this.auto_select.Enabled = false;

            Control_lotto();

            label3.Text += number轉字串(lotto);
            if (auto_numbers.Count == 0)
                Control_下一個();
            fn對獎();
            show_listbox();
        }
        public void Control_電腦選號()
        {
            if (series.Value > 0)
            {
                auto_select.Enabled = false;
                listBox1.Items.Add("電腦選號:");
                int i = 0;
                while (i < series.Value)
                {
                    string s = "";
                    int[] n = new int[6];
                    fn亂數取號(0, n);
                    sort(n);
                    auto_numbers.Add(n);

                    s = this.listBox1.Items.Count + ": " + number轉字串(n);
                    listBox1.Items.Add(s);
                    i++;
                }
                T_獎金 = T_獎金+ (int)(series.Value * 50);
            }
            
        }
        public void Control_清除()
        {
            this.label1.Text = "自選號: ";
            this.label3.Text = "開獎號: ";
            this.open.Enabled = true;
            this.next.Enabled = true;
            this.auto_select.Enabled = true;

            foreach (var i in Getnumber())
            {
                if(i>0 && i<50)
                {
                    allbtn[i.ToString()].BackColor = Color.SkyBlue;
                    Number = i.ToString();
                }
            }
            lotto = new int[7];
            this.auto_numbers.Clear();
            this.bingo_number.Clear();
            this.listBox1.Items.Clear();
            this.listBox1.Items.Add("號碼區");
            this.listBox2.Items.Clear();
            this.listBox2.Items.Add("開獎區");
            this.selected = 0;
            this.series.Value = 0;
            T_獎金 = T_獎金 < 10000000 ? 10000000 : T_獎金;
            W_獎金 = 0;
        }
        public void show_listbox()
        {
            listBox1.Items.Clear();
            listBox1.Items.Add("開獎號碼:");
            listBox1.Items.Add(" " + number轉字串(lotto));
            listBox1.Items.Add($"您有{auto_numbers.Count}個號碼，共花了{auto_numbers.Count*50}元");
            listBox1.Items.Add("你的號碼:");
            foreach (var num in auto_numbers)
            {
                this.listBox1.Items.Add(listBox1.Items.Count - 3 + ": " + number轉字串(num));
            }
            listBox1.Items.Add("---------------------------------------");
            long win = 0;
            if (bingo_number.Count > 0)
            {
                listBox2.Items.Add("開獎號碼:");
                listBox2.Items.Add(number轉字串(lotto));
                listBox2.Items.Add("中獎號碼");
                
                foreach(var num in bingo_number)
                {
                    switch (num[0])
                    {
                        case 3:
                            win += 400;
                            break;
                        case 4:
                            win += 2000;
                            break;
                        case 5:
                            win += (long)(T_獎金 * 0.09);
                            break;
                        case 6:
                            win += (long)(T_獎金 * 0.58);
                            break;
                        default: break;
                    }
                    var s = $"中{ num[0] }個號碼: ";
                    num[0] = 0;
                    s += number轉字串(num);
                    listBox2.Items.Add(s);
                }
                listBox2.Items.Add($"您獲得獎金 {W_獎金} 元");
                T_獎金 -= W_獎金;
            }
            if((W_獎金 - auto_numbers.Count * 50) <0)
            {
                if (bingo_number.Count < 1)
                    listBox2.Items.Add("非常遺憾，您沒有中獎");
                listBox2.Items.Add($"您總共輸了 {auto_numbers.Count * 50 - W_獎金} 元");
            }
            else
                listBox2.Items.Add($"您總共贏得 {W_獎金 - auto_numbers.Count * 50} 元");
        }
        public void listbox_點擊(object sender, EventArgs e)
        {
            foreach (var i in Getnumber())
            {
                if (i > 0 && i < 50)
                    allbtn[i.ToString()].BackColor = Color.SkyBlue;
                Number = i.ToString();
            }
            selected = 0;
            this.label1.Text = "自選號: ";
            ListBox lb = sender as ListBox;

            if (lb.SelectedItem != null)
            {
                string s = lb.SelectedItem.ToString();
                string[] element = s.Split((new char[] { ' ' }), StringSplitOptions.RemoveEmptyEntries);
                foreach (var i in element)
                {
                    if (int.TryParse(i, out int n))
                    {
                        if (n > 0 && n < 50)
                        {
                            allbtn[n.ToString()].BackColor = Color.Yellow;
                            Number = i;
                        }
                    }
                }
                selected = 6;
                label1.Text += Number;
            }
        }
        public void btncolorchange(Button btn)
        {
            if (selected < 6)
            {
                if (btn.BackColor != Color.Yellow)
                {
                    btn.BackColor = Color.Yellow;
                }
                else
                    btn.BackColor = Color.SkyBlue;
            }
            else
                btn.BackColor = Color.SkyBlue;
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
        public void fn對獎()
        {
            foreach (var an in auto_numbers)
            {
                var bingo = an.Intersect(lotto).ToArray();
                var temp = new int[7];                
                if(bingo.Length>=3)
                {
                    temp[0] = bingo.Length;
                    Array.Copy(an, 0, temp, 1, an.Length);
                    bingo_number.Add(temp);
                }
                switch (bingo.Length)
                {
                    case 3:
                        W_獎金 += 400;
                        break;
                    case 4:
                        if (an.Contains(lotto[6]))
                            W_獎金 += 1000;
                        else
                            W_獎金 += 2000;
                        break;
                    case 5:
                        if (an.Contains(lotto[6]))
                            W_獎金 += (long)(T_獎金 * 4.5 / 100);
                        else
                            W_獎金 += (long)(T_獎金 * 7 / 100);
                        break;
                    case 6:
                        if (an.Contains(lotto[6]))
                            W_獎金 += (long)(T_獎金 * 6.5 / 100);
                        else
                            W_獎金 += (long)(T_獎金 * 82 / 100);
                        break;
                }
            }
        }
        public void fn亂數取號(int count, int[] num)
        {
            while (count < 6)
            {
                int n = (int)(R.NextDouble() * 100);
                if (n > 0 && n < 50 && !is是否號碼被取過(n, num))
                {
                    for(int i =0;i<6;i++)
                    {
                        if(num[i]==0)
                        {
                            num[i] = n;
                            count++;
                            break;
                        }
                    }
                }
            }
        }
        public int[] sort(int[] num)
        {
            for (int i = 0; i < num.Length; i++)
            {
                for (int j = 0; j < num.Length - 1; j++)
                {
                    if (num[j + 1] < num[j])
                    {
                        int big = num[j + 1];
                        num[j + 1] = num[j];
                        num[j] = big;
                    }
                }
            }
            return num;
        }
        public string number轉字串(int[] n)
        {
            string s = "";
            for (int i = 0; i < n.Length; i++)
            {
                if (n[i] > 9) s += n[i] + "  ";
                else if (n[i] > 0) s += "0" + n[i] + "  ";
            }
            return s;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (bouns.Left > 0 && string_run==1)
            {
                bouns.Left -= 3;
            }
            else
            {
                bouns.Left += 3;
                string_run = 0;
                if ((bouns.Left + bouns.Width) >= pictureBox1.Left)
                    string_run = 1;
            }
        }
        private void Control_lotto()
        {
            //視窗二回傳
            if (C_lotto != null)
            {
                string[] element = C_lotto.Split((new char[] { ' ' }), StringSplitOptions.RemoveEmptyEntries);
                int Length= 0;
                for (int i = 0; i < element.Length; i++)
                {
                    if (int.TryParse(element[i], out int n))
                    {
                        if (n > 0 && n < 50)
                        {
                            lotto[i] = n;
                            Length++;
                        }
                        else 
                            break;
                    }
                }
                if (Length < 7)
                {
                    lotto[6] = 50;
                    fn亂數取號(Length, lotto);
                    sort(lotto);
                    bool YorN = true;//取特別號
                    while (YorN)
                    {
                        int n = (int)(R.NextDouble() * 100);
                        if (n > 0 && n < 50 && !is是否號碼被取過(n, lotto) && YorN)
                        {
                            lotto[6] = n;
                            YorN = false;
                        }
                    }
                }
                C_lotto = null;
            }
            else
            {
                lotto[6] = 50;
                fn亂數取號(0, lotto);
                sort(lotto);
                bool YorN = true;//取特別號
                while (YorN)
                {
                    int n = (int)(R.NextDouble() * 100);
                    if (n > 0 && n < 50 && !is是否號碼被取過(n, lotto) && YorN)
                    {
                        lotto[6] = n;
                        YorN = false;
                    }
                }
            }
        }
    }
}
