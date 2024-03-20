using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class Requisition_Received_RequisitionReceived : System.Web.UI.Page
{
    string connectionString = ConfigurationManager.ConnectionStrings["Ginie"].ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Search_DD_RequistionNo();

            //Session["UserId"] = "10021";
        }
    }





    //=========================={ Paging & Alert }==========================
    protected void gridSearch_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        //binding GridView to PageIndex object
        gridSearch.PageIndex = e.NewPageIndex;

        DataTable pagination = (DataTable)Session["PaginationDataSource"];

        gridSearch.DataSource = pagination;
        gridSearch.DataBind();
    }

    private void alert(string mssg)
    {
        // alert pop - up with only message
        string message = mssg;
        string script = $"alert('{message}');";
        ScriptManager.RegisterStartupScript(this, this.GetType(), "messageScript", script, true);
    }



    //=========================={ Sweet Alert JS }==========================
    private void getSweetAlertSuccessOnly()
    {
        string title = "Saved!";
        string message = "Record saved successfully!";
        string icon = "success";
        string confirmButtonText = "OK";

        string sweetAlertScript =
            $@"<script>
                Swal.fire({{ 
                    title: '{title}', 
                    text: '{message}', 
                    icon: '{icon}', 
                    confirmButtonText: '{confirmButtonText}' 
                }});
            </script>";
        ClientScript.RegisterStartupScript(this.GetType(), "sweetAlert", sweetAlertScript, false);
    }

    // sweet alert - success redirect
    private void getSweetAlertSuccessRedirect(string redirectUrl)
    {
        string title = "Saved!";
        string message = "Record saved successfully!";
        string icon = "success";
        string confirmButtonText = "OK";
        string allowOutsideClick = "false";

        string sweetAlertScript =
            $@"<script>
                Swal.fire({{ 
                    title: '{title}', 
                    text: '{message}', 
                    icon: '{icon}', 
                    confirmButtonText: '{confirmButtonText}',
                    allowOutsideClick: {allowOutsideClick}
                }}).then((result) => {{
                    if (result.isConfirmed) {{
                        window.location.href = '{redirectUrl}';
                    }}
                }});
            </script>";
        ClientScript.RegisterStartupScript(this.GetType(), "sweetAlert", sweetAlertScript, false);
    }

    // sweet alert - success redirect block
    private void getSweetAlertSuccessRedirectMandatory(string titles, string mssg, string redirectUrl)
    {
        string title = titles;
        string message = mssg;
        string icon = "success";
        string confirmButtonText = "OK";
        string allowOutsideClick = "false"; // Prevent closing on outside click

        string sweetAlertScript =
        $@"<script>
            Swal.fire({{ 
                title: '{title}', 
                text: '{message}', 
                icon: '{icon}', 
                confirmButtonText: '{confirmButtonText}', 
                allowOutsideClick: {allowOutsideClick}
            }}).then((result) => {{
                if (result.isConfirmed) {{
                    window.location.href = '{redirectUrl}';
                }}
            }});
        </script>";
        ClientScript.RegisterStartupScript(this.GetType(), "sweetAlert", sweetAlertScript, false);
    }

    // sweet alert - error only block
    private void getSweetAlertErrorMandatory(string titles, string mssg)
    {
        string title = titles;
        string message = mssg;
        string icon = "error";
        string confirmButtonText = "OK";
        string allowOutsideClick = "false"; // Prevent closing on outside click

        string sweetAlertScript =
        $@"<script>
            Swal.fire({{ 
                title: '{title}', 
                text: '{message}', 
                icon: '{icon}', 
                confirmButtonText: '{confirmButtonText}', 
                allowOutsideClick: {allowOutsideClick}
            }});
        </script>";
        ClientScript.RegisterStartupScript(this.GetType(), "sweetAlert", sweetAlertScript, false);
    }

    // info
    private void getSweetAlertInfoMandatory(string titles, string mssg)
    {
        string title = titles;
        string message = mssg;
        string icon = "info";
        string confirmButtonText = "OK";
        string allowOutsideClick = "false"; // Prevent closing on outside click

        string sweetAlertScript =
        $@"<script>
            Swal.fire({{ 
                title: '{title}', 
                text: '{message}', 
                icon: '{icon}', 
                confirmButtonText: '{confirmButtonText}', 
                allowOutsideClick: {allowOutsideClick}
            }});
        </script>";
        ClientScript.RegisterStartupScript(this.GetType(), "sweetAlert", sweetAlertScript, false);
    }




    //=========================={ Binding Search Dropdowns }==========================
    private void Search_DD_RequistionNo()
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "select * from Requisition1891 order by ReqNo desc";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();

            ddScRequisitionNo.DataSource = dt;
            ddScRequisitionNo.DataTextField = "ReqNo";
            ddScRequisitionNo.DataValueField = "ReqNo";
            ddScRequisitionNo.DataBind();
            ddScRequisitionNo.Items.Insert(0, new ListItem("------Select Requisition No------", "0"));
        }
    }



    //=========================={ Dropdown Event }==========================
    protected void ddTaxOrNot_SelectedIndexChanged(object sender, EventArgs e)
    {
        string taxOrNot = ddTaxOrNot.SelectedValue;

        if (ddTaxOrNot.SelectedValue == "Yes")
        {
            divTaxHead.Visible = true;

            string req1RefNo = Session["Req1ReferenceNo"].ToString();
            FillTaxHead(req1RefNo);
        }
        else
        {
            divTaxHead.Visible = false;
        }
    }



    //=========================={ Fetch Data }==========================

    private DataTable CheckForExistingAvailabilityStatus(string req1RefNo)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = $@"select * 
                            from reqreceived891  as rec 
                            inner join paymentstatus891 as pay on pay.Req1RefNo = rec.Req1RefNo 
                            where rec.Req1RefNo = @Req1RefNo";

            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Req1RefNo", req1RefNo);
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();

            return dt;
        }
    }

    private DataTable GetRequisitionDT(string ReqNo)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "select * from Requisition1891 where ReqNo = @ReqNo";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@ReqNo", ReqNo);
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();

            return dt;
        }
    }

    private DataTable getAccountHeadExisting(string req1RefNo)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "select * from ReqTaxHead891 where Req1RefNo = @Req1RefNo";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Req1RefNo", req1RefNo);
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();

            return dt;
        }
    }

    private DataTable getAccountHead()
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "select * from AcHeads891 where AcHGName = '1000005' AND IsTaxApplied = '1000001'";
            SqlCommand cmd = new SqlCommand(sql, con);
            //cmd.Parameters.AddWithValue("@icNumber", imprestCardNo);
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();

            return dt;
        }
    }



    private bool CheckForExistingReqTaxHead(string req1RefNo, SqlConnection con, SqlTransaction transaction)
    {
        string sql = "SELECT * FROM ReqTaxHead891 WHERE Req1RefNo = @Req1RefNo";

        SqlCommand cmd = new SqlCommand(sql, con, transaction);
        cmd.Parameters.AddWithValue("@Req1RefNo", req1RefNo);
        cmd.ExecuteNonQuery();

        SqlDataAdapter ad = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        ad.Fill(dt);

        if (dt.Rows.Count > 0) return true;
        else return false;
    }

    private string GetNewBillTaxRefNo(SqlConnection con, SqlTransaction transaction)
    {
        string nextRefNo = "1000001";

        string sql = "SELECT ISNULL(MAX(CAST(RefNO AS INT)), 1000000) + 1 AS NextRefNo FROM ReqTaxHead891";
        SqlCommand cmd = new SqlCommand(sql, con, transaction);
        cmd.ExecuteNonQuery();

        SqlDataAdapter ad = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        ad.Fill(dt);

        if (dt.Rows.Count > 0) return dt.Rows[0]["NextRefNo"].ToString();
        else return nextRefNo;
    }





    //=========================={ Search Button Event }==========================
    protected void btnNewBill_Click(object sender, EventArgs e)
    {
        Response.Redirect("RequisitionReceived.aspx");
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        BindGridView();
    }

    private void BindGridView()
    {
        searchGridDiv.Visible = true;

        // dropdown values
        string reqNo = ddScRequisitionNo.SelectedValue;

        DateTime fromDate;
        DateTime toDate;

        if (!DateTime.TryParse(ScFromDate.Text, out fromDate)) { fromDate = SqlDateTime.MinValue.Value; }
        if (!DateTime.TryParse(ScToDate.Text, out toDate)) { toDate = SqlDateTime.MaxValue.Value; }

        // DTs
        DataTable reqDT = GetRequisitionDT(reqNo);

        // dt values
        string requisitionNo = (reqDT.Rows.Count > 0) ? reqDT.Rows[0]["ReqNo"].ToString() : string.Empty;

        DataTable searchResultDT = SearchRecords(requisitionNo, fromDate, toDate);

        // binding the search grid
        gridSearch.DataSource = searchResultDT;
        gridSearch.DataBind();

        Session["PaginationDataSource"] = searchResultDT;
    }

    public DataTable SearchRecords(string requisitionNo, DateTime fromDate, DateTime toDate)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string sql = $@"SELECT DISTINCT req1.*, org.OrgTyp, um.InstiName, 
                            (select count(*) from Requisition2891 as req2 where req2.BillRefNo = req1.RefNo) as Req2Count, 
                            CASE WHEN rec.Req1RefNo is not null THEN 'Updated' ELSE 'Pending' END as AvailabilityStatus,
                            CASE WHEN pay.Req1RefNo is not null THEN 'Pyament Done' ELSE 'Payment Pending' END as PaymentStatus, 
                            case when (select count(*) from billdocupload891 as doc where doc.BillRefNo = req1.RefNo) > 0 
                            then 'Uploaded' else 'Not Uploaded' end as DocStatus 
                            FROM Requisition1891 as req1 
                            LEFT JOIN Requisition2891 as req2 ON req1.RefNo = req2.BillRefNo 
                            INNER JOIN UserMaster891 as um ON req1.SaveBy = um.UserID 
                            INNER JOIN OrgType891 as org ON req2.OrgType = org.RefID 
                            LEFT JOIN reqreceived891 as rec ON rec.Req1RefNo = req1.RefNo 
                            LEFT JOIN paymentstatus891 as pay ON pay.Req1RefNo = req1.RefNo 
                            WHERE 1=1";

            if (!string.IsNullOrEmpty(requisitionNo))
            {
                sql += " AND ReqNo = @ReqNo";
            }

            if (fromDate != null)
            {
                sql += " AND ReqDte >= @FromDate";
            }

            if (toDate != null)
            {
                sql += " AND ReqDte <= @ToDate";
            }

            sql += " ORDER BY req1.RefNo DESC";




            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                if (!string.IsNullOrEmpty(requisitionNo))
                {
                    command.Parameters.AddWithValue("@ReqNo", requisitionNo);
                }

                if (fromDate != null)
                {
                    command.Parameters.AddWithValue("@FromDate", fromDate);
                }
                
                if (toDate != null)
                {
                    command.Parameters.AddWithValue("@ToDate", toDate);
                }

                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }
    }




    //=========================={ Item Grid OnRowDataBound }==========================
    protected void ItemGrid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow && (e.Row.RowState & DataControlRowState.Edit) > 0)
        {
            // Set all row in edit mode
            //e.Row.RowState = e.Row.RowState ^ DataControlRowState.Edit;

            int rowIndex = e.Row.RowIndex;
            TextBox txtAvailableQty = (TextBox)e.Row.FindControl("txtAvailableQty");
            DataRowView rowView = (DataRowView)e.Row.DataItem;

            if (rowView != null)
            {
                // Retrieve AvailableQty value from the data source
                string availableQty = rowView["AvailableQty"].ToString();

                // Set the value to the TextBox
                txtAvailableQty.Text = availableQty;
            }
        }
    }

    protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    protected void chkAvailStatus_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox chkAvailStatus = (CheckBox)sender;
        GridViewRow row = (GridViewRow)chkAvailStatus.NamingContainer;

        TextBox txtEdit = (TextBox)row.FindControl("txtEdit");
        txtEdit.Visible = !chkAvailStatus.Checked;

        // updating item reamrk on checking the checkbox (remark not needed hence == "")
        if (chkAvailStatus.Checked) txtEdit.Text = "";

        // updating available qty if checkbox is checked
        TextBox txtRequiredQty = (TextBox)row.FindControl("txtRequiredQty");

        //DataRowView rowView = (DataRowView)row.DataItem;
        //string requiredQty = rowView["Quty"].ToString();

        string requiredQty = row.Cells[3].Text.ToString(); // 3rd index -> Quty (required Qty)
        TextBox txtAvailableQty = (TextBox)row.FindControl("txtAvailableQty");
        TextBox txtBalanceQty = (TextBox)row.FindControl("txtBalanceQty");


        if (chkAvailStatus.Checked)
        {
            txtAvailableQty.Text = requiredQty;
            txtBalanceQty.Text = "0";
        }
        else
        {
            txtAvailableQty.Text = "0";
            txtBalanceQty.Text = requiredQty;
        }

        // calculating available services amount
        DataTable requisition2DT = (DataTable)Session["ReqDetails"]; // requisition 2 details

        double totalBill = 0.00;

        // calculating totl aervice amount
        foreach (GridViewRow itemRow in itemGrid.Rows)
        {
            int rowIndex = itemRow.RowIndex;

            TextBox TextAvailableQty = itemRow.FindControl("txtAvailableQty") as TextBox;
            double availableQty = Convert.ToDouble(TextAvailableQty.Text);

            double servicePrice = Convert.ToDouble(requisition2DT.Rows[rowIndex]["ServicePrice"]);

            //string availableStatus = ((HtmlInputCheckBox)row.FindControl("chkAvailStatus")).Checked.ToString(); // input - checkbox
            string availableStatus = ((CheckBox)itemRow.FindControl("chkAvailStatus")).Checked.ToString(); // asp:checkbox

            // fetching the sub item price textbox field
            TextBox subItemPrice = (TextBox)itemRow.FindControl("SubItemPrice");

            if (availableStatus == "True")
            {
                totalBill = totalBill + (availableQty * servicePrice);
                subItemPrice.Text = (availableQty * servicePrice).ToString();
                
            }
            else
            {
                subItemPrice.Text = (0).ToString();
            }
        }

        // final bill basic amount
        TotalServiceAmnt.Text = totalBill.ToString("");
        Session["TotalBillAmount"] = TotalServiceAmnt.Text;

        // re-calculating the tax heads
        string req1RefNo = Session["Req1ReferenceNo"].ToString();
        FillTaxHead(req1RefNo);
    }






    //=========================={ Update - Fill Details }==========================

    protected void gridSearch_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "lnkView")
        {
            int rowId = Convert.ToInt32(e.CommandArgument);
            Session["Req1ReferenceNo"] = rowId;

            bool isDocsUploaded = CheckForDocUploaded(rowId.ToString());

            if (isDocsUploaded)
            {
                searchGridDiv.Visible = false;
                divTopSearch.Visible = false;
                UpdateDiv.Visible = true;

                FillHeader(rowId.ToString());

                FillItemDetails(rowId);

                // fetching the availability remark if exists
                FillAvailabilityRemark(rowId.ToString());

                // autofill new or existing tax heads (deduct, add and net amount)
                FillTaxHead(rowId.ToString());

                // autofill customer uploaded documents
                FillUploadedDocuments(rowId.ToString());
            }
            else
            {
                getSweetAlertInfoMandatory("Documents Not Uploaded", $"No Documents were Uploaded For Requisition No: {rowId.ToString()}, Kindly Ask Him/Her To Upload Documents First To Proceed Further");
            }
        }
    }

    private bool CheckForDocUploaded(string req1RefNo)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = $@"select * 
                            from billdocupload891 as doc 
                            inner join Requisition1891 as req1 on req1.RefNo = doc.BillRefNo
                            where req1.RefNo = @RefNo and doc.DocCategory = 'Document'";

            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@RefNo", req1RefNo);
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();

            return (dt.Rows.Count > 0);
        }
    }

    private void FillHeader(string requisitionRefNo)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();

            // checking if the availability status is already done, so to make it non-editable
             DataTable existingAvailabilityDT = CheckForExistingAvailabilityStatus(requisitionRefNo);

            if(existingAvailabilityDT.Rows.Count > 0)
            {
                getSweetAlertInfoMandatory("Payment Done", $"The Payment Has Been Done For Requisition: {requisitionRefNo}. Hence, The Availability Status Is For View Only.");

                // making gridview non-editable
                itemGrid.Enabled = false;
                GridTax.Enabled = false;

                // making buttons non-editable
                btnReCalTax.Enabled = false;
                btnSubmit.Enabled = false;
                btnSubmit.Visible = false;

                // making dropdown non-editalbe
                ddTaxOrNot.Enabled = false;

                // remark non-editable
                RemarkInput.Disabled = true;
            }

            string sql = $@"select * 
                            from Requisition1891 as req1 
                            INNER JOIN UserMaster891 as um ON req1.SaveBy = um.UserID 
                            INNER JOIN OrgType891 as org ON um.OrgType = org.RefID 
                            where req1.RefNo = @RefNo";

            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@RefNo", requisitionRefNo);
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();

            // 1st row
            txtReqNo.Text = dt.Rows[0]["ReqNo"].ToString();

            DateTime reqDate = DateTime.Parse(dt.Rows[0]["ReqDte"].ToString());
            dtReqDate.Text = reqDate.ToString("yyyy-MM-dd");

            // 2nd row
            OrgType.Text = dt.Rows[0]["OrgTyp"].ToString();
            InstituteName.Text = dt.Rows[0]["InstiName"].ToString();

            // remark (below text area)
            RemarkInput.Value = dt.Rows[0]["AvailabilityRemark"].ToString();

            // tax applied or not ?
            string isTaxApplied = dt.Rows[0]["TaxApplied"].ToString();
            if (isTaxApplied == "Yes") ddTaxOrNot.SelectedValue = "Yes";
            else ddTaxOrNot.SelectedValue = "No";
        }
    }

    private void FillItemDetails(int requisitionRefNo)
    {
        itemDiv.Visible = true;

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();

            string sql = $@"select req2.*, service.ServName 
                            from Requisition2891 as req2 
                            INNER JOIN ServMaster891 as service ON req2.ServiceName = service.RefID 
                            where req2.BillRefNo = @BillRefNo";

            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@BillRefNo", requisitionRefNo.ToString());
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);




            // manually adding new custom column
            dt.Columns.Add("AvailableQty", typeof(double));
            dt.Columns.Add("BalanceQty", typeof(double));

            foreach (DataRow row in dt.Rows)
            {
                row["AvailableQty"] = 0;
            }

            // adding the new column with checkboxes
            DataColumn checkboxColumn = new DataColumn("AvailableStatus", typeof(bool));
            checkboxColumn.DefaultValue = false;
            dt.Columns.Add(checkboxColumn);




            con.Close();

            itemGrid.DataSource = dt;
            itemGrid.DataBind();

            CheckingForCheckboxStatus(dt);

            ViewState["ReqDetailsVS"] = dt;
            Session["ReqDetails"] = dt;

            // total service amount
            //double? totalServiceAmount = dt.AsEnumerable().Sum(row => row["ServicePrice"] is DBNull ? (double?)null : Convert.ToDouble(row["ServicePrice"])) ?? 0.0;
            //TotalServiceAmnt.Text = totalServiceAmount.HasValue ? totalServiceAmount.Value.ToString("N2") : "0.00";
        }
    }

    private void CheckingForCheckboxStatus(DataTable Requisition2DT)
    {
        DataTable ReqReceivedDT = new DataTable();

        foreach (DataRow row in Requisition2DT.Rows)
        {
            string requisition2Refno = row["RefNo"].ToString();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string sql = "SELECT * FROM ReqReceived891 WHERE Req2RefNo = @Req2RefNo";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Req2RefNo", requisition2Refno);
                con.Open();

                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(ReqReceivedDT);
            }

            // checking prechecked checkbox
            foreach (DataRow reqReceivedRow in ReqReceivedDT.Rows)
            {
                // calculating balance qty for each row and assigning the value
                row["BalanceQty"] = Convert.ToDouble(reqReceivedRow["ReqQty"]) - Convert.ToDouble(reqReceivedRow["AvailableQty"]);

                row["SubItemPrice"] = Convert.ToDouble(reqReceivedRow["SubItemPrice"]);

                bool availableStatus = Convert.ToBoolean(reqReceivedRow["AvailableStatus"]);

                TextBox txtEdit = (TextBox)itemGrid.Rows[Requisition2DT.Rows.IndexOf(row)].FindControl("txtEdit");
                txtEdit.Text = reqReceivedRow["NonAvailabilityRemark"].ToString();

                // code for html input (type checkbox)
                //HtmlInputCheckBox chkAvailStatus = (HtmlInputCheckBox)itemGrid.Rows[Requisition2DT.Rows.IndexOf(row)].FindControl("chkAvailStatus");

                // code for asp:checkbox
                CheckBox chkAvailStatus = (CheckBox)itemGrid.Rows[Requisition2DT.Rows.IndexOf(row)].FindControl("chkAvailStatus");
                if (chkAvailStatus != null)
                {
                    // Use Checked property of asp:CheckBox
                    chkAvailStatus.Checked = availableStatus;
                    txtEdit.Visible = !availableStatus;
                }
            }
        }

        if (ReqReceivedDT.Rows.Count > 0)
        {
            // calculating total aervice amount if checkboxs are pre-checked
            double totalBill = 0.00;
            foreach (GridViewRow row in itemGrid.Rows)
            {
                int rowIndex = row.RowIndex;

                double requiredQty = Convert.ToDouble(ReqReceivedDT.Rows[rowIndex]["ReqQty"]);
                double availableQty = Convert.ToDouble(ReqReceivedDT.Rows[rowIndex]["AvailableQty"]);
                double servicePrice = Convert.ToDouble(ReqReceivedDT.Rows[rowIndex]["ServicePrice"]);
                double subItemPrice = Convert.ToDouble(ReqReceivedDT.Rows[rowIndex]["SubItemPrice"]);

                // to get actual previously entered available qty values
                TextBox TextAvailableQty = row.FindControl("txtAvailableQty") as TextBox;
                TextAvailableQty.Text = availableQty.ToString();

                // to get actual previously entered sub item price value
                TextBox SubItemPrice = row.FindControl("SubItemPrice") as TextBox;
                SubItemPrice.Text = subItemPrice.ToString();

                // checking the balance qty
                TextBox txtBalanceQty = row.FindControl("txtBalanceQty") as TextBox;
                txtBalanceQty.Text = (requiredQty - availableQty).ToString();

                // code for html input (type checkbox)
                //string availableStatus = ((HtmlInputCheckBox)row.FindControl("chkAvailStatus")).Checked.ToString(); // input - checkbox

                // code for asp:checkbox
                CheckBox chkAvailStatus = (CheckBox)row.FindControl("chkAvailStatus");
                string availableStatus = chkAvailStatus.Checked.ToString();

                if (availableStatus == "True")
                {
                    totalBill = totalBill + (availableQty * servicePrice);
                    SubItemPrice.Text = subItemPrice.ToString();
                }
                else
                {
                    SubItemPrice.Text = (0).ToString();
                }
            }

            TotalServiceAmnt.Text = totalBill.ToString("");
            Session["TotalBillAmount"] = totalBill;
        }
    }





    private void FillAvailabilityRemark(string req1RefNo)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();

            //string sql = $@"select * from Requisition1891 where RefNo = @RefNo";

            string sql = $@"select * from Requisition1891 where RefNo = @RefNo";

            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@RefNo", req1RefNo);
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();

            RemarkInput.Value = dt.Rows[0]["AvailabilityRemark"].ToString();
        }
    }


    private void FillUploadedDocuments(string req1RefNo)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "select * from BillDocUpload891 where BillRefNo = @BillRefNo AND DocCategory = 'Document'";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@BillRefNo", req1RefNo);
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();

            docGrid.Visible = true;

            GridDocument.DataSource = dt;
            GridDocument.DataBind();

            ViewState["DocDetails_VS"] = dt;
            Session["DocUploadDT"] = dt;
        }
    }





    //=========================={ Tax Head }==========================
    protected void GridTax_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow && (e.Row.RowState & DataControlRowState.Edit) > 0)
        {
            // Set the row in edit mode
            e.Row.RowState = e.Row.RowState ^ DataControlRowState.Edit;
        }

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            string req1RefNo = Session["Req1ReferenceNo"].ToString();

            // fetching acount head or taxes
            DataTable accountHeadExistingDT = getAccountHeadExisting(req1RefNo);
            DataTable accountHeadNewDT = getAccountHead();

            if (accountHeadExistingDT.Rows.Count > 0)
            {
                //=================================={ Add/Less column }========================================
                DropDownList ddlAddLess = (DropDownList)e.Row.FindControl("AddLess");

                if (ddlAddLess != null)
                {
                    string addLessValue = accountHeadExistingDT.Rows[e.Row.RowIndex]["AddLess"].ToString();
                    ddlAddLess.SelectedValue = addLessValue;
                }

                //=================================={ Percentage/Amount column }========================================
                DropDownList ddlPerOrAmnt = (DropDownList)e.Row.FindControl("PerOrAmnt");

                if (ddlPerOrAmnt != null)
                {
                    string perOrAmntValue = accountHeadExistingDT.Rows[e.Row.RowIndex]["PerOrAmnt"].ToString();
                    ddlPerOrAmnt.SelectedValue = perOrAmntValue;
                }
            }
            else
            {
                //=================================={ Add/Less column }========================================
                DropDownList ddlAddLess = (DropDownList)e.Row.FindControl("AddLess");

                if (ddlAddLess != null)
                {
                    string addLessValue = accountHeadNewDT.Rows[e.Row.RowIndex]["AddLess"].ToString();
                    ddlAddLess.SelectedValue = addLessValue;
                }

                //=================================={ Percentage/Amount column }========================================
                DropDownList ddlPerOrAmnt = (DropDownList)e.Row.FindControl("PerOrAmnt");

                if (ddlPerOrAmnt != null)
                {
                    string perOrAmntValue = accountHeadNewDT.Rows[e.Row.RowIndex]["PerOrAmnt"].ToString();
                    ddlPerOrAmnt.SelectedValue = perOrAmntValue;
                }
            }
        }
    }

    private void FillTaxHead(string req1RefNo)
    {
        string isTaxApplied = ddTaxOrNot.SelectedValue;
        if (isTaxApplied == "Yes")
        {
            string bills1Refno = req1RefNo;

            // total bill amount
            //double totalBillAmount = Convert.ToDouble(Session["TotalBillAmount"]);

            double totalBillAmount = 0.00;
            if (!string.IsNullOrEmpty(TotalServiceAmnt.Text.ToString()))
            {
                totalBillAmount = Convert.ToDouble(TotalServiceAmnt.Text);
            }

            if (totalBillAmount > 0.00) divTaxHead.Visible = true;
            else divTaxHead.Visible = false;


            // fetching acount head or taxes
            DataTable accountHeadExistingDT = getAccountHeadExisting(bills1Refno);
            DataTable accountHeadNewDT = getAccountHead();


            if (accountHeadExistingDT.Rows.Count > 0)
            {
                //ddTaxOrNot.SelectedValue = "Yes";

                Session["AccountHeadDT"] = accountHeadExistingDT;

                // fill tax heads
                autoFilltaxHeads(accountHeadExistingDT, totalBillAmount);
            }
            else
            {
                Session["AccountHeadDT"] = accountHeadNewDT;

                // fill tax heads
                autoFilltaxHeads(accountHeadNewDT, totalBillAmount);
            }
        }
    }

    private void autoFilltaxHeads(DataTable accountHeadDT, double bscAmnt)
    {
        double basicAmount = bscAmnt;
        double totalDeduction = 0.00;
        double totalAddition = 0.00;
        double netAmount = 0.00;

        foreach (DataRow row in accountHeadDT.Rows)
        {
            double percentage = Convert.ToDouble(row["TaxRate"]);

            double factorInPer = (basicAmount * percentage) / 100;

            if (row["AddLess"].ToString() == "Add")
            {
                totalAddition = totalAddition + factorInPer;
            }
            else
            {
                totalDeduction = totalDeduction + factorInPer;
            }

            row["TaxAmount"] = factorInPer;
        }

        GridTax.DataSource = accountHeadDT;
        GridTax.DataBind();

        // filling total deduction
        txtTotalDeduct.Text = Math.Abs(totalDeduction).ToString("");

        // filling total addition
        txtTotalAdd.Text = totalAddition.ToString("");

        // Net Amount after tax deductions or addition
        netAmount = (basicAmount + totalAddition) - Math.Abs(totalDeduction);
        txtNetAmnt.Text = netAmount.ToString("");
    }

    protected void btnReCalTax_Click(object sender, EventArgs e)
    {
        // Account Head DataTable
        DataTable dt = (DataTable)Session["AccountHeadDT"];

        // Perform calculations or other logic based on the updated values
        double totalBill = Convert.ToDouble(TotalServiceAmnt.Text);
        double totalDeduction = 0.00;
        double totalAddition = 0.00;
        double netAmount = 0.00;

        foreach (GridViewRow row in GridTax.Rows)
        {
            // Accessing the updated dropdown values from the grid
            string updatedAddLessValue = ((DropDownList)row.FindControl("AddLess")).SelectedValue;
            string updatedPerOrAmntValue = ((DropDownList)row.FindControl("PerOrAmnt")).SelectedValue;

            int rowIndex = row.RowIndex;

            // reading parameters from gridview
            TextBox AcHeadNameTxt = row.FindControl("AcHeadName") as TextBox;
            TextBox FactorInPerTxt = row.FindControl("TaxRate") as TextBox;
            DropDownList perOrAmntDropDown = row.FindControl("PerOrAmnt") as DropDownList;
            DropDownList AddLessDropown = row.FindControl("AddLess") as DropDownList;
            TextBox TaxAccountHeadAmount = row.FindControl("TaxAmount") as TextBox;

            string accountHeadName = (AcHeadNameTxt.Text).ToString();
            double taxRate = Convert.ToDouble(FactorInPerTxt.Text);
            string perOrAmnt = perOrAmntDropDown.SelectedValue;
            string addLess = AddLessDropown.SelectedValue;
            double taxAmount = Convert.ToDouble(TaxAccountHeadAmount.Text);

            if (perOrAmnt == "Amount")
            {
                taxAmount = taxRate;

                // setting tax head amount
                TaxAccountHeadAmount.Text = Math.Abs(taxAmount).ToString("");

                if (addLess == "Add")
                {
                    totalAddition = totalAddition + taxAmount;
                }
                else
                {
                    totalDeduction = totalDeduction + taxAmount;
                }
            }
            else
            {
                // tax amount (based on add or less)
                taxAmount = (totalBill * taxRate) / 100;

                // setting tax head amount
                TaxAccountHeadAmount.Text = Math.Abs(taxAmount).ToString("");

                if (addLess == "Add")
                {
                    totalAddition = totalAddition + taxAmount;
                }
                else
                {
                    totalDeduction = totalDeduction + taxAmount;
                }
            }
        }

        // setting total deduction
        txtTotalDeduct.Text = Math.Abs(totalDeduction).ToString("");

        // setting total addition
        txtTotalAdd.Text = totalAddition.ToString("");

        // Net Amount after tax deductions or addition
        netAmount = (totalBill + totalAddition) - Math.Abs(totalDeduction);
        txtNetAmnt.Text = netAmount.ToString("");
    }




    //=========================={ Submit Button Click Event }==========================
    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("RequisitionReceived.aspx");
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        string reqReferenceNo = Session["Req1ReferenceNo"].ToString();

        bool isCBChecked = false;

        if (Convert.ToDouble(TotalServiceAmnt.Text) > 0)
        {
            foreach (GridViewRow row in itemGrid.Rows)
            {
                // code to fetch checkbox status from html input (type - checkbox)
                //string availableStatus = ((HtmlInputCheckBox)row.FindControl("chkAvailStatus")).Checked.ToString();

                // code to fetch checkbox status from asp:checkbox
                string availableStatus = ((CheckBox)row.FindControl("chkAvailStatus")).Checked.ToString();

                if (availableStatus == "True")
                {
                    isCBChecked = true;
                }
            }

            if (isCBChecked)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlTransaction transaction = con.BeginTransaction();

                    try
                    {
                        // updating availability remark
                        UpdateHeader(reqReferenceNo, con, transaction);

                        // updating item details
                        InsertAvailableServices(reqReferenceNo, con, transaction);

                        if (ddTaxOrNot.SelectedValue == "Yes")
                        {
                            SaveReqBillTaxation(reqReferenceNo, con, transaction);
                        }

                        if (transaction != null) transaction.Commit();

                        getSweetAlertSuccessRedirectMandatory("Service(s) Received!", "Available Services Received Successfully", "");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                    }
                    finally
                    {
                        con.Close();
                        transaction.Dispose();
                    }
                }
            }
            else
            {
                getSweetAlertErrorMandatory("No Item Selected!", "Kindly Select Minimum 1 Item To Proceed");
            }
        }
        else
        {
            getSweetAlertErrorMandatory("No Item Selected!", "Kindly Select Minimum 1 Item To Proceed");
        }
    }




    private void UpdateHeader(string req1RefNo, SqlConnection con, SqlTransaction transaction)
    {
        string availabilityRemark = RemarkInput.Value;

        string isTaxApplied = ddTaxOrNot.SelectedValue;

        string sql = $@"update Requisition1891 set 
                        AvailabilityRemark = @AvailabilityRemark, TaxApplied=@TaxApplied where RefNo = @RefNo";

        SqlCommand cmd = new SqlCommand(sql, con, transaction);
        cmd.Parameters.AddWithValue("@AvailabilityRemark", availabilityRemark);
        cmd.Parameters.AddWithValue("@TaxApplied", isTaxApplied);
        cmd.Parameters.AddWithValue("@RefNo", req1RefNo);
        cmd.ExecuteNonQuery();
    }

    private void InsertAvailableServices(string reqReferenceNo, SqlConnection con, SqlTransaction transaction)
    {
        string requisition1Refno = reqReferenceNo;

        DataTable requisition2DT = (DataTable)Session["ReqDetails"]; // requisition 2 details

        string userID = Session["UserId"].ToString();


        foreach (GridViewRow row in itemGrid.Rows)
        {
            int rowIndex = row.RowIndex;

            string requisition2Refno = requisition2DT.Rows[rowIndex]["RefNo"].ToString();

            string serviceName = requisition2DT.Rows[rowIndex]["ServiceName"].ToString();
            string cellEnlistName = requisition2DT.Rows[rowIndex]["NmeCell"].ToString();
            string qty = requisition2DT.Rows[rowIndex]["Quty"].ToString();
            double servicePrice = Convert.ToDouble(requisition2DT.Rows[rowIndex]["ServicePrice"]);

            //bool availableStatus = ((CheckBox)row.FindControl("chkAvailStatus")).Checked; // asp:checkox
            //bool availableStatus = ((HtmlInputCheckBox)row.FindControl("chkAvailStatus")).Checked; // input - checkbox
            //int availableStatusValue = ((HtmlInputCheckBox)row.FindControl("chkAvailStatus")).Checked ? 1 : 0;

            string availableQty = ((TextBox)row.FindControl("txtAvailableQty")).Text; // editable text field
            string availableStatus = ((CheckBox)row.FindControl("chkAvailStatus")).Checked.ToString(); //  asp:checkox
            string non_availability_Reason = ((TextBox)row.FindControl("txtEdit")).Text; // editable text field
            string subItemPrice = ((TextBox)row.FindControl("SubItemPrice")).Text; // editable text field

            // calculating balance qty
            double balanceQty = Convert.ToDouble(qty) - Convert.ToDouble(availableQty);

            double totalServiceAmount = Convert.ToDouble(TotalServiceAmnt.Text);


            string reqRefNo = requisition2DT.Rows[rowIndex]["RefNo"].ToString();

            bool isItemExists = IsItemExists(reqRefNo, con, transaction);

            if (isItemExists) // update
            {
                string sql = $@"UPDATE ReqReceived891 SET 
                                AvailableQty=@AvailableQty, BalanceQty=@BalanceQty, AvailableStatus=@AvailableStatus, NonAvailabilityRemark=@NonAvailabilityRemark, SubItemPrice=@SubItemPrice 
                                WHERE Req1RefNo=@Req1RefNo AND Req2RefNo=@Req2RefNo";

                SqlCommand cmd = new SqlCommand(sql, con, transaction);
                cmd.Parameters.AddWithValue("@AvailableQty", availableQty);
                cmd.Parameters.AddWithValue("@BalanceQty", balanceQty);
                cmd.Parameters.AddWithValue("@AvailableStatus", availableStatus);
                cmd.Parameters.AddWithValue("@Req1RefNo", requisition1Refno);
                cmd.Parameters.AddWithValue("@Req2RefNo", requisition2Refno);
                cmd.Parameters.AddWithValue("@NonAvailabilityRemark", non_availability_Reason);
                cmd.Parameters.AddWithValue("@SubItemPrice", subItemPrice);
                cmd.ExecuteNonQuery();
            }
            else // insert
            {
                // getting new ref id for item
                string reqReceivedNewRefNo = GetItemRefNo(con, transaction);

                string sql = $@"INSERT INTO ReqReceived891 
                                (RefNo,Req1RefNo, Req2RefNo, ServiceName, CellName, ReqQty, AvailableQty, BalanceQty, ServicePrice, AvailableStatus, NonAvailabilityRemark, SubItemPrice, SaveBy) 
                                VALUES 
                                (@RefNo, @Req1RefNo, @Req2RefNo, @ServiceName, @CellName, @ReqQty, @AvailableQty, @BalanceQty, @ServicePrice, @AvailableStatus, @NonAvailabilityRemark, @SubItemPrice, @SaveBy)";

                SqlCommand cmd = new SqlCommand(sql, con, transaction);
                cmd.Parameters.AddWithValue("@RefNo", reqReceivedNewRefNo);
                cmd.Parameters.AddWithValue("@Req1RefNo", requisition1Refno);
                cmd.Parameters.AddWithValue("@Req2RefNo", requisition2Refno);
                cmd.Parameters.AddWithValue("@ServiceName", serviceName);
                cmd.Parameters.AddWithValue("@CellName", cellEnlistName);
                cmd.Parameters.AddWithValue("@ReqQty", qty);
                cmd.Parameters.AddWithValue("@AvailableQty", availableQty);
                cmd.Parameters.AddWithValue("@BalanceQty", balanceQty);
                cmd.Parameters.AddWithValue("@ServicePrice", servicePrice);
                cmd.Parameters.AddWithValue("@AvailableStatus", availableStatus);
                cmd.Parameters.AddWithValue("@NonAvailabilityRemark", non_availability_Reason);
                cmd.Parameters.AddWithValue("@SubItemPrice", subItemPrice);
                cmd.Parameters.AddWithValue("@SaveBy", userID);
                cmd.ExecuteNonQuery();
            }
        }

    }

    private bool IsItemExists(string reqRefNo, SqlConnection con, SqlTransaction transaction)
    {
        string sql = "select * from ReqReceived891 where Req2RefNo = @Req2RefNo";

        SqlCommand cmd = new SqlCommand(sql, con, transaction);
        cmd.Parameters.AddWithValue("@Req2RefNo", reqRefNo);
        cmd.ExecuteNonQuery();

        SqlDataAdapter ad = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        ad.Fill(dt);

        if (dt.Rows.Count > 0) return true;
        else return false;
    }

    private string GetItemRefNo(SqlConnection con, SqlTransaction transaction)
    {
        string nextRefNo = "1000001";

        string sql = "SELECT ISNULL(MAX(CAST(RefNo AS INT)), 10000) + 1 AS NextRefNo FROM ReqReceived891";
        SqlCommand cmd = new SqlCommand(sql, con, transaction);

        SqlDataAdapter ad = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        ad.Fill(dt);

        if (dt.Rows.Count > 0) return dt.Rows[0]["NextRefNo"].ToString();
        else return nextRefNo;
    }




    private void SaveReqBillTaxation(string req1RefNo, SqlConnection con, SqlTransaction transaction)
    {
        // Account Head DataTable
        DataTable dt = (DataTable)Session["AccountHeadDT"];

        if (dt != null)
        {
            bool isTaxHeadExists = CheckForExistingReqTaxHead(req1RefNo, con, transaction);

            foreach (GridViewRow row in GridTax.Rows)
            {
                int rowIndex = row.RowIndex;

                // additional details
                string AccountHeadCode = dt.Rows[rowIndex]["AcHCode"].ToString();


                // Tax Head Grid Details
                TextBox AcHNameStr = row.FindControl("AcHeadName") as TextBox;
                string AccountHeadName = (AcHNameStr.Text).ToString();

                TextBox TaxRateTXT = row.FindControl("TaxRate") as TextBox;
                double taxRate = Convert.ToDouble(TaxRateTXT.Text);

                DropDownList perOrAmntDropDown = row.FindControl("PerOrAmnt") as DropDownList;
                string perOrAmnt = perOrAmntDropDown.SelectedValue;

                DropDownList AddLessDropown = row.FindControl("AddLess") as DropDownList;
                string addLess = AddLessDropown.SelectedValue;

                TextBox TaxAccountHeadAmount = row.FindControl("TaxAmount") as TextBox;
                double taxAmount = Convert.ToDouble(TaxAccountHeadAmount.Text);

                string userID = Session["UserId"].ToString();


                if (isTaxHeadExists) // update
                {
                    string existingReqRefNo = dt.Rows[rowIndex]["RefNO"].ToString(); // existing req refno

                    string sql = $@"UPDATE ReqTaxHead891 
                                    SET 
                                    TaxRate=@TaxRate, PerOrAmnt=@PerOrAmnt, AddLess=@AddLess, TaxAmount=@TaxAmount 
                                    WHERE RefNO=@RefNO";

                    SqlCommand cmd = new SqlCommand(sql, con, transaction);
                    cmd.Parameters.AddWithValue("@TaxRate", taxRate);
                    cmd.Parameters.AddWithValue("@PerOrAmnt", perOrAmnt);
                    cmd.Parameters.AddWithValue("@AddLess", addLess);
                    cmd.Parameters.AddWithValue("@TaxAmount", taxAmount);
                    cmd.Parameters.AddWithValue("@RefNO", existingReqRefNo);
                    cmd.ExecuteNonQuery();
                }
                else // insert
                {
                    // getting new bill tax ref IDs
                    string newBillRefNo = GetNewBillTaxRefNo(con, transaction);

                    string sql = $@"INSERT INTO ReqTaxHead891 
                                    (RefNO, Req1RefNo, AcHeadName, AcHCode, TaxRate, PerOrAmnt, AddLess, TaxAmount, SaveBy) 
                                    VALUES 
                                    (@RefNO, @Req1RefNo, @AcHeadName, @AcHCode, @TaxRate, @PerOrAmnt, @AddLess, @TaxAmount, @SaveBy)";

                    SqlCommand cmd = new SqlCommand(sql, con, transaction);
                    cmd.Parameters.AddWithValue("@RefNO", newBillRefNo);
                    cmd.Parameters.AddWithValue("@Req1RefNo", req1RefNo);
                    cmd.Parameters.AddWithValue("@AcHeadName", AccountHeadName);
                    cmd.Parameters.AddWithValue("@AcHCode", AccountHeadCode);
                    cmd.Parameters.AddWithValue("@TaxRate", taxRate);
                    cmd.Parameters.AddWithValue("@PerOrAmnt", perOrAmnt);
                    cmd.Parameters.AddWithValue("@AddLess", addLess);
                    cmd.Parameters.AddWithValue("@TaxAmount", taxAmount);
                    cmd.Parameters.AddWithValue("@SaveBy", userID);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }

}