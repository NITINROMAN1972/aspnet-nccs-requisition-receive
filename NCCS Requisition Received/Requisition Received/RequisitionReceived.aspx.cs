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
        if (Session["UserID"] == null)
        {
            Response.Redirect("http://101.53.144.92/nccs/Ginie/Render/Login");
        }

        if (!IsPostBack)
        {

            Search_DD_RequistionNo();
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


    //=========================={ Fetch Datatable }==========================

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

            //string sql = "SELECT * FROM Requisition1891 WHERE 1=1";
            string sql = $@"SELECT *, org.OrgTyp 
                            FROM Requisition1891 as req1 
                            INNER JOIN Requisition2891 as req2 ON req1.RefNo = req2.BillRefNo 
                            INNER JOIN OrgType891 as org ON req2.OrgType = org.RefID 
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




    //=========================={ Update - Fill Details }==========================
    protected void gridSearch_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "lnkView")
        {
            int rowId = Convert.ToInt32(e.CommandArgument);
            Session["ReqReferenceNo"] = rowId;

            searchGridDiv.Visible = false;
            divTopSearch.Visible = false;
            UpdateDiv.Visible = true;

            FillRequisitionDetails(rowId);

            FillItemDetails(rowId);
        }
    }

    private void FillRequisitionDetails(int requisitionRefNo)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "select* from Requisition1891 where RefNo = @RefNo";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@RefNo", requisitionRefNo);
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();


            txtReqNo.Text = dt.Rows[0]["ReqNo"].ToString();

            DateTime reqDate = DateTime.Parse(dt.Rows[0]["ReqDte"].ToString());
            dtReqDate.Text = reqDate.ToString("yyyy-MM-dd");
        }
    }

    private void FillItemDetails(int requisitionRefNo)
    {
        itemDiv.Visible = true;

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = $@"select req.*, service.ServName 
                            from Requisition2891 as req 
                            INNER JOIN ServMaster891 as service ON req.ServiceName = service.RefID 
                            where BillRefNo = @BillRefNo";

            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@BillRefNo", requisitionRefNo.ToString());
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);

            // manually adding new custom editable column
            dt.Columns.Add("AvailableQty", typeof(string));

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
                bool availableStatus = Convert.ToBoolean(reqReceivedRow["AvailableStatus"]);


                HtmlInputCheckBox chkAvailStatus = (HtmlInputCheckBox)itemGrid.Rows[Requisition2DT.Rows.IndexOf(row)].FindControl("chkAvailStatus");
                if (chkAvailStatus != null)
                {
                    chkAvailStatus.Checked = availableStatus;
                }
            }
        }

        // calculating totl aervice amount if checkboxs are pre-checked
        double totalBill = 0.00;
        foreach (GridViewRow row in itemGrid.Rows)
        {
            int rowIndex = row.RowIndex;

            double servicePrice = Convert.ToDouble(Requisition2DT.Rows[rowIndex]["ServicePrice"]);

            string availableStatus = ((HtmlInputCheckBox)row.FindControl("chkAvailStatus")).Checked.ToString(); // input - checkbox

            if (availableStatus == "True")
            {
                totalBill = totalBill + servicePrice;
            }
        }

        TotalServiceAmnt.Text = totalBill.ToString("N2");
    }






    //=========================={ Calculate Button Event }==========================
    protected void btnTotalBill_Click(object sender, EventArgs e)
    {
        DataTable requisition2DT = (DataTable)Session["ReqDetails"]; // requisition 2 details

        double totalBill = 0.00;

        // calculating totl aervice amount
        foreach (GridViewRow row in itemGrid.Rows)
        {
            int rowIndex = row.RowIndex;

            double servicePrice = Convert.ToDouble(requisition2DT.Rows[rowIndex]["ServicePrice"]);

            string availableStatus = ((HtmlInputCheckBox)row.FindControl("chkAvailStatus")).Checked.ToString(); // input - checkbox

            if (availableStatus == "True")
            {
                totalBill = totalBill + servicePrice;
            }

            TotalServiceAmnt.Text = totalBill.ToString("N2");
        }
    }





    //=========================={ Submit Button Click Event }==========================
    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("RequisitionReceived.aspx");
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        string reqReferenceNo = Session["ReqReferenceNo"].ToString();

        // updating item details
        InsertAvailableServices(reqReferenceNo);

        getSweetAlertSuccessRedirectMandatory("Service(s) Received!", "Available Services Received", "");
    }


    private void InsertAvailableServices(string reqReferenceNo)
    {
        string requisition1Refno = reqReferenceNo;

        DataTable requisition2DT = (DataTable)Session["ReqDetails"]; // requisition 2 details

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();

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
                string availableStatus = ((HtmlInputCheckBox)row.FindControl("chkAvailStatus")).Checked.ToString(); // input - checkbox

                double totalServiceAmount = Convert.ToDouble(TotalServiceAmnt.Text);




                string reqRefNo = requisition2DT.Rows[rowIndex]["RefNo"].ToString();

                bool isItemExists = IsItemExists(reqRefNo);

                if (isItemExists) // update
                {
                    string sql = $@"UPDATE ReqReceived891 SET 
                                    AvailableQty=@AvailableQty, AvailableStatus=@AvailableStatus 
                                    WHERE Req2RefNo=@Req2RefNo";

                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@AvailableQty", availableQty);
                    cmd.Parameters.AddWithValue("@AvailableStatus", availableStatus);
                    cmd.Parameters.AddWithValue("@Req2RefNo", requisition2Refno);
                    cmd.ExecuteNonQuery();
                }
                else // insert
                {
                    // getting new ref id for item
                    string reqReceivedNewRefNo = GetItemRefNo().ToString();

                    string sql = $@"INSERT INTO ReqReceived891 
                                    (RefNo, Req2RefNo, ServiceName, CellName, ReqQty, AvailableQty, ServicePrice, AvailableStatus) 
                                    VALUES 
                                    (@RefNo, @Req2RefNo, @ServiceName, @CellName, @ReqQty, @AvailableQty, @ServicePrice, @AvailableStatus)";

                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@RefNo", reqReceivedNewRefNo);
                    cmd.Parameters.AddWithValue("@Req2RefNo", requisition2Refno);
                    cmd.Parameters.AddWithValue("@ServiceName", serviceName);
                    cmd.Parameters.AddWithValue("@CellName", cellEnlistName);
                    cmd.Parameters.AddWithValue("@ReqQty", qty);
                    cmd.Parameters.AddWithValue("@AvailableQty", availableQty);
                    cmd.Parameters.AddWithValue("@ServicePrice", servicePrice);
                    cmd.Parameters.AddWithValue("@AvailableStatus", availableStatus);
                    cmd.ExecuteNonQuery();
                }
            }

            con.Close();
        }

    }

    private bool IsItemExists(string reqRefNo)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "select * from ReqReceived891 where Req2RefNo = @Req2RefNo";

            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Req2RefNo", reqRefNo);
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);

            con.Close();

            if (dt.Rows.Count > 0) return true;
            else return false;
        }
    }

    private int GetItemRefNo()
    {
        string nextRefID = "1000001";

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "SELECT ISNULL(MAX(CAST(RefNo AS INT)), 10000) + 1 AS NextRefID FROM ReqReceived891";
            SqlCommand cmd = new SqlCommand(sql, con);

            object result = cmd.ExecuteScalar();
            if (result != null && result != DBNull.Value) { nextRefID = result.ToString(); }
            return Convert.ToInt32(nextRefID);
        }
    }

}