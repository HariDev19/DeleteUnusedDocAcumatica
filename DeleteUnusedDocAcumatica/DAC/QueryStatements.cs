using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeleteUnusedDocAcumatica.DAC
{
    public class QueryStatements
    {
        public string getQuery { get; set; }

        public static string getStringQueryFin(int flg, string finYear)
        {
            string ret = "";
            if (flg == 0)
                ret = "select [Year] " +
                            "from FinYear " +
                            "where CompanyID = 2 " +
                                "and [Year] >= 2017 and [Year] <= 2023 " +
                                "and OrganizationID = 1 " +
                            "order by [Year] asc";
            else if(flg == 1)
                ret = "select FinPeriodID " +
                            "from FinPeriod " +
                            "where CompanyID = 2 " +
                                "and OrganizationID = 1 " +
                                "and FinYear = '"+finYear+"' " +
                            "order by FinPeriodID asc";
            return ret;
        }

        public static string getStringQueryData(int flg, 
                                                string finYear, 
                                                string finPeriod, 
                                                string docType, 
                                                string docStatus)
        {
            string ret = "";
            if (flg == 0)
                ret = "select distinct " +
                            "case  " +
                                "when a.DocType = 'ACR' then 'Credit Adj.'  " +
                                "when a.DocType = 'ADR' then 'Debit Adj.'  " +
                                "when a.DocType = 'INV' then 'Bill'  " +
                                "when a.DocType = 'PPM' then 'Prepayment'  " +
                            "end as DocType " +
                            ", a.RefNbr " +
                            ", a.DocDate " +
                            ", a.FinPeriodID " +
                            ", c.InvoiceNbr " +
                            ", b.AcctCD " +
                            ", b.AcctName " +
                            ", a.CuryOrigDocAmt " +
                            ", case  " +
                                "when a.Status = 'H' then 'Hold' " +
                                "when a.Status = 'B' then 'Balanced' " +
                                "when a.Status = 'C' then 'Closed' " +
                                "when a.Status = 'N' then 'Open' " +
                                "when a.Status = 'P' then 'Paid' " +
                                "when a.Status = 'V' then 'Voided' " +
                              "end as [Status] " +
                        "from APRegister a " +
                            "inner join BAccount b on a.VendorID = b.BAccountID and a.CompanyID = b.CompanyID  " +
                            "inner join APInvoice c on a.DocType = c.DocType and a.RefNbr = c.RefNbr and a.CompanyID = c.CompanyID " +
                            "inner join APTran d on a.DocType = d.TranType and a.RefNbr = d.RefNbr and a.CompanyID = d.CompanyID " +
                        "where a.CompanyID = 2  " +
                            "and a.Released = 0  " +
                            "and a.Status = '" + docStatus + "' " +
                            "and a.DocType = '" + docType + "' " +
                            "and a.FinPeriodID like '" + finYear + "%'  " +
                        "--order by a.DocType, AcctCD, AcctName, c.InvoiceNbr, RefNbr asc";
            else if (flg == 1)
                ret = "select distinct " +
                            "case  " +
                                "when a.DocType = 'ACR' then 'Credit Adj.'  " +
                                "when a.DocType = 'ADR' then 'Debit Adj.'  " +
                                "when a.DocType = 'INV' then 'Bill'  " +
                                "when a.DocType = 'PPM' then 'Prepayment'  " +
                            "end as DocType " +
                            ", a.RefNbr " +
                            ", a.DocDate " +
                            ", a.FinPeriodID " +
                            ", c.InvoiceNbr " +
                            ", b.AcctCD " +
                            ", b.AcctName " +
                            ", a.CuryOrigDocAmt " +
                            ", case  " +
                                "when a.Status = 'H' then 'Hold' " +
                                "when a.Status = 'B' then 'Balanced' " +
                                "when a.Status = 'C' then 'Closed' " +
                                "when a.Status = 'N' then 'Open' " +
                                "when a.Status = 'P' then 'Paid' " +
                                "when a.Status = 'V' then 'Voided' " +
                              "end as [Status] " +
                        "from APRegister a " +
                            "inner join BAccount b on a.VendorID = b.BAccountID and a.CompanyID = b.CompanyID  " +
                            "inner join APInvoice c on a.DocType = c.DocType and a.RefNbr = c.RefNbr and a.CompanyID = c.CompanyID " +
                            "left join APTran d on a.DocType = d.TranType and a.RefNbr = d.RefNbr and a.CompanyID = d.CompanyID " +
                        "where a.CompanyID = 2  " +
                            "and a.Released = 0  " +
                            "and a.Status = '" + docStatus + "' " +
                            "and a.DocType = '" + docType + "' " +
                            "and a.FinPeriodID = '" + finPeriod + "'  " +
                        "--order by a.DocType, AcctCD, AcctName, c.InvoiceNbr, RefNbr asc";

            else if (flg == 2)
                ret = "select distinct " +
                        "case   " +
                            "when a.DocType = 'CHK' then 'Check'   " +
                            "when a.DocType = 'REF' then 'Vendor Refund'   " +
                            "when a.DocType = 'VCK' then 'Voided Check' " +
                        "end as DocType  " +
                        ", a.RefNbr  " +
                        ", a.DocDate  " +
                        ", a.FinPeriodID  " +
                        ", b.ExtRefNbr as InvoiceNbr  " +
                        ", d.AcctCD  " +
                        ", d.AcctName  " +
                        ", a.CuryOrigDocAmt  " +
                        ", case   " +
                            "when a.Status = 'H' then 'Hold'  " +
                            "when a.Status = 'B' then 'Balanced'  " +
                            "when a.Status = 'C' then 'Closed'  " +
                            "when a.Status = 'N' then 'Open'  " +
                            "when a.Status = 'P' then 'Paid'  " +
                            "when a.Status = 'V' then 'Voided'  " +
                            "end as [Status]  " +
                    "from APRegister a " +
                        "inner join APPayment  b on a.RefNbr = b.RefNbr and a.DocType = b.DocType " +
                        "left join APAdjust c on a.CompanyID = c.CompanyID and  a.RefNbr = c.AdjgRefNbr and a.DocType = c.AdjgDocType and c.Released = 0 " +
                        "inner join  BAccount d on a.VendorID = d.BAccountID and a.CompanyID = d.CompanyID   " +
                    "where a.CompanyID = 2 " +
                        "and a.Released = 0 " +
                        "and a.Status = '"+ docStatus + "' " +
                        "and a.DocType = '"+docType+"'  " +
                        "and a.FinPeriodID like '" + finYear + "%'   " +
                    "--order by a.RefNbr, a.FinPeriodID asc";

            return ret;
        }
    }
}
