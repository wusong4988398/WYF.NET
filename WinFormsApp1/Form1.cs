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

            // ���ô�������
            this.Text = "һ��ת��";
            this.Size = new System.Drawing.Size(400, 300); // ��ʼ��С
      

            // ����TableLayoutPanel
            this.tableLayoutPanel.ColumnCount = 1;
            this.tableLayoutPanel.RowCount = 3;
            this.tableLayoutPanel.Dock = DockStyle.Fill; // �����������
            this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); // ռ���������
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F)); // �����߶�
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F)); // ��ť�߶�
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // �����ռ��ʣ��ռ�
            this.Controls.Add(this.tableLayoutPanel);

            // ��������TextBox
            this.textBoxInput.Dock = DockStyle.Fill;
            this.tableLayoutPanel.Controls.Add(this.textBoxInput, 0, 0);

            // ����ת����ť
            this.buttonConvert.Text = "ת��";
            this.buttonConvert.Dock = DockStyle.Fill;
            this.buttonConvert.Click += new EventHandler(this.ButtonConvert_Click);
            this.tableLayoutPanel.Controls.Add(this.buttonConvert, 0, 1);

            // ���ý����ʾTextBox
            this.textBoxResult.Multiline = true; // ֧�ֶ���
            this.textBoxResult.ScrollBars = ScrollBars.Both; // ͬʱ����ˮƽ�ʹ�ֱ������
            this.textBoxResult.ReadOnly = true; // ֻ��
            this.textBoxResult.WordWrap = false; // ���Զ�����
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
                MessageBox.Show("������Ҫת�����ַ�����", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

                // ����results
                results = 0;
            }

            // ׷�ӵ�ǰʱ������½��
            textBoxResult.AppendText($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {k}ת�����:\r\n{result.ToString()}\r\n\r\n");

            // �Զ��������ײ�
            textBoxResult.SelectionStart = textBoxResult.TextLength;
            textBoxResult.ScrollToCaret();
        }
    }

}







