<%@ Page Language="C#" UnobtrusiveValidationMode="None" MaintainScrollPositionOnPostback="true" AutoEventWireup="true" CodeFile="RequisitionReceived.aspx.cs" Inherits="Requisition_Received_RequisitionReceived" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Requisition Received</title>

    <!-- Boottrap CSS -->
    <link href="../assests/css/bootstrap.min.css" rel="stylesheet" />
    <link href="../assests/css/bootstrap1.min.css" rel="stylesheet" />

    <!-- Bootstrap JS -->
    <script src="../assests/js/bootstrap.bundle.min.js"></script>
    <%--<script src="../assests/js/bootstrap1.min.js"></script>--%>

    <!-- Popper.js -->
    <script src="../assests/js/popper.min.js"></script>
    <script src="../assests/js/popper1.min.js"></script>

    <!-- jQuery -->
    <script src="../assests/js/jquery-3.6.0.min.js"></script>
    <script src="../assests/js/jquery.min.js"></script>
    <script src="../assests/js/jquery-3.3.1.slim.min.js"></script>

    <!-- Select2 library CSS and JS -->
    <link href="../assests/select2/select2.min.css" rel="stylesheet" />
    <script src="../assests/select2/select2.min.js"></script>

    <!-- Sweet Alert CSS and JS -->
    <link href="../assests/sweertalert/sweetalert2.min.css" rel="stylesheet" />
    <script src="../assests/sweertalert/sweetalert2.all.min.js"></script>

    <!-- Sumo Select CSS and JS -->
    <link href="../assests/sumoselect/sumoselect.min.css" rel="stylesheet" />
    <script src="../assests/sumoselect/jquery.sumoselect.min.js"></script>

    <!-- DataTables CSS & JS -->
    <link href="../assests/DataTables/datatables.min.css" rel="stylesheet" />
    <script src="../assests/DataTables/datatables.min.js"></script>

    <script src="requisition-received.js"></script>
    <link rel="stylesheet" href="requisition-received.css" />

</head>
<body>
    <form id="form1" runat="server">

        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true"></asp:ScriptManager>






        <!-- top Searching div Starts -->
        <div id="divTopSearch" runat="server" visible="true">
            <div class="col-md-12 mx-auto">


                <!-- Heading -->
                <div class="fw-normal fs-3 fw-medium ps-0 pb-2 text-body-secondary mb-3">
                    <asp:Literal Text="Update Availability" runat="server"></asp:Literal>
                </div>

                <!-- Control UI Starts -->
                <div class="card mt-2 shadow-sm">
                    <div class="card-body">

                        <!-- Req No -->
                        <div class="row mb-2">
                            <div class="col-md-4 align-self-end">
                                <div class="mb-1 text-body-tertiary fw-semibold fs-6">
                                    <asp:Literal ID="Literal15" Text="" runat="server">Requisition Number</asp:Literal>
                                </div>
                                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                                    <ContentTemplate>
                                        <asp:DropDownList ID="ddScRequisitionNo" runat="server" AutoPostBack="true" class="form-control is-invalid" CssClass=""></asp:DropDownList>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>

                            <!-- From Date -->
                            <div class="col-md-4 align-self-end">
                                <div class="mb-1 text-body-tertiary fw-semibold fs-6">
                                    <asp:Literal ID="Literal22" Text="" runat="server">From Date</asp:Literal>
                                </div>
                                <asp:TextBox runat="server" ID="ScFromDate" type="date" CssClass="form-control border border-secondary-subtle bg-light rounded-1 fs-6 fw-light py-1"></asp:TextBox>
                            </div>

                            <!-- To Date -->
                            <div class="col-md-4 align-self-end">
                                <div class="mb-1 text-body-tertiary fw-semibold fs-6">
                                    <asp:Literal ID="Literal23" Text="" runat="server">To Date</asp:Literal>
                                </div>
                                <asp:TextBox runat="server" ID="ScToDate" type="date" CssClass="form-control border border-secondary-subtle bg-light rounded-1 fs-6 fw-light py-1"></asp:TextBox>
                            </div>
                        </div>

                        <!-- Search Button -->
                        <div class="row mb-2 mt-4">
                            <div class="col-md-10"></div>
                            <div class="col-md-2">
                                <div class="text-end">
                                    <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" CssClass="col-md-10 btn btn-custom text-white col-md-2 shadow" />
                                </div>
                            </div>
                        </div>

                    </div>
                </div>
                <!-- Control UI Ends -->


                <!-- Search Grid Starts -->
                <div id="searchGridDiv" visible="false" runat="server" class="mt-5">
                    <asp:GridView ShowHeaderWhenEmpty="true" ID="gridSearch" runat="server" AutoGenerateColumns="false" OnRowCommand="gridSearch_RowCommand" AllowPaging="true" PageSize="10"
                        CssClass="datatable table table-bordered border border-1 border-dark-subtle table-hover text-center grid-custom" OnPageIndexChanging="gridSearch_PageIndexChanging" PagerStyle-CssClass="gridview-pager">
                        <HeaderStyle CssClass="" />
                        <Columns>
                            <asp:TemplateField ControlStyle-CssClass="col-md-1" HeaderText="Sr.No">
                                <ItemTemplate>
                                    <asp:HiddenField ID="id" runat="server" Value="id" />
                                    <span>
                                        <%#Container.DataItemIndex + 1%>
                                    </span>
                                </ItemTemplate>
                                <ItemStyle CssClass="col-md-1" />
                            </asp:TemplateField>
                            <asp:BoundField DataField="ReqNo" HeaderText="Requisition Number" ItemStyle-CssClass="col-xs-3 align-middle text-start fw-light" />
                            <asp:BoundField DataField="ReqDte" HeaderText="Requisition Date" ItemStyle-CssClass="col-xs-3 align-middle text-start fw-light" DataFormatString="{0:dd/MM/yyyy}" />
                            <asp:BoundField DataField="OrgTyp" HeaderText="Institute Type" ItemStyle-CssClass="col-xs-3 align-middle text-start fw-light" />
                            <asp:BoundField DataField="InstiName" HeaderText="Institute Name" ItemStyle-CssClass="col-xs-3 align-middle text-start fw-light" />
                            <asp:BoundField DataField="Req2Count" HeaderText="Requisition Count" ItemStyle-CssClass="col-xs-3 align-middle text-start fw-light" />
                            <asp:BoundField DataField="AvailabilityStatus" HeaderText="Availability Status" ItemStyle-CssClass="col-xs-3 align-middle text-start fw-light" />
                            <asp:BoundField DataField="PaymentStatus" HeaderText="Payment Status" ItemStyle-CssClass="col-xs-3 align-middle text-start fw-light" />
                            <asp:BoundField DataField="DocStatus" HeaderText="Document Status" ItemStyle-CssClass="col-xs-3 align-middle text-start fw-light" />

                            <asp:TemplateField HeaderText="View">
                                <ItemTemplate>
                                    <asp:LinkButton runat="server" ID="btnedit" CommandArgument='<%# Eval("RefNo") %>' CommandName="lnkView" ToolTip="Edit" CssClass="shadow-sm">
                                        <asp:Image runat="server" ImageUrl="../assests/img/pencil-square.svg" AlternateText="Edit" style="width: 16px; height: 16px;"/>
                                    </asp:LinkButton>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" Width="100px" />
                            </asp:TemplateField>

                        </Columns>
                    </asp:GridView>
                </div>
                <!-- Search Grid Ends -->


            </div>
        </div>
        <!-- top Searching div Ends -->





        <!-- Update Div Starts -->
        <div id="UpdateDiv" runat="server" visible="false">

            <!-- Heading -->
            <div class="col-md-12 mx-auto fw-normal fs-3 fw-medium ps-0 pb-2 text-body-secondary mb-3">
                <asp:Literal Text="Update Availability" runat="server"></asp:Literal>
            </div>

            <!-- Bill Header UI Starts-->
            <div class="card col-md-12 mx-auto mt-2 py-2 shadow-sm rounded-3">
                <div class="card-body">

                    <!-- Heading -->
                    <div class="fw-normal fs-5 fw-medium text-body-secondary border-bottom">
                        <asp:Literal Text="Requisition Details" runat="server"></asp:Literal>
                    </div>

                    <!-- 1st Row Starts -->
                    <div class="row mb-2 mt-3">
                        <div class="col-md-6 align-self-end">
                            <div class="mb-1 text-body-tertiary fw-semibold fs-6">
                                <asp:Literal ID="Literal10" Text="" runat="server">Requisition Number</asp:Literal>
                            </div>
                            <asp:TextBox runat="server" ID="txtReqNo" type="text" ReadOnly="true" CssClass="form-control border border-secondary-subtle bg-light rounded-1 fs-6 fw-light py-1"></asp:TextBox>
                        </div>
                        <div class="col-md-6 align-self-end">
                            <div class="mb-1 text-body-tertiary fw-semibold fs-6">
                                <asp:Literal ID="Literal12" Text="" runat="server">Requisition Date</asp:Literal>
                            </div>
                            <asp:TextBox runat="server" ID="dtReqDate" type="date" ReadOnly="true" CssClass="form-control border border-secondary-subtle bg-light rounded-1 fs-6 fw-light py-1"></asp:TextBox>
                        </div>
                    </div>
                    <!-- 1st Row Ends -->

                    <!-- 2nd Row Starts -->
                    <div class="row mb-2 mt-3">
                        <div class="col-md-6 align-self-end">
                            <div class="mb-1 text-body-tertiary fw-semibold fs-6">
                                <asp:Literal ID="Literal2" Text="" runat="server">Institute Type</asp:Literal>
                            </div>
                            <asp:TextBox ID="OrgType" type="text" ReadOnly="true" runat="server" CssClass="form-control border border-secondary-subtle bg-light rounded-1 fs-6 fw-light py-1"></asp:TextBox>
                        </div>
                        <div class="col-md-6 align-self-end">
                            <div class="mb-1 text-body-tertiary fw-semibold fs-6">
                                <asp:Literal ID="Literal3" Text="" runat="server">Institute Name</asp:Literal>
                            </div>
                            <asp:TextBox ID="InstituteName" type="text" ReadOnly="true" runat="server" CssClass="form-control border border-secondary-subtle bg-light rounded-1 fs-6 fw-light py-1"></asp:TextBox>
                        </div>
                    </div>
                    <!-- 2nd Row Ends -->

                    <!-- Remark -->
                    <div class="col-md-12 px-0 align-self-end">
                        <div class="mb-1 text-body-tertiary fw-semibold fs-6">
                            <asp:Literal ID="Literal5" Text="" runat="server">Remark</asp:Literal>
                        </div>
                        <textarea id="RemarkInput" name="w3review" rows="2" cols="50" class="form-control border border-secondary-subtle bg-light rounded-1 fs-6 fw-light py-1" runat="server"></textarea>
                    </div>

                </div>
            </div>
            <!-- Bill Header UI Ends-->

            <!-- Item UI Starts -->
            <div class="card col-md-12 mx-auto mt-5 rounded-3">
                <div class="card-body">

                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>

                            <!-- Heading -->
                            <div class="fw-normal fs-5 fw-medium text-body-secondary border-bottom">
                                <asp:Literal Text="Services Available" runat="server"></asp:Literal>
                            </div>


                            <!-- Item GridView Starts -->
                            <div id="itemDiv" runat="server" visible="false" class="mt-3 mb-3">
                                <asp:GridView ShowHeaderWhenEmpty="true" ID="itemGrid" runat="server" AutoGenerateColumns="false" OnRowDataBound="ItemGrid_RowDataBound"
                                    CssClass="table table-bordered  border border-1 border-dark-subtle text-center grid-custom mb-3"
                                    OnSelectedIndexChanged="GridView1_SelectedIndexChanged">
                                    <HeaderStyle CssClass="align-middle" />
                                    <RowStyle VerticalAlign="Top" />
                                    <Columns>

                                        <asp:TemplateField ControlStyle-CssClass="col-md-1" HeaderText="Sr.No">
                                            <ItemTemplate>
                                                <asp:HiddenField ID="id" runat="server" Value="id" />
                                                <span>
                                                    <%#Container.DataItemIndex + 1%>
                                                </span>
                                            </ItemTemplate>
                                            <ItemStyle CssClass="col-md-1" />
                                            <ItemStyle Font-Size="15px" />
                                        </asp:TemplateField>

                                        <asp:BoundField DataField="ServName" HeaderText="Cell/Service Name" ItemStyle-Font-Size="15px" ItemStyle-CssClass="align-middle text-start fw-light" />
                                        <asp:BoundField DataField="NmeCell" HeaderText="Cell/Service Name (Manual)" ItemStyle-Font-Size="15px" ItemStyle-CssClass="align-middle text-start fw-light" />
                                        <asp:BoundField DataField="Quty" HeaderText="Required Qty" ItemStyle-Font-Size="15px" ItemStyle-CssClass="align-middle text-start fw-light" />
                                        <asp:BoundField DataField="UOM" HeaderText="UOM" ItemStyle-Font-Size="15px" ItemStyle-CssClass="align-middle text-start fw-light" />

                                        <asp:TemplateField HeaderText="Available Qty" ItemStyle-Font-Size="15px" ItemStyle-CssClass="col-md-2 align-middle">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtAvailableQty" Text='<%# Bind("AvailableQty") %>' type="number" step="any" title="please enter two digits number" runat="server" Enabled="true" CssClass="col-md-12 fw-light border border-secondary-subtle shadow-sm rounded-1 py-1 px-2"></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Balance Qty" ItemStyle-Font-Size="15px" ItemStyle-CssClass="col-md-1 align-middle">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtBalanceQty" Text='<%# Bind("BalanceQty") %>' type="number" step="any" title="please enter two digits number" runat="server" Enabled="false" CssClass="col-md-12 bg-white fw-light border-0 border-secondary-subtle rounded-1 py-1 px-2"></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:BoundField DataField="ServicePrice" HeaderText="Service Price" ItemStyle-Font-Size="15px" ItemStyle-CssClass="align-middle text-start fw-light" />

                                        <asp:TemplateField HeaderText="Sub Item Total Price" ItemStyle-Font-Size="15px" ItemStyle-CssClass="col-md-1 align-middle text-start fw-light">
                                            <ItemTemplate>
                                                <asp:TextBox ID="SubItemPrice" runat="server" Text='<%# Eval("SubItemPrice") %>' Enabled="false" CssClass="col-md-12 bg-white fw-light border-0 border-secondary-subtle rounded-1 py-1 px-2"></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>


                                        <asp:TemplateField HeaderText="Available Status" ItemStyle-CssClass="align-middle">
                                            <ItemTemplate>
                                                <%--<input type="checkbox" id="chkAvailStatus" style="width: 1.7em; height: 1.7em; display: inline-block;" runat="server" />--%>
                                                <asp:CheckBox ID="chkAvailStatus" runat="server" AutoPostBack="true" OnCheckedChanged="chkAvailStatus_CheckedChanged" />
                                                <tr class="additional-row mb-5">
                                                    <td colspan="10">
                                                        <asp:RequiredFieldValidator ID="rr2" ControlToValidate="txtEdit" InitialValue="" ValidationGroup="finalSubmit" CssClass="invalid-feedback" runat="server" ErrorMessage="(Please enter the reason for service non-availability)" SetFocusOnError="True" Display="Dynamic" ToolTip="Required"></asp:RequiredFieldValidator>
                                                        <asp:TextBox ID="txtEdit" runat="server" placeholder="Kindly specify the reason for service non-availability" CssClass="additional-row border border-dark-subtle fw-lighter fs-6 rounded-3 shadow-sm p-3" Width="100%" TextMode="MultiLine" Height="60px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" Width="50px" />
                                        </asp:TemplateField>

                                        <%--<asp:CommandField ShowSelectButton="True" SelectText="Reason" />--%>
                                    </Columns>
                                </asp:GridView>
                            </div>
                            <!-- Item GridView Ends -->






                            <!-- Total Bill -->
                            <div class="row px-0 mb-3 mt-5">

                                <!-- DD Apply Tax Or Not -->
                                <div class="col-md-2">
                                    <asp:DropDownList ID="ddTaxOrNot" runat="server" OnSelectedIndexChanged="ddTaxOrNot_SelectedIndexChanged" AutoPostBack="true" CssClass="col-md-12 text-center fw-light border border-secondary-subtle shadow-sm rounded-1 py-1 px-2">
                                        <asp:ListItem Text="Apply Tax Head" Value="Yes"></asp:ListItem>
                                        <asp:ListItem Text="No Tax Head" Value="No"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>

                                <div class="col-md-3"></div>
                                <div class="col-md-3"></div>

                                <!-- total service bill textbox -->
                                <div class="col-md-4 align-self-end text-end">
                                    <asp:Literal ID="Literal6" Text="" runat="server">Available Services Amount (Against Requisition)</asp:Literal>
                                    <div class="input-group">
                                        <span class="input-group-text fs-5 fw-semibold">₹</span>
                                        <asp:TextBox runat="server" ID="TotalServiceAmnt" CssClass="form-control fw-lighter border border-2" ReadOnly="true" placeholder="Total Service Amount"></asp:TextBox>
                                    </div>
                                </div>
                            </div>




                            <!-- Tax Grid Starts -->
                            <div id="divTaxHead" runat="server" visible="false">

                                <!-- Heading Document -->
                                <div class="border-top border-bottom border-secondary-subtle py-2 mt-4 mb-4">
                                    <div class="fw-normal fs-5 fw-medium text-body-secondary">
                                        <asp:Literal Text="Account Head" runat="server"></asp:Literal>
                                    </div>
                                </div>

                                <asp:GridView ShowHeaderWhenEmpty="true" ID="GridTax" runat="server" AutoGenerateColumns="false" OnRowDataBound="GridTax_RowDataBound" CssClass="table text-center">
                                    <HeaderStyle CssClass="align-middle table-secondary fw-light" />
                                    <Columns>

                                        <asp:TemplateField HeaderText="Account Head" ItemStyle-Font-Size="15px" ItemStyle-CssClass="col-md-4 align-middle text-start fw-light">
                                            <ItemTemplate>
                                                <asp:TextBox ID="AcHeadName" Text='<%# Bind("AcHeadName") %>' runat="server" Enabled="false" CssClass="col-md-9 fw-light bg-white border-0 py-1 px-2"></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Factor in %" ItemStyle-Font-Size="15px" ItemStyle-CssClass="col-md-2 align-middle">
                                            <ItemTemplate>
                                                <asp:TextBox ID="TaxRate" Text='<%# Bind("TaxRate") %>' type="number" step="0.01" title="Enter a number two decimals" runat="server" Enabled="true" CssClass="col-md-9 fw-light border border-secondary-subtle shadow-sm rounded-1 py-1 px-2"></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="% / Amount" ItemStyle-Font-Size="15px" ItemStyle-CssClass="col-md-2 align-middle">
                                            <ItemTemplate>
                                                <asp:DropDownList ID="PerOrAmnt" runat="server" CssClass="col-md-6 text-center fw-light border border-secondary-subtle shadow-sm rounded-1 py-1 px-2">
                                                    <asp:ListItem Text="%" Value="Percentage"></asp:ListItem>
                                                    <asp:ListItem Text="₹" Value="Amount"></asp:ListItem>
                                                </asp:DropDownList>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Add / Less" ItemStyle-Font-Size="15px" ItemStyle-CssClass="col-md-2 align-middle">
                                            <ItemTemplate>
                                                <asp:DropDownList ID="AddLess" runat="server" CssClass="col-md-6 text-center fw-light border border-secondary-subtle shadow-sm rounded-1 py-1 px-2">
                                                    <asp:ListItem Text="+" Value="Add"></asp:ListItem>
                                                    <asp:ListItem Text="-" Value="Less"></asp:ListItem>
                                                </asp:DropDownList>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Amount" ItemStyle-Font-Size="15px" ItemStyle-CssClass="col-md-3 align-middle">
                                            <ItemTemplate>
                                                <asp:TextBox ID="TaxAmount" Text='<%# Bind("TaxAmount") %>' type="number" step="0.01" runat="server" Enabled="true" ReadOnly="true" CssClass="col-md-9 fw-light border border-secondary-subtle shadow-sm rounded-1 py-1 px-2"></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                    </Columns>
                                </asp:GridView>

                                <!-- Re-Calculate Tax -->

                                <div class="mt-4 mb-3">
                                    <div class="text-end">
                                        <asp:Button ID="btnReCalTax" runat="server" Text="Re-Calculate" OnClick="btnReCalTax_Click" CssClass="btn btn-custom text-white mb-3" />
                                    </div>
                                </div>


                                <!-- Net Deduction, Addition & Total Bill Amounts -->
                                <div class="row mb-3">
                                    <!-- Total Deduction -->
                                    <div class="col-md-3 align-self-end">
                                        <asp:Literal ID="Literal28" Text="Total Deductions :" runat="server"></asp:Literal>
                                        <div class="input-group text-end">
                                            <span class="input-group-text fs-5 fw-light">₹</span>
                                            <asp:TextBox runat="server" ID="txtTotalDeduct" CssClass="form-control fw-lighter border border-2" ReadOnly="true"></asp:TextBox>
                                        </div>
                                    </div>

                                    <!-- Total Addition -->
                                    <div class="col-md-3 align-self-end">
                                        <asp:Literal ID="Literal29" Text="Total Additions :" runat="server"></asp:Literal>
                                        <div class="input-group text-end">
                                            <span class="input-group-text fs-5 fw-light">₹</span>
                                            <asp:TextBox runat="server" ID="txtTotalAdd" CssClass="form-control fw-lighter border border-2" ReadOnly="true"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-3 align-self-end text-end"></div>

                                    <!-- Net Amount -->
                                    <div class="col-md-3 align-self-end text-end">
                                        <asp:Literal ID="Literal30" Text="Net Amount" runat="server"></asp:Literal>
                                        <div class="input-group text-end">
                                            <span class="input-group-text fs-5 fw-light">₹</span>
                                            <asp:TextBox runat="server" ID="txtNetAmnt" CssClass="form-control fw-lighter border border-2" ReadOnly="true"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>

                            </div>
                            <!-- Tax Grid Ends -->

                        </ContentTemplate>
                        <Triggers>
                        </Triggers>
                    </asp:UpdatePanel>



                    <!-- Document Section Starts -->
                    <div class="mt-5 mb-5">

                        <!-- Heading Document -->
                        <div class="border-top border-bottom border-secondary-subtle py-2">
                            <div class="fw-normal fs-5 fw-medium text-body-secondary">
                                <asp:Literal Text="Customer Document" runat="server"></asp:Literal>
                            </div>
                        </div>

                        <!-- Document Grid Starts -->
                        <div id="docGrid" class="mt-4" runat="server" visible="true">
                            <asp:GridView ShowHeaderWhenEmpty="true" ID="GridDocument" EnableViewState="true" runat="server" AutoGenerateColumns="false"
                                CssClass="table table-bordered border border-light-subtle text-start mt-3 grid-custom">
                                <HeaderStyle CssClass="align-middle fw-light fs-6" />
                                <Columns>

                                    <asp:TemplateField ControlStyle-CssClass="col-md-1" HeaderText="Sr.No">
                                        <ItemTemplate>
                                            <asp:HiddenField ID="id" runat="server" Value="id" />
                                            <span>
                                                <%#Container.DataItemIndex + 1%>
                                            </span>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:BoundField DataField="DocName" HeaderText="File Name" ReadOnly="true" ItemStyle-Font-Size="15px" ItemStyle-CssClass="align-middle text-start fw-light" />

                                    <asp:TemplateField HeaderText="View Document" ItemStyle-Font-Size="15px" ItemStyle-CssClass="align-middle text-start fw-light">
                                        <ItemTemplate>
                                            <asp:HyperLink ID="DocPath" runat="server" Text="View Uploaded Document" NavigateUrl='<%# Eval("DocPath") %>' Target="_blank" CssClass="text-decoration-none"></asp:HyperLink>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                </Columns>
                            </asp:GridView>
                        </div>
                        <!-- Document Grid Ends -->

                    </div>
                    <!-- Document Section Ends -->




                    <!-- Submit Button -->
                    <div class="">
                        <div class="row mt-5 mb-2">
                            <div class="col-md-6 text-start">
                                <asp:Button ID="btnBack" runat="server" Text="Back" OnClick="btnBack_Click" CssClass="btn btn-custom text-white shadow mb-5" />
                            </div>
                            <div class="col-md-6 text-end">
                                <asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_Click" ValidationGroup="finalSubmit" CssClass="btn btn-custom text-white shadow mb-5" />
                            </div>
                        </div>
                    </div>


                </div>
            </div>
            <!-- Item UI Ends -->

        </div>
        <!-- Update Div Ends -->



    </form>
</body>
</html>
