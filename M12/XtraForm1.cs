﻿using System;
using System.Text;
using DBConnection;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Extensions;
using MDS00;
using System.Drawing;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors;

namespace M12
{
    public partial class XtraForm1 : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private Functionality.Function FUNC = new Functionality.Function();
        private string selCode = "";
        public XtraForm1()
        {
            InitializeComponent();
            UserLookAndFeel.Default.StyleChanged += MyStyleChanged;
            iniConfig = new IniFile("Config.ini");
            UserLookAndFeel.Default.SetSkinStyle(iniConfig.Read("SkinName", "DevExpress"), iniConfig.Read("SkinPalette", "DevExpress"));
            // This line of code is generated by Data Source Configuration Wizard
            // Fill the JsonDataSource asynchronously
            jsonDataSource1.FillAsync();
            // This line of code is generated by Data Source Configuration Wizard
            // Fill the JsonDataSource asynchronously
            //jsonDataSource1.FillAsync();
        }

        private IniFile iniConfig;

        private void MyStyleChanged(object sender, EventArgs e)
        {
            UserLookAndFeel userLookAndFeel = (UserLookAndFeel)sender;
            LookAndFeelChangedEventArgs lookAndFeelChangedEventArgs = (DevExpress.LookAndFeel.LookAndFeelChangedEventArgs)e;
            //MessageBox.Show("MyStyleChanged: " + lookAndFeelChangedEventArgs.Reason.ToString() + ", " + userLookAndFeel.SkinName + ", " + userLookAndFeel.ActiveSvgPaletteName);
            iniConfig.Write("SkinName", userLookAndFeel.SkinName, "DevExpress");
            iniConfig.Write("SkinPalette", userLookAndFeel.ActiveSvgPaletteName, "DevExpress");
        }

        private void XtraForm1_Load(object sender, EventArgs e)
        {
            glueCode.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            glueCode.Properties.AcceptEditorTextAsNewValue = DevExpress.Utils.DefaultBoolean.True;

            bbiNew.PerformClick();
        }

        private void LoadData()
        {
            StringBuilder sbSQL = new StringBuilder();
            //Vendor
            sbSQL.Append("SELECT Code, Name ");
            sbSQL.Append("FROM  Vendor ");
            sbSQL.Append("ORDER BY Code, Name ");
            new ObjDevEx.setGridLookUpEdit(glueCode, sbSQL, "Code", "Code").getData(true);

            //Payment Term
            sbSQL.Clear();
            sbSQL.Append("SELECT Name, Description, OIDPayment AS ID ");
            sbSQL.Append("FROM PaymentTerm ");
            sbSQL.Append("ORDER BY OIDPayment ");
            new ObjDevEx.setSearchLookUpEdit(slueTerm, sbSQL, "Name", "ID").getData(true);

            //Payment Currency
            sbSQL.Clear();
            sbSQL.Append("SELECT OIDCURR AS ID, Currency ");
            sbSQL.Append("FROM Currency ");
            sbSQL.Append("ORDER BY OIDCURR ");
            new ObjDevEx.setGridLookUpEdit(glueCurrency, sbSQL, "Currency", "ID").getData(true);

            //Calendar No.
            sbSQL.Clear();
            sbSQL.Append("SELECT CompanyName AS VendorName, Year, OIDCALENDAR AS No  ");
            sbSQL.Append("FROM CalendarMaster C  ");
            sbSQL.Append("CROSS APPLY(SELECT Name AS CompanyName FROM Vendor WHERE OIDVEND = C.OIDCompany) D  ");
            sbSQL.Append("WHERE CompanyType = 2  ");
            sbSQL.Append("ORDER BY Year DESC, CompanyName, OIDCALENDAR  ");
            new ObjDevEx.setGridLookUpEdit(glueCalendar, sbSQL, "VendorName", "No").getData(true);

            //Vendor Detail
            sbSQL.Clear();
            sbSQL.Append("SELECT    A.OIDVEND AS No, A.Code, A.Name, A.ShotName, A.Contacts, A.Email, A.Address1, A.Address2, A.Address3, A.City, A.Country, A.TelephoneNo, A.FaxNo, A.VendorType, E.VenderTypeName, A.PaymentTerm AS PaymentTermID, ");
            sbSQL.Append("          B.Name AS PaymentTermName, A.PaymentCurrency AS CurrencyID, C.Currency AS CurrencyName, A.VendorEvaluation, A.CalendarNo, D.CompanyType, D.CompanyName, A.ProductionLeadTime, A.DeliveryLeadtime, A.ArrivalLeadTime, A.POCancelPeriod, A.Remark1, A.Remark2, A.CreatedBy, ");
            sbSQL.Append("          A.CreatedDate, A.UpdatedBy, A.UpdatedDate ");
            sbSQL.Append("FROM      Vendor AS A LEFT OUTER JOIN ");
            sbSQL.Append("          PaymentTerm AS B ON A.PaymentTerm = B.OIDPayment LEFT OUTER JOIN ");
            sbSQL.Append("          Currency AS C ON A.PaymentCurrency = C.OIDCURR LEFT OUTER JOIN ");
            sbSQL.Append("          (SELECT OIDCALENDAR AS No, 'Vendor' AS CompanyType, CompanyName, Year ");
            sbSQL.Append("           FROM   CalendarMaster CX ");
            sbSQL.Append("                  CROSS APPLY(SELECT Name AS CompanyName FROM Vendor WHERE OIDVEND = CX.OIDCompany) D ");
            sbSQL.Append("           WHERE CompanyType = 2) AS D ON A.CalendarNo = D.No LEFT OUTER JOIN ");
            sbSQL.Append("          (SELECT '0' AS VendorType, 'Finished Good' AS VenderTypeName ");
            sbSQL.Append("           UNION ALL ");
            sbSQL.Append("           SELECT '1' AS VendorType, 'Fabric' AS VenderTypeName ");
            sbSQL.Append("           UNION ALL ");
            sbSQL.Append("           SELECT '2' AS VendorType, 'Accessory' AS VenderTypeName ");
            sbSQL.Append("           UNION ALL ");
            sbSQL.Append("           SELECT '3' AS VendorType, 'Packaging' AS VenderTypeName) AS E ON A.VendorType = E.VendorType ");
            new ObjDevEx.setGridControl(gcVendor, gvVendor, sbSQL).getData(false, false, false, true);

        }

        private void NewData()
        {
            glueVendor.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;

            lblStatus.Text = "* Add Vendor";
            lblStatus.ForeColor = Color.Green;

            txeID.Text = new DBQuery("SELECT CASE WHEN ISNULL(MAX(OIDVEND), '') = '' THEN 1 ELSE MAX(OIDVEND) + 1 END AS NewNo FROM Vendor").getString();
            glueCode.EditValue = "";
            txeName.Text = "";
            txeShortName.Text = "";
            txeContact.Text = "";
            txeEmail.Text = "";
            txeAddr1.Text = "";
            txeAddr2.Text = "";
            txeAddr3.Text = "";
            txeCountry.Text = "";
            txeTel.Text = "";
            txeFax.Text = "";

            glueVendor.EditValue = "";
            slueTerm.EditValue = "";
            glueCurrency.EditValue = "";
            txeEval.Text = "";
            glueCalendar.EditValue = "";

            spePLT.Value = 0;
            speDLT.Value = 0;
            speALT.Value = 0;
            spePCP.Value = 0;

            txeCREATE.Text = "0";
            txeCDATE.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            txeUPDATE.Text = "0";
            txeUDATE.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            selCode = "";
            ////txeID.Focus();
        }

        private void bbiNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
            NewData();
        }



        // This event is generated by Data Source Configuration Wizard
        void unboundSource1_ValueNeeded(object sender, DevExpress.Data.UnboundSourceValueNeededEventArgs e)
        {
            // Handle this event to obtain data from your data source
            // e.Value = something /* TODO: Assign the real data here.*/
        }

        // This event is generated by Data Source Configuration Wizard
        void unboundSource1_ValuePushed(object sender, DevExpress.Data.UnboundSourceValuePushedEventArgs e)
        {
            // Handle this event to save modified data back to your data source
            // something = e.Value; /* TODO: Propagate the value into the storage.*/
        }

        private void glueVendor_EditValueChanged(object sender, EventArgs e)
        {
            slueTerm.Focus();
        }

        private void glueCode_EditValueChanged(object sender, EventArgs e)
        {
            //txeName.Focus();
            //LoadCode(glueCode.Text);
        }

        private void glueCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeName.Focus();
            }
        }

        private void glueCode_LostFocus(object sender, EventArgs e)
        {
            

        }

        private void LoadCode(string strCODE)
        {
            strCODE = strCODE.ToUpper().Trim();
            txeID.Text = "";
            txeName.Text = "";
            txeShortName.Text = "";
            txeContact.Text = "";
            txeEmail.Text = "";
            txeAddr1.Text = "";
            txeAddr2.Text = "";
            txeAddr3.Text = "";
            txeCountry.Text = "";
            txeTel.Text = "";
            txeFax.Text = "";

            glueVendor.EditValue = "";
            slueTerm.EditValue = "";
            glueCurrency.EditValue = "";
            txeEval.Text = "";
            glueCalendar.EditValue = "";

            spePLT.Value = 0;
            speDLT.Value = 0;
            speALT.Value = 0;
            spePCP.Value = 0;

            txeCREATE.Text = "0";
            txeCDATE.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            txeUPDATE.Text = "0";
            txeUDATE.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            //selCode = "";
            txeName.Focus();

            StringBuilder sbSQL = new StringBuilder();
            sbSQL.Append("SELECT TOP (1) OIDVEND, Code, Name, ShotName, Contacts, Email, Address1, Address2, Address3, Country, TelephoneNo, FaxNo, VendorType, PaymentTerm, PaymentCurrency, VendorEvaluation, CalendarNo,  ");
            sbSQL.Append("       ISNULL(ProductionLeadTime, 0) AS ProductionLeadTime, ISNULL(DeliveryLeadtime, 0) AS DeliveryLeadtime, ISNULL(ArrivalLeadTime, 0) AS ArrivalLeadTime, ISNULL(POCancelPeriod, 0) AS POCancelPeriod, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate ");
            sbSQL.Append("FROM   Vendor ");
            sbSQL.Append("WHERE (Code = N'" + strCODE.Replace("'", "''") + "') ");
            string[] arrVendor = new DBQuery(sbSQL).getMultipleValue();
            if (arrVendor.Length > 0)
            {
                txeID.Text = arrVendor[0];
                lblStatus.Text = "* Edit Vendor";
                lblStatus.ForeColor = Color.Red;

                txeName.Text = arrVendor[2];
                txeShortName.Text = arrVendor[3];
                txeContact.Text = arrVendor[4];
                txeEmail.Text = arrVendor[5];
                txeAddr1.Text = arrVendor[6];
                txeAddr2.Text = arrVendor[7];
                txeAddr3.Text = arrVendor[8];
                txeCountry.Text = arrVendor[9];
                txeTel.Text = arrVendor[10];
                txeFax.Text = arrVendor[11];

                glueVendor.EditValue = arrVendor[12];
                slueTerm.EditValue = arrVendor[13];
                glueCurrency.EditValue = arrVendor[14];
                txeEval.Text = arrVendor[15];
                glueCalendar.EditValue = arrVendor[16];

                spePLT.Value = Convert.ToInt32(arrVendor[17]);
                speDLT.Value = Convert.ToInt32(arrVendor[18]);
                speALT.Value = Convert.ToInt32(arrVendor[19]);
                spePCP.Value = Convert.ToInt32(arrVendor[20]);

                txeCREATE.Text = arrVendor[21];
                txeCDATE.Text = arrVendor[22];
                txeUPDATE.Text = arrVendor[23];
                txeUDATE.Text = arrVendor[24];
            }


            //Check new vendor or edit vendor
            sbSQL.Clear();
            sbSQL.Append("SELECT OIDVEND FROM Vendor WHERE (OIDVEND = '" + txeID.EditValue.ToString() + "') ");
            string strCHKID = new DBQuery(sbSQL).getString();
            if (strCHKID == "")
            {
                lblStatus.Text = "* Add Vendor";
                lblStatus.ForeColor = Color.Green;
            }
            else
            {
                lblStatus.Text = "* Edit Vendor";
                lblStatus.ForeColor = Color.Red;
            }
        }

        private void gvVendor_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            
        }

        private void gvVendor_RowStyle(object sender, RowStyleEventArgs e)
        {
            
        }

        private void bbiExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string pathFile = new ObjSet.Folder(@"C:\MDS\Export\").GetPath() + "VendorList_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx";
            gvVendor.ExportToXlsx(pathFile);
            System.Diagnostics.Process.Start(pathFile);
        }

        private void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (glueCode.Text.Trim() == "")
            {
                FUNC.msgWarning("Please input vendor code.");
                glueCode.Focus();
            }
            else if (txeName.Text.Trim() == "")
            {
                FUNC.msgWarning("Please input vendor name.");
                txeName.Focus();
            }
            else
            {
                if (FUNC.msgQuiz("Confirm save data ?") == true)
                {
                    StringBuilder sbSQL = new StringBuilder();

                    string strCREATE = "0";
                    if (txeCREATE.Text.Trim() != "")
                    {
                        strCREATE = txeCREATE.Text.Trim();
                    }

                    string strUPDATE = "0";
                    if (txeUPDATE.Text.Trim() != "")
                    {
                        strUPDATE = txeUPDATE.Text.Trim();
                    }

                    if (lblStatus.Text == "* Add Vendor")
                    {
                        sbSQL.Append("  INSERT INTO Vendor(Code, Name, ShotName, Contacts, Email, Address1, Address2, Address3, City, Country, TelephoneNo, FaxNo, VendorType, PaymentTerm, PaymentCurrency, VendorEvaluation, CalendarNo, ProductionLeadTime, DeliveryLeadtime, ArrivalLeadTime, POCancelPeriod, Remark1, Remark2, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) ");
                        sbSQL.Append("  VALUES(N'" + glueCode.Text.Trim().Replace("'", "''") + "', N'" + txeName.Text.Trim().Replace("'", "''") + "', N'" + txeShortName.Text.Trim().Replace("'", "''") + "', N'" + txeContact.Text.Trim().Replace("'", "''") + "', N'" + txeEmail.Text.Trim() + "', N'" + txeAddr1.Text.Trim() + "', N'" + txeAddr2.Text.Trim() + "', N'" + txeAddr3.Text.Trim() + "', N'', N'" + txeCountry.Text.Trim() + "', N'" + txeTel.Text.Trim() + "', ");
                        sbSQL.Append("         N'" + txeFax.Text.Trim() + "', '" + glueVendor.EditValue.ToString() + "', '" + slueTerm.EditValue.ToString() + "', '" + glueCurrency.EditValue.ToString() + "',  N'" + txeEval.Text.Trim() + "', '" + glueCalendar.EditValue.ToString() + "', '" + spePLT.Value.ToString() + "', '" + speDLT.Value.ToString() + "', '" + speALT.Value.ToString() + "', '" + spePCP.Value.ToString() + "', N'', N'', '" + strCREATE + "', GETDATE(), '" + strUPDATE + "', GETDATE()) ");
                    }
                    else if (lblStatus.Text == "* Edit Vendor")
                    {
                        sbSQL.Append("  UPDATE Vendor SET ");
                        sbSQL.Append("      Code=N'" + glueCode.Text.Trim().Replace("'", "''") + "', Name=N'" + txeName.Text.Trim().Replace("'", "''") + "', ShotName=N'" + txeShortName.Text.Trim().Replace("'", "''") + "', Contacts=N'" + txeContact.Text.Trim().Replace("'", "''") + "', Email=N'" + txeEmail.Text.Trim() + "', Address1=N'" + txeAddr1.Text.Trim() + "', ");
                        sbSQL.Append("      Address2=N'" + txeAddr2.Text.Trim() + "', Address3=N'" + txeAddr3.Text.Trim() + "', City=N'', Country=N'" + txeCountry.Text.Trim() + "', TelephoneNo=N'" + txeTel.Text.Trim() + "', FaxNo = N'" + txeFax.Text.Trim() + "', VendorType = '" + glueVendor.EditValue.ToString() + "', PaymentTerm = '" + slueTerm.EditValue.ToString() + "', ");
                        sbSQL.Append("      PaymentCurrency = '" + glueCurrency.EditValue.ToString() + "', VendorEvaluation = N'" + txeEval.Text.Trim() + "', CalendarNo = '" + glueCalendar.EditValue.ToString() + "', ProductionLeadTime = '" + spePLT.Value.ToString() + "', DeliveryLeadtime = '" + speDLT.Value.ToString() + "', ArrivalLeadTime = '" + speALT.Value.ToString() + "', ");
                        sbSQL.Append("      POCancelPeriod = '" + spePCP.Value.ToString() + "', Remark1 = N'', Remark2 = N'', UpdatedBy = '" + strUPDATE + "', UpdatedDate = GETDATE() ");
                        sbSQL.Append("  WHERE(OIDVEND = '" + txeID.Text.Trim() + "') ");
                    }

                    //sbSQL.Append("IF NOT EXISTS(SELECT Code FROM Vendor WHERE Code = N'" + glueCode.Text.Trim().Replace("'", "''") + "') ");
                    //sbSQL.Append(" BEGIN ");
                    //sbSQL.Append("  INSERT INTO Vendor(Code, Name, ShotName, Contacts, Email, Address1, Address2, Address3, City, Country, TelephoneNo, FaxNo, VendorType, PaymentTerm, PaymentCurrency, VendorEvaluation, CalendarNo, ProductionLeadTime, DeliveryLeadtime, ArrivalLeadTime, POCancelPeriod, Remark1, Remark2, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) ");
                    //sbSQL.Append("  VALUES(N'" + glueCode.Text.Trim().Replace("'", "''") + "', N'" + txeName.Text.Trim().Replace("'", "''") + "', N'" + txeShortName.Text.Trim().Replace("'", "''") + "', N'" + txeContact.Text.Trim().Replace("'", "''") + "', N'" + txeEmail.Text.Trim() + "', N'" + txeAddr1.Text.Trim() + "', N'" + txeAddr2.Text.Trim() + "', N'" + txeAddr3.Text.Trim() + "', N'', N'" + txeCountry.Text.Trim() + "', N'" + txeTel.Text.Trim() + "', ");
                    //sbSQL.Append("         N'" + txeFax.Text.Trim() + "', '" + glueVendor.EditValue.ToString() + "', '" + slueTerm.EditValue.ToString() + "', '" + glueCurrency.EditValue.ToString() + "',  N'" + txeEval.Text.Trim() + "', '" + glueCalendar.EditValue.ToString() + "', '" + spePLT.Value.ToString() + "', '" + speDLT.Value.ToString() + "', '" + speALT.Value.ToString() + "', '" + spePCP.Value.ToString() + "', N'', N'', '" + strCREATE + "', GETDATE(), '" + strUPDATE + "', GETDATE()) ");
                    //sbSQL.Append(" END ");
                    //sbSQL.Append("ELSE ");
                    //sbSQL.Append(" BEGIN ");
                    //sbSQL.Append("  UPDATE Vendor SET ");
                    //sbSQL.Append("      Code=N'" + glueCode.Text.Trim().Replace("'", "''") + "', Name=N'" + txeName.Text.Trim().Replace("'", "''") + "', ShotName=N'" + txeShortName.Text.Trim().Replace("'", "''") + "', Contacts=N'" + txeContact.Text.Trim().Replace("'", "''") + "', Email=N'" + txeEmail.Text.Trim() + "', Address1=N'" + txeAddr1.Text.Trim() + "', ");
                    //sbSQL.Append("      Address2=N'" + txeAddr2.Text.Trim() + "', Address3=N'" + txeAddr3.Text.Trim() + "', City=N'', Country=N'" + txeCountry.Text.Trim() + "', TelephoneNo=N'" + txeTel.Text.Trim() + "', FaxNo = N'" + txeFax.Text.Trim() + "', VendorType = '" + glueVendor.EditValue.ToString() + "', PaymentTerm = '" + slueTerm.EditValue.ToString() + "', ");
                    //sbSQL.Append("      PaymentCurrency = '" + glueCurrency.EditValue.ToString() + "', VendorEvaluation = N'" + txeEval.Text.Trim() + "', CalendarNo = '" + glueCalendar.EditValue.ToString() + "', ProductionLeadTime = '" + spePLT.Value.ToString() + "', DeliveryLeadtime = '" + speDLT.Value.ToString() + "', ArrivalLeadTime = '" + speALT.Value.ToString() + "', ");
                    //sbSQL.Append("      POCancelPeriod = '" + spePCP.Value.ToString() + "', Remark1 = N'', Remark2 = N'', UpdatedBy = '" + strUPDATE + "', UpdatedDate = GETDATE() ");
                    //sbSQL.Append("  WHERE(OIDVEND = '" + txeID.Text.Trim() + "') ");
                    //sbSQL.Append(" END ");
                    //MessageBox.Show(sbSQL.ToString());
                    if (sbSQL.Length > 0)
                    {
                        try
                        {
                            bool chkSAVE = new DBQuery(sbSQL).runSQL();
                            if (chkSAVE == true)
                            {
                                bbiNew.PerformClick();
                                FUNC.msgInfo("Save complete.");
                            }
                        }
                        catch (Exception)
                        { }
                    }
                }

            }
        }

        private void txeName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeShortName.Focus();
            }
        }

        private void txeShortName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeContact.Focus();
            }
        }

        private void txeContact_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeEmail.Focus();
            }
        }

        private void txeEmail_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeAddr1.Focus();
            }
        }

        private void txeAddr1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeAddr2.Focus();
            }
        }

        private void txeAddr2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeAddr3.Focus();
            }
        }

        private void txeAddr3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeCountry.Focus();
            }
        }

        private void txeCountry_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeTel.Focus();
            }
        }

        private void txeTel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeFax.Focus();
            }
        }

        private void txeFax_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                glueVendor.Focus();
            }
        }

        private void slueTerm_EditValueChanged(object sender, EventArgs e)
        {
            glueCurrency.Focus();
        }

        private void glueCurrency_EditValueChanged(object sender, EventArgs e)
        {
            txeEval.Focus();
        }

        private void txeEval_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                glueCalendar.Focus();
            }
        }

        private void glueCalendar_EditValueChanged(object sender, EventArgs e)
        {
            spePLT.Focus();
        }

        private void spePLT_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                speDLT.Focus();
            }
        }

        private void speDLT_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                speALT.Focus();
            }
        }

        private void speALT_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                spePCP.Focus();
            }
        }

        private void spePCP_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txeCREATE.Focus();
            }
        }

        private void glueCode_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            glueCode.Focus();
            txeName.Focus();
        }

        private void glueCode_ProcessNewValue(object sender, DevExpress.XtraEditors.Controls.ProcessNewValueEventArgs e)
        {
            GridLookUpEdit gridLookup = sender as GridLookUpEdit;
            if (e.DisplayValue == null) return;
            string newValue = e.DisplayValue.ToString();
            if (newValue == String.Empty) return;
        }

        private void gvVendor_RowClick(object sender, RowClickEventArgs e)
        {
            if (gvVendor.IsFilterRow(e.RowHandle)) return;
            lblStatus.Text = "* Edit Vendor";
            lblStatus.ForeColor = Color.Red;

            txeID.Text = gvVendor.GetFocusedRowCellValue("No").ToString();
            glueCode.EditValue = gvVendor.GetFocusedRowCellValue("Code").ToString();
            txeName.Text = gvVendor.GetFocusedRowCellValue("Name").ToString();
            txeShortName.Text = gvVendor.GetFocusedRowCellValue("ShotName").ToString();
            txeContact.Text = gvVendor.GetFocusedRowCellValue("Contacts").ToString();
            txeEmail.Text = gvVendor.GetFocusedRowCellValue("Email").ToString();
            txeAddr1.Text = gvVendor.GetFocusedRowCellValue("Address1").ToString();
            txeAddr2.Text = gvVendor.GetFocusedRowCellValue("Address2").ToString();
            txeAddr3.Text = gvVendor.GetFocusedRowCellValue("Address3").ToString();
            txeCountry.Text = gvVendor.GetFocusedRowCellValue("Country").ToString();
            txeTel.Text = gvVendor.GetFocusedRowCellValue("TelephoneNo").ToString();
            txeFax.Text = gvVendor.GetFocusedRowCellValue("FaxNo").ToString();

            glueVendor.EditValue = gvVendor.GetFocusedRowCellValue("VendorType").ToString();
            slueTerm.EditValue = gvVendor.GetFocusedRowCellValue("PaymentTermID").ToString();
            glueCurrency.EditValue = gvVendor.GetFocusedRowCellValue("CurrencyID").ToString();
            txeEval.Text = gvVendor.GetFocusedRowCellValue("VendorEvaluation").ToString();
            glueCalendar.EditValue = gvVendor.GetFocusedRowCellValue("CalendarNo").ToString();

            int PLT = 0;
            if (gvVendor.GetFocusedRowCellValue("ProductionLeadTime").ToString() != "")
            {
                PLT = Convert.ToInt32(gvVendor.GetFocusedRowCellValue("ProductionLeadTime").ToString());
            }
            spePLT.Value = PLT;

            int DLT = 0;
            if (gvVendor.GetFocusedRowCellValue("DeliveryLeadtime").ToString() != "")
            {
                DLT = Convert.ToInt32(gvVendor.GetFocusedRowCellValue("DeliveryLeadtime").ToString());
            }
            spePLT.Value = PLT;

            int ALT = 0;
            if (gvVendor.GetFocusedRowCellValue("ArrivalLeadTime").ToString() != "")
            {
                ALT = Convert.ToInt32(gvVendor.GetFocusedRowCellValue("ArrivalLeadTime").ToString());
            }
            spePLT.Value = PLT;

            int PCP = 0;
            if (gvVendor.GetFocusedRowCellValue("POCancelPeriod").ToString() != "")
            {
                PCP = Convert.ToInt32(gvVendor.GetFocusedRowCellValue("POCancelPeriod").ToString());
            }

            spePLT.Value = PLT;
            speDLT.Value = DLT;
            speALT.Value = ALT;
            spePCP.Value = PCP;

            txeCREATE.Text = gvVendor.GetFocusedRowCellValue("CreatedBy").ToString();
            txeCDATE.Text = gvVendor.GetFocusedRowCellValue("CreatedDate").ToString();
            txeUPDATE.Text = gvVendor.GetFocusedRowCellValue("UpdatedBy").ToString();
            txeUDATE.Text = gvVendor.GetFocusedRowCellValue("UpdatedDate").ToString();
        }

        private void bbiPrintPreview_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gcVendor.ShowPrintPreview();
        }

        private void bbiPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gcVendor.Print();
        }

        private void glueCode_Leave(object sender, EventArgs e)
        {
            if (glueCode.Text.Trim() != "" && glueCode.Text.ToUpper().Trim() != selCode)
            {
                glueCode.Text = glueCode.Text.ToUpper().Trim();
                selCode = glueCode.Text;
                LoadCode(glueCode.Text);
            }
        }
    }
}