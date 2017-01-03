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
using System.Data.OracleClient;





namespace SqlToFrom
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            dataGridView1.ReadOnly = true; //设置三个数据窗口不可编辑
            dataGridView2.ReadOnly = true;
            dataGridView3.ReadOnly = true;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;//自动设置单元格格式,缩放列到合适宽度
            dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;//设置datagridview字段的高度以显示全部的内容;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView2.DefaultCellStyle.WrapMode = DataGridViewTriState.True;//设置datagridview字段的高度以显示全部的内容;
            dataGridView3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView3.DefaultCellStyle.WrapMode = DataGridViewTriState.True;//设置datagridview字段的高度以显示全部的内容;

        }


        private void label1_Click(object sender, EventArgs e) 
        {

        }

        //--------------------------------定义汉字转拼音首字母函数---开始-----------------
        static public string GetChineseSpell(string strText)
        {
            int len = strText.Length;
            string myStr = "";
            for (int i = 0; i < len; i++)
            {
                myStr += getSpell(strText.Substring(i, 1));
            }
            return myStr;
        }

        static public string getSpell(string cnChar)
        {
            byte[] arrCN = Encoding.Default.GetBytes(cnChar);
            if (arrCN.Length > 1)
            {
                int area = (short)arrCN[0];
                int pos = (short)arrCN[1];
                int code = (area << 8) + pos;
                int[] areacode = { 45217, 45253, 45761, 46318, 46826, 47010, 47297, 47614, 48119, 48119, 49062, 49324, 49896, 50371, 50614, 50622, 50906, 51387, 51446, 52218, 52698, 52698, 52698, 52980, 53689, 54481 };
                for (int i = 0; i < 26; i++)
                {
                    int max = 55290;
                    if (i != 25) max = areacode[i + 1];
                    if (areacode[i] <= code && code < max)
                    {
                        return Encoding.Default.GetString(new byte[] { (byte)(65 + i) });
                    }
                }
                return "*";
            }
            else return cnChar;
        }
        //--------------------------------定义汉字转拼音首字母函数-----结束---------------

        private void textBox1_TextChanged(object sender, EventArgs e) //单位输入框输入改变时
        {

            comboBox2.DataSource = null;//首先清空任务下拉框
            comboBox2.Items.Clear();

            //--------------------------------开始连接体检数据库--------------------------
            DataSet ds4 = new DataSet();
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "server=192.168.140.58;uid=sa;pwd=zoneking;database=JZCIS";
                conn.Open();
                using (SqlCommand command = conn.CreateCommand())
                {
                    String sql3 = @"select 
                                    DWPROJECT.ProjectID,
                                    DWPROJECT.projectName,
                                    DWMC.DWMC 
                               from  DWMC, DWPROJECT 
                               where  DWPROJECT.DWDM= DWMC.DWDM 
                               and (DWMC.DWDM='" + textBox1.Text + "' or DWMC.DWMC= '" + textBox1.Text + "')";//查看单位的体检任务
                    command.CommandText = sql3;
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    try
                    {
                        da.Fill(ds4);
                    }
                    catch
                    { }
                }
                conn.Close();
            }
            //--------------------------------断开体检数据库--------------------------
            DataTable dt2 = ds4.Tables[0];


            comboBox2.DataSource = dt2;
            comboBox2.DisplayMember = "projectName";  //要显示的字段名
            comboBox2.ValueMember = "ProjectID";





            // label5.Text = dt.Rows.Count.ToString();




            //label6.Text = comboBox1.SelectedItem.ToString();

            //label6.Text = comboBox1.Text;

            //如果是Web程序的话，加上comboBox1.DataBind();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) //当选择了单位的任务后
        {
            comboBox1.DataSource = null; //首先清空分组框
            comboBox1.Items.Clear();
            //--------------------------------开始连接体检数据库--------------------------
            DataSet ds3 = new DataSet();
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "server=192.168.140.58;uid=sa;pwd=zoneking;database=JZCIS";
                conn.Open();
                using (SqlCommand command = conn.CreateCommand())
                {
                    String sql2 = @"select DWFZ.DWFZMC, 
                                    DWFZ.DWFZDM, 
                                    DWPROJECT.ProjectID,
                                    DWPROJECT.projectName,
                                    DWMC.DWMC 
                               from DWFZ, DWMC, DWPROJECT 
                               where DWFZ.DWDM=DWMC.DWDM 
                               and DWPROJECT.ProjectID= DWFZ.ProjectID
                               and DWPROJECT.projectName='" + comboBox2.Text + @"'
                               and (DWFZ.DWDM='" + textBox1.Text + "' or DWMC.DWMC= '" + textBox1.Text + "')";//查看此次任务所有分组
                    command.CommandText = sql2;
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    try
                    {
                        da.Fill(ds3);
                    }
                    catch
                    { }
                }
                conn.Close();
            }
            //--------------------------------断开体检数据库--------------------------
            DataTable dt = ds3.Tables[0];

            comboBox1.DataSource = dt;
            comboBox1.DisplayMember = "DWFZMC";  //要显示的字段名
            comboBox1.ValueMember = "DWFZDM";

            if (dt.Rows.Count >= 1)//如果调用数据有内容时，显示单位名称
            {
                label5.Text = dt.Rows[0]["DWMC"].ToString();
            }
            else
            {
                label5.Text = "单位名称";
            }
        }

        private void button1_Click(object sender, EventArgs e)  //点击查询窗口
        {
            label6.Text = comboBox1.Text+"("+GetChineseSpell(comboBox1.Text)+")"; //右下角显示单位分组信息
            //--------体检数据库操作开始------
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "server=192.168.140.58;uid=sa;pwd=zoneking;database=JZCIS";
                conn.Open();
                using (SqlCommand command = conn.CreateCommand())
                {
                    command.CommandText = @"Select DWFZ.DWFZMC as 分组,XM as 姓名,JCXX.ID as 体检号,JCXX.CLINIC_NO as 门诊号,VIPID,JCXX.XB as 性别,JCXX.JDRQ as 建档日期,CYRQ as 采样日期,
Sum(JCSFXM.YJE) as 应收金额,JCXX.DJJE as 未检费用,Sum(JCSFXM.JE) as 实收金额,DWMC.DWMC as 单位名称,ProjectName as 任务名称 From JCXX
 Left Outer Join XMZH on XMZH.XMDM = JCXX.ZHXMDM
 Left Outer Join DWMC on DWMC.DWDM = JCXX.DWDM
 Left Outer Join GWMC on GWMC.GWDM = JCXX.GWDM
 Left Outer Join DWFZ on DWFZ.DWDM = JCXX.DWDM and DWFZ.DWFZDM = JCXX.DWFZDM
 Left Outer Join DWProject on DWFZ.ProjectID = DWProject.ProjectID
 Left Outer Join JCSFXM on JCXX.id = JCSFXM.id
 --Left Outer Join FZ_JCSFXM on FZ_JCSFXM.SFXMDM = JCSFXM.SFXMDM and FZ_JCSFXM.= JCSFXM.id
 Where 1 = 1
 and (JCXX.DWDM ='" + textBox1.Text + @"' or DWMC.DWMC='" + textBox1.Text + @"') 
 and JCSFXM.SFLBDM <> '999999' and JCSFXM.TFBJ = '0'
 and DWFZ.DWFZMC ='" + comboBox1.Text + @"'
 and DWProject.ProjectName ='" + comboBox2.Text + @"'
 group by JCXX.ID,VIPID,XM,JCXX.XB,JCXX.JDRQ,CYRQ,JCXX.JE ,JCXX.DJJE,DWMC.DWMC,ProjectName,JCXX.SSJE,JCXX.XJJE,JCSFXM.ID,DWFZ.DWFZMC,JCXX.CLINIC_NO
 Order By JCXX.ID";//获取体检任务的详情
                    SqlDataAdapter da = new SqlDataAdapter(command);

                    try
                    {
                        da.Fill(ds);
                    }
                    catch
                    { }
                }
                conn.Close();
            }
            DataTable dt = new DataTable();
            dt = ds.Tables[0].Copy();
            this.dataGridView1.DataSource = dt;  //绑定到datagridview中显示

            //--------体检数据库操作结束------


        }

  


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) //点击当前行
        {
            String tj_no = this.dataGridView1.CurrentRow.Cells[2].Value.ToString();//获取当前行第三列值（体检号）
            String his_no = this.dataGridView1.CurrentRow.Cells[3].Value.ToString();//获取当前行第四列值（his门诊号）
            String DWMC = this.dataGridView1.CurrentRow.Cells[12].Value.ToString();//获取当前行第十二列值（单位名称）

            // label5.Text = DWMC;

            //--------体检数据库操作开始------
            DataSet ds2 = new DataSet(); 
           
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "server=192.168.140.58;uid=sa;pwd=zoneking;database=JZCIS";
                conn.Open();
                using (SqlCommand command = conn.CreateCommand())
                {
                    String sql1 = @" Select JCXX.XM as 姓名,SFXMMC as 收费项目,JCSFXM.YJE as 应收,JCSFXM.JE as 实收 From JCSFXM Left Outer Join SFXM on JCSFXM.SFXMDM = SFXM.SFXMDM
Left Outer Join KSBM on SFXM.KSBM = KSBM.KSBM Left Outer Join YBDM On SFXM.YBDM = YBDM.YBDM Left Outer Join HYGLDM on SFXM.HYGLDM = HYGLDM.HYGLDM Left Outer Join JCXX on JCSFXM.ID= JCXX.ID
Where JCSFXM.ID = '" + tj_no + @"' and TFBJ = '0'
union all
select '','合计',sum(JCSFXM.YJE),sum(JCSFXM.JE) From JCSFXM Left Outer Join SFXM on JCSFXM.SFXMDM = SFXM.SFXMDM
Left Outer Join KSBM on SFXM.KSBM = KSBM.KSBM Left Outer Join YBDM On SFXM.YBDM = YBDM.YBDM Left Outer Join HYGLDM on SFXM.HYGLDM = HYGLDM.HYGLDM Left Outer Join JCXX on JCSFXM.ID= JCXX.ID
Where JCSFXM.ID = '" + tj_no + @"' and TFBJ = '0'
Order by JCSFXM.JE DESC
";//获取体检中病人体检的信息
                    command.CommandText = sql1;
                    SqlDataAdapter da = new SqlDataAdapter(command); //执行sql

                    try
                    {
                        da.Fill(ds2);
                    }
                    catch
                    { }
                }
                conn.Close();
            }
            DataTable dt = new DataTable();
            this.dataGridView3.DataSource = null;
            dt = ds2.Tables[0].Copy();
            this.dataGridView3.DataSource = dt; //得到的数据显示到dateGridView3
            //--------体检数据库操作结束------

            //--------------------------------开始连接oracle--------------------------

    

            //创建和数据库的连接
            OracleConnection oraCon = new OracleConnection("user id=PhonePlan;data source=his;password=i39");
            //新建一个DataAdapter用于填充DataSet
            string ora_sql = @"select 姓名,
                                      摘要 as 收费项目,
                                      to_char(sum(实收金额),'99999.9999') as 实收金额 
                                      from 门诊费用记录 
                             where 病人id in (select 病人id from 病人信息 where 门诊号=" + his_no + @") group by 姓名,摘要 
                    union all
                           select '','合计',to_char(sum(实收金额),'9999.9999') from
                                     (select 姓名,
                                      摘要 as 收费项目,
                                       sum(实收金额) as 实收金额 
                                      from 门诊费用记录 
                             where 病人id in (select 病人id from 病人信息 where 门诊号=" + his_no + @") group by 姓名,摘要 )
                
                order by 实收金额 desc";//查看his病人的体检费用信息
            OracleDataAdapter oraDap = new OracleDataAdapter(ora_sql, oraCon);
            //新建一个DataSet
            DataSet ds = new DataSet();
            //填充DataSet
            oraDap.Fill(ds);
            //新建一个DataTable
            DataTable _table = ds.Tables[0];
            //查看表中数据的列数
            int count = _table.Rows.Count;
            dataGridView2.DataSource = _table;

            String name = this.dataGridView2.Rows[1].Cells[0].Value.ToString();//获取his当前行第二列值（病人姓名）
            label6.Text = comboBox1.Text + "---姓名： " + name; //设置右下角显示当前正在操作的病人
            oraCon.Close();
            //--------------------------------关闭oracle连接--------------------------
        }


        private void label2_Click(object sender, EventArgs e)
        {

        }



        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_Load(object sender, EventArgs e)
        {
  

        
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: 这行代码将数据加载到表“jZCISDataSet.DWMC”中。您可以根据需要移动或删除它。
            this.dWMCTableAdapter.Fill(this.jZCISDataSet.DWMC);

          

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

       
    }
}
