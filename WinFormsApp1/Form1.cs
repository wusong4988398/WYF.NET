using System.Text;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent1();
        }

        private void InitializeComponent1()
        {
            this.tableLayoutPanel = new TableLayoutPanel();
            this.textBoxInput = new TextBox();
            this.buttonConvert = new Button();
            this.textBoxResult = new TextBox();

            // 设置窗体属性
            this.Text = "一如转换";
            this.Size = new System.Drawing.Size(400, 300); // 初始大小
      

            // 设置TableLayoutPanel
            this.tableLayoutPanel.ColumnCount = 1;
            this.tableLayoutPanel.RowCount = 3;
            this.tableLayoutPanel.Dock = DockStyle.Fill; // 填充整个窗体
            this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); // 占满整个宽度
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F)); // 输入框高度
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F)); // 按钮高度
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // 结果框占满剩余空间
            this.Controls.Add(this.tableLayoutPanel);

            // 设置输入TextBox
            this.textBoxInput.Dock = DockStyle.Fill;
            this.tableLayoutPanel.Controls.Add(this.textBoxInput, 0, 0);

            // 设置转换按钮
            this.buttonConvert.Text = "转换";
            this.buttonConvert.Dock = DockStyle.Fill;
            this.buttonConvert.Click += new EventHandler(this.ButtonConvert_Click);
            this.tableLayoutPanel.Controls.Add(this.buttonConvert, 0, 1);

            // 设置结果显示TextBox
            this.textBoxResult.Multiline = true; // 支持多行
            this.textBoxResult.ScrollBars = ScrollBars.Both; // 同时启用水平和垂直滚动条
            this.textBoxResult.ReadOnly = true; // 只读
            this.textBoxResult.WordWrap = false; // 不自动换行
            this.textBoxResult.Dock = DockStyle.Fill;
            this.tableLayoutPanel.Controls.Add(this.textBoxResult, 0, 2);
        }

        private TableLayoutPanel tableLayoutPanel;
        private TextBox textBoxInput;
        private Button buttonConvert;
        private TextBox textBoxResult;

        private void ButtonConvert_Click(object sender, EventArgs e)
        {
            string k = textBoxInput.Text;
            if (string.IsNullOrEmpty(k))
            {
                MessageBox.Show("请输入要转换的字符串。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            StringBuilder result = new StringBuilder();
            int results = 0;

            for (int i = 0; i < k.Length; i++)
            {
                results = i + 1;
                if (k[i] == '1')
                {
                    result.Append($"<font color='red'>{results}</font>");
                }
                else
                {
                    result.Append($"{results},");
                }

                // 重置results
                results = 0;
            }

            // 追加当前时间戳和新结果
            textBoxResult.AppendText($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {k}转换结果:\r\n{result.ToString()}\r\n\r\n");

            // 自动滚动到底部
            textBoxResult.SelectionStart = textBoxResult.TextLength;
            textBoxResult.ScrollToCaret();
        }
    }

}







