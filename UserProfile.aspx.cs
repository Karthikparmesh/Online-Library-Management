using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace GPELibrary
{
    public partial class UserProfile : System.Web.UI.Page
    {
        string strcon = ConfigurationManager.ConnectionStrings["GPELibraryDBConnectionString"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
                try
                {
                    if (Session["UserName"].ToString() == "" || Session["UserName"] == null)
                    {
                        Response.Write("<script>alert('Session Expired Login Again');</script>");
                        Response.Redirect("UserLogin.aspx");
                    }
                    else
                    {
                        getUserBookData();

                        if (!Page.IsPostBack)
                        {
                            getUserPersonalDetails();
                        }

                    }
                }
                catch (Exception)
                {

                    Response.Write("<script>alert('Session Expired Login Again');</script>");
                    Response.Redirect("UserLogin.aspx");
                }
            }

            // update button click
            protected void Button1_Click(object sender, EventArgs e)
            {
                if (Session["UserName"].ToString() == "" || Session["UserName"] == null)
                {
                    Response.Write("<script>alert('Session Expired Login Again');</script>");
                    Response.Redirect("UserLogin.aspx");
                }
                else
                {
                    updateUserPersonalDetails();

                }
            }



            // user defined function


            void updateUserPersonalDetails()
            {
                string password = "";
                if (TextBox10.Text.Trim() == "")
                {
                    password = TextBox9.Text.Trim();
                }
                else
                {
                    password = TextBox10.Text.Trim();
                }
                try
                {
                    SqlConnection con = new SqlConnection(strcon);
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }


                    SqlCommand cmd = new SqlCommand("UPDATE MemberMaster_Table set Full_Name=@Full_Name, DOB=@DOB, Phone_Number=@Phone_Number, E_Mail=@E_Mail, State=@State, City=@City, PinCode=@PinCode, Full_Address=@Full_Address, Password=@Password, Account_Status=@Account_Status WHERE Member_ID='" + Session["UserName"].ToString().Trim() + "'", con);

                    cmd.Parameters.AddWithValue("@Full_Name", TextBox1.Text.Trim());
                    cmd.Parameters.AddWithValue("@DOB", TextBox2.Text.Trim());
                    cmd.Parameters.AddWithValue("@Phone_Number", TextBox3.Text.Trim());
                    cmd.Parameters.AddWithValue("@E_Mail", TextBox4.Text.Trim());
                    cmd.Parameters.AddWithValue("@State", DropDownList1.SelectedItem.Value);
                    cmd.Parameters.AddWithValue("@City", TextBox6.Text.Trim());
                    cmd.Parameters.AddWithValue("@PinCode", TextBox7.Text.Trim());
                    cmd.Parameters.AddWithValue("@Full_Address", TextBox5.Text.Trim());
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@Account_Status", "pending");

                    int result = cmd.ExecuteNonQuery();
                    con.Close();
                    if (result > 0)
                    {

                        Response.Write("<script>alert('Your Details Updated Successfully');</script>");
                        getUserPersonalDetails();
                        getUserBookData();
                    }
                    else
                    {
                        Response.Write("<script>alert('Invaid entry');</script>");
                    }

                }
                catch (Exception ex)
                {
                    Response.Write("<script>alert('" + ex.Message + "');</script>");
                }
            }


            void getUserPersonalDetails()
            {
                try
                {
                    SqlConnection con = new SqlConnection(strcon);
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    SqlCommand cmd = new SqlCommand("SELECT * from MemberMaster_Table where Member_ID='" + Session["UserName"].ToString() + "';", con);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    TextBox1.Text = dt.Rows[0]["Full_Name"].ToString();
                    TextBox2.Text = dt.Rows[0]["DOB"].ToString();
                    TextBox3.Text = dt.Rows[0]["Phone_Number"].ToString();
                    TextBox4.Text = dt.Rows[0]["E_Mail"].ToString();
                    DropDownList1.SelectedValue = dt.Rows[0]["State"].ToString().Trim();
                    TextBox6.Text = dt.Rows[0]["City"].ToString();
                    TextBox7.Text = dt.Rows[0]["PinCode"].ToString();
                    TextBox5.Text = dt.Rows[0]["Full_Address"].ToString();
                    TextBox8.Text = dt.Rows[0]["Member_ID"].ToString();
                    TextBox9.Text = dt.Rows[0]["Password"].ToString();

                    Label1.Text = dt.Rows[0]["Account_Status"].ToString().Trim();

                    if (dt.Rows[0]["Account_Status"].ToString().Trim() == "active")
                    {
                        Label1.Attributes.Add("class", "badge badge-pill badge-success");
                    }
                    else if (dt.Rows[0]["Account_Status"].ToString().Trim() == "pending")
                    {
                        Label1.Attributes.Add("class", "badge badge-pill badge-warning");
                    }
                    else if (dt.Rows[0]["Account_Status"].ToString().Trim() == "deactive")
                    {
                        Label1.Attributes.Add("class", "badge badge-pill badge-danger");
                    }
                    else
                    {
                        Label1.Attributes.Add("class", "badge badge-pill badge-info");
                    }


                }
                catch (Exception ex)
                {
                    Response.Write("<script>alert('" + ex.Message + "');</script>");

                }
            }

            void getUserBookData()
            {
                try
                {
                    SqlConnection con = new SqlConnection(strcon);
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    SqlCommand cmd = new SqlCommand("SELECT * from BookIssue_Table where Member_ID='" + Session["UserName"].ToString() + "';", con);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    GridView.DataSource = dt;
                    GridView.DataBind();
                }
                catch (Exception ex)
                {
                    Response.Write("<script>alert('" + ex.Message + "');</script>");

                }
            }

            protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
            {
                try
                {
                    if (e.Row.RowType == DataControlRowType.DataRow)
                    {
                        //Check your condition here
                        DateTime dt = Convert.ToDateTime(e.Row.Cells[5].Text);
                        DateTime today = DateTime.Today;
                        if (today > dt)
                        {
                            e.Row.BackColor = System.Drawing.Color.PaleVioletRed;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Response.Write("<script>alert('" + ex.Message + "');</script>");
                }
            }
        }
    }