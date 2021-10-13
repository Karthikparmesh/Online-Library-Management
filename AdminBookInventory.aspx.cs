using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace GPELibrary
{
    public partial class AdminBookInventory : System.Web.UI.Page
    {
        string strcon = ConfigurationManager.ConnectionStrings["GPELibraryDBConnectionString"].ConnectionString;
        static string global_filepath;
        static int global_actual_stock, global_current_stock, global_issued_books;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                fillAuthorPublisherValues();
            }

            GridView.DataBind();
        }

        // go button click
        protected void Button4_Click(object sender, EventArgs e)
        {
            getBookByID();
        }


        // update button click
        protected void Button3_Click(object sender, EventArgs e)
        {
            updateBookByID();
        }
        // delete button click
        protected void Button2_Click(object sender, EventArgs e)
        {
            deleteBookByID();
        }
        // add button click
        protected void Button1_Click(object sender, EventArgs e)
        {
            if (checkIfBookExists())
            {
                Response.Write("<script>alert('Book Already Exists, try some other Book ID');</script>");
            }
            else
            {
                addNewBook();
            }
        }



        // user defined functions

        void deleteBookByID()
        {
            if (checkIfBookExists())
            {
                try
                {
                    SqlConnection con = new SqlConnection(strcon);
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    SqlCommand cmd = new SqlCommand("DELETE from BookMaster_Table WHERE Book_ID='" + TextBox1.Text.Trim() + "'", con);

                    cmd.ExecuteNonQuery();
                    con.Close();
                    Response.Write("<script>alert('Book Deleted Successfully');</script>");

                    GridView.DataBind();

                }
                catch (Exception ex)
                {
                    Response.Write("<script>alert('" + ex.Message + "');</script>");
                }

            }
            else
            {
                Response.Write("<script>alert('Invalid Member ID');</script>");
            }
        }

        void updateBookByID()
        {

            if (checkIfBookExists())
            {
                try
                {

                    int actual_stock = Convert.ToInt32(TextBox4.Text.Trim());
                    int current_stock = Convert.ToInt32(TextBox5.Text.Trim());

                    if (global_actual_stock == actual_stock)
                    {

                    }
                    else
                    {
                        if (actual_stock < global_issued_books)
                        {
                            Response.Write("<script>alert('Actual Stock value cannot be less than the Issued books');</script>");
                            return;


                        }
                        else
                        {
                            current_stock = actual_stock - global_issued_books;
                            TextBox5.Text = "" + current_stock;
                        }
                    }

                    string genres = "";
                    foreach (int i in ListBox1.GetSelectedIndices())
                    {
                        genres = genres + ListBox1.Items[i] + ",";
                    }
                    genres = genres.Remove(genres.Length - 1);

                    string filepath = "~/imgs/books1.png";
                    string filename = Path.GetFileName(FileUpload1.PostedFile.FileName);
                    if (filename == "" || filename == null)
                    {
                        filepath = global_filepath;

                    }
                    else
                    {
                        FileUpload1.SaveAs(Server.MapPath("imgs/" + filename));
                        filepath = "~/imgs/" + filename;
                    }

                    SqlConnection con = new SqlConnection(strcon);
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    SqlCommand cmd = new SqlCommand("UPDATE BookMaster_Table set Book_Name=@Book_Name, Genre=@Genre, Author_Name=@Author_Name, Publisher_Name=@Publisher_Name, Publish_Date=@Publish_Date, Language=@Language, Edition=@Edition, Book_Cost=@Book_Cost, Number_Of_Pages=@Number_Of_Pages, Book_Description=@Book_Description, Actual_Stock=@Actual_Stock, Current_Stock=@Current_Stock, Book_Image_Link=@Book_Image_Link where Book_ID='" + TextBox1.Text.Trim() + "'", con);

                    cmd.Parameters.AddWithValue("@Book_Name", TextBox2.Text.Trim());
                    cmd.Parameters.AddWithValue("@Genre", genres);
                    cmd.Parameters.AddWithValue("@Author_Name", DropDownList3.SelectedItem.Value);
                    cmd.Parameters.AddWithValue("@Publisher_Name", DropDownList2.SelectedItem.Value);
                    cmd.Parameters.AddWithValue("@Publish_Date", TextBox3.Text.Trim());
                    cmd.Parameters.AddWithValue("@Language", DropDownList1.SelectedItem.Value);
                    cmd.Parameters.AddWithValue("@Edition", TextBox9.Text.Trim());
                    cmd.Parameters.AddWithValue("@Book_Cost", TextBox10.Text.Trim());
                    cmd.Parameters.AddWithValue("@Number_Of_Pages", TextBox11.Text.Trim());
                    cmd.Parameters.AddWithValue("@Book_Description", TextBox6.Text.Trim());
                    cmd.Parameters.AddWithValue("@Actual_Stock", actual_stock.ToString());
                    cmd.Parameters.AddWithValue("@Current_Stock", current_stock.ToString());
                    cmd.Parameters.AddWithValue("@Book_Image_Link", filepath);


                    cmd.ExecuteNonQuery();
                    con.Close();
                    GridView.DataBind();
                    Response.Write("<script>alert('Book Updated Successfully');</script>");


                }
                catch (Exception ex)
                {
                    Response.Write("<script>alert('" + ex.Message + "');</script>");
                }
            }
            else
            {
                Response.Write("<script>alert('Invalid Book ID');</script>");
            }
        }


        void getBookByID()
        {
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand("SELECT * from BookMaster_Table WHERE Book_ID='" + TextBox1.Text.Trim() + "';", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count >= 1)
                {
                    TextBox2.Text = dt.Rows[0]["Book_Name"].ToString();
                    TextBox3.Text = dt.Rows[0]["Publish_Date"].ToString();
                    TextBox9.Text = dt.Rows[0]["Edition"].ToString();
                    TextBox10.Text = dt.Rows[0]["Book_Cost"].ToString().Trim();
                    TextBox11.Text = dt.Rows[0]["Number_Of_Pages"].ToString().Trim();
                    TextBox4.Text = dt.Rows[0]["Actual_Stock"].ToString().Trim();
                    TextBox5.Text = dt.Rows[0]["Current_Stock"].ToString().Trim();
                    TextBox6.Text = dt.Rows[0]["Book_Description"].ToString();
                    TextBox7.Text = "" + (Convert.ToInt32(dt.Rows[0]["Actual_Stock"].ToString()) - Convert.ToInt32(dt.Rows[0]["Current_Stock"].ToString()));

                    DropDownList1.SelectedValue = dt.Rows[0]["Language"].ToString().Trim();
                    DropDownList2.SelectedValue = dt.Rows[0]["Publisher_Name"].ToString().Trim();
                    DropDownList3.SelectedValue = dt.Rows[0]["Author_Name"].ToString().Trim();

                    ListBox1.ClearSelection();
                    string[] genre = dt.Rows[0]["Genre"].ToString().Trim().Split(',');
                    for (int i = 0; i < genre.Length; i++)
                    {
                        for (int j = 0; j < ListBox1.Items.Count; j++)
                        {
                            if (ListBox1.Items[j].ToString() == genre[i])
                            {
                                ListBox1.Items[j].Selected = true;

                            }
                        }
                    }

                    global_actual_stock = Convert.ToInt32(dt.Rows[0]["Actual_Stock"].ToString().Trim());
                    global_current_stock = Convert.ToInt32(dt.Rows[0]["Current_Stock"].ToString().Trim());
                    global_issued_books = global_actual_stock - global_current_stock;
                    global_filepath = dt.Rows[0]["Book_Image_Link"].ToString();

                }
                else
                {
                    Response.Write("<script>alert('Invalid Book ID');</script>");
                }

            }
            catch (Exception ex)
            {

            }
        }

        void fillAuthorPublisherValues()
        {
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand("SELECT Author_Name from AuthorMaster_Table;", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                DropDownList3.DataSource = dt;
                DropDownList3.DataValueField = "Author_Name";
                DropDownList3.DataBind();

                cmd = new SqlCommand("SELECT Publisher_Name from PublisherMaster_Table;", con);
                da = new SqlDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);
                DropDownList2.DataSource = dt;
                DropDownList2.DataValueField = "Publisher_Name";
                DropDownList2.DataBind();

            }
            catch (Exception ex)
            {

            }
        }

        bool checkIfBookExists()
        {
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                SqlCommand cmd = new SqlCommand("SELECT * from BookMaster_Table where Book_ID='" + TextBox1.Text.Trim() + "' OR Book_Name='" + TextBox2.Text.Trim() + "';", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count >= 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }


            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('" + ex.Message + "');</script>");
                return false;
            }
        }

        void addNewBook()
        {
            try
            {
                string genres = "";
                foreach (int i in ListBox1.GetSelectedIndices())
                {
                    genres = genres + ListBox1.Items[i] + ",";
                }
                // genres = Adventure,Self Help,
                genres = genres.Remove(genres.Length - 1);

                string filepath = "~/imgs/books1.png";
                string filename = Path.GetFileName(FileUpload1.PostedFile.FileName);
                FileUpload1.SaveAs(Server.MapPath("imgs/" + filename));
                filepath = "~/imgs/" + filename;


                SqlConnection con = new SqlConnection(strcon);
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                SqlCommand cmd = new SqlCommand("INSERT INTO BookMaster_Table(Book_ID,Book_Name,Genre,Author_Name,Publisher_Name,Publish_Date,Language,Edition,Book_Cost,Number_Of_Pages,Book_Description,Actual_Stock,Current_Stock,Book_Image_Link) values(@Book_ID,@Book_Name,@Genre,@Author_Name,@Publisher_Name,@Publish_Date,@Language,@Edition,@Book_Cost,@Number_Of_Pages,@Book_Description,@Actual_Stock,@Current_Stock,@Book_Image_Link)", con);

                cmd.Parameters.AddWithValue("@Book_ID", TextBox1.Text.Trim());
                cmd.Parameters.AddWithValue("@Book_Name", TextBox2.Text.Trim());
                cmd.Parameters.AddWithValue("@Genre", genres);
                cmd.Parameters.AddWithValue("@Author_Name", DropDownList3.SelectedItem.Value);
                cmd.Parameters.AddWithValue("@Publisher_Name", DropDownList2.SelectedItem.Value);
                cmd.Parameters.AddWithValue("@Publish_Date", TextBox3.Text.Trim());
                cmd.Parameters.AddWithValue("@Language", DropDownList1.SelectedItem.Value);
                cmd.Parameters.AddWithValue("@Edition", TextBox9.Text.Trim());
                cmd.Parameters.AddWithValue("@Book_Cost", TextBox10.Text.Trim());
                cmd.Parameters.AddWithValue("@Number_Of_Pages", TextBox11.Text.Trim());
                cmd.Parameters.AddWithValue("@Book_Description", TextBox6.Text.Trim());
                cmd.Parameters.AddWithValue("@Actual_Stock", TextBox4.Text.Trim());
                cmd.Parameters.AddWithValue("@Current_Stock", TextBox4.Text.Trim());
                cmd.Parameters.AddWithValue("@Book_Image_Link", filepath);

                cmd.ExecuteNonQuery();
                con.Close();
                Response.Write("<script>alert('Book Added Successfully.');</script>");
                GridView.DataBind();

            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('" + ex.Message + "');</script>");
            }
        }
    }
}