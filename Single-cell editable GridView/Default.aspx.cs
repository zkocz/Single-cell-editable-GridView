using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Single_cell_editable_GridView
{
	public partial class _Default : System.Web.UI.Page
	{
		//not all cells we want to allow to edit
		//you can ignore this const if you'll implement custom logic to allow/deny editing
		const int FIRST_EDITABLE_CELL = 1;

		protected void Page_Load(object sender, EventArgs e)
		{
			GridView1.RowDataBound += new GridViewRowEventHandler(GridView1_RowDataBound);

			if (!IsPostBack)
			{
				GridView1.DataSource = CreateDataSource();
				GridView1.DataBind();
			}
		}

		#region row rendering

		void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				//create cells
				AddCell(FIRST_EDITABLE_CELL, "Author", e);
				AddCell(FIRST_EDITABLE_CELL + 1, "name", e);
				AddCell(FIRST_EDITABLE_CELL + 2, "Price", e);
			}
		}

		/// <summary>
		/// Add cell to GridView row; this code is common so we can create a method
		/// </summary>
		/// <param name="cellIndex">index of column</param>
		/// <param name="cellName">name of controls without prefix;
		/// if you want to use another convention, you may not use this method, but
		/// you can always use the code directly in GridView1_RowDataBound
		/// </param>
		/// <param name="e">Event args with info about the row</param>
		void AddCell(int cellIndex, string cellName, GridViewRowEventArgs e)
		{
			//we will add javascript events to the controls in the cell
			Label lab = (Label)e.Row.Cells[cellIndex].FindControl(string.Format("lab{0}", cellName));
			TextBox txt = (TextBox)e.Row.Cells[cellIndex].FindControl(string.Format("txt{0}", cellName));
			Button btn = (Button)e.Row.Cells[cellIndex].FindControl(string.Format("btn{0}", cellName));

			//if text is empty, we need to grow label's width to ensure user can click this label
			if (string.IsNullOrEmpty(lab.Text))
			{
				lab.Width = 100;
				lab.Text = "&nbsp;";
			}

			//add javascript events to:
			//label (we want to hide this label and show textbox on click)
			lab.Attributes.Add("onclick", string.Format("return HideLabel('{0}', event, '{1}');", lab.ClientID, txt.ClientID));
			//and to textbox
			txt.Attributes.Add("class", "editctrl");
			txt.Attributes.Add("onkeydown", string.Format("return SaveDataOnEnter('{0}', event, '{1}', '{2}');", txt.ClientID, lab.ClientID, btn.ClientID));
			//todo: there is an issue with onblur, I am not JS master, but hope it is just a small issue

			txt.Attributes.Add("onblur", string.Format("return SaveDataOnLostFocus('{0}', '{1}');", txt.ClientID, btn.ClientID));

			//highlight a text in textbox
			txt.Attributes.Add("onfocus", "select()");

			//we need to know what row and cell was edited
			//you can use anything else instead, e.g. session or whatever
			btn.CommandName = e.Row.RowIndex.ToString();
			btn.CommandArgument = cellIndex.ToString();

			//set a cursor style
			e.Row.Attributes["style"] += "cursor:pointer;cursor:hand;"; //just a cosmetic thing :)
		}

		#endregion row rendering

		#region methods

		/// <summary>
		/// The common code for all "edit" events
		/// </summary>
		/// <param name="txtName">textbox control's name</param>
		/// <param name="labName">label control's name</param>
		/// <param name="columnName">DB column's name</param>
		/// <param name="e">Event args to get row and column index</param>
		protected void UpdateValue(string txtName, string labName, string columnName, CommandEventArgs e)
		{
			//todo: you can create e.g. a class and use it instead command argument
			int row = int.Parse(e.CommandName);
			int cell = int.Parse(e.CommandArgument.ToString());

			if (GridView1.Rows[row].RowType == DataControlRowType.DataRow)
			{
				//get the record ID
				Label labId = (Label)GridView1.Rows[row].Cells[0].FindControl("labId");
				//get the new value
				TextBox txt = (TextBox)GridView1.Rows[row].Cells[cell].FindControl(txtName);
				//update the label, if data are not re-bounded
				Label lab = (Label)GridView1.Rows[row].Cells[cell].FindControl(labName);
				lab.Text = txt.Text;

				//save to DB
			}
		}

		/// <summary>
		/// Sample (unsecure!) code to save changes to DB
		/// </summary>
		/// <param name="columnName">DB column name</param>
		/// <param name="id">record ID</param>
		/// <param name="value">new value to save</param>
		protected void UpdateDb(string columnName, int id, object value)
		{
			using (SqlConnection connect = new SqlConnection(ConfigurationManager.ConnectionStrings["MSSQL"].ConnectionString))
			{
				using (SqlCommand cmdins = new SqlCommand(string.Format("UPDATE books SET {0}=@value WHERE id=@id", columnName), connect))
				{
					cmdins.Parameters.AddWithValue("@value", value);
					cmdins.Parameters.AddWithValue("@id", id);
					connect.Open();
					cmdins.ExecuteNonQuery();
					connect.Close();
				}
			}
		}

		#endregion methods

		#region events

		protected void txtAuthor_Changed(object sender, CommandEventArgs e)
		{
			//names of controls in GridView row
			string txtName = "txtAuthor";	//textbox
			string labName = "labAuthor";	//label
			//DB column
			string columnName = "Author";

			UpdateValue(txtName, labName, columnName, e);
		}

		protected void txtName_Changed(object sender, CommandEventArgs e)
		{
			string txtName = "txtName";
			string labName = "labName";
			string columnName = "Name";

			UpdateValue(txtName, labName, columnName, e);
		}

		protected void txtPrice_Changed(object sender, CommandEventArgs e)
		{
			string txtName = "txtPrice";
			string labName = "labPrice";
			string columnName = "Price";

			UpdateValue(txtName, labName, columnName, e);
		}

		#endregion events

		#region sample data

		private DataSet CreateDataSource()
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("id", typeof(int));
			dataTable.Columns.Add("author", typeof(string));
			dataTable.Columns.Add("name", typeof(string));
			dataTable.Columns.Add("price", typeof(float));

			dataTable.Rows.Add(1, "Jack London", "White Fang", 2.99);
			dataTable.Rows.Add(2, "Daniel Defoe", "Robinson Crusoe", 1.99);
			dataTable.Rows.Add(3, "Alexandre Dumas", "The Three Musketeers", 3.99);
			dataTable.Rows.Add(4, "Jules Verne", "Two Years Vacation", 2.99);

			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(dataTable);
			return  dataSet;
		}
		
		#endregion sample data
	}
}
