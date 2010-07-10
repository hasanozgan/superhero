using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using DataLayer;
using Netology.Helpers;
using System.IO;

namespace Netology.SuperHero
{
    public partial class frmMain : Form
    {
        public TableList tableList;

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            button1.Enabled = false;
            tableList = Table.GetTables();

            listBox1.Items.Clear();
            foreach (Table tbl in tableList)
            {
                string line = string.Format("[{0}].[{1}].[{2}]", tbl.Catalog, tbl.Schema, tbl.Name);

                listBox1.Items.Add(line);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string allStoredProcedures = string.Empty;
            string generateDir = textBox1.Text.Trim().Trim(new char[] { '/' }) + "/DataAccessLayer/";
            try
            {
                DirectoryInfo dirInfo = Directory.CreateDirectory(generateDir);
                DirectoryInfo dirInfo2 = Directory.CreateDirectory(generateDir+"GeneratedCode");
                DirectoryInfo dirInfo3 = Directory.CreateDirectory(generateDir+"UserCode");
                DirectoryInfo dirInfo4 = Directory.CreateDirectory(generateDir+"SQLFiles");
            }
            catch
            {
                MessageBox.Show("Directory not valid or check permission !");
                return;
            }

            this.Size = new Size(this.Size.Width, 287);
            progressBar1.Visible = true;

            for (int i = 0; i < tableList.Count; i++)
            {
                tableList[i].ClassFile = CodeGenerator.PrepareTableClass(tableList[i]);

                tableList[i].UserClassFile = CodeGenerator.PrepareUserTableClass(tableList[i]);

                tableList[i].StoredProcedures = string.Empty;                
                tableList[i].StoredProcedures += CodeGenerator.PrepareInsertStoredProcedure(tableList[i]);
                tableList[i].StoredProcedures += CodeGenerator.PrepareUpdateStoredProcedure(tableList[i]);
                tableList[i].StoredProcedures += CodeGenerator.PrepareDeleteStoredProcedure(tableList[i]);
                tableList[i].StoredProcedures += CodeGenerator.PrepareSelectStoredProcedure(tableList[i]);
                tableList[i].StoredProcedures += CodeGenerator.PrepareGetListStoredProcedure(tableList[i]);
                allStoredProcedures += tableList[i].StoredProcedures;

                string classFile = string.Format("{0}/GeneratedCode/{1}.cs", generateDir, tableList[i].Name);
                TextWriter tw = new StreamWriter(classFile);
                tw.WriteLine(tableList[i].ClassFile);
                tw.Close();

                string userClassFile = string.Format("{0}/UserCode/{1}.cs", generateDir, tableList[i].Name);
                if (!File.Exists(userClassFile))
                {
                    tw = new StreamWriter(userClassFile);
                    tw.WriteLine(tableList[i].UserClassFile);
                    tw.Close();
                }
                
                string sqlFile = string.Format("{0}/SQLFiles/{1}.sql", generateDir, tableList[i].Name);
                tw = new StreamWriter(sqlFile);
                tw.WriteLine(tableList[i].StoredProcedures);
                tw.Close();

                label2.Text = tableList[i].Name;
                label1.Refresh();
                label2.Refresh();
                progressBar1.Value = (int)Math.Ceiling(Convert.ToDouble((i+1) * 100 / tableList.Count));
            }

            string allSP = string.Format("{0}/SQLFiles/SP_AllQueries.sql", generateDir);
            TextWriter tw2 = new StreamWriter(allSP);
            tw2.WriteLine(allStoredProcedures);
            tw2.Close();

            File.Copy(CodeGenerator.TemplatesPath + "/DAL.cs.tpl", generateDir+"/DAL.cs", true);
            File.Copy(CodeGenerator.TemplatesPath + "/DALBase.cs.tpl", generateDir + "/DALBase.cs", true);

            MessageBox.Show("Files generated succees!");

            this.Size = new Size(this.Size.Width, 258);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            textBox1.Text = folderBrowserDialog1.SelectedPath;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
                button1.Enabled = true;
            else
                button1.Enabled = false;
        }
    }
}