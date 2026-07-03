<%@ Page Language="VB" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeBehind="ProjectTasks.aspx.vb" Inherits="Projects_ProjectTasks" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">Task Proyek</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="card">
        <h2><asp:Literal ID="litProjectName" runat="server" /></h2>
        <a class="btn btn-outline btn-sm" href="Projects.aspx">&larr; Kembali ke Daftar Proyek</a>
    </div>

    <div class="card" class="max-w-2xl">
        <h2>Tambah Task Baru</h2>
        <div class="form-grid-2">
            <div class="form-row">
                <label>Nama Task *</label>
                <asp:TextBox ID="txtTaskName" runat="server" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtTaskName"
                    ErrorMessage="Nama task wajib diisi" CssClass="hint" ForeColor="#dc2626" Display="Dynamic" />
            </div>
            <div class="form-row">
                <label>Ditugaskan ke</label>
                <asp:TextBox ID="txtAssignedTo" runat="server" />
            </div>
        </div>
        <div class="form-grid-2">
            <div class="form-row">
                <label>Tanggal Mulai</label>
                <asp:TextBox ID="txtStartDate" runat="server" TextMode="Date" />
            </div>
            <div class="form-row">
                <label>Deadline</label>
                <asp:TextBox ID="txtDueDate" runat="server" TextMode="Date" />
            </div>
        </div>
        <div class="form-grid-2">
            <div class="form-row">
                <label>Priority</label>
                <asp:DropDownList ID="ddlPriority" runat="server">
                    <asp:ListItem Text="Low" Value="Low" />
                    <asp:ListItem Text="Medium" Value="Medium" Selected="True" />
                    <asp:ListItem Text="High" Value="High" />
                </asp:DropDownList>
            </div>
            <div class="form-row">
                <label>Status</label>
                <asp:DropDownList ID="ddlStatus" runat="server">
                    <asp:ListItem Text="To Do" Value="ToDo" />
                    <asp:ListItem Text="In Progress" Value="InProgress" />
                    <asp:ListItem Text="Done" Value="Done" />
                </asp:DropDownList>
            </div>
        </div>
        <asp:Button ID="btnAddTask" runat="server" Text="+ Tambah Task" CssClass="btn btn-primary" OnClick="btnAddTask_Click" />
    </div>

    <div class="card">
        <h2>Daftar Task</h2>
        <asp:GridView ID="gvTasks" runat="server" CssClass="grid" AutoGenerateColumns="false"
            GridLines="None" EmptyDataText="Belum ada task untuk proyek ini." DataKeyNames="TaskID">
            <Columns>
                <asp:BoundField DataField="TaskName" HeaderText="Task" />
                <asp:BoundField DataField="AssignedTo" HeaderText="PIC" />
                <asp:BoundField DataField="DueDate" HeaderText="Deadline" DataFormatString="{0:dd-MM-yyyy}" />
                <asp:BoundField DataField="Priority" HeaderText="Priority" />
                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <span class='badge badge-<%# Eval("Status").ToString().ToLower() %>'><%# Eval("Status") %></span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Aksi">
                    <ItemTemplate>
                        <asp:LinkButton runat="server" CssClass="btn btn-outline btn-sm" CommandArgument='<%# Eval("TaskID") %>'
                            OnClick="lnkMarkDone_Click" Text="Tandai Selesai" />
                        <asp:LinkButton runat="server" CssClass="btn btn-danger btn-sm" CommandArgument='<%# Eval("TaskID") %>'
                            OnClientClick='<%# "return confirmDelete(""" & Eval("TaskName") & """);" %>'
                            OnClick="lnkDeleteTask_Click" Text="Hapus" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

</asp:Content>
