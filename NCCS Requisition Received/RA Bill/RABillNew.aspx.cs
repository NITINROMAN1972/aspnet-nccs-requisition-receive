using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class RA_Bill_New_RABillNew : System.Web.UI.Page
{
    string connectionString = ConfigurationManager.ConnectionStrings["Ginie"].ConnectionString;

    string selectedCategoryRefID;
    string selectedSubCategoryRefID;
    string selectedAODetails;
    string selectedProjectMasterRefID;
    string selectedWorkOrderRefID;
    string selectedVendorRefID;
    string selectedAbstractNO;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Bind_Role_ProjectMaster();

            Session["AccountHeadDT"] = "";
        }

        // alert pop-up with only message
        //string message = "updatedPerOrAmntValue  " + updatedPerOrAmntValue;
        //string script = $"alert('{message}');";
        //ScriptManager.RegisterStartupScript(this, this.GetType(), "messageScript", script, true);
    }

    //=============================={ Sweet Alert }============================================

    // sweet alert - success only
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

    //==========================={ Dropdown Bind }===========================

    public void Bind_Role_ProjectMaster()
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "select * from ProjectMaster874";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();

            ddProjectMaster.DataSource = dt;
            ddProjectMaster.DataTextField = "ProjectName";
            ddProjectMaster.DataValueField = "RefID";
            ddProjectMaster.DataBind();
            ddProjectMaster.Items.Insert(0, new ListItem("------Select Sub Category------", "0"));
        }
    }

    public void Bind_Role_WorkOrderDetails(string selectedProjectMasterRefID)
    {
        DataTable projectMasterDt = getProjectMaster(selectedProjectMasterRefID); // project ref id

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "select * from WorkOrder874 where woProject=@woProject";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@woProject", projectMasterDt.Rows[0]["RefID"].ToString());
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();

            ddWorkOrder.DataSource = dt;
            ddWorkOrder.DataTextField = "woTitle";
            ddWorkOrder.DataValueField = "RefID";
            ddWorkOrder.DataBind();
            ddWorkOrder.Items.Insert(0, new ListItem("------Select Work Order------", "0"));
        }
    }

    public void Bind_Role_VendorName(string selectedWorkOrderRefID)
    {
        DataTable WorkOrderDT = getWorkOrderDetails(selectedWorkOrderRefID); // wo ref id

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "SELECT v.vName, v.RefID FROM VendorMaster874 as v INNER JOIN WorkOrder874 AS w ON v.RefID = w.woVendor AND w.RefID = @RefID";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@RefID", WorkOrderDT.Rows[0]["RefID"].ToString());
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();

            ddVender.DataSource = dt;
            ddVender.DataTextField = "vName";
            ddVender.DataValueField = "RefID";
            ddVender.DataBind();
            ddVender.Items.Insert(0, new ListItem("------Select Vendor------", "0"));
        }
    }

    public void Bind_Role_MileStone(string selectedWorkOrderRefID)
    {

    }

    public void Bind_Role_AbstractNo(string selectedProjectMasterRefID, string selectedWorkOrderRefID)
    {
        DataTable projectMasterDt = getProjectMaster(selectedProjectMasterRefID); // project ref id
        DataTable WorkOrderDT = getWorkOrderDetails(selectedWorkOrderRefID); // wo ref id

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "SELECT * from AbstApproval874 where AbsProj = @AbsProj AND AbsWO = @AbsWO";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@AbsProj", projectMasterDt.Rows[0]["RefID"].ToString());
            cmd.Parameters.AddWithValue("@AbsWO", WorkOrderDT.Rows[0]["RefID"].ToString());
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();

            ddAbstractNo.DataSource = dt;
            ddAbstractNo.DataTextField = "AbsNo";
            ddAbstractNo.DataValueField = "AbsNo";
            ddAbstractNo.DataBind();
            ddAbstractNo.Items.Insert(0, new ListItem("------Select Abstract No.------", "0"));
        }
    }

    public void Bind_Role_DocType()
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "SELECT * FROM DocType874";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();

            ddDocType.DataSource = dt;
            ddDocType.DataTextField = "DocType";
            ddDocType.DataValueField = "DocType";
            ddDocType.DataBind();
            ddDocType.Items.Insert(0, new ListItem("------Select Doc------", "0"));
        }
    }

    public void Bind_Role_Stages()
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "select * from WorkStage874";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();

            ddStage.DataSource = dt;
            ddStage.DataTextField = "StageLevel";
            ddStage.DataValueField = "StageLevel";
            ddStage.DataBind();
            ddStage.Items.Insert(0, new ListItem("------Select Stage Level------", "0"));
        }
    }

    //==========================={ Dropdown Event }===========================

    protected void ddProjectMaster_SelectedIndexChanged(object sender, EventArgs e)
    {
        selectedProjectMasterRefID = ddProjectMaster.SelectedValue; // Project Master RefID

        if (ddProjectMaster.SelectedValue != "0")
        {
            Bind_Role_WorkOrderDetails(selectedProjectMasterRefID);
        }
        else
        {
            // Clear the values of follwing dropdowns on the server side
            ddWorkOrder.Items.Clear();
            ddVender.Items.Clear();

            // Clear the values of ddWorkOrder & ddVender on the client side using JavaScript
            ScriptManager.RegisterStartupScript(this, GetType(), "ClearWorkOrderDropdown", "ClearWorkOrderDropdown();", true);
            ScriptManager.RegisterStartupScript(this, GetType(), "ClearVendorDropdown", "clearVendorDropdown();", true);
        }
    }

    protected void ddWorkOrder_SelectedIndexChanged(object sender, EventArgs e)
    {
        selectedProjectMasterRefID = ddProjectMaster.SelectedValue; // Project ref id
        selectedWorkOrderRefID = ddWorkOrder.SelectedValue; // WO RefID

        if (ddWorkOrder.SelectedValue != "0")
        {
            Bind_Role_VendorName(selectedWorkOrderRefID);
            Bind_Role_AbstractNo(selectedProjectMasterRefID, selectedWorkOrderRefID);

            // checking if there is only one vendor
            if (ddVender.Items.Count == 2)
            {
                ddVender.SelectedIndex = 1;
            }
            else
            {
                ddVender.SelectedIndex = 0;
            }
        }
        else
        {
            // Clear the values of ddVender on the server side
            ddVender.Items.Clear();
            ddAbstractNo.Items.Clear();

            // Clear the values of ddVender on the client side using JavaScript
            ScriptManager.RegisterStartupScript(this, GetType(), "ClearVendorDropdown", "clearVendorDropdown();", true);
        }
    }

    protected void ddVender_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    protected void ddMileStone_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    protected void ddAbstractNo_SelectedIndexChanged(object sender, EventArgs e)
    {
        selectedProjectMasterRefID = ddProjectMaster.SelectedValue; // Project ref id
        selectedWorkOrderRefID = ddWorkOrder.SelectedValue; // WO RefID
        selectedAbstractNO = ddAbstractNo.SelectedValue; // Abstract No. RefID

        FillGridViewWithBoqDetails(selectedProjectMasterRefID, selectedWorkOrderRefID, selectedAbstractNO);
    }

    //==========================={ Fetching data }===========================

    private DataTable getProjectMaster(string selectedProjectMasterRefIDCode)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "SELECT * FROM ProjectMaster874 where RefID=@RefID";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@RefID", selectedProjectMasterRefIDCode.ToString());
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();
            return dt;
        }
    }

    private DataTable getWorkOrderDetails(string selectedWorkOrderRefID)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "SELECT * FROM WorkOrder874 where RefID=@RefID";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@RefID", selectedWorkOrderRefID.ToString());
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();
            return dt;
        }
    }

    private DataTable getVendorDetails(string selectedVendorRefID)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "SELECT * FROM VendorMaster874 where RefID=@RefID";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@RefID", selectedVendorRefID.ToString());
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();
            return dt;
        }
    }

    private DataTable getAbstractNoDetails(string selectedAbstractNO)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "SELECT * FROM AbstApproval874 where AbsNo=@AbsNo";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@AbsNo", selectedAbstractNO.ToString());
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();
            return dt;
        }
    }

    private DataTable getEmbHeader(string selectedAbstracEmbtHeader)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "SELECT * FROM EmbMaster874 as emb, AbstApproval874 as abs where abs.AbsEmbHeader = emb.EmbMasRefId and abs.AbsEmbHeader = @AbsEmbHeader";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@AbsEmbHeader", selectedAbstracEmbtHeader.ToString());
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();
            return dt;
        }
    }

    private DataTable getEmbDetails(string embHeaderRefId)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "SELECT * FROM EmbRecords874 where EmbHeaderId = @EmbHeaderId";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@EmbHeaderId", embHeaderRefId.ToString());
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
            string sql = "SELECT * FROM AccountHead874 WHERE AccHeadGroup = 'Duties & Taxes'";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();
            return dt;
        }
    }

    //=============================={ Fill BoQ Grid Records }============================================

    protected void GridTax_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow && (e.Row.RowState & DataControlRowState.Edit) > 0)
        {
            // Set the row in edit mode
            e.Row.RowState = e.Row.RowState ^ DataControlRowState.Edit;
        }

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            // fetching acount head or taxes
            DataTable accountHeadDT = getAccountHead();

            //=================================={ Add/Less column }========================================
            DropDownList ddlAddLess = (DropDownList)e.Row.FindControl("AddLess");

            if (ddlAddLess != null)
            {
                string addLessValue = accountHeadDT.Rows[e.Row.RowIndex]["AddLess"].ToString();
                ddlAddLess.SelectedValue = addLessValue;
            }

            //=================================={ Percentage/Amount column }========================================
            DropDownList ddlPerOrAmnt = (DropDownList)e.Row.FindControl("PerOrAmnt");

            if (ddlPerOrAmnt != null)
            {
                string perOrAmntValue = accountHeadDT.Rows[e.Row.RowIndex]["PerOrAmnt"].ToString();
                ddlPerOrAmnt.SelectedValue = perOrAmntValue;
            }
        }
    }

    private void BindGridDocuments()
    {
       
    }

    private void autoFilltaxHeads(DataTable accountHeadDT, double bscAmnt)
    {
        double basicAmount = bscAmnt;
        double totalDeduction = 0.00;
        double totalAddition = 0.00;
        double netAmount = 0.00;

        foreach (DataRow row in accountHeadDT.Rows)
        {
            double percentage = Convert.ToDouble(row["FactorInPer"]);

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
        txtTotalDeduct.Text = Math.Abs(totalDeduction).ToString("N2");

        // filling total addition
        txtTotalAdd.Text = totalAddition.ToString("N2");

        // Net Amount after tax deductions or addition
        netAmount = (basicAmount + totalAddition) - Math.Abs(totalDeduction);
        txtNetAmnt.Text = netAmount.ToString("N2");
    }

    private void FillGridViewWithBoqDetails(string selectedProjectMasterRefID, string selectedWorkOrderRefID, string selectedAbstractNoHeader)
    {
        DataTable projectMasterDt = getProjectMaster(selectedProjectMasterRefID); // project ref id
        DataTable WorkOrderDT = getWorkOrderDetails(selectedWorkOrderRefID); // wo ref id
        DataTable AbstractDT = getAbstractNoDetails(selectedAbstractNoHeader); // abstract emb header

        // assign work order amount
        int woAmount = Convert.ToInt32(WorkOrderDT.Rows[0]["woTendrValue"]);
        //txtWoAmnt.Text = "₹ " + woAmount.ToString("N0");
        txtWoAmnt.Text = woAmount.ToString();

        // fetching emb header using abstract
        DataTable embHeaderDT = getEmbHeader(AbstractDT.Rows[0]["AbsEmbHeader"].ToString());

        // fetching emb details
        DataTable emdDetailsDT = getEmbDetails(embHeaderDT.Rows[0]["EmbMasRefId"].ToString());
        Session["BoQDT"] = emdDetailsDT;

        // fetching acount head or taxes
        DataTable accountHeadDT = getAccountHead();
        Session["AccountHeadDT"] = accountHeadDT;

        // binding document dropdowns
        Bind_Role_DocType();
        Bind_Role_Stages();

        if (emdDetailsDT.Rows.Count > 0)
        {
            gridEmbDiv.Visible = true;
            divTax.Visible = true;
            btnReCalTax.Enabled = true;

            txtBasicAmt.Text = emdDetailsDT.Rows[0]["BasicAmount"].ToString();
            double basicAmount = Convert.ToDouble(txtBasicAmt.Text);

            // fill tax heads
            autoFilltaxHeads(accountHeadDT, basicAmount);

            // grid bind for emb details
            gridDynamicBOQ.DataSource = emdDetailsDT;
            gridDynamicBOQ.DataBind();
        }
        else
        {
            // alert pop-up with only message
            string message1 = "No EMB Details Found";
            string script1 = $"alert('{message1}');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "messageScript", script1, true);
        }
    }

    //=============================={ Button Click }============================================

    protected void btnBasicAmount_Click(object sender, EventArgs e)
    {
        DataTable dt = (DataTable)Session["BoQDT"];

        double basicAmount = 0;

        if (dt != null)
        {
            // Iterate through each row in the GridView
            foreach (GridViewRow row in gridDynamicBOQ.Rows)
            {
                // code to read values from row-bound grid columns
                //TextBox boqQty = row.FindControl("QtyMeasure") as TextBox;
                //int qtyMeasuredValue = Convert.ToInt32(boqQty.Text);

                int rowIndex = row.RowIndex; // current row

                // code to read normal grid column values
                double qtyMeasured = Convert.ToInt32(dt.Rows[rowIndex]["BoqQtyMeas"]);
                double boqUnitRate = Convert.ToInt32(dt.Rows[rowIndex]["BoQItemRate"]);

                double prod = (qtyMeasured * boqUnitRate);
                basicAmount = basicAmount + prod;
            }

            string basicAmountStr = basicAmount.ToString("N0");

            txtBasicAmt.CssClass = "form-control fw-normal border border-2";
            txtBasicAmt.Text = basicAmountStr;
        }
    }

    protected void btnReCalTax_Click(object sender, EventArgs e)
    {
        // Account Head DataTable
        DataTable dt = (DataTable)Session["AccountHeadDT"];

        // Perform calculations or other logic based on the updated values
        double basicAmount = Convert.ToDouble(txtBasicAmt.Text);
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
            TextBox DeductionHeadStr = row.FindControl("DeductionHead") as TextBox;
            TextBox FactorInPercentage = row.FindControl("FactorInPer") as TextBox;
            DropDownList perOrAmntDropDown = row.FindControl("PerOrAmnt") as DropDownList;
            DropDownList AddLessDropown = row.FindControl("AddLess") as DropDownList;
            TextBox TaxAccountHeadAmount = row.FindControl("TaxAmount") as TextBox;

            string DeductionHead = (DeductionHeadStr.Text).ToString();
            double factorInPer = Convert.ToDouble(FactorInPercentage.Text);
            string perOrAmnt = perOrAmntDropDown.SelectedValue;
            string addLess = AddLessDropown.SelectedValue;
            double taxAmount = Convert.ToDouble(TaxAccountHeadAmount.Text);

            if (perOrAmnt == "Amount")
            {
                taxAmount = factorInPer;

                // setting tax head amount
                TaxAccountHeadAmount.Text = Math.Abs(taxAmount).ToString("N2");

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
                taxAmount = (basicAmount * factorInPer) / 100;

                // setting tax head amount
                TaxAccountHeadAmount.Text = Math.Abs(taxAmount).ToString("N2");

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
        txtTotalDeduct.Text = Math.Abs(totalDeduction).ToString("N2");

        // setting total addition
        txtTotalAdd.Text = totalAddition.ToString("N2");

        // Net Amount after tax deductions or addition
        netAmount = (basicAmount + totalAddition) - Math.Abs(totalDeduction);
        txtNetAmnt.Text = netAmount.ToString("N2");
    }

    protected void btnDocUpload_Click(object sender, EventArgs e)
    {
        // setting the file size in web.config file (web.config should not be read only)
        //settingHttpRuntimeForFileSize();

        string docTypeCode = ddDocType.SelectedValue;
        string stageCode = ddStage.SelectedValue;

        if (fileDoc.HasFile)
        {
            string FileExtension = System.IO.Path.GetExtension(fileDoc.FileName);

            if (FileExtension == ".xlsx" || FileExtension == ".xls")
            {

            }

            // file name
            string onlyFileNameWithExtn = fileDoc.FileName.ToString();

            // getting unique file name
            string strFileName = GenerateUniqueId(onlyFileNameWithExtn);

            // saving and getting file path
            string filePath = getServerFilePath(strFileName);

            // Retrieve DataTable from ViewState or create a new one
            DataTable dt = ViewState["DocDetailsDataTable"] as DataTable ?? CreateDocDetailsDataTable();

            // getting new document ref id
            int DocUploadRefID = getDocUploadedRefID();

            // filling document details datatable
            AddRowToDocDetailsDataTable(DocUploadRefID.ToString() , dt, docTypeCode, stageCode, onlyFileNameWithExtn, filePath);

            // Save DataTable to ViewState
            ViewState["DocDetailsDataTable"] = dt;
            Session["DocuUploadDT"] = dt;

            if (dt.Rows.Count > 0)
            {
                // binding document details gridview
                GridDocument.DataSource = dt;
                GridDocument.DataBind();

                //btnSubmit.Enabled = true;
            }
        }
    }

    private void settingHttpRuntimeForFileSize()
    {
        // Get the current HttpRuntime configuration
        HttpRuntimeSection httpRuntimeSection = (HttpRuntimeSection)System.Configuration.ConfigurationManager.GetSection("system.web/httpRuntime");

        // Set a larger maxRequestLength value (e.g., 100 MB)
        httpRuntimeSection.MaxRequestLength = 102400;

        // Save the changes to the configuration
        System.Configuration.Configuration config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
        config.Save();
    }

    private string GenerateUniqueId(string strFileName)
    {
        long timestamp = DateTime.Now.Ticks;
        //string guid = Guid.NewGuid().ToString("N"); //N to remove hypen "-" from GUIDs
        string guid = Guid.NewGuid().ToString(); //N to remove hypen "-" from GUIDs
        string uniqueID = timestamp + "_" + guid + "_" + strFileName;
        return uniqueID;
    }

    private string getServerFilePath(string strFileName)
    {
        string orgFilePath = Server.MapPath("~/Portal/Public/" + strFileName);

        // saving file
        fileDoc.SaveAs(orgFilePath);

        //string filePath = Server.MapPath("~/Portal/Public/" + strFileName);
        //file:///C:/HostingSpaces/PAWAN/cdsmis.in/wwwroot/Pms2/Portal/Public/638399011215544557_926f9320-275e-49ad-8f59-32ecb304a9f1_EMB%20Recording.pdf

        // replacing server-specific path with the desired URL
        string baseUrl = "http://101.53.144.92/pms2/Ginie/External?url=..";
        string relativePath = orgFilePath.Replace(Server.MapPath("~/Portal/Public/"), "Portal/Public/");

        // Full URL for the hyperlink
        string fullUrl = $"{baseUrl}/{relativePath}";

        return fullUrl;
    }

    private DataTable CreateDocDetailsDataTable()
    {
        DataTable dt = new DataTable();

        // document ref id
        DataColumn RefID = new DataColumn("RefID", typeof(string));
        dt.Columns.Add(RefID);

        // document type
        DataColumn docType = new DataColumn("docType", typeof(string));
        dt.Columns.Add(docType);

        // stage level
        DataColumn stageLevel = new DataColumn("stageLevel", typeof(string));
        dt.Columns.Add(stageLevel);

        // file name
        DataColumn onlyFileName = new DataColumn("onlyFileName", typeof(string));
        dt.Columns.Add(onlyFileName);

        // Doc uploaded path
        DataColumn docPath = new DataColumn("docPath", typeof(string));
        dt.Columns.Add(docPath);

        return dt;
    }

    private void AddRowToDocDetailsDataTable(string RefID, DataTable dt, string docTypeCode, string stageCode, string onlyFileNameWithExtn, string filePath)
    {
        // Create a new row
        DataRow row = dt.NewRow();

        // Set values for the new row
        row["RefID"] = docTypeCode;
        row["docType"] = docTypeCode;
        row["stageLevel"] = stageCode;
        row["onlyFileName"] = onlyFileNameWithExtn;
        row["docPath"] = filePath;

        // Add the new row to the DataTable
        dt.Rows.Add(row);
    }

    //=============================={ Submit Button }============================================

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("Update/RABillUpdate.aspx");
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        // dropd downs
        selectedProjectMasterRefID = ddProjectMaster.SelectedValue; // Project Master RefID
        selectedWorkOrderRefID = ddWorkOrder.SelectedValue; // Work Order RefID
        selectedVendorRefID = ddVender.SelectedValue; // Vendor RefID
        selectedAbstractNO = ddAbstractNo.SelectedValue; // Abstract No

        // strings
        string remarks = txtRemarks.Value.ToString(); // remarks
        string raBillNo = txtBillNo.Text; // ra bill no

        //// Datatables
        DataTable projectMasterDt = getProjectMaster(selectedProjectMasterRefID); // Ref Id
        DataTable workOrderDt = getWorkOrderDetails(selectedWorkOrderRefID); // Ref Id
        DataTable vendorDt = getVendorDetails(selectedVendorRefID); // Ref Id
        DataTable abstractDt = getAbstractNoDetails(selectedAbstractNO); // Ref Id

        // Numbers
        int workOrderAmount = Convert.ToInt32(workOrderDt.Rows[0]["woTendrValue"].ToString()); // Work Order Amount
        int totalBillBookedAmount = 0; // Total Ra Bill Booked
        double basicAmount = Convert.ToDouble(txtBasicAmt.Text); // basic amount
        double totalDeduct = Convert.ToDouble(txtTotalDeduct.Text); // total deduction
        double totalAdd = Convert.ToDouble(txtTotalAdd.Text); // total addition
        double netAmount = Convert.ToDouble(txtNetAmnt.Text); // net amount adter taxes

        // selected Dates
        // DateTime? abstractDate = !String.IsNullOrEmpty(dateAbstract.Text) ? DateTime.Parse(dateAbstract.Text) : (DateTime?)null;
        DateTime raBillDate = DateTime.Parse(dateBillDate.Text);
        DateTime paymentDueDate = DateTime.Parse(datePayDueDate.Text);

        // Ref ID Generation
        int RaRefID = getRaHeaderRefID();

        // checking for any documents uplaoded or not
        if(GridDocument.Rows.Count > 0)
        {
            // inserting RA Header
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string sql = "INSERT INTO RaHeader874 " +
                             "(RefID, RaHeaderID, RaProj, RaWO, RaVendor, RaAbstNo, RaWoAmount, RaBillBookAmnt, RaRemarks, RaBillDate, RaBillNo, RaPayDueDate, RaBasicAmount, " +
                                "RaTotalDeduct, RaTotalAdd, RaNetAmount) " +

                             "VALUES " +
                             "(@RefID, @RaHeaderID, @RaProj, @RaWO, @RaVendor, @RaAbstNo, @RaWoAmount, @RaBillBookAmnt, @RaRemarks, @RaBillDate, @RaBillNo, @RaPayDueDate, @RaBasicAmount, " +
                                "@RaTotalDeduct, @RaTotalAdd, @RaNetAmount) " +

                             "SELECT SCOPE_IDENTITY();";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@RefID", RaRefID.ToString());
                cmd.Parameters.AddWithValue("@RaHeaderID", RaRefID.ToString());
                cmd.Parameters.AddWithValue("@RaProj", projectMasterDt.Rows[0]["ProjectName"].ToString());
                cmd.Parameters.AddWithValue("@RaWO", workOrderDt.Rows[0]["woTitle"].ToString());
                cmd.Parameters.AddWithValue("@RaVendor", vendorDt.Rows[0]["vName"].ToString());
                cmd.Parameters.AddWithValue("@RaAbstNo", abstractDt.Rows[0]["AbsNo"].ToString());
                cmd.Parameters.AddWithValue("@RaWoAmount", workOrderAmount);
                cmd.Parameters.AddWithValue("@RaBillBookAmnt", totalBillBookedAmount);
                cmd.Parameters.AddWithValue("@RaRemarks", remarks);
                cmd.Parameters.AddWithValue("@RaBillDate", raBillDate.ToString("dd-MM-yyyy"));
                cmd.Parameters.AddWithValue("@RaBillNo", raBillNo);
                cmd.Parameters.AddWithValue("@RaPayDueDate", paymentDueDate.ToString("dd-MM-yyyy"));
                cmd.Parameters.AddWithValue("@RaBasicAmount", basicAmount);
                cmd.Parameters.AddWithValue("@RaTotalDeduct", totalDeduct);
                cmd.Parameters.AddWithValue("@RaTotalAdd", totalAdd);
                cmd.Parameters.AddWithValue("@RaNetAmount", netAmount);
                //cmd.ExecuteNonQuery();

                int RaHeaderId = Convert.ToInt32(cmd.ExecuteScalar());
                con.Close();

                // inserting EMB details
                insertRaDetails(RaRefID);

                // inserting tax heads
                insertActionHead(RaRefID);

                // insert uploaded documents from gridview
                insertUploadedDocuments(RaRefID);

                // sweet alert - success redirect
                getSweetAlertSuccessRedirect("Update/RABillUpdate.aspx");
            }
        }
        else
        {
            // sweet alert - error only block
            getSweetAlertErrorMandatory("Document Not Found", "Please upload minimum one document!");
        }
    }

    private void insertRaDetails(int RaHeaderId)
    {
        DataTable dt = (DataTable)Session["BoQDT"];

        try
        {
            if (dt != null)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    foreach (GridViewRow row in gridDynamicBOQ.Rows)
                    {
                        // getting ref IDs for all boq individual records
                        int RaDetailsRefID = getRaDetailsRefID();

                        // to get the current row index
                        int rowIndex = row.RowIndex;

                        // current uploaded boq index, to be used for updating the original value
                        // string currentBoQRefID = dt.Rows[rowIndex]["RefID"].ToString();

                        string boqItemName = dt.Rows[rowIndex]["BoQItemName"].ToString();
                        string uom = dt.Rows[rowIndex]["BoQUOM"].ToString();
                        double boqQty = Convert.ToDouble(dt.Rows[rowIndex]["BoqQty"]);
                        double boqQtyMeasured = Convert.ToDouble(dt.Rows[rowIndex]["BoqQtyMeas"]);
                        double boqQtyDiff = Convert.ToDouble(dt.Rows[rowIndex]["BoqQtyDIff"]);
                        double boqQtyRate = Convert.ToDouble(dt.Rows[rowIndex]["BoQItemRate"]);

                        // inserting RA Details
                        string sql = "INSERT INTO RaDetails874 " +
                                     "(RefID, RaHeaderID, RaBoqItem, RaUom, RaBoqQty, RaAbstQty, RaDiffQty, RaItemRate) " +
                                     "VALUES " +
                                     "(@RefID, @RaHeaderID, @RaBoqItem, @RaUom, @RaBoqQty, @RaAbstQty, @RaDiffQty, @RaItemRate)";

                        SqlCommand cmd = new SqlCommand(sql, con);
                        cmd.Parameters.AddWithValue("@RefID", RaDetailsRefID.ToString());
                        cmd.Parameters.AddWithValue("@RaHeaderID", RaHeaderId);
                        cmd.Parameters.AddWithValue("@RaBoqItem", boqItemName.ToString());
                        cmd.Parameters.AddWithValue("@RaUom", uom);
                        cmd.Parameters.AddWithValue("@RaBoqQty", boqQty);
                        cmd.Parameters.AddWithValue("@RaAbstQty", boqQtyMeasured);
                        cmd.Parameters.AddWithValue("@RaDiffQty", boqQtyDiff);
                        cmd.Parameters.AddWithValue("@RaItemRate", boqQtyRate);
                        cmd.ExecuteNonQuery();
                    }

                    con.Close();
                }
            }
            else
            {
                string message = "BoQ DataTabale is null";
                string script = $"alert('{message}');";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "messageScript", script, true);
            }
        }
        catch (SqlException ex)
        {
            string errorMessage = "Exception at RA Details insertion";
            string script = $"alert('{errorMessage}');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "messageScript", script, true);
        }
    }

    private void insertActionHead(int RaRefID)
    {
        // Account Head DataTable
        DataTable dt = (DataTable)Session["AccountHeadDT"];

        try
        {
            if (dt != null)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    foreach (GridViewRow row in GridTax.Rows)
                    {
                        // getting ref IDs for all tax heads
                        int AccountHeadRefID = getAccountHeadRefID();

                        // to get the current row index
                        int rowIndex = row.RowIndex;

                        // parameters
                        string refID = AccountHeadRefID.ToString();
                        double RaHeaderID = Convert.ToDouble(RaRefID);
                        string AccHeadGroup = dt.Rows[rowIndex]["AccHeadGroup"].ToString();
                        string DeductionHead = dt.Rows[rowIndex]["DeductionHead"].ToString();
                        string DedHeadCode = dt.Rows[rowIndex]["DedHeadCode"].ToString();
                        double FactorInPer = Convert.ToDouble(dt.Rows[rowIndex]["FactorInPer"]);
                        string PerOrAmnt = dt.Rows[rowIndex]["PerOrAmnt"].ToString();
                        string AddLess = dt.Rows[rowIndex]["AddLess"].ToString();
                        double TaxAmount = Convert.ToDouble(dt.Rows[rowIndex]["TaxAmount"]);

                        // inserting into Ra Tax
                        string sql = "INSERT INTO RaTax874 " +
                                     "(RefID, RaHeaderID, AccHeadGroup, DeductionHead, DedHeadCode, FactorInPer, PerOrAmnt, AddLess, TaxAmount) " +
                                     "VALUES " +
                                     "(@RefID, @RaHeaderID, @AccHeadGroup, @DeductionHead, @DedHeadCode, @FactorInPer, @PerOrAmnt, @AddLess, @TaxAmount)";

                        SqlCommand cmd = new SqlCommand(sql, con);
                        cmd.Parameters.AddWithValue("@RefID", refID.ToString());
                        cmd.Parameters.AddWithValue("@RaHeaderID", RaHeaderID);
                        cmd.Parameters.AddWithValue("@AccHeadGroup", AccHeadGroup);
                        cmd.Parameters.AddWithValue("@DeductionHead", DeductionHead);
                        cmd.Parameters.AddWithValue("@DedHeadCode", DedHeadCode);
                        cmd.Parameters.AddWithValue("@FactorInPer", FactorInPer);
                        cmd.Parameters.AddWithValue("@PerOrAmnt", PerOrAmnt);
                        cmd.Parameters.AddWithValue("@AddLess", AddLess);
                        cmd.Parameters.AddWithValue("@TaxAmount", TaxAmount);
                        cmd.ExecuteNonQuery();
                    }

                    con.Close();
                }
            }
        }
        catch (Exception ex)
        {

        }
    }

    private void insertUploadedDocuments(int RaRefID)
    {
        DataTable dt = (DataTable)Session["DocuUploadDT"];

        if (dt != null)
        {
            foreach (GridViewRow row in GridDocument.Rows)
            {
                // getting new doc ref id
                //int DocUploadRefID = getDocUploadedRefID();

                // to get the current row index
                int rowIndex = row.RowIndex;

                // dropdown values
                int docRedID = getDocUploadedRefID();
                string docType = dt.Rows[rowIndex]["docType"].ToString();
                string stageLevel = dt.Rows[rowIndex]["stageLevel"].ToString();
                string onlyFileName = dt.Rows[rowIndex]["onlyFileName"].ToString();

                // Find the HyperLink control in the current row
                HyperLink hypDocPath = (HyperLink)row.FindControl("hypDocPath");
                // Get the NavigateUrl property from the HyperLink control
                string navigateUrl = hypDocPath.NavigateUrl;

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string sql = "insert into DocUpload874 (RefID, RaHeaderID, DocType, StageLevel, DocName, DocPath) values (@RefID, @RaHeaderID, @DocType, @StageLevel, @DocName, @DocPath)";

                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@RefID", docRedID.ToString());
                    cmd.Parameters.AddWithValue("@RaHeaderID", RaRefID.ToString());
                    cmd.Parameters.AddWithValue("@DocType", docType.ToString());
                    cmd.Parameters.AddWithValue("@StageLevel", stageLevel.ToString());
                    cmd.Parameters.AddWithValue("@DocName", onlyFileName.ToString());
                    cmd.Parameters.AddWithValue("@DocPath", navigateUrl.ToString());
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }

    //===================================={ Ref IDs }====================================

    private int getRaHeaderRefID()
    {
        string nextRefID = "1000001";

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "SELECT ISNULL(MAX(CAST(RefID AS INT)), 1000000) + 1 AS NextRefID FROM RaHeader874";
            SqlCommand cmd = new SqlCommand(sql, con);

            object result = cmd.ExecuteScalar();
            if (result != null && result != DBNull.Value) { nextRefID = result.ToString(); }
            return Convert.ToInt32(nextRefID);
        }
    }

    private int getRaDetailsRefID()
    {
        string nextRefID = "1000001";

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "SELECT ISNULL(MAX(CAST(RefID AS INT)), 1000000) + 1 AS NextRefID FROM RaDetails874";
            SqlCommand cmd = new SqlCommand(sql, con);

            object result = cmd.ExecuteScalar();
            if (result != null && result != DBNull.Value) { nextRefID = result.ToString(); }
            return Convert.ToInt32(nextRefID);
        }
    }

    private int getAccountHeadRefID()
    {
        string nextRefID = "1000001";

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "SELECT ISNULL(MAX(CAST(RefID AS INT)), 1000000) + 1 AS NextRefID FROM RaTax874";
            SqlCommand cmd = new SqlCommand(sql, con);

            object result = cmd.ExecuteScalar();
            if (result != null && result != DBNull.Value) { nextRefID = result.ToString(); }
            return Convert.ToInt32(nextRefID);
        }
    }

    private int getDocUploadedRefID()
    {
        string nextRefID = "1000001";

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "SELECT ISNULL(MAX(CAST(RefID AS INT)), 1000000) + 1 AS NextRefID FROM DocUpload874";
            SqlCommand cmd = new SqlCommand(sql, con);

            object result = cmd.ExecuteScalar();
            if (result != null && result != DBNull.Value) { nextRefID = result.ToString(); }
            return Convert.ToInt32(nextRefID);
        }
    }
}