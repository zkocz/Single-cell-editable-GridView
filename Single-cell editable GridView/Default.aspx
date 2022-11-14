<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Single_cell_editable_GridView._Default" EnableEventValidation="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	<title>Single-cell editable GridView sample</title>
	<%-- import javascript code --%>
	<script src="GridView.js" type="text/javascript"></script>
</head>
<body>
	<form id="form1" runat="server">
	<div>
		<asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="false">
			<Columns>
				<asp:TemplateField HeaderText="ID" SortExpression="ID">
					<ItemTemplate>
						<%--we don't want to edit ID, so we need one label only--%>
						<asp:Label ID="labId" runat="server" Text='<%# Eval("id") %>' ></asp:Label>                        
					</ItemTemplate>               
				</asp:TemplateField>
				
				<asp:TemplateField HeaderText="Author" SortExpression="Author">
					<ItemTemplate>
						<%--we want to edit author's name, so we need one label for view, one textbox for edit and one button to save new value--%>
						<asp:Label ID="labAuthor" runat="server" Text='<%# Eval("author") %>'></asp:Label>
						<asp:TextBox ID="txtAuthor" runat="server" Text='<%# Eval("author") %>' Width="175px" style="display:none" ></asp:TextBox>
						<asp:Button id="btnAuthor" runat="server" Text="" OnCommand="txtAuthor_Changed" style="display:none" />
					</ItemTemplate>
				</asp:TemplateField>
				
				<asp:TemplateField HeaderText="Name" SortExpression="Name">
					<ItemTemplate>
						<asp:Label ID="labName" runat="server" Text='<%# Eval("Name") %>'></asp:Label>
						<asp:TextBox ID="txtName" runat="server" Text='<%# Eval("Name") %>' Width="175px" style="display:none" ></asp:TextBox>
						<asp:Button id="btnName" runat="server" Text="" OnCommand="txtName_Changed" style="display:none" />
					</ItemTemplate>
				</asp:TemplateField>
				
				<asp:TemplateField HeaderText="Price" SortExpression="Price">
					<ItemTemplate>
						<asp:Label ID="labPrice" runat="server" Text='<%# Eval("Price") %>'></asp:Label>
						<asp:TextBox ID="txtPrice" runat="server" Text='<%# Eval("Price") %>' Width="175px" style="display:none" ></asp:TextBox>
						<asp:Button id="btnPrice" runat="server" Text="" OnCommand="txtPrice_Changed" style="display:none" />
					</ItemTemplate>
				</asp:TemplateField>
			</Columns>		
		</asp:GridView>
	</div>
	</form>
</body>
</html>
