using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DeleteUnusedDocAcumatica.ServicesAcum;
using System.IO;
using System.Reflection;
using ComponentFactory.Krypton.Toolkit;

namespace DeleteUnusedDocAcumatica
{
    public partial class frmMain : KryptonForm
    {
        DAC.DataAccessClass dacCls = new DAC.DataAccessClass();
        ServicesAcum.Screen context = new ServicesAcum.Screen();
        public string serverName, dbName, userDb, passDb, url, loginAcum, passAcum;

        public string strSelect;
        DataTable dtTbl = new DataTable();

        public frmMain()
        {
            InitializeComponent();
            serverName = Properties.Settings.Default.serverName;
            dbName = Properties.Settings.Default.databaseName;
            userDb = Properties.Settings.Default.userDb;
            passDb = Properties.Settings.Default.passDb;
            url = Properties.Settings.Default.serviceUrl;
            loginAcum = Properties.Settings.Default.loginAcumatica;
            passAcum = Properties.Settings.Default.passAcumatica;
        }

        private void searchData(string dcstatus, string dctype)
        {
            if (cmbFinPeriod.Text.Trim() == "")
                searchDataByYear(dcstatus, dctype, cmbFinYear.Text.Trim(), "");
            else
                searchcDataByFinPeriod(dcstatus, dctype, cmbFinYear.Text.Trim(), cmbFinPeriod.Text.Trim());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dgvData.GetType().GetProperty("DoubleBuffered", 
                                        BindingFlags.Instance | BindingFlags.NonPublic).SetValue(dgvData, true, null);

            funRetrieveFromConfig();
        }

        private void funRetrieveFromConfig()
        {
            txtUrlWebServiceN.Text = url;
            txtServerNameN.Text = serverName;
            txtDbNameN.Text = dbName;
        }

        private void rbBillAdj_CheckedChanged(object sender, EventArgs e)
        {
            cmbDocType.Text = "";
            cmbDocType.Items.Clear();
            if (rbBillAdj.Checked)
            {
                cmbDocType.Items.Clear();
                cmbDocType.Items.Add("Bill");
                cmbDocType.Items.Add("Prepayment");
                cmbDocType.Items.Add("Credit Adj.");
                cmbDocType.Items.Add("Debit Adj.");
            }
        }
        private void rbCheckPay_CheckedChanged(object sender, EventArgs e)
        {
            cmbDocType.Text = "";
            cmbDocType.Items.Clear();
            if (rbCheckPay.Checked)
            {
                cmbDocType.Items.Clear();
                cmbDocType.Items.Add("Check");
                cmbDocType.Items.Add("Vendor Refund");
                cmbDocType.Items.Add("Voided Refund");
                cmbDocType.Items.Add("Voided Check");
            }
        }


        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.ExitThread();
        }

        private void btnSearchN_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            string dcStatus = "";
            string dcType = "";
            if (cmbFinYear.Text.Trim() == "")
            {
                MessageBox.Show("Please select Fin. Year first !");
                cmbFinYear.Select();
            }
            else if (!rbBillAdj.Checked && !rbCheckPay.Checked)
            {
                MessageBox.Show("Please select Screen Type, Bills and Adjusments or Checks and Payments ? ");
                pnlDoc.Select();
            }
            else if (cmbDocType.Text.Trim() == "")
            {
                MessageBox.Show("Please select Doc. Type first !");
                cmbDocType.Select();
            }
            else if (!rbBal.Checked && !rbHld.Checked)
            {
                MessageBox.Show("Please select Document Status, Hold or Balanced ? ");
            }
            else
            {
                if (rbHld.Checked)
                    dcStatus = "H";
                else if (rbBal.Checked)
                    dcStatus = "B";
                if (cmbDocType.Text.Trim() == "Bill")
                    dcType = "INV";
                else if (cmbDocType.Text.Trim() == "Prepayment")
                    dcType = "PPM";
                else if (cmbDocType.Text.Trim() == "Credit Adj.")
                    dcType = "ACR";
                else if (cmbDocType.Text.Trim() == "Debit Adj.")
                    dcType = "ADR";
                else if (cmbDocType.Text.Trim() == "Check")
                    dcType = "CHK";
                else if (cmbDocType.Text.Trim() == "Vendor Refund")
                    dcType = "REF";
                else if (cmbDocType.Text.Trim() == "Voided Check")
                    dcType = "VCK";
                searchData(dcStatus, dcType);
            }
            Cursor = Cursors.Default;
        }

        private void btnDeleteAllN_Click(object sender, EventArgs e)
        {
            if (dgvData.Rows.Count == 0)
                MessageBox.Show("Data Grid is Blank, please search data first.");

            DialogResult dialogResult = MessageBox.Show("Are you sure will delete all the following records ?",
                                                        "Delete Documents",
                                                        MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                if (actionDeleteDoc()){ MessageBox.Show("Document has been deleted."); dgvData.Rows.Clear(); }    
                else
                    MessageBox.Show("Some data cannot be deleted.");
            }
            else if (dialogResult == DialogResult.No)
            {
                return;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmLogin frmLogin = new frmLogin();
            frmLogin.Show();
        }

        private void btnMaximize_Click(object sender, EventArgs e)
        {
            
        }

        private void cmbFinYear_DropDown(object sender, EventArgs e)
        {
            DAC.QueryStatements qry = new DAC.QueryStatements();
            qry.getQuery = DAC.QueryStatements.getStringQueryFin(0, "");
            dtTbl = DAC.DataAccessClass.getDataTable(serverName, dbName, userDb, passDb, qry.getQuery);
            cmbFinYear.Items.Clear();
            if (dtTbl.Rows.Count > 0)
                for (int x = 0; x <= dtTbl.Rows.Count - 1; x++)
                    cmbFinYear.Items.Add(dtTbl.Rows[x]["Year"].ToString().Trim());
        }

        private void cmbFinPeriod_DropDown(object sender, EventArgs e)
        {
            if (cmbFinYear.Text.Trim() == "")
            {
                MessageBox.Show("Please select Fin. Year first !");
                cmbFinYear.Select();
            }
            else
            {
                DAC.QueryStatements qry = new DAC.QueryStatements();
                qry.getQuery = DAC.QueryStatements.getStringQueryFin(1, cmbFinYear.Text.Trim());
                dtTbl = DAC.DataAccessClass.getDataTable(serverName, dbName, userDb, passDb, qry.getQuery);
                cmbFinPeriod.Items.Clear();
                if (dtTbl.Rows.Count > 0)
                    for (int x = 0; x <= dtTbl.Rows.Count - 1; x++)
                        cmbFinPeriod.Items.Add(dtTbl.Rows[x]["FinPeriodID"].ToString().Trim());
            }
        }

        
        private void cmbFinPeriod_DropDownClosed(object sender, EventArgs e)
        {
            cmbFinYear.Select();
        }

        private void cmbFinPeriodN_DropDownClosed(object sender, EventArgs e)
        {
            cmbFinYearN.Select();
        }

        private void searchcDataByFinPeriod(string docStatus, string docType, string finYear, string finPeriod)
        {
            DAC.QueryStatements qry = new DAC.QueryStatements();
            qry.getQuery = DAC.QueryStatements.getStringQueryData(1, finYear, finPeriod, docType, docStatus);
            dtTbl = DAC.DataAccessClass.getDataTable(serverName, dbName, userDb, passDb,qry.getQuery);
            dgvData.Rows.Clear();
            if (dtTbl.Rows.Count > 0)
            {
                for (int x = 0; x <= dtTbl.Rows.Count - 1; x++)
                {
                    dgvData.Rows.Add();
                    dgvData.Rows[x].Cells["colRecNo"].Value = (x + 1).ToString();
                    dgvData.Rows[x].Cells["colDocType"].Value = dtTbl.Rows[x]["DocType"].ToString();
                    dgvData.Rows[x].Cells["colRefNbr"].Value = dtTbl.Rows[x]["RefNbr"].ToString();
                    dgvData.Rows[x].Cells["colDocDate"].Value = dtTbl.Rows[x]["DocDate"].ToString();
                    dgvData.Rows[x].Cells["colFinPeriod"].Value = dtTbl.Rows[x]["FinPeriodID"].ToString();
                    dgvData.Rows[x].Cells["colInvoiceNbr"].Value = dtTbl.Rows[x]["InvoiceNbr"].ToString();
                    dgvData.Rows[x].Cells["colVendorCode"].Value = dtTbl.Rows[x]["AcctCD"].ToString();
                    dgvData.Rows[x].Cells["colVendorName"].Value = dtTbl.Rows[x]["AcctName"].ToString();
                    dgvData.Rows[x].Cells["colAmountDocBal"].Value = Convert.ToDecimal(dtTbl.Rows[x]["CuryOrigDocAmt"]).ToString("N2");
                    dgvData.Rows[x].Cells["colStatus"].Value = dtTbl.Rows[x]["Status"].ToString();
                }
                txtTotalRecordsN.Text = Convert.ToDecimal(dtTbl.Rows.Count).ToString("N0");
            }
            else
            {
                txtTotalRecordsN.Text = "0";
                MessageBox.Show("Data not found !");
            }
        }

        private void searchDataByYear(string docStatus, string docType, string finYear, string finPeriod)
        {
            DAC.QueryStatements qry = new DAC.QueryStatements();
            if(docType == "CHK" || docType == "VCK" || docType == "REF")
                qry.getQuery = DAC.QueryStatements.getStringQueryData(2, finYear, finPeriod, docType, docStatus);
            else
                qry.getQuery = DAC.QueryStatements.getStringQueryData(0, finYear, finPeriod, docType, docStatus);

            dtTbl = DAC.DataAccessClass.getDataTable(serverName, dbName, userDb, passDb, qry.getQuery);
            dgvData.Rows.Clear();
            if (dtTbl.Rows.Count > 0)
            {
                for (int x = 0; x <= dtTbl.Rows.Count - 1; x++)
                {
                    dgvData.Rows.Add();
                    dgvData.Rows[x].Cells["colRecNo"].Value = (x + 1).ToString();
                    dgvData.Rows[x].Cells["colDocType"].Value = dtTbl.Rows[x]["DocType"].ToString();
                    dgvData.Rows[x].Cells["colRefNbr"].Value = dtTbl.Rows[x]["RefNbr"].ToString();
                    dgvData.Rows[x].Cells["colDocDate"].Value = dtTbl.Rows[x]["DocDate"].ToString();
                    dgvData.Rows[x].Cells["colFinPeriod"].Value = dtTbl.Rows[x]["FinPeriodID"].ToString();
                    dgvData.Rows[x].Cells["colInvoiceNbr"].Value = dtTbl.Rows[x]["InvoiceNbr"].ToString();
                    dgvData.Rows[x].Cells["colVendorCode"].Value = dtTbl.Rows[x]["AcctCD"].ToString();
                    dgvData.Rows[x].Cells["colVendorName"].Value = dtTbl.Rows[x]["AcctName"].ToString();
                    dgvData.Rows[x].Cells["colAmountDocBal"].Value = Convert.ToDecimal(dtTbl.Rows[x]["CuryOrigDocAmt"]).ToString("N2");
                    dgvData.Rows[x].Cells["colStatus"].Value = dtTbl.Rows[x]["Status"].ToString();
                }
                txtTotalRecordsN.Text = Convert.ToDecimal(dtTbl.Rows.Count).ToString("N0");
            }
            else
            {
                txtTotalRecordsN.Text = "0";
                MessageBox.Show("Data not found !");
            }
        }

        private void btnDeleteAll_Click(object sender, EventArgs e)
        {
            if (dgvData.Rows.Count == 0)
                MessageBox.Show("Data Grid is Blank, please search data first.");

            DialogResult dialogResult = MessageBox.Show("Are you sure will delete all the following records ?", 
                                                        "Delete Documents", 
                                                        MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                if (actionDeleteDoc())
                    MessageBox.Show("Document has been deleted.");
                else
                    MessageBox.Show("Some data cannot be deleted.");
            }
            else if (dialogResult == DialogResult.No)
            {
                return;
            }
        }

        private bool actionDeleteDoc()
        {
            Cursor = Cursors.WaitCursor;

            bool retFlg = true;
            if (dgvData.Rows.Count > 0)
            {
                for (int x = 0; x <= dgvData.Rows.Count - 1; x++)
                {
                    if (!DAC.DataAccessClass.getLoginAcumatica(context, url, loginAcum, passAcum))
                    {
                        MessageBox.Show("Cannot connect to the Acumatica System.");
                        retFlg = false;
                    }
                    else
                    {
                        if(rbBillAdj.Checked)
                        {
                            #region Bill and Adjustments
                            AP301000Content konten = context.AP301000GetSchema();
                            List<Command> oCmds = new List<Command>();
                            oCmds.Add(new Value { Value = cmbDocType.Text.Trim(), LinkedCommand = konten.DocumentSummary.Type });
                            oCmds.Add(new Value { Value = dgvData.Rows[x].Cells["colRefNbr"].Value.ToString().Trim(), LinkedCommand = konten.DocumentSummary.ReferenceNbr });
                            oCmds.Add(konten.Actions.Delete);
                            try
                            {
                                context.AP301000Submit(oCmds.ToArray());
                                DAC.DataAccessClass.getLogoutAcumatica(context);
                                retFlg = true;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                                DAC.DataAccessClass.getLogoutAcumatica(context);
                                retFlg = false;
                                return retFlg;
                            }
                            #endregion
                        }
                        else if(rbCheckPay.Checked)
                        {
                            #region Checks and Payments
                            AP302000Content konten = context.AP302000GetSchema();
                            List<Command> oCmds = new List<Command>();
                            oCmds.Add(new Value { Value = cmbDocType.Text.Trim(), LinkedCommand = konten.PaymentSummary.Type });
                            if(rbBalance.Checked)
                                oCmds.Add(new Value { Value = "Balanced", LinkedCommand = konten.PaymentSummary.Status });
                            else
                                oCmds.Add(new Value { Value = "Hold", LinkedCommand = konten.PaymentSummary.Status });
                            oCmds.Add(new Value { Value = dgvData.Rows[x].Cells["colRefNbr"].Value.ToString().Trim(), LinkedCommand = konten.PaymentSummary.ReferenceNbr });
                            oCmds.Add(new Value { Value = dgvData.Rows[x].Cells["colInvoiceNbr"].Value.ToString().Trim(), LinkedCommand = konten.PaymentSummary.PaymentRef });
                            oCmds.Add(new Value { Value = dgvData.Rows[x].Cells["colVendorCode"].Value.ToString().Trim(), LinkedCommand = konten.PaymentSummary.Vendor });
                            oCmds.Add(konten.Actions.Delete);
                            try
                            {
                                context.AP302000Submit(oCmds.ToArray());
                                DAC.DataAccessClass.getLogoutAcumatica(context);
                                retFlg = true;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                                DAC.DataAccessClass.getLogoutAcumatica(context);
                                retFlg = false;
                                return retFlg;
                            }
                            #endregion
                        }
                    }
                }
            }
            Cursor = Cursors.Default;

            return retFlg;
        }
    }
}
