Imports System.Data.SqlClient
Imports System.Globalization

Partial Public Class Inventory_ItemEdit
    Inherits System.Web.UI.Page


    Private ReadOnly Property ItemId As Integer?
        Get
            If Not String.IsNullOrEmpty(Request.QueryString("id")) Then
                Return Convert.ToInt32(Request.QueryString("id"))
            End If
            Return Nothing
        End Get
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            If ItemId.HasValue Then
                litFormTitle.Text = "Edit Item"
                LoadItem(ItemId.Value)
            Else
                litFormTitle.Text = "Item Baru"
            End If
        End If
    End Sub

    Private Sub LoadItem(id As Integer)
        Dim dt = DBHelper.ExecuteQuery("SELECT * FROM dbo.Items WHERE ItemID = @id", New SqlParameter("@id", id))
        If dt.Rows.Count = 0 Then
            Response.Redirect("Items.aspx")
            Return
        End If
        Dim row = dt.Rows(0)
        txtItemCode.Text = row("ItemCode").ToString()
        txtItemName.Text = row("ItemName").ToString()
        ddlItemType.SelectedValue = row("ItemType").ToString()
        txtCategory.Text = row("Category").ToString()
        txtUnit.Text = row("Unit").ToString()
        txtUnitPrice.Text = Convert.ToDecimal(row("UnitPrice")).ToString(CultureInfo.InvariantCulture)
        txtReorderLevel.Text = row("ReorderLevel").ToString()
    End Sub

    Protected Sub btnSave_Click(sender As Object, e As EventArgs)
        If Not Page.IsValid Then Return

        Dim unitPrice As Decimal = 0
        Decimal.TryParse(txtUnitPrice.Text, NumberStyles.Any, CultureInfo.InvariantCulture, unitPrice)
        Dim reorderLevel As Integer = 0
        Integer.TryParse(txtReorderLevel.Text, reorderLevel)

        If ItemId.HasValue Then
            DBHelper.ExecuteNonQuery(
                "UPDATE dbo.Items SET ItemCode=@ItemCode, ItemName=@ItemName, ItemType=@ItemType, Category=@Category, " &
                "Unit=@Unit, UnitPrice=@UnitPrice, ReorderLevel=@ReorderLevel WHERE ItemID=@id",
                New SqlParameter("@ItemCode", txtItemCode.Text.Trim()),
                New SqlParameter("@ItemName", txtItemName.Text.Trim()),
                New SqlParameter("@ItemType", ddlItemType.SelectedValue),
                New SqlParameter("@Category", txtCategory.Text.Trim()),
                New SqlParameter("@Unit", txtUnit.Text.Trim()),
                New SqlParameter("@UnitPrice", unitPrice),
                New SqlParameter("@ReorderLevel", reorderLevel),
                New SqlParameter("@id", ItemId.Value))
        Else
            DBHelper.ExecuteNonQuery(
                "INSERT INTO dbo.Items (ItemCode, ItemName, ItemType, Category, Unit, UnitPrice, ReorderLevel) " &
                "VALUES (@ItemCode, @ItemName, @ItemType, @Category, @Unit, @UnitPrice, @ReorderLevel)",
                New SqlParameter("@ItemCode", txtItemCode.Text.Trim()),
                New SqlParameter("@ItemName", txtItemName.Text.Trim()),
                New SqlParameter("@ItemType", ddlItemType.SelectedValue),
                New SqlParameter("@Category", txtCategory.Text.Trim()),
                New SqlParameter("@Unit", txtUnit.Text.Trim()),
                New SqlParameter("@UnitPrice", unitPrice),
                New SqlParameter("@ReorderLevel", reorderLevel))
        End If

        Response.Redirect("Items.aspx")
    End Sub

End Class
