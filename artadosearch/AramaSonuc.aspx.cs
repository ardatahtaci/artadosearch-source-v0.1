﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
using System.Net;
using System.IO;
using System.ServiceModel.PeerResolvers;
using System.Net.Mail;
using CsQuery.ExtensionMethods;
using HtmlAgilityPack;
using System.Web.Routing;
using System.Web.Mvc;
using System.Text;
using System.Net.Http;
using System.Security.Policy;
using System.Xml;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Threading;
using System.Globalization;
using Google.Cloud.Translation.V2;

public partial class AramaSonuc : System.Web.UI.Page
{

    private static string theme2;
    static double a, b;
    static string d;

    PagedDataSource pdsakademik = new PagedDataSource();
    PagedDataSource pds = new PagedDataSource();
    DataTable dt = new DataTable(); 

    Stopwatch watch = new Stopwatch();

    string con = System.Configuration.ConfigurationManager.ConnectionStrings["con"].ConnectionString.ToString();
    public string admn = System.Configuration.ConfigurationManager.ConnectionStrings["admin"].ConnectionString.ToString();

    public void Start()
    {
        System.Globalization.CultureInfo kultur = System.Threading.Thread.CurrentThread.CurrentUICulture;
        string lang = kultur.TwoLetterISOLanguageName;
        System.Web.HttpCookie cookielang = HttpContext.Current.Request.Cookies["Lang"];

        SqlConnection baglanti = new SqlConnection(con);
        SqlConnection conadmin = new SqlConnection(admn);
        
        string url = Request.Url.Query;

        if (url.StartsWith("?i=<"))
        {
            Response.Redirect("/Home?i=+");
        }

        string aranan = Request.QueryString["i"];

        if (aranan == null)
        {
            aranan = Request.QueryString["q"];
            if (aranan == null)
            {
                aranan = Request.QueryString["p"];
                if (aranan == null)
                {
                    aranan = Request.QueryString["ei"];
                }
            }
            Response.Redirect("search?i=" + aranan + "&page=1");
        }

        Page.Title = aranan + " - Artado Search";
        arama_çubugu2.Attributes.Add("Value", aranan);
        aranan = aranan.Trim();

        //Google Araması
        int google;
        google = aranan.ToLower().IndexOf(".g");

        //Reddit Araması
        int reddit; 
        reddit = aranan.ToLower().IndexOf(".r");

        //Instagram Araması
        int insta;
        insta = aranan.ToLower().IndexOf(".i");

        //Youtube Araması
        int yt;
        yt = aranan.ToLower().IndexOf(".yt");

        //Vikipedi Araması
        int wiki;
        wiki = aranan.ToLower().IndexOf(".wiki");

        //DuckDuckGO Araması
        int ddg;
        ddg = aranan.ToLower().IndexOf(".ddg");

        //Ekşi Sözlük Araması
        int eksi;
        eksi = aranan.ToLower().IndexOf(".eksi");

        //Twitter Araması
        int t;
        t = aranan.ToLower().IndexOf(".t");

        //Yaay Araması
        int y;
        y = aranan.ToLower().IndexOf(".y");

        //Quora Araması
        int q;
        q = aranan.ToLower().IndexOf(".q");

        if(aranan.StartsWith(".g") || aranan.StartsWith(".r") || aranan.StartsWith(".i") || aranan.StartsWith(".yt") || aranan.StartsWith(".ddg") || aranan.StartsWith(".eksi") || aranan.StartsWith(".t") || aranan.StartsWith(".y") || aranan.StartsWith(".q") || aranan.StartsWith(".wiki"))
        {
            if (google >= 0)
            {
                string yenikelime = aranan.Substring(3, aranan.Length - 3);
                Response.Redirect("https://www.google.com/search?hl=tr&q=" + yenikelime);
            }
            else if (reddit >= 0)
            {
                string subreddit = aranan.Substring(3, aranan.Length - 3);
                Response.Redirect("https://www.reddit.com/r/" + subreddit);
            }
            else if (insta >= 0)
            {
                string iprofile = aranan.Substring(3, aranan.Length - 3);
                Response.Redirect("https://www.instagram.com/" + iprofile);
            }
            else if (yt >= 0)
            {
                string ytarama = aranan.Substring(3, aranan.Length - 3);
                Response.Redirect("https://www.youtube.com/results?search_query=" + ytarama);
            }
            else if (ddg >= 0)
            {
                string ddgarama = aranan.Substring(5, aranan.Length - 5);
                Response.Redirect("https://duckduckgo.com/?q=" + ddgarama);
            }
            else if (eksi >= 0)
            {
                string eksiarama = aranan.Substring(5, aranan.Length - 5);
                Response.Redirect("https://eksisozluk.com/?q=" + eksiarama);
            }
            else if (t >= 0)
            {
                string tarama = aranan.Substring(3, aranan.Length - 3);
                Response.Redirect("https://twitter.com/search?q=" + tarama);
            }
            else if (y >= 0)
            {
                string yarama = aranan.Substring(3, aranan.Length - 3);
                Response.Redirect("https://yaay.com.tr/search?q=" + yarama);
            }
            else if (q >= 0)
            {
                string qarama = aranan.Substring(3, aranan.Length - 3);
                Response.Redirect("https://www.quora.com/search?q=" + qarama);
            }
            else if (wiki >= 0)
            {
                string wikiarama = aranan.Substring(5, aranan.Length - 5);
                Response.Redirect("https://tr.wikipedia.org/w/index.php?search=" + wikiarama);
            }
        }

        string[] s;
        s = aranan.Split(' ');

        Results.Visible = true;
        DropDownList2.Items.FindByValue("Google").Enabled = true;
        DropDownList2.Items.FindByValue("Artado").Enabled = true;
        DropDownList2.Items.FindByValue("Bing").Enabled = true;
        DropDownList2.Items.FindByValue("Yahoo").Enabled = true;
        DropDownList2.Items.FindByValue("Github").Enabled = true;
        DropDownList2.Items.FindByValue("Scholar").Enabled = true;
        DropDownList2.Items.FindByValue("Base").Enabled = true;

        try
        {
            if (DropDownList2.SelectedValue == "Artado")
            {
                Google.Visible = false;
                rptAramaSonuclari.Visible = true;
                Filtre.Visible = true;
                ScholarFiltre.Visible = false;
                Lang.Visible = false;
                PageSelect.Visible = true;
                Text.Visible = true;
                GoogleImage.Visible = false;
                OtherResults.Visible = false;

                //Web arama
                if (WebArama.Visible == true && rptAramaSonuclari.Visible == true)
                {
                    foreach (string kelime in s)
                    {
                        if (DropDownList1.SelectedValue == "Alaka")
                        {
                            SqlDataAdapter adp = new SqlDataAdapter("select *, Title from arda.Sonuçlar where Title Like @aranan or Content1 Like @aranan or Keywords Like @aranan", baglanti);
                            adp.SelectCommand.Parameters.Add(new SqlParameter
                            {
                                ParameterName = "@aranan",
                                Value = "%" + kelime + "%",
                            });
                            DataTable dt = new DataTable();
                            adp.Fill(dt);
                            pds.DataSource = dt.DefaultView;
                            pds.AllowPaging = true;
                            pds.PageSize = 10;
                            int currentPage;
                            if (Request.QueryString["page"] != null)
                            {
                                currentPage = Int32.Parse(Request.QueryString["page"]);
                                if (currentPage < 1)
                                {
                                    currentPage = 1;
                                }
                            }
                            else
                            {
                                currentPage = 1;
                            }
                            pds.CurrentPageIndex = currentPage - 1;
                            Label2.Text = "Sayfa: " + currentPage + " / " + pds.PageCount;
                            if (currentPage <= 1)
                            {
                                HyperLink1.Visible = false;
                            }
                            if (!pds.IsFirstPage)
                            {
                                HyperLink1.NavigateUrl = "search?i=" + aranan + "&page=" + (currentPage - 1);
                            }
                            if (!pds.IsLastPage)
                            {
                                HyperLink2.NavigateUrl = "search?i=" + aranan + "&page=" + (currentPage + 1);
                            }
                            rptAramaSonuclari.DataSource = pds;
                            rptAramaSonuclari.DataBind();
                        }
                        else if (DropDownList1.SelectedValue == "Puan")
                        {
                            SqlDataAdapter adp = new SqlDataAdapter("select *, Title from arda.Sonuçlar where Title Like @aranan or Content1 Like @aranan or Keywords Like @aranan order by Rank desc", baglanti);
                            adp.SelectCommand.Parameters.Add(new SqlParameter
                            {
                                ParameterName = "@aranan",
                                Value = "%" + kelime + "%",
                            });
                            DataTable dt = new DataTable();
                            adp.Fill(dt);
                            pds.DataSource = dt.DefaultView;
                            pds.AllowPaging = true;
                            pds.PageSize = 10;
                            int currentPage;
                            if (Request.QueryString["page"] != null)
                            {
                                currentPage = Int32.Parse(Request.QueryString["page"]);
                            }
                            else
                            {
                                currentPage = 1;
                            }
                            pds.CurrentPageIndex = currentPage - 1;
                            Label2.Text = "Sayfa: " + currentPage + " / " + pds.PageCount;
                            if (!pds.IsFirstPage)
                            {
                                HyperLink1.NavigateUrl = "search?i=" + aranan + "&page=" + (currentPage - 1);
                            }
                            if (!pds.IsLastPage)
                            {
                                HyperLink2.NavigateUrl = "search?i=" + aranan + "&page=" + (currentPage + 1);
                            }
                            rptAramaSonuclari.DataSource = pds;
                            rptAramaSonuclari.DataBind();
                        }
                        else
                        {
                            SqlDataAdapter adp = new SqlDataAdapter("select *, Title from arda.Sonuçlar where Title Like @aranan or Content1 Like @aranan or Keywords Like @aranan", baglanti);
                            adp.SelectCommand.Parameters.Add(new SqlParameter
                            {
                                ParameterName = "@aranan",
                                Value = "%" + kelime + "%",
                            });
                            DataTable dt = new DataTable();
                            adp.Fill(dt);
                            pds.DataSource = dt.DefaultView;
                            pds.AllowPaging = true;
                            pds.PageSize = 10;
                            int currentPage;
                            if (Request.QueryString["page"] != null)
                            {
                                currentPage = Int32.Parse(Request.QueryString["page"]);
                            }
                            else
                            {
                                currentPage = 1;
                            }
                            pds.CurrentPageIndex = currentPage - 1;
                            Label2.Text = "Sayfa: " + currentPage + " / " + pds.PageCount;
                            if (!pds.IsFirstPage)
                            {
                                HyperLink1.NavigateUrl = "search?i=" + aranan + "&page=" + (currentPage - 1);
                            }
                            if (!pds.IsLastPage)
                            {
                                HyperLink2.NavigateUrl = "search?i=" + aranan + "&page=" + (currentPage + 1);
                            }
                            rptAramaSonuclari.DataSource = pds;
                            rptAramaSonuclari.DataBind();
                        }
                    }
                }
            }
            else if (DropDownList2.SelectedValue == "Google")
            {
                Google.Visible = true;
                rptAramaSonuclari.Visible = false;
                Filtre.Visible = false;
                PageSelect.Visible = false;
                Text.Visible = false;
                GoogleImage.Visible = true;
                OtherResults.Visible = false;
            }
            else if (DropDownList2.SelectedValue == "Bing")
            {
                Google.Visible = false;
                rptAramaSonuclari.Visible = false;
                Filtre.Visible = false;
                PageSelect.Visible = false;
                Text.Visible = false;
                ScholarFiltre.Visible = false;
                Lang.Visible = false;
                GoogleImage.Visible = false;
                OtherResults.Visible = true;
                int currentPage;
                if (Request.QueryString["page"] != null)
                {
                    currentPage = Int32.Parse(Request.QueryString["page"]);
                }
                else
                {
                    currentPage = 1;
                }
                string bing = "https://www.bing.com/search?q=" + aranan + "&qs=n&sp=-1&pq=" + aranan + "&sc=8-6&sk=&cvid=E61D587280A143E4B2B331964F17D6C8&first=" + currentPage;
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(bing.Trim());
                request.Referer = "https://www.bing.com/";
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:91.0) Gecko/20100101 Firefox/91.0";
                WebResponse response = request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string htmlText = reader.ReadToEnd();
                reader.Close();
                response.Close();
                int results1 = htmlText.IndexOf("<ol id=\"b_results\" class=\"\">".ToLower()) + 28;
                int results2 = htmlText.Substring(results1).IndexOf("</ol>");
                string resulttext = htmlText.Substring(results1, results2);
                ResultsTxt.Text = resulttext;

                if (currentPage <= 1)
                {
                    HyperLink5.Visible = false;
                }
                else
                {
                    HyperLink5.Visible = true;
                    HyperLink5.NavigateUrl = "search?i=" + aranan + "&page=" + (currentPage - 8);
                }

                HyperLink6.NavigateUrl = "search?i=" + aranan + "&page=" + (currentPage + 8);
            }
            else if (DropDownList2.SelectedValue == "Baidu")
            {
                Google.Visible = false;
                rptAramaSonuclari.Visible = false;
                Filtre.Visible = false;
                PageSelect.Visible = false;
                Text.Visible = false;
                ScholarFiltre.Visible = false;
                Lang.Visible = false;
                GoogleImage.Visible = false;
                OtherResults.Visible = true;
                int currentPage;
                if (Request.QueryString["page"] != null)
                {
                    currentPage = Int32.Parse(Request.QueryString["page"]);
                }
                else
                {
                    currentPage = 1;
                }
                string bing = "https://www.baidu.com/s?ie=utf-8&f=3&rsv_bp=1&rsv_idx=1&tn=baidu&wd=" + aranan + "&fenlei=256&rsv_pq=ef57db050000f0b2&rsv_t=88e5JBNivf%2FaSP%2F24Yg6X0RdeXzJ97dPN6DuEe69ZvM5ktzkxcvQP64Zgkc&rqlang=cn&rsv_dl=ih_1&rsv_sug3=1&rsv_enter=1&rsv_sug1=1&rsv_sug7=001&rsv_sug2=1&rsv_btype=i&rsp=1&rsv_sug9=es_2_1&rsv_sug4=2472&rsv_sug=4";
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(bing.Trim());
                request.Referer = "https://www.baidu.com/";
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/93.0.4577.58 Safari/537.36";
                WebResponse response = request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string htmlText = reader.ReadToEnd();
                reader.Close();
                response.Close();
                int results1 = htmlText.IndexOf("<div id=\"container\" class=\"sam_newgrid container_s\" data-w=\"1280\">".ToLower()) + 66;
                int results2 = htmlText.Substring(results1).IndexOf("</table></div></div></div>");
                string resulttext = htmlText.Substring(results1, results2);
                ResultsTxt.Text = resulttext;

                if (currentPage <= 1)
                {
                    HyperLink5.Visible = false;
                }
                else
                {
                    HyperLink5.Visible = true;
                    HyperLink5.NavigateUrl = "search?i=" + aranan + "&page=" + (currentPage - 1);
                }

                HyperLink6.NavigateUrl = "search?i=" + aranan + "&page=" + (currentPage + 1);
            }
            else if (DropDownList2.SelectedValue == "Yahoo")
            {
                Google.Visible = false;
                rptAramaSonuclari.Visible = false;
                Filtre.Visible = true;
                DropDownList1.Visible = false;
                ScholarFiltre.Visible = false;
                Lang.Visible = false;
                Sort.Visible = false;
                PageSelect.Visible = false;
                Text.Visible = false;
                GoogleImage.Visible = false;
                OtherResults.Visible = true;
                int currentPage;
                if (Request.QueryString["page"] != null)
                {
                    currentPage = Int32.Parse(Request.QueryString["page"]);
                }
                else
                {
                    currentPage = 0;
                }
                string github = "https://search.yahoo.com/search?p=" + aranan + "&b=" + currentPage + 0;
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(github.Trim());
                WebResponse response = request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string htmlText = reader.ReadToEnd();
                reader.Close();
                response.Close();
                string a = "https://search.yahoo.com/search";
                int href = htmlText.IndexOf(a);
                while (href >= 0)
                {
                    htmlText = htmlText.Replace("https://search.yahoo.com/search", "/search");
                    if (a.Length < htmlText.Length)
                    {
                        href = htmlText.IndexOf(a, a.Length);
                    }
                    else
                    {
                        href = -1;
                    }
                }
                try
                {
                    int results1 = htmlText.IndexOf("<div id=\"results\"><style type=\"text/css\">") + 0;
                    int results2 = htmlText.Substring(results1).IndexOf("</div></div><div id=\"right\"></div>");
                    string resulttext = htmlText.Substring(results1, results2);
                    ResultsTxt.Text = resulttext;
                }
                catch
                {
                    int results1 = htmlText.IndexOf("<div id=\"left\">") + 0;
                    int results2 = htmlText.Substring(results1).IndexOf("</ol></div>");
                    string resulttext = htmlText.Substring(results1, results2);
                    ResultsTxt.Text = resulttext;
                }

                if (currentPage < 1)
                {
                    HyperLink5.Visible = false;
                }
                else
                {
                    HyperLink5.Visible = true;
                    HyperLink5.NavigateUrl = "search?i=" + aranan + "&page=" + (currentPage - 1);
                }

                HyperLink6.NavigateUrl = "search?i=" + aranan + "&page=" + (currentPage + 1);
            }
            else if (DropDownList2.SelectedValue == "Github")
            {
                Google.Visible = false;
                rptAramaSonuclari.Visible = false;
                Filtre.Visible = false;
                PageSelect.Visible = false;
                Text.Visible = false;
                GoogleImage.Visible = false;
                OtherResults.Visible = true;
                int currentPage;
                if (Request.QueryString["page"] != null)
                {
                    currentPage = Int32.Parse(Request.QueryString["page"]);
                }
                else
                {
                    currentPage = 0;
                }
                string github = "https://github.com/search?p=" + currentPage + "&q=" + aranan + "&type=Repositories";
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(github.Trim());
                WebResponse response = request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string htmlText = reader.ReadToEnd();
                reader.Close();
                response.Close();
                string a = "href=\"/";
                int href = htmlText.IndexOf(a);
                while (href >= 0)
                {
                    htmlText = htmlText.Replace("\"/", "\"https://github.com/");
                    if (a.Length < htmlText.Length)
                    {
                        href = htmlText.IndexOf(a, a.Length);
                    }
                    else
                    {
                        href = -1;
                    }
                }
                int results1 = htmlText.IndexOf("<ul class=\"repo-list\">") + 22;
                int results2 = htmlText.Substring(results1).IndexOf("</ul>");
                string resulttext = htmlText.Substring(results1, results2);
                ResultsTxt.Text = resulttext;

                if (currentPage < 1)
                {
                    HyperLink5.Visible = false;
                }
                else
                {
                    HyperLink5.Visible = true;
                    HyperLink5.NavigateUrl = "search?i=" + aranan + "&page=" + (currentPage - 1);
                }

                HyperLink6.NavigateUrl = "search?i=" + aranan + "&page=" + (currentPage + 1);
            }
            else if (DropDownList2.SelectedValue == "Scholar")
            {
                if (cookielang != null && cookielang.Value != null)
                {
                    if (cookielang.Value == "en-AU")
                    {
                        ScholarFiltre.SelectedValue = "az";
                    }
                    else if (cookielang.Value == "en-BZ")
                    {
                        ScholarFiltre.SelectedValue = "ba";
                    }
                    else
                    {
                        ScholarFiltre.SelectedValue = cookielang.Value;
                    }
                }
                else
                {
                    try
                    {
                        ScholarFiltre.SelectedValue = lang;
                    }
                    catch
                    {
                        ScholarFiltre.SelectedValue = "en-US";
                    }
                }

                Google.Visible = false;
                rptAramaSonuclari.Visible = false;
                Filtre.Visible = true;
                DropDownList1.Visible = false;
                Sort.Visible = false;
                PageSelect.Visible = false;
                Text.Visible = false;
                GoogleImage.Visible = false;
                OtherResults.Visible = true;
                int currentPage;
                if (Request.QueryString["page"] != null)
                {
                    currentPage = Int32.Parse(Request.QueryString["page"]);
                }
                else
                {
                    currentPage = 0;
                }
                string github = "https://scholar.google.com/scholar?start=" + currentPage + "&q=" + aranan + "&hl=" + ScholarFiltre.SelectedValue;
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(github.Trim());
                WebResponse response = request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string htmlText = reader.ReadToEnd();
                reader.Close();
                response.Close();
                string a = "href=\"/";
                int href = htmlText.IndexOf(a);
                while (href >= 0)
                {
                    htmlText = htmlText.Replace("\"/", "\"https://scholar.google.com/");
                    if (a.Length < htmlText.Length)
                    {
                        href = htmlText.IndexOf(a, a.Length);
                    }
                    else
                    {
                        href = -1;
                    }
                }
                int results1 = htmlText.IndexOf("<div id=\"gs_res_ccl_mid\">") + 25;
                int results2 = htmlText.Substring(results1).IndexOf("</div><div id=\"gs_res_ccl_bot\">");
                string resulttext = htmlText.Substring(results1, results2);
                ResultsTxt.Text = resulttext;

                if (currentPage < 1)
                {
                    HyperLink5.Visible = false;
                }
                else
                {
                    HyperLink5.Visible = true;
                    HyperLink5.NavigateUrl = "search?i=" + aranan + "&page=" + (currentPage - 1);
                }

                HyperLink6.NavigateUrl = "search?i=" + aranan + "&page=" + (currentPage + 1);
            }
            else if (DropDownList2.SelectedValue == "Base")
            {
                Google.Visible = false;
                rptAramaSonuclari.Visible = false;
                Filtre.Visible = true;
                DropDownList1.Visible = false;
                ScholarFiltre.Visible = false;
                Lang.Visible = false;
                Sort.Visible = false;
                PageSelect.Visible = false;
                Text.Visible = false;
                GoogleImage.Visible = false;
                OtherResults.Visible = true;
                int currentPage;
                if (Request.QueryString["page"] != null)
                {
                    currentPage = Int32.Parse(Request.QueryString["page"]);
                }
                else
                {
                    currentPage = 0;
                }
                string github = "https://www.base-search.net/Search/Results?lookfor=" + aranan + "&type=all&page=" + currentPage;
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(github.Trim());
                WebResponse response = request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string htmlText = reader.ReadToEnd();
                reader.Close();
                response.Close();
                string a = "href=\"/";
                int href = htmlText.IndexOf(a);
                while (href >= 0)
                {
                    htmlText = htmlText.Replace("\"/", "\"https://www.base-search.net/");
                    if (a.Length < htmlText.Length)
                    {
                        href = htmlText.IndexOf(a, a.Length);
                    }
                    else
                    {
                        href = -1;
                    }
                }
                int results1 = htmlText.IndexOf("<div id=\"hit-list\">") + 19;
                int results2 = htmlText.Substring(results1).IndexOf("<div class=\"row\" id=\"all-hits-export-row\">");
                string resulttext = htmlText.Substring(results1, results2);
                ResultsTxt.Text = resulttext;

                if (currentPage < 1)
                {
                    HyperLink5.Visible = false;
                }
                else
                {
                    HyperLink5.Visible = true;
                    HyperLink5.NavigateUrl = "search?i=" + aranan + "&page=" + (currentPage - 1);
                }

                HyperLink6.NavigateUrl = "search?i=" + aranan + "&page=" + (currentPage + 1);
            }
        }
        catch (Exception error)
        {
            Google.Visible = false;
            rptAramaSonuclari.Visible = false;
            Filtre.Visible = false;
            PageSelect.Visible = false;
            Text.Visible = false;
            GoogleImage.Visible = false;
            ResultsTxt.Visible = true;
            ResultsTxt.Text = "Upps! Bir hata oluştu.<br/><br/> Upps! Something went wrong.<br/><br/> Opps! Etwas ist schief gelaufen.<br/><br/> Oups! Quelque chose s'est mal passé.<br/><br/> Ой! Что-то пошло не так.<br/><br/> 出问题了。<br/><br/> Hata Mesajı: " + error;
        }

        WebArama.Visible = true;
        Panel3.Visible = true;
        Panel4.Visible = true;
        Panel5.Visible = true;
        Panel6.Visible = true;
        GörselArama.Visible = false;
        SözlükArama.Visible = false;
        MüzikArama.Visible = false;
        Panel3.Visible = false;
        Makaleler.Visible = false;

        //Boş Arama engelleme
        if (aranan == "")
        {
            Response.Redirect("Home?empty=true");
        }

        watch.Start();

        int sonuçlar = rptAramaSonuclari.Items.Count * pds.PageCount;

        //Sözlük Öneri
        foreach (string kelime in s)
        {
            PagedDataSource pdssoz = new PagedDataSource();
            SqlDataAdapter adpsoz = new SqlDataAdapter("select *, Anlam from arda.Sözlük where Kelime Like @aranan", baglanti);
            adpsoz.SelectCommand.Parameters.Add(new SqlParameter
            {
                ParameterName = "@aranan",
                Value = "%" + kelime + "%",
            });
            DataTable dtınfo = new DataTable();
            adpsoz.Fill(dtınfo);
            pdssoz.DataSource = dtınfo.DefaultView;
            pdssoz.AllowPaging = true;  
            pdssoz.PageSize = 1;
            SozlukOneri.DataSource = pdssoz;
            SozlukOneri.DataBind();
        }
        if (SozlukOneri.Items.Count == 0)
        {
            Panel6.Visible = false;
        }

        //Showcase
        foreach (string kelime in s)
        {
            PagedDataSource pdssoz = new PagedDataSource();
            SqlDataAdapter adpsoz = new SqlDataAdapter("select * from arda.Ürünler where Title Like @aranan or Source Like @aranan", baglanti);
            adpsoz.SelectCommand.Parameters.Add(new SqlParameter
            {
                ParameterName = "@aranan",
                Value = "%" + kelime + "%",
            });
            DataTable dtınfo = new DataTable();
            adpsoz.Fill(dtınfo);
            pdssoz.DataSource = dtınfo.DefaultView;
            pdssoz.AllowPaging = true;
            pdssoz.PageSize = 10;
            Ürünler.DataSource = pdssoz;
            Ürünler.DataBind();
        }
        if (Ürünler.Items.Count < 3)
        {
            Showcase.Visible = false;
        }
        Ürünler.RepeatColumns = 20;

        //Bilgi Kutusu Arama
        PagedDataSource pdsinfo = new PagedDataSource();
        if (pds.IsFirstPage)
        {
            if (cookielang != null && cookielang.Value != null)
            {
                if (cookielang.Value == "tr-TR")
                {
                    PagedDataSource pdssoz = new PagedDataSource();
                    SqlDataAdapter adpınfo = new SqlDataAdapter("select *, InfoLink from dbo.Infos where InfoTitle Like @aranan and Lang='tr'", baglanti);
                    adpınfo.SelectCommand.Parameters.Add(new SqlParameter
                    {
                        ParameterName = "@aranan",
                        Value = "%" + aranan + "%",
                    });
                    DataTable dtınfo = new DataTable();
                    adpınfo.Fill(dtınfo);
                    pdsinfo.DataSource = dtınfo.DefaultView;
                    pdsinfo.AllowPaging = true;
                    pdsinfo.PageSize = 1;
                    InfoBox.DataSource = pdsinfo;
                    InfoBox.DataBind();

                    if(InfoBox.Items.Count > 0)
                    {
                        //Bilgi Kutusu Onay Yazısı
                        Label onaytxt = InfoBox.Items[0].FindControl("OnayTxt") as Label;
                        Label title = InfoBox.Items[0].FindControl("title") as Label;
                        Panel onaypanel = InfoBox.Items[0].FindControl("Onay") as Panel;
                        SqlCommand sqlcmd = new SqlCommand("select Onay from dbo.Infos where InfoTitle=@aranan", baglanti);
                        sqlcmd.Parameters.AddWithValue("@aranan", title.Text);
                        if (baglanti.State == ConnectionState.Closed)
                        {
                            baglanti.Open();
                        }
                        string onay = sqlcmd.ExecuteScalar().ToString();
                        if (baglanti.State.ToString() == "Open")
                        {
                            baglanti.Close();
                            SqlConnection.ClearPool(baglanti);
                        }

                        if (onay == "Waiting")
                        {
                            onaypanel.Visible = true;
                            onaytxt.Text = "Bu bilgi kutusu daha onaylanmadı.";
                        }
                        else
                        {
                            onaypanel.Visible = false;
                        }
                    }
                }
                else
                {
                    PagedDataSource pdssoz = new PagedDataSource();
                    SqlDataAdapter adpınfo = new SqlDataAdapter("select *, InfoLink from dbo.Infos where InfoTitle Like @aranan and Lang='en'", baglanti);
                    adpınfo.SelectCommand.Parameters.Add(new SqlParameter
                    {
                        ParameterName = "@aranan",
                        Value = "%" + aranan + "%",
                    });
                    DataTable dtınfo = new DataTable();
                    adpınfo.Fill(dtınfo);
                    pdsinfo.DataSource = dtınfo.DefaultView;
                    pdsinfo.AllowPaging = true;
                    pdsinfo.PageSize = 1;
                    InfoBox.DataSource = pdsinfo;
                    InfoBox.DataBind();

                    if(InfoBox.Items.Count > 0)
                    {
                        //Bilgi Kutusu Onay Yazısı
                        Label onaytxt = InfoBox.Items[0].FindControl("OnayTxt") as Label;
                        Label title = InfoBox.Items[0].FindControl("title") as Label;
                        Panel onaypanel = InfoBox.Items[0].FindControl("Onay") as Panel;
                        SqlCommand sqlcmd = new SqlCommand("select Onay from dbo.Infos where InfoTitle=@aranan", baglanti);
                        sqlcmd.Parameters.AddWithValue("@aranan", title.Text);
                        if (baglanti.State == ConnectionState.Closed)
                        {
                            baglanti.Open();
                        }
                        string onay = sqlcmd.ExecuteScalar().ToString();
                        if (baglanti.State.ToString() == "Open")
                        {
                            baglanti.Close();
                            SqlConnection.ClearPool(baglanti);
                        }

                        if (onay == "Waiting")
                        {
                            onaypanel.Visible = true;
                            onaytxt.Text = "This infobox has not been confirmed yet.";
                        }
                        else
                        {
                            onaypanel.Visible = false;
                        }
                    }
                }
            }
            else
            {
                if (lang == "tr".ToLower())
                {
                    PagedDataSource pdssoz = new PagedDataSource();
                    SqlDataAdapter adpınfo = new SqlDataAdapter("select *, InfoLink from dbo.Infos where InfoTitle Like @aranan and Lang='tr'", baglanti);
                    adpınfo.SelectCommand.Parameters.Add(new SqlParameter
                    {
                        ParameterName = "@aranan",
                        Value = "%" + aranan + "%",
                    });
                    DataTable dtınfo = new DataTable();
                    adpınfo.Fill(dtınfo);
                    pdsinfo.DataSource = dtınfo.DefaultView;
                    pdsinfo.AllowPaging = true;
                    pdsinfo.PageSize = 1;
                    InfoBox.DataSource = pdsinfo;
                    InfoBox.DataBind();

                    if (InfoBox.Items.Count > 0)
                    {
                        //Bilgi Kutusu Onay Yazısı
                        Label onaytxt = InfoBox.Items[0].FindControl("OnayTxt") as Label;
                        Label title = InfoBox.Items[0].FindControl("title") as Label;
                        Panel onaypanel = InfoBox.Items[0].FindControl("Onay") as Panel;
                        SqlCommand sqlcmd = new SqlCommand("select Onay from dbo.Infos where InfoTitle=@aranan", baglanti);
                        sqlcmd.Parameters.AddWithValue("@aranan", title.Text);
                        if (baglanti.State == ConnectionState.Closed)
                        {
                            baglanti.Open();
                        }
                        string onay = sqlcmd.ExecuteScalar().ToString();
                        if (baglanti.State.ToString() == "Open")
                        {
                            baglanti.Close();
                            SqlConnection.ClearPool(baglanti);
                        }

                        if (onay == "Waiting")
                        {
                            onaypanel.Visible = true;
                            onaytxt.Text = "Bu bilgi kutusu daha onaylanmadı.";
                        }
                        else
                        {
                            onaypanel.Visible = false;
                        }
                    }
                }
                else
                {
                    PagedDataSource pdssoz = new PagedDataSource();
                    SqlDataAdapter adpınfo = new SqlDataAdapter("select *, InfoLink from dbo.Infos where InfoTitle Like @aranan and Lang='en'", baglanti);
                    adpınfo.SelectCommand.Parameters.Add(new SqlParameter
                    {
                        ParameterName = "@aranan",
                        Value = "%" + aranan + "%",
                    });
                    DataTable dtınfo = new DataTable();
                    adpınfo.Fill(dtınfo);
                    pdsinfo.DataSource = dtınfo.DefaultView;
                    pdsinfo.AllowPaging = true;
                    pdsinfo.PageSize = 1;
                    InfoBox.DataSource = pdsinfo;
                    InfoBox.DataBind();

                    if(InfoBox.Items.Count > 0)
                    {
                        //Bilgi Kutusu Onay Yazısı
                        Label onaytxt = InfoBox.Items[0].FindControl("OnayTxt") as Label;
                        Label title = InfoBox.Items[0].FindControl("title") as Label;
                        Panel onaypanel = InfoBox.Items[0].FindControl("Onay") as Panel;
                        SqlCommand sqlcmd = new SqlCommand("select Onay from dbo.Infos where InfoTitle=@aranan", baglanti);
                        sqlcmd.Parameters.AddWithValue("@aranan", title.Text);
                        if (baglanti.State == ConnectionState.Closed)
                        {
                            baglanti.Open();
                        }
                        string onay = sqlcmd.ExecuteScalar().ToString();
                        if (baglanti.State.ToString() == "Open")
                        {
                            baglanti.Close();
                            SqlConnection.ClearPool(baglanti);
                        }

                        if (onay == "Waiting")
                        {
                            onaypanel.Visible = true;
                            onaytxt.Text = "This infobox has not been confirmed yet.";
                        }
                        else
                        {
                            onaypanel.Visible = false;
                        }
                    }
                }
            }
            if (InfoBox.Items.Count == 0)
            {
                Panel4.Visible = false;
            }
        }
        else
        {
            Panel4.Visible = false;
        }

        //if (pds.IsFirstPage && Panel4.Visible == true)
        //{
        //    Label title = InfoBox.Items[0].FindControl("title") as Label;
        //    Suggestions.RepeatColumns = 3;
        //    if (cookielang != null && cookielang.Value != null)
        //    {
        //        PagedDataSource pdssoz = new PagedDataSource();
        //        SqlDataAdapter adpınfo = new SqlDataAdapter("select *, InfoLink from dbo.Infos where InfoTitle Like @aranan and Lang='" + cookielang + "' or Info Like @aranan and where not InfoTitle='" + title.Text + "'", baglanti);
        //        adpınfo.SelectCommand.Parameters.Add(new SqlParameter
        //        {
        //            ParameterName = "@aranan",
        //            Value = "%" + aranan + "%",
        //        });
        //        DataTable dtınfo = new DataTable();
        //        adpınfo.Fill(dtınfo);
        //        pdsinfo.DataSource = dtınfo.DefaultView;
        //        pdsinfo.AllowPaging = true;
        //        pdsinfo.PageSize = 3;
        //        Suggestions.DataSource = pdsinfo;
        //        Suggestions.DataBind();
        //        if (Suggestions.Items.Count == 0)
        //        {
        //            Panel10.Visible = false;
        //        }
        //    }
        //    else
        //    {
        //        PagedDataSource pdssoz = new PagedDataSource();
        //        SqlDataAdapter adpınfo = new SqlDataAdapter("select *, InfoLink from dbo.Infos where InfoTitle Like @aranan and Lang='" + cookielang + "' or Info Like @aranan and not InfoTitle='" + title.Text + "'", baglanti);
        //        adpınfo.SelectCommand.Parameters.Add(new SqlParameter
        //        {
        //            ParameterName = "@aranan",
        //            Value = "%" + aranan + "%",
        //        });
        //        DataTable dtınfo = new DataTable();
        //        adpınfo.Fill(dtınfo);
        //        pdsinfo.DataSource = dtınfo.DefaultView;
        //        pdsinfo.AllowPaging = true;
        //        pdsinfo.PageSize = 3;
        //        Suggestions.DataSource = pdsinfo;
        //        Suggestions.DataBind();
        //        if (Suggestions.Items.Count == 0)
        //        {
        //            Panel10.Visible = false;
        //        }
        //    }
        //    if (Suggestions.Items.Count <= 3)
        //    {
        //        Panel10.Visible = false;
        //    }
        //}
        //else
        //{
        //    Panel10.Visible = false;
        //}
        Panel10.Visible = false;

        //Haberler
        foreach (string kelime in s)
        {
            PagedDataSource pdssoz = new PagedDataSource();
            SqlDataAdapter adpsoz = new SqlDataAdapter("select * from arda.News where Title Like @aranan order by Date desc", baglanti);
            adpsoz.SelectCommand.Parameters.Add(new SqlParameter
            {
                ParameterName = "@aranan",
                Value = "%" + kelime + "%",
            });
            DataTable dtınfo = new DataTable();
            adpsoz.Fill(dtınfo);
            pdssoz.DataSource = dtınfo.DefaultView;
            pdssoz.AllowPaging = true;
            pdssoz.PageSize = 1;
            News_Results.DataSource = pdssoz;
            News_Results.DataBind();
        }
        if (News_Results.Items.Count == 0)
        { 
            News.Visible = false;
        }

        //Arananı veritabanına kaydediyor
        string admin = System.Configuration.ConfigurationManager.ConnectionStrings["admin"].ConnectionString.ToString();

        if (cookielang != null && cookielang.Value != null)
        {
            SqlConnection baglantiistek = new SqlConnection(admin);
            if (baglantiistek.State == ConnectionState.Closed)
                baglantiistek.Open();
            string istek = "insert into dbo.Arananlar(Kelime, Lang, Date, Source) values (@Kelime, @Lang, @Date, @Source)";
            SqlCommand komut = new SqlCommand(istek, baglantiistek);
            komut.Parameters.AddWithValue("@Kelime", "[anon]");
            komut.Parameters.AddWithValue("@Lang", cookielang.Value);
            komut.Parameters.AddWithValue("@Date", DateTime.Now.ToLongDateString());
            komut.Parameters.AddWithValue("@Source", DropDownList2.SelectedValue);
            komut.ExecuteNonQuery();
            baglantiistek.Close();
        }
        else
        {
            SqlConnection baglantiistek = new SqlConnection(admin);
            if (baglantiistek.State == ConnectionState.Closed)
                baglantiistek.Open();
            string istek = "insert into dbo.Arananlar(Kelime, Lang, Date, Source) values (@Kelime, @Lang, @Date, @Source)";
            SqlCommand komut = new SqlCommand(istek, baglantiistek);
            komut.Parameters.AddWithValue("@Kelime", "[anon]");
            komut.Parameters.AddWithValue("@Lang", lang);
            komut.Parameters.AddWithValue("@Date", DateTime.Now.ToLongDateString());
            komut.Parameters.AddWithValue("@Source", DropDownList2.SelectedValue);
            komut.ExecuteNonQuery();
            baglantiistek.Close();
        }

        int dolar;
        dolar = aranan.Trim().ToLower().IndexOf("dolar");

        int euro;
        euro = aranan.Trim().ToLower().IndexOf("euro");

        int sterlin;
        sterlin = aranan.Trim().ToLower().IndexOf("sterlin");

        int love;
        love = aranan.Trim().ToLower().IndexOf("i love you");

        //Dolar
        if (aranan.ToLower() == "dolar" || dolar >= 0)
        {
            Panel1.Visible = true;
            Image2.ImageUrl = "https://external-content.duckduckgo.com/iu/?u=http%3A%2F%2Fcdn.onlinewebfonts.com%2Fsvg%2Fdownload_60944.png&f=1&nofb=1";
            Ad.Text = "Amerikan Doları / Türk Lirası";
            Meslek.Text = "1 Dolar";

            try
            {
                XmlDocument xmlVerisi = new XmlDocument();
                xmlVerisi.Load("http://www.tcmb.gov.tr/kurlar/today.xml");

                decimal dolartxt = Convert.ToDecimal(xmlVerisi.SelectSingleNode(string.Format("Tarih_Date/Currency[@Kod='{0}']/ForexSelling", "USD")).InnerText.Replace('.', ','));

                Doğum.Text = dolartxt.ToString() + " Türk Lirası";
            }
            catch
            {
                Doğum.Text = "Veri bulunamadı. Detarlar için <a href=http://www.tcmb.gov.tr/kurlar/today.xml> buraya tıklayınız </a>";
            }

        }
        else if (aranan.ToLower() == "euro" || euro >= 0)
        {
            Panel1.Visible = true;
            Image2.ImageUrl = "https://thelongandshort.org/assets/images/_articleImageFullWidth/Symbols_euro.png";
            Ad.Text = "Euro / Türk Lirası";
            Meslek.Text = "1 Euro";

            try
            {
                XmlDocument xmlVerisi = new XmlDocument();
                xmlVerisi.Load("http://www.tcmb.gov.tr/kurlar/today.xml");

                decimal eurotxt = Convert.ToDecimal(xmlVerisi.SelectSingleNode(string.Format("Tarih_Date/Currency[@Kod='{0}']/ForexSelling", "EUR")).InnerText.Replace('.', ','));

                Doğum.Text = eurotxt.ToString() + " Türk Lirası";
            }
            catch
            {
                Doğum.Text = "Veri bulunamadı. Detarlar için <a href=http://www.tcmb.gov.tr/kurlar/today.xml> buraya tıklayınız </a>";
            }
        }
        else if (aranan.ToLower() == "sterlin" || sterlin >= 0)
        {
            Panel1.Visible = true;
            Image2.ImageUrl = "https://seeklogo.com/images/G/great-britain-pound-gbp-logo-4E3D714CFB-seeklogo.com.png";
            Ad.Text = "Sterlin / Türk Lirası";
            Meslek.Text = "1 Sterlin";

            try
            {
                XmlDocument xmlVerisi = new XmlDocument();
                xmlVerisi.Load("http://www.tcmb.gov.tr/kurlar/today.xml");

                decimal sterlintxt = Convert.ToDecimal(xmlVerisi.SelectSingleNode(string.Format("Tarih_Date/Currency[@Kod='{0}']/ForexSelling", "GBP")).InnerText.Replace('.', ','));

                Doğum.Text = sterlintxt.ToString() + " Türk Lirası";
            }
            catch
            {
                Doğum.Text = "Veri bulunamadı. Detarlar için <a href=http://www.tcmb.gov.tr/kurlar/today.xml> buraya tıklayınız </a>";
            }
        }
        else if (aranan.ToLower() == "i love you" || love >= 0)
        {
            Panel1.Visible = true;
            Image2.ImageUrl = "/Icons/my-love.png";
            Ad.Text = "I love you too";
            Meslek.Text = "<3";
        }
        else if (aranan.ToLower() == "seni seviyorum")
        {
            Panel1.Visible = true;
            Image2.ImageUrl = "/Icons/my-love.png";
            Ad.Text = "Bende seni seviyorum!";
            Meslek.Text = "<3";
        }
        else
        {
            Panel1.Visible = false;
        }

        if (aranan == "hesap makinesi".Trim().ToLower())
        {
            Panel5.Visible = true;
        }
        else
        {
            Panel5.Visible = false;
        }

        int ıpcontrol;
        ıpcontrol = aranan.IndexOf(" IP");

        int ıpcontrol2;
        ıpcontrol2 = aranan.IndexOf(" ip");

        if (ıpcontrol >= 0 || ıpcontrol2 >= 0 || aranan == "what is my ip?")
        {
            IPPanel.Visible = true;
            var ipAddress = string.Empty;
            if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
            {
                ipAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
                IP.Text = ipAddress;
            }
            else if (HttpContext.Current.Request.ServerVariables["HTTP_CLIENT_IP"] != null && HttpContext.Current.Request.ServerVariables["HTTP_CLIENT_IP"].Length != 0)
            {
                ipAddress = HttpContext.Current.Request.ServerVariables["HTTP_CLIENT_IP"];
                IP.Text = ipAddress;
            }
            else if (HttpContext.Current.Request.UserHostAddress.Length != 0)
            {
                ipAddress = HttpContext.Current.Request.UserHostName;
                IP.Text = ipAddress;
            }
        }
        else
        {
            IPPanel.Visible = false;
        }

        int hava = aranan.ToLower().IndexOf("hava");
        int hava2 = aranan.ToLower().IndexOf("weather");
        int hava3 = aranan.ToLower().IndexOf("wetter");
        int hava4 = aranan.ToLower().IndexOf("conditions météorologiques");
        int hava5 = aranan.ToLower().IndexOf("Погода");
        int hava6 = aranan.ToLower().IndexOf("天气");
        if (hava >= 0)
        {
            string api = "2f0a475faa72b7ade6066d6279ee5ca5";

            var ipAddress = string.Empty;
            if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
            {
                ipAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            }
            else if (HttpContext.Current.Request.ServerVariables["HTTP_CLIENT_IP"] != null && HttpContext.Current.Request.ServerVariables["HTTP_CLIENT_IP"].Length != 0)
            {
                ipAddress = HttpContext.Current.Request.ServerVariables["HTTP_CLIENT_IP"];
            }
            else if (HttpContext.Current.Request.UserHostAddress.Length != 0)
            {
                ipAddress = HttpContext.Current.Request.UserHostName;
            }

            string location = "http://api.ipinfodb.com/v3/ip-city/?key=827d056c355bcb601ce33da0280d1e8e583a0cc6b213e06908cc15844f64a9bb&ip=" + ipAddress + "&format=xml";
            XmlTextReader oku = new XmlTextReader(location);
            try
            {
                if (oku.Read())
                {
                    if (oku.NodeType == XmlNodeType.Element)
                    {
                        switch (oku.Name)
                        {
                            case "regionName":
                                City.Text = Convert.ToString(oku.ReadString());
                                break;
                        }
                    }
                }
                oku.Close();
            }
            catch
            {
                City.Text = "Istanbul";
            }

            if (cookielang != null && cookielang.Value != null)
            {
                try
                {
                    string havabaglanti = "http://api.openweathermap.org/data/2.5/weather?q=" + City.Text + "&mode=xml&lang=" + lang + "&units=metric&appid=" + api;
                    XDocument Hava = XDocument.Load(havabaglanti);
                    var sunrise = Hava.Descendants("sun").ElementAt(0).Attribute("rise").Value;
                    var sunset = Hava.Descendants("sun").ElementAt(0).Attribute("set").Value;
                    var sicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("value").Value;
                    var minsicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("min").Value;
                    var maxsicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("max").Value;
                    var feels_like = Hava.Descendants("feels_like").ElementAt(0).Attribute("value").Value;
                    var icon = Hava.Descendants("weather").ElementAt(0).Attribute("icon").Value;
                    var durum = Hava.Descendants("weather").ElementAt(0).Attribute("value").Value;
                    WeatherImg.ImageUrl = "http://openweathermap.org/img/w/" + icon + ".png";
                    Weathertext.Text = sicaklik + " ºC";
                    Weathertext2.Text = durum;
                    Feels_Like.Text = "Hissedilen Sıcaklık :  " + feels_like + "ºC";
                }
                catch
                {
                    string havabaglanti = "http://api.openweathermap.org/data/2.5/weather?q=" + "Istanbul" + "&mode=xml&lang=" + cookielang + "&units=metric&appid=" + api;
                    XDocument Hava = XDocument.Load(havabaglanti);
                    var sunrise = Hava.Descendants("sun").ElementAt(0).Attribute("rise").Value;
                    var sunset = Hava.Descendants("sun").ElementAt(0).Attribute("set").Value;
                    var sicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("value").Value;
                    var minsicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("min").Value;
                    var maxsicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("max").Value;
                    var feels_like = Hava.Descendants("feels_like").ElementAt(0).Attribute("value").Value;
                    var icon = Hava.Descendants("weather").ElementAt(0).Attribute("icon").Value;
                    var durum = Hava.Descendants("weather").ElementAt(0).Attribute("value").Value;
                    WeatherImg.ImageUrl = "http://openweathermap.org/img/w/" + icon + ".png";
                    Weathertext.Text = sicaklik + " ºC";
                    Weathertext2.Text = durum;
                    Feels_Like.Text = "Hissedilen Sıcaklık :  " + feels_like + "ºC";
                    City.Text = "Istanbul";
                }
            }
            else
            {
                try
                {
                    string havabaglanti = "http://api.openweathermap.org/data/2.5/weather?q=" + City.Text + "&mode=xml&lang=" + lang + "&units=metric&appid=" + api;
                    XDocument Hava = XDocument.Load(havabaglanti);
                    var sunrise = Hava.Descendants("sun").ElementAt(0).Attribute("rise").Value;
                    var sunset = Hava.Descendants("sun").ElementAt(0).Attribute("set").Value;
                    var sicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("value").Value;
                    var minsicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("min").Value;
                    var maxsicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("max").Value;
                    var feels_like = Hava.Descendants("feels_like").ElementAt(0).Attribute("value").Value;
                    var icon = Hava.Descendants("weather").ElementAt(0).Attribute("icon").Value;
                    var durum = Hava.Descendants("weather").ElementAt(0).Attribute("value").Value;
                    WeatherImg.ImageUrl = "http://openweathermap.org/img/w/" + icon + ".png";
                    Weathertext.Text = sicaklik + " ºC";
                    Weathertext2.Text = durum;
                    Feels_Like.Text = "Hissedilen Sıcaklık :  " + feels_like + "ºC";
                }
                catch
                {
                    string havabaglanti = "http://api.openweathermap.org/data/2.5/weather?q=" + "Istanbul" + "&mode=xml&lang=" + cookielang + "&units=metric&appid=" + api;
                    XDocument Hava = XDocument.Load(havabaglanti);
                    var sunrise = Hava.Descendants("sun").ElementAt(0).Attribute("rise").Value;
                    var sunset = Hava.Descendants("sun").ElementAt(0).Attribute("set").Value;
                    var sicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("value").Value;
                    var minsicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("min").Value;
                    var maxsicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("max").Value;
                    var feels_like = Hava.Descendants("feels_like").ElementAt(0).Attribute("value").Value;
                    var icon = Hava.Descendants("weather").ElementAt(0).Attribute("icon").Value;
                    var durum = Hava.Descendants("weather").ElementAt(0).Attribute("value").Value;
                    WeatherImg.ImageUrl = "http://openweathermap.org/img/w/" + icon + ".png";
                    Weathertext.Text = sicaklik + " ºC";
                    Weathertext2.Text = durum;
                    Feels_Like.Text = "Hissedilen Sıcaklık :  " + feels_like + "ºC";
                    City.Text = "Istanbul";
                }
            }

            if (cookielang != null && cookielang.Value != null)
            {
                if (Ülkeler.Text == "Türkiye")
                {
                    string havabaglanti = "http://api.openweathermap.org/data/2.5/weather?q=" + İllerTR.Text + "&mode=xml&lang=" + cookielang + "&units=metric&appid=" + api;
                    XDocument Hava = XDocument.Load(havabaglanti);
                    var sunrise = Hava.Descendants("sun").ElementAt(0).Attribute("rise").Value;
                    var sunset = Hava.Descendants("sun").ElementAt(0).Attribute("set").Value;
                    var sicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("value").Value;
                    var minsicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("min").Value;
                    var maxsicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("max").Value;
                    var feels_like = Hava.Descendants("feels_like").ElementAt(0).Attribute("value").Value;
                    var icon = Hava.Descendants("weather").ElementAt(0).Attribute("icon").Value;
                    var durum = Hava.Descendants("weather").ElementAt(0).Attribute("value").Value;
                    HavaImg.ImageUrl = "http://openweathermap.org/img/w/" + icon + ".png";
                    SıcaklıkTxt.Text = sicaklik + " ºC";
                    DurumTxt.Text = durum;
                    Min.Text = "Minimum Sıcaklık :   " + minsicaklik + "ºC";
                    Max.Text = "Maksimum Sıcaklık :   " + maxsicaklik + "ºC";
                    Hissedilen.Text = "Hissedilen Sıcaklık :   " + feels_like + "ºC";

                    Doğuş.Text = "Güneş Doğuşu :   " + sunrise;
                    Batış.Text = "Güneş Batışı :   " + sunset;
                    HavaDurumu.Visible = true;
                    İllerEN.Visible = false;
                    İllerTR.Visible = true;
                    İllerFR.Visible = false;
                    İllerDE.Visible = false;
                    İllerAZ.Visible = false;
                    İllerKKTC.Visible = false;
                }
                else if (Ülkeler.Text == "İngiltere")
                {
                    string havabaglanti = "http://api.openweathermap.org/data/2.5/weather?q=" + İllerEN.Text + "&mode=xml&lang=" + cookielang + "&units=metric&appid=" + api;
                    XDocument Hava = XDocument.Load(havabaglanti);
                    var sunrise = Hava.Descendants("sun").ElementAt(0).Attribute("rise").Value;
                    var sunset = Hava.Descendants("sun").ElementAt(0).Attribute("set").Value;
                    var sicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("value").Value;
                    var minsicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("min").Value;
                    var maxsicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("max").Value;
                    var feels_like = Hava.Descendants("feels_like").ElementAt(0).Attribute("value").Value;
                    var icon = Hava.Descendants("weather").ElementAt(0).Attribute("icon").Value;
                    var durum = Hava.Descendants("weather").ElementAt(0).Attribute("value").Value;
                    HavaImg.ImageUrl = "http://openweathermap.org/img/w/" + icon + ".png";
                    SıcaklıkTxt.Text = sicaklik + " ºC";
                    DurumTxt.Text = durum;
                    Min.Text = "Minimum Sıcaklık :   " + minsicaklik + "ºC";
                    Max.Text = "Maksimum Sıcaklık :   " + maxsicaklik + "ºC";
                    Hissedilen.Text = "Hissedilen Sıcaklık :   " + feels_like + "ºC";

                    Doğuş.Text = "Güneş Doğuşu :   " + sunrise;
                    Batış.Text = "Güneş Batışı :   " + sunset;
                    HavaDurumu.Visible = true;
                    İllerTR.Visible = false;
                    İllerEN.Visible = true;
                    İllerFR.Visible = false;
                    İllerDE.Visible = false;
                    İllerAZ.Visible = false;
                    İllerKKTC.Visible = false;
                }
                else if (Ülkeler.Text == "Fransa")
                {
                    string havabaglanti = "http://api.openweathermap.org/data/2.5/weather?q=" + İllerFR.Text + "&mode=xml&lang=" + cookielang + "&units=metric&appid=" + api;
                    XDocument Hava = XDocument.Load(havabaglanti);
                    var sunrise = Hava.Descendants("sun").ElementAt(0).Attribute("rise").Value;
                    var sunset = Hava.Descendants("sun").ElementAt(0).Attribute("set").Value;
                    var sicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("value").Value;
                    var minsicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("min").Value;
                    var maxsicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("max").Value;
                    var feels_like = Hava.Descendants("feels_like").ElementAt(0).Attribute("value").Value;
                    var icon = Hava.Descendants("weather").ElementAt(0).Attribute("icon").Value;
                    var durum = Hava.Descendants("weather").ElementAt(0).Attribute("value").Value;
                    HavaImg.ImageUrl = "http://openweathermap.org/img/w/" + icon + ".png";
                    SıcaklıkTxt.Text = sicaklik + " ºC";
                    DurumTxt.Text = durum;
                    Min.Text = "Minimum Sıcaklık :   " + minsicaklik + "ºC";
                    Max.Text = "Maksimum Sıcaklık :   " + maxsicaklik + "ºC";
                    Hissedilen.Text = "Hissedilen Sıcaklık :   " + feels_like + "ºC";

                    Doğuş.Text = "Güneş Doğuşu :   " + sunrise;
                    Batış.Text = "Güneş Batışı :   " + sunset;
                    HavaDurumu.Visible = true;
                    İllerTR.Visible = false;
                    İllerEN.Visible = false;
                    İllerFR.Visible = true;
                    İllerDE.Visible = false;
                    İllerAZ.Visible = false;
                    İllerKKTC.Visible = false;
                }
                else if (Ülkeler.Text == "Almanya")
                {
                    string havabaglanti = "http://api.openweathermap.org/data/2.5/weather?q=" + İllerDE.Text + "&mode=xml&lang=" + cookielang + "&units=metric&appid=" + api;
                    XDocument Hava = XDocument.Load(havabaglanti);
                    var sunrise = Hava.Descendants("sun").ElementAt(0).Attribute("rise").Value;
                    var sunset = Hava.Descendants("sun").ElementAt(0).Attribute("set").Value;
                    var sicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("value").Value;
                    var minsicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("min").Value;
                    var maxsicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("max").Value;
                    var feels_like = Hava.Descendants("feels_like").ElementAt(0).Attribute("value").Value;
                    var icon = Hava.Descendants("weather").ElementAt(0).Attribute("icon").Value;
                    var durum = Hava.Descendants("weather").ElementAt(0).Attribute("value").Value;
                    HavaImg.ImageUrl = "http://openweathermap.org/img/w/" + icon + ".png";
                    SıcaklıkTxt.Text = sicaklik + " ºC";
                    DurumTxt.Text = durum;
                    Min.Text = "Minimum Sıcaklık :   " + minsicaklik + "ºC";
                    Max.Text = "Maksimum Sıcaklık :   " + maxsicaklik + "ºC";
                    Hissedilen.Text = "Hissedilen Sıcaklık :   " + feels_like + "ºC";

                    Doğuş.Text = "Güneş Doğuşu :   " + sunrise;
                    Batış.Text = "Güneş Batışı :   " + sunset;
                    HavaDurumu.Visible = true;
                    İllerTR.Visible = false;
                    İllerEN.Visible = false;
                    İllerFR.Visible = false;
                    İllerDE.Visible = true;
                    İllerAZ.Visible = false;
                    İllerKKTC.Visible = false;
                }
                else if (Ülkeler.Text == "Azerbaycan")
                {
                    string havabaglanti = "http://api.openweathermap.org/data/2.5/weather?q=" + İllerAZ.Text + "&mode=xml&lang=" + cookielang + "&units=metric&appid=" + api;
                    XDocument Hava = XDocument.Load(havabaglanti);
                    var sunrise = Hava.Descendants("sun").ElementAt(0).Attribute("rise").Value;
                    var sunset = Hava.Descendants("sun").ElementAt(0).Attribute("set").Value;
                    var sicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("value").Value;
                    var minsicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("min").Value;
                    var maxsicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("max").Value;
                    var feels_like = Hava.Descendants("feels_like").ElementAt(0).Attribute("value").Value;
                    var icon = Hava.Descendants("weather").ElementAt(0).Attribute("icon").Value;
                    var durum = Hava.Descendants("weather").ElementAt(0).Attribute("value").Value;
                    HavaImg.ImageUrl = "http://openweathermap.org/img/w/" + icon + ".png";
                    SıcaklıkTxt.Text = sicaklik + " ºC";
                    DurumTxt.Text = durum;
                    Min.Text = "Minimum Sıcaklık :   " + minsicaklik + "ºC";
                    Max.Text = "Maksimum Sıcaklık :   " + maxsicaklik + "ºC";
                    Hissedilen.Text = "Hissedilen Sıcaklık :   " + feels_like + "ºC";

                    Doğuş.Text = "Güneş Doğuşu :   " + sunrise;
                    Batış.Text = "Güneş Batışı :   " + sunset;
                    HavaDurumu.Visible = true;
                    İllerTR.Visible = false;
                    İllerEN.Visible = false;
                    İllerFR.Visible = false;
                    İllerDE.Visible = false;
                    İllerAZ.Visible = true;
                    İllerKKTC.Visible = false;
                }
                else if (Ülkeler.Text == "KKTC")
                {
                    string havabaglanti = "http://api.openweathermap.org/data/2.5/weather?q=" + İllerKKTC.Text + "&mode=xml&lang=" + cookielang + "&units=metric&appid=" + api;
                    XDocument Hava = XDocument.Load(havabaglanti);
                    var sunrise = Hava.Descendants("sun").ElementAt(0).Attribute("rise").Value;
                    var sunset = Hava.Descendants("sun").ElementAt(0).Attribute("set").Value;
                    var sicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("value").Value;
                    var minsicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("min").Value;
                    var maxsicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("max").Value;
                    var feels_like = Hava.Descendants("feels_like").ElementAt(0).Attribute("value").Value;
                    var icon = Hava.Descendants("weather").ElementAt(0).Attribute("icon").Value;
                    var durum = Hava.Descendants("weather").ElementAt(0).Attribute("value").Value;
                    HavaImg.ImageUrl = "http://openweathermap.org/img/w/" + icon + ".png";
                    SıcaklıkTxt.Text = sicaklik + " ºC";
                    DurumTxt.Text = durum;
                    Min.Text = "Minimum Sıcaklık :   " + minsicaklik + "ºC";
                    Max.Text = "Maksimum Sıcaklık :   " + maxsicaklik + "ºC";
                    Hissedilen.Text = "Hissedilen Sıcaklık :   " + feels_like + "ºC";

                    Doğuş.Text = "Güneş Doğuşu :   " + sunrise;
                    Batış.Text = "Güneş Batışı :   " + sunset;
                    HavaDurumu.Visible = true;
                    İllerTR.Visible = false;
                    İllerEN.Visible = false;
                    İllerFR.Visible = false;
                    İllerDE.Visible = false;
                    İllerAZ.Visible = false;
                    İllerKKTC.Visible = true;
                }
            }
            else
            {
                if (Ülkeler.Text == "Türkiye")
                {
                    string havabaglanti = "http://api.openweathermap.org/data/2.5/weather?q=" + İllerTR.Text + "&mode=xml&lang=" + lang + "&units=metric&appid=" + api;
                    XDocument Hava = XDocument.Load(havabaglanti);
                    var sunrise = Hava.Descendants("sun").ElementAt(0).Attribute("rise").Value;
                    var sunset = Hava.Descendants("sun").ElementAt(0).Attribute("set").Value;
                    var sicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("value").Value;
                    var minsicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("min").Value;
                    var maxsicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("max").Value;
                    var feels_like = Hava.Descendants("feels_like").ElementAt(0).Attribute("value").Value;
                    var icon = Hava.Descendants("weather").ElementAt(0).Attribute("icon").Value;
                    var durum = Hava.Descendants("weather").ElementAt(0).Attribute("value").Value;
                    HavaImg.ImageUrl = "http://openweathermap.org/img/w/" + icon + ".png";
                    SıcaklıkTxt.Text = sicaklik + " ºC";
                    DurumTxt.Text = durum;
                    Min.Text = "Minimum Sıcaklık :   " + minsicaklik + "ºC";
                    Max.Text = "Maksimum Sıcaklık :   " + maxsicaklik + "ºC";
                    Hissedilen.Text = "Hissedilen Sıcaklık :   " + feels_like + "ºC";

                    Doğuş.Text = "Güneş Doğuşu :   " + sunrise;
                    Batış.Text = "Güneş Batışı :   " + sunset;
                    HavaDurumu.Visible = true;
                    İllerEN.Visible = false;
                    İllerTR.Visible = true;
                    İllerFR.Visible = false;
                    İllerDE.Visible = false;
                    İllerAZ.Visible = false;
                    İllerKKTC.Visible = false;
                }
                else if (Ülkeler.Text == "İngiltere")
                {
                    string havabaglanti = "http://api.openweathermap.org/data/2.5/weather?q=" + İllerEN.Text + "&mode=xml&lang=" + lang + "&units=metric&appid=" + api;
                    XDocument Hava = XDocument.Load(havabaglanti);
                    var sunrise = Hava.Descendants("sun").ElementAt(0).Attribute("rise").Value;
                    var sunset = Hava.Descendants("sun").ElementAt(0).Attribute("set").Value;
                    var sicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("value").Value;
                    var minsicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("min").Value;
                    var maxsicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("max").Value;
                    var feels_like = Hava.Descendants("feels_like").ElementAt(0).Attribute("value").Value;
                    var icon = Hava.Descendants("weather").ElementAt(0).Attribute("icon").Value;
                    var durum = Hava.Descendants("weather").ElementAt(0).Attribute("value").Value;
                    HavaImg.ImageUrl = "http://openweathermap.org/img/w/" + icon + ".png";
                    SıcaklıkTxt.Text = sicaklik + " ºC";
                    DurumTxt.Text = durum;
                    Min.Text = "Minimum Sıcaklık :   " + minsicaklik + "ºC";
                    Max.Text = "Maksimum Sıcaklık :   " + maxsicaklik + "ºC";
                    Hissedilen.Text = "Hissedilen Sıcaklık :   " + feels_like + "ºC";

                    Doğuş.Text = "Güneş Doğuşu :   " + sunrise;
                    Batış.Text = "Güneş Batışı :   " + sunset;
                    HavaDurumu.Visible = true;
                    İllerTR.Visible = false;
                    İllerEN.Visible = true;
                    İllerFR.Visible = false;
                    İllerDE.Visible = false;
                    İllerAZ.Visible = false;
                    İllerKKTC.Visible = false;
                }
                else if (Ülkeler.Text == "Fransa")
                {
                    string havabaglanti = "http://api.openweathermap.org/data/2.5/weather?q=" + İllerFR.Text + "&mode=xml&lang=" + lang + "&units=metric&appid=" + api;
                    XDocument Hava = XDocument.Load(havabaglanti);
                    var sunrise = Hava.Descendants("sun").ElementAt(0).Attribute("rise").Value;
                    var sunset = Hava.Descendants("sun").ElementAt(0).Attribute("set").Value;
                    var sicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("value").Value;
                    var minsicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("min").Value;
                    var maxsicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("max").Value;
                    var feels_like = Hava.Descendants("feels_like").ElementAt(0).Attribute("value").Value;
                    var icon = Hava.Descendants("weather").ElementAt(0).Attribute("icon").Value;
                    var durum = Hava.Descendants("weather").ElementAt(0).Attribute("value").Value;
                    HavaImg.ImageUrl = "http://openweathermap.org/img/w/" + icon + ".png";
                    SıcaklıkTxt.Text = sicaklik + " ºC";
                    DurumTxt.Text = durum;
                    Min.Text = "Minimum Sıcaklık :   " + minsicaklik + "ºC";
                    Max.Text = "Maksimum Sıcaklık :   " + maxsicaklik + "ºC";
                    Hissedilen.Text = "Hissedilen Sıcaklık :   " + feels_like + "ºC";

                    Doğuş.Text = "Güneş Doğuşu :   " + sunrise;
                    Batış.Text = "Güneş Batışı :   " + sunset;
                    HavaDurumu.Visible = true;
                    İllerTR.Visible = false;
                    İllerEN.Visible = false;
                    İllerFR.Visible = true;
                    İllerDE.Visible = false;
                    İllerAZ.Visible = false;
                    İllerKKTC.Visible = false;
                }
                else if (Ülkeler.Text == "Almanya")
                {
                    string havabaglanti = "http://api.openweathermap.org/data/2.5/weather?q=" + İllerDE.Text + "&mode=xml&lang=" + lang + "&units=metric&appid=" + api;
                    XDocument Hava = XDocument.Load(havabaglanti);
                    var sunrise = Hava.Descendants("sun").ElementAt(0).Attribute("rise").Value;
                    var sunset = Hava.Descendants("sun").ElementAt(0).Attribute("set").Value;
                    var sicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("value").Value;
                    var minsicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("min").Value;
                    var maxsicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("max").Value;
                    var feels_like = Hava.Descendants("feels_like").ElementAt(0).Attribute("value").Value;
                    var icon = Hava.Descendants("weather").ElementAt(0).Attribute("icon").Value;
                    var durum = Hava.Descendants("weather").ElementAt(0).Attribute("value").Value;
                    HavaImg.ImageUrl = "http://openweathermap.org/img/w/" + icon + ".png";
                    SıcaklıkTxt.Text = sicaklik + " ºC";
                    DurumTxt.Text = durum;
                    Min.Text = "Minimum Sıcaklık :   " + minsicaklik + "ºC";
                    Max.Text = "Maksimum Sıcaklık :   " + maxsicaklik + "ºC";
                    Hissedilen.Text = "Hissedilen Sıcaklık :   " + feels_like + "ºC";

                    Doğuş.Text = "Güneş Doğuşu :   " + sunrise;
                    Batış.Text = "Güneş Batışı :   " + sunset;
                    HavaDurumu.Visible = true;
                    İllerTR.Visible = false;
                    İllerEN.Visible = false;
                    İllerFR.Visible = false;
                    İllerDE.Visible = true;
                    İllerAZ.Visible = false;
                    İllerKKTC.Visible = false;
                }
                else if (Ülkeler.Text == "Azerbaycan")
                {
                    string havabaglanti = "http://api.openweathermap.org/data/2.5/weather?q=" + İllerAZ.Text + "&mode=xml&lang=" + lang + "&units=metric&appid=" + api;
                    XDocument Hava = XDocument.Load(havabaglanti);
                    var sunrise = Hava.Descendants("sun").ElementAt(0).Attribute("rise").Value;
                    var sunset = Hava.Descendants("sun").ElementAt(0).Attribute("set").Value;
                    var sicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("value").Value;
                    var minsicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("min").Value;
                    var maxsicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("max").Value;
                    var feels_like = Hava.Descendants("feels_like").ElementAt(0).Attribute("value").Value;
                    var icon = Hava.Descendants("weather").ElementAt(0).Attribute("icon").Value;
                    var durum = Hava.Descendants("weather").ElementAt(0).Attribute("value").Value;
                    HavaImg.ImageUrl = "http://openweathermap.org/img/w/" + icon + ".png";
                    SıcaklıkTxt.Text = sicaklik + " ºC";
                    DurumTxt.Text = durum;
                    Min.Text = "Minimum Sıcaklık :   " + minsicaklik + "ºC";
                    Max.Text = "Maksimum Sıcaklık :   " + maxsicaklik + "ºC";
                    Hissedilen.Text = "Hissedilen Sıcaklık :   " + feels_like + "ºC";

                    Doğuş.Text = "Güneş Doğuşu :   " + sunrise;
                    Batış.Text = "Güneş Batışı :   " + sunset;
                    HavaDurumu.Visible = true;
                    İllerTR.Visible = false;
                    İllerEN.Visible = false;
                    İllerFR.Visible = false;
                    İllerDE.Visible = false;
                    İllerAZ.Visible = true;
                    İllerKKTC.Visible = false;
                }
                else if (Ülkeler.Text == "KKTC")
                {
                    string havabaglanti = "http://api.openweathermap.org/data/2.5/weather?q=" + İllerKKTC.Text + "&mode=xml&lang=" + lang + "&units=metric&appid=" + api;
                    XDocument Hava = XDocument.Load(havabaglanti);
                    var sunrise = Hava.Descendants("sun").ElementAt(0).Attribute("rise").Value;
                    var sunset = Hava.Descendants("sun").ElementAt(0).Attribute("set").Value;
                    var sicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("value").Value;
                    var minsicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("min").Value;
                    var maxsicaklik = Hava.Descendants("temperature").ElementAt(0).Attribute("max").Value;
                    var feels_like = Hava.Descendants("feels_like").ElementAt(0).Attribute("value").Value;
                    var icon = Hava.Descendants("weather").ElementAt(0).Attribute("icon").Value;
                    var durum = Hava.Descendants("weather").ElementAt(0).Attribute("value").Value;
                    HavaImg.ImageUrl = "http://openweathermap.org/img/w/" + icon + ".png";
                    SıcaklıkTxt.Text = sicaklik + " ºC";
                    DurumTxt.Text = durum;
                    Min.Text = "Minimum Sıcaklık :   " + minsicaklik + "ºC";
                    Max.Text = "Maksimum Sıcaklık :   " + maxsicaklik + "ºC";
                    Hissedilen.Text = "Hissedilen Sıcaklık :   " + feels_like + "ºC";

                    Doğuş.Text = "Güneş Doğuşu :   " + sunrise;
                    Batış.Text = "Güneş Batışı :   " + sunset;
                    HavaDurumu.Visible = true;
                    İllerTR.Visible = false;
                    İllerEN.Visible = false;
                    İllerFR.Visible = false;
                    İllerDE.Visible = false;
                    İllerAZ.Visible = false;
                    İllerKKTC.Visible = true;
                }
            }
        }
        else
        {
            HavaDurumu.Visible = false;
            Panel8.Visible = false;
        }

        if (Panel1.Visible == true)
        {
            Panel6.Visible = false;
        }

        //int translate;
        //translate = aranan.IndexOf("çeviri");
        //if (translate >= 0)
        //{
        //    Translate.Visible = true;
        //}
        //else
        //{

        //}
        Translate.Visible = false;

        

        Web.ForeColor = System.Drawing.Color.BlueViolet;
        Web.Font.Bold = true;

        Görsel.ForeColor = System.Drawing.Color.LightBlue;
        Görsel.Font.Bold = false;

        Sözlük.ForeColor = System.Drawing.Color.LightBlue;
        Sözlük.Font.Bold = false;

        Müzik.ForeColor = System.Drawing.Color.LightBlue;
        Müzik.Font.Bold = false;

        Akademik.ForeColor = System.Drawing.Color.LightBlue;
        Akademik.Font.Bold = false;

        //Güvenlik
        for (int i = 0; i < rptAramaSonuclari.Items.Count; i++)
        {
            Label http = rptAramaSonuclari.Items[i].FindControl("Link") as Label;
            Image danger = rptAramaSonuclari.Items[i].FindControl("danger") as Image;
            danger.Visible = false;
            if (http.Text.StartsWith("https://"))
            {
                danger.Visible = false;
            }
            else
            {
                danger.Visible = true;
            }
        }

        //Favicon
        for (int i = 0; i < rptAramaSonuclari.Items.Count; i++)
        {
            Image favicon = rptAramaSonuclari.Items[i].FindControl("Image4") as Image;
            favicon.Visible = false;
            if (favicon.ImageUrl == null || favicon.ImageUrl.Length == 0)
            {
                favicon.Visible = false;
            }
            else
            {
                favicon.Visible = true;
            }
        }

        //Popüler Sonuçlar
        for (int i = 0; i < rptAramaSonuclari.Items.Count; i++)
        {
            SqlConnection baglanti2 = new SqlConnection(con);
            baglanti2.Open();
            Label http = rptAramaSonuclari.Items[i].FindControl("Link") as Label;
            Panel popular = rptAramaSonuclari.Items[i].FindControl("Popular") as Panel;
            int rank;
            SqlCommand sqlcmd = new SqlCommand("select Rank from arda.Sonuçlar where Link='" + http.Text + "'", baglanti2);
            rank = Convert.ToInt32(sqlcmd.ExecuteScalar());

            if (rank > 120)
            {
                popular.Visible = true;
            }
            else
            {
                popular.Visible = false;
            }
            baglanti2.Close();
        }

        if (cookielang != null && cookielang.Value != null)
        {
            if (cookielang.Value == "tr-TR")
            {
                Label1.Text = watch.Elapsed.Seconds + " saniyede " + sonuçlar + " sonuç bulundu";
            }
            else if (cookielang.Value == "en-US")
            {
                Label1.Text = sonuçlar + " results found in " + watch.Elapsed.Seconds + " second";
            }
            else if (cookielang.Value == "fr-FR")
            {
                Label1.Text = sonuçlar + " résultats trouvés en " + watch.Elapsed.Seconds + " seconde";
            }
            else if (cookielang.Value == "de-DE")
            {
                Label1.Text = sonuçlar + " Ergebnisse in " + watch.Elapsed.Seconds + " Sekunde gefunden";
            }
            else if (cookielang.Value == "en-AU")
            {
                Label1.Text = watch.Elapsed.Seconds + " saniyədə " + sonuçlar + " nəticə tapıldı";
            }
            else if (cookielang.Value == "it-IT")
            {
                Label1.Text = sonuçlar + " risultati trovati in " + watch.Elapsed.Seconds + " secondi";
            }
            else if (cookielang.Value == "ru-RU")
            {
                Label1.Text = sonuçlar + " результатов найдено за " + watch.Elapsed.Seconds + " секунд";
            }
            else if (cookielang.Value == "zh-CHS")
            {
                Label1.Text = sonuçlar + "秒内找到" + watch.Elapsed.Seconds + "个结果";
            }
            else if (cookielang.Value == "es-ES")
            {
                Label1.Text = sonuçlar + " resultados encontrados en " + watch.Elapsed.Seconds + " segundos";
            }
            else if (cookielang.Value == "pt-PT")
            {
                Label1.Text = sonuçlar + " resultados encontrados em " + watch.Elapsed.Seconds + " segundos";
            }
            else if (cookielang.Value == "ko-KR")
            {
                Label1.Text = watch.Elapsed.Seconds + " 초 안에 " + sonuçlar + " 개의 결과를 찾았습니다";
            }
            else if (cookielang.Value == "ja-JP")
            {
                Label1.Text = watch.Elapsed.Seconds + "秒で" + sonuçlar + "件の結果が見つかりました";
            }
            else if (cookielang.Value == "hu-HU")
            {
                Label1.Text = sonuçlar + " találat " + watch.Elapsed.Seconds + " másodperc alatt";
            }
            else if (cookielang.Value == "bg-BG")
            {
                Label1.Text = sonuçlar + " резултата намерени за " + watch.Elapsed.Seconds + " секунди";
            }
            else if (cookielang.Value == "en-BZ")
            {
                Label1.Text = sonuçlar + " rezultata pronađeno u " + watch.Elapsed.Seconds + " sekundi";
            }
            else
            {
                Label1.Text = sonuçlar + " results found in " + watch.Elapsed.Seconds + " second";
            }
        }
        else
        {
            if (lang == "tr".ToLower())
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("tr-TR");

                Page.MetaDescription = "Artado Search Türkiye - Yerli, Reklamsız, Gizliliğe Önem Veren, Güvenli ve Sade Tasarımlı Arama Motoru";
                Label1.Text = watch.Elapsed.Seconds + " saniyede " + sonuçlar + " sonuç bulundu";
            }
            else if (lang == "en".ToLower())
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

                Page.MetaDescription = "Artado Search - Anonymous and secure search engine";
                Label1.Text = sonuçlar + " results found in " + watch.Elapsed.Seconds + " second";
            }
            else if (lang == "fr".ToLower())
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");

                Page.MetaDescription = "Artado Search France - Moteur de recherche anonyme et sécurisé";
                Label1.Text = sonuçlar + " résultats trouvés en " + watch.Elapsed.Seconds + " seconde";
            }
            else if (lang == "de".ToLower())
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("de-DE");

                Page.MetaDescription = "Artado Search Deutschland - Anonyme und sichere Suchmaschine";
                Label1.Text = sonuçlar + " Ergebnisse in " + watch.Elapsed.Seconds + " Sekunde gefunden";
            }
            else if (lang == "az".ToLower())
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-AU");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-AU");

                Page.MetaDescription = "Artado Search Azerbaijan - Anonim və təhlükəsiz axtarış sistemi";
                Label1.Text = watch.Elapsed.Seconds + " saniyədə " + sonuçlar + " nəticə tapıldı";
            }
            else if (lang == "it".ToLower())
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("it-IT");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("it-IT");

                Page.MetaDescription = "Artado Search Italy - Motore di ricerca anonimo e sicuro";
                Label1.Text = sonuçlar + " risultati trovati in " + watch.Elapsed.Seconds + " secondi";
            }
            else if (lang == "ru".ToLower())
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");

                Page.MetaDescription = "Artado Search Russia - анонимная и безопасная поисковая система";
                Label1.Text = sonuçlar + " результатов найдено за " + watch.Elapsed.Seconds + " секунд";
            }
            else if (lang == "zh".ToLower())
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("zh-CHS");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-CHS");

                Page.MetaDescription = "Artado Search China - 匿名和安全的搜索引擎";
                Label1.Text = sonuçlar + "秒内找到" + watch.Elapsed.Seconds + "个结果";
            }
            else if (lang == "es".ToLower())
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("es-ES");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("es-ES");

                Page.MetaDescription = "Artado Search Spain - Buscador anónimo y seguro";
                Label1.Text = sonuçlar + " resultados encontrados en " + watch.Elapsed.Seconds + " segundos";
            }
            else if (lang == "pt".ToLower())
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-PT");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("pt-PT");

                Page.MetaDescription = "Artado Search Portugal - Motor de pesquisa anónimo e seguro";
                Label1.Text = sonuçlar + " resultados encontrados em " + watch.Elapsed.Seconds + " segundos";
            }
            else if (lang == "ko".ToLower())
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("ko-KR");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("ko-KR");

                Page.MetaDescription = "Artado Search 대한민국 - 익명의 안전한 검색 엔진";
                Label1.Text = watch.Elapsed.Seconds + " 초 안에 " + sonuçlar + " 개의 결과를 찾았습니다";
            }
            else if (lang == "jp".ToLower())
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("ja-JP");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("ja-JP");

                Page.MetaDescription = "Artado Search Japan - 匿名で安全な検索エンジン";
                Label1.Text = watch.Elapsed.Seconds + "秒で" + sonuçlar + "件の結果が見つかりました";
            }
            else if (lang == "hu".ToLower())
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("hu-HU");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("hu-HU");

                Page.MetaDescription = "Artado Search Hungary - Névtelen és biztonságos kereső";
                Label1.Text = sonuçlar + " találat " + watch.Elapsed.Seconds + " másodperc alatt";
            }
            else if (lang == "bg".ToLower())
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("bg-BG");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("bg-BG");

                Page.MetaDescription = "Artado Search Bulgaria - Анонимна и сигурна търсачка";
                Label1.Text = sonuçlar + " резултата намерени за " + watch.Elapsed.Seconds + " секунди";
            }
            else if (lang == "bs".ToLower())
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-BZ");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-BZ");

                Page.MetaDescription = "Artado Search Bosna i Hercegovina - Anonimna i sigurna pretraživač";
                Label1.Text = sonuçlar + " rezultata pronađeno u " + watch.Elapsed.Seconds + " sekundi";
            }
            else
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

                Page.MetaDescription = "Anonymous and secure search engine";
                Label1.Text = sonuçlar + " results found in " + watch.Elapsed.Seconds + " second";
            }
        }
        watch.Stop();
        baglanti.Close();

        for (int i = 0; i < rptAramaSonuclari.Items.Count; i++)
        {
            Label title = rptAramaSonuclari.Items[i].FindControl("Title") as Label;
            int prn = title.Text.ToLower().IndexOf("porno");
            if (prn >= 0)
            {
                if (cookielang != null && cookielang.Value != null)
                {
                    if (cookielang.Value == "tr-TR")
                    {
                        title.Text = "[Uygunsuz İçerik]";
                    }
                    else if (cookielang.Value == "en-US")
                    {
                        title.Text = "[Inappropriate Content]";
                    }
                    else if (cookielang.Value == "fr-FR")
                    {
                        title.Text = "[Contenu Inapproprié]";
                    }
                    else if (cookielang.Value == "de-DE")
                    {
                        title.Text = "[Unpassender Inhalt]";
                    }
                    else if (cookielang.Value == "en-AU")
                    {
                        title.Text = "[Uyğun Olmayan Məzmun]";
                    }
                    else if (cookielang.Value == "it-IT")
                    {
                        title.Text = "[Contenuto Inappropriato]";
                    }
                    else if (cookielang.Value == "ru-RU")
                    {
                        title.Text = "[Неприемлемое содержание]";
                    }
                    else if (cookielang.Value == "zh-CHS")
                    {
                        title.Text = "[不适当的内容]";
                    }
                    else if (cookielang.Value == "es-ES")
                    {
                        title.Text = "[Contenido inapropiado]";
                    }
                    else if (cookielang.Value == "pt-PT")
                    {
                        title.Text = "[Conteúdo inapropriado]";
                    }
                    else if (cookielang.Value == "ko-KR")
                    {
                        title.Text = "[부적절한 콘텐츠]";
                    }
                    else if (cookielang.Value == "ja-JP")
                    {
                        title.Text = "【不適切な内容】";
                    }
                    else if (cookielang.Value == "hu-HU")
                    {
                        title.Text = "[Nem megfelelő tartalom]";
                    }
                    else if (cookielang.Value == "bg-BG")
                    {
                        title.Text = "[Неподходящо съдържание]";
                    }
                    else if (cookielang.Value == "en-BZ")
                    {
                        title.Text = "[Neprimjeren sadržaj]";
                    }
                    else
                    {
                        title.Text = "[Inappropriate Content]";
                    }
                }
                else
                {
                    if (lang == "tr".ToLower())
                    {
                        title.Text = "[Uygunsuz İçerik]";
                    }
                    else if (lang == "en".ToLower())
                    {
                        title.Text = "[Inappropriate Content]";
                    }
                    else if (lang == "fr".ToLower())
                    {
                        title.Text = "[Contenu Inapproprié]";
                    }
                    else if (lang == "de".ToLower())
                    {
                        title.Text = "[Unpassender Inhalt]";
                    }
                    else if (lang == "az".ToLower())
                    {
                        title.Text = "[Uyğun Olmayan Məzmun]";
                    }
                    else if (lang == "it".ToLower())
                    {
                        title.Text = "[Contenuto Inappropriato]";
                    }
                    else if (lang == "ru".ToLower())
                    {
                        title.Text = "[Неприемлемое содержание]";
                    }
                    else if (lang == "zh".ToLower())
                    {
                        title.Text = "[不适当的内容]";
                    }
                    else if (lang == "es".ToLower())
                    {
                        title.Text = "[Contenido inapropiado]";
                    }
                    else if (lang == "pr".ToLower())
                    {
                        title.Text = "[Conteúdo inapropriado]";
                    }
                    else if (lang == "ko".ToLower())
                    {
                        title.Text = "[부적절한 콘텐츠]";
                    }
                    else if (lang == "jp".ToLower())
                    {
                        title.Text = "【不適切な内容】";
                    }
                    else if (lang == "hu".ToLower())
                    {
                        title.Text = "[Nem megfelelő tartalom]";
                    }
                    else if (lang == "bg".ToLower())
                    {
                        title.Text = "[Неподходящо съдържание]";
                    }
                    else if (lang == "bs".ToLower())
                    {
                        title.Text = "[Neprimjeren sadržaj]";
                    }
                    else
                    {
                        title.Text = "[Inappropriate Content]";
                    }
                }
            }
        }
    }

    public void Image()
    {
        Results.Visible = true;
        //Görsel Arama
        SqlConnection baglanti = new SqlConnection(con);
        string aranan = Request.QueryString["i"];
        if (aranan == null)
        {
            aranan = Request.QueryString["q"];
            if (aranan == null)
            {
                aranan = Request.QueryString["p"];
            }
            Response.Redirect("search?i=" + aranan + "&page=1");
        }
        string[] s;
        aranan = aranan.Replace(", ", "").Replace(": ", "").Replace(". ", "").Replace("; ", "").Replace(" için ", "").Replace(" ile ", "");
        s = aranan.Split(' ');

        //Boş Arama engelleme
        if (aranan == "")
        {
            Response.Redirect("Home?empty=true");
        }

        arama_çubugu2.Attributes.Add("Value", aranan);
        Page.Title = aranan + " - Artado Search";

        DropDownList2.Items.FindByValue("Google").Enabled = true;
        DropDownList2.Items.FindByValue("Artado").Enabled = true;
        DropDownList2.Items.FindByValue("Bing").Enabled = false;
        DropDownList2.Items.FindByValue("Yahoo").Enabled = false;
        DropDownList2.Items.FindByValue("Github").Enabled = false;
        DropDownList2.Items.FindByValue("Scholar").Enabled = false;
        DropDownList2.Items.FindByValue("Base").Enabled = false;

        if (DropDownList2.SelectedValue == "Artado")
        {
            GoogleImage.Visible = false;
            GorselSonuclar.Visible = true;
        }
        else if (DropDownList2.SelectedValue == "Google")
        {
            GorselSonuclar.Visible = false;
            GoogleImage.Visible = true;
        }
        else
        {
            GorselSonuclar.Visible = false;
            GoogleImage.Visible = true;
            DropDownList2.SelectedValue = "Google";
        }
        WebArama.Visible = false;
        GörselArama.Visible = true;
        MüzikArama.Visible = false;
        SözlükArama.Visible = false;
        Panel3.Visible = false;
        Panel4.Visible = false;
        Panel5.Visible = false;
        Panel6.Visible = false;
        Makaleler.Visible = false;
        HavaDurumu.Visible = false;
        Translate.Visible = false;
        Panel1.Visible = false;
        IPPanel.Visible = false;
        Google.Visible = false;

        if (GorselSonuclar.Visible == true)
        {
            foreach (string kelime in s)
            {
                SqlDataAdapter adpgorsel = new SqlDataAdapter("select *, GörselLink from dbo.Görseller where GorselTitle Like '%" + kelime + "%'", baglanti);
                DataTable dtgorsel = new DataTable();
                adpgorsel.Fill(dtgorsel);
                GorselSonuclar.DataSource = dtgorsel;
                GorselSonuclar.DataBind();
            }
            if (Request.Browser.IsMobileDevice == true)
            {
                GorselSonuclar.RepeatColumns = 1;
            }
            else
            {
                GorselSonuclar.RepeatColumns = 5;
            }
        }

        Web.ForeColor = System.Drawing.Color.LightBlue;
        Web.Font.Bold = false;

        Görsel.ForeColor = System.Drawing.Color.BlueViolet;
        Görsel.Font.Bold = true;

        Sözlük.ForeColor = System.Drawing.Color.LightBlue;
        Sözlük.Font.Bold = false;

        Müzik.ForeColor = System.Drawing.Color.LightBlue;
        Müzik.Font.Bold = false;

        Akademik.ForeColor = System.Drawing.Color.LightBlue;
        Akademik.Font.Bold = false;

        int sonuçlar = GorselSonuclar.Items.Count;
        string lang = Request.ServerVariables["HTTP_ACCEPT_LANGUAGE"].Substring(0, 2);

        System.Web.HttpCookie cookielang = HttpContext.Current.Request.Cookies["Lang"];
        if (cookielang != null && cookielang.Value != null)
        {
            if (cookielang.Value == "tr-TR")
            {
                Label1.Text = watch.Elapsed.Seconds + " saniyede " + sonuçlar + " görsel bulundu";
            }
            else if (cookielang.Value == "en-US")
            {
                Label1.Text = sonuçlar + " images found in " + watch.Elapsed.Seconds + " second";
            }
            else if (cookielang.Value == "fr-FR")
            {
                Label1.Text = sonuçlar + " images trouvés en " + watch.Elapsed.Seconds + " seconde";
            }
            else if (cookielang.Value == "de-DE")
            {
                Label1.Text = sonuçlar + " Bilder in " + watch.Elapsed.Seconds + " Sekunde gefunden";
            }
            else if (cookielang.Value == "en-AU")
            {
                Label1.Text = watch.Elapsed.Seconds + " saniyədə " + sonuçlar + " şəkil tapıldı";
            }
            else if (cookielang.Value == "it-IT")
            {
                Label1.Text = sonuçlar + "immagini trovate in " + watch.Elapsed.Seconds + " secondi";
            }
            else if (cookielang.Value == "ru-RU")
            {
                Label1.Text = sonuçlar + " изображений найдено за " + watch.Elapsed.Seconds + " секунд";
            }
            else if (cookielang.Value == "zh-CHS")
            {
                Label1.Text = sonuçlar + "秒内找到" + watch.Elapsed.Seconds + "个结果";
            }
            else if (cookielang.Value == "es-ES")
            {
                Label1.Text = sonuçlar + " imágenes encontrados en " + watch.Elapsed.Seconds + " segundos";
            }
            else if (cookielang.Value == "pt-PT")
            {
                Label1.Text = sonuçlar + " imagens encontrados em " + watch.Elapsed.Seconds + " segundos";
            }
            else if (cookielang.Value == "ko-KR")
            {
                Label1.Text = watch.Elapsed.Seconds + " 초 안에 " + sonuçlar + "개의 이미지 찾음";
            }
            else if (cookielang.Value == "ja-JP")
            {
                Label1.Text = watch.Elapsed.Seconds + "秒で" + sonuçlar + "枚の画像が見つかりました";
            }
            else if (cookielang.Value == "hu-HU")
            {
                Label1.Text = sonuçlar + " kép található " + watch.Elapsed.Seconds + " másodperc alatt";
            }
            else if (cookielang.Value == "bg-BG")
            {
                Label1.Text = sonuçlar + " изображение намерени за " + watch.Elapsed.Seconds + " секунди";
            }
            else if (cookielang.Value == "en-BZ")
            {
                Label1.Text = sonuçlar + " slika pronađeno u " + watch.Elapsed.Seconds + " sekunde";
            }
            else
            {
                Label1.Text = sonuçlar + " images found in " + watch.Elapsed.Seconds + " second";
            }
        }
        else
        {
            if (lang == "tr".ToLower())
            {
                Label1.Text = watch.Elapsed.Seconds + " saniyede " + sonuçlar + " görsel bulundu";
            }
            else if (lang == "en".ToLower())
            {
                Label1.Text = sonuçlar + " images found in " + watch.Elapsed.Seconds + " second";
            }
            else if (lang == "fr".ToLower())
            {
                Label1.Text = sonuçlar + " images trouvés en " + watch.Elapsed.Seconds + " seconde";
            }
            else if (lang == "de".ToLower())
            {
                Label1.Text = sonuçlar + " Bilder in " + watch.Elapsed.Seconds + " Sekunde gefunden";
            }
            else if (lang == "az".ToLower())
            {
                Label1.Text = watch.Elapsed.Seconds + " saniyədə " + sonuçlar + " şəkil tapıldı";
            }
            else if (lang == "it".ToLower())
            {
                Label1.Text = sonuçlar + " immagini trovate in " + watch.Elapsed.Seconds + " secondi";
            }
            else if (lang == "ru".ToLower())
            {
                Label1.Text = sonuçlar + " изображений найдено за " + watch.Elapsed.Seconds + " секунд";
            }
            else if (lang == "zh".ToLower())
            {
                Label1.Text = sonuçlar + "秒内找到" + watch.Elapsed.Seconds + "个结果";
            }
            else if (lang == "es".ToLower())
            {
                Label1.Text = sonuçlar + " imágenes encontrados en " + watch.Elapsed.Seconds + " segundos";
            }
            else if (lang == "pt".ToLower())
            {
                Label1.Text = sonuçlar + " imagens encontrados em " + watch.Elapsed.Seconds + " segundos";
            }
            else if (lang == "ko".ToLower())
            {
                Label1.Text = watch.Elapsed.Seconds + " 초 안에 " + sonuçlar + " 개의 이미지 찾음";
            }
            else if (lang == "jp".ToLower())
            {
                Label1.Text = watch.Elapsed.Seconds + "秒で" + sonuçlar + "枚の画像が見つかりました";
            }
            else if (lang == "hu".ToLower())
            {
                Label1.Text = sonuçlar + " kép található " + watch.Elapsed.Seconds + " másodperc alatt";
            }
            else if (lang == "bg".ToLower())
            {
                Label1.Text = sonuçlar + " изображение намерени за " + watch.Elapsed.Seconds + " секунди";
            }
            else if (lang == "bs".ToLower())
            {
                Label1.Text = sonuçlar + " slika pronađeno u " + watch.Elapsed.Seconds + " sekundi";
            }
            else
            {
                Label1.Text = sonuçlar + " images found in " + watch.Elapsed.Seconds + " second";
            }
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        string botdetect = Request.UserAgent;
        int bot = botdetect.IndexOf("bot".Trim().ToLower());
        if (bot >= 0)
        {
            Response.Redirect("/");
        }

        HttpCookie old = HttpContext.Current.Request.Cookies["icon"];
        if (old != null && old.Value != null)
        {
            if(old.Value == "/Icons/artado_searchv2.png")
            {
                Image1.ImageUrl = "/Icons/android-chrome-192x192.png";
            }
            else if(old.Value == "/Icons/artado_searchv3.png")
            {
                Image1.ImageUrl = "/Icons/artadov3-colorful-icon.png";
            }
            else if (old.Value == "/Icons/LGBT/artado_searchv2_lgbt.png")
            {
                Image1.ImageUrl = "/Icons/LGBT/artadov3-lgbt.png";
            }
            else if (old.Value == "/Icons/tr/artado_searchv2_tr.png")
            {
                Image1.ImageUrl = "/Icons/tr/artadov3_tr2.png";
            }
            else if (old.Value == "/Icons/fr/artado_searchv2_fr.png")
            {
                Image1.ImageUrl = "/Icons/fr/artado_fr.png";
            }
            else if (old.Value == "/Icons/de/artado_searchv2_de.png")
            {
                Image1.ImageUrl = "/Icons/de/artado_de.png";
            }
            else if (old.Value == "/Icons/uk/artado_searchv2_uk.png")
            {
                Image1.ImageUrl = "/Icons/uk/artado-uk.png";
            }
            else if (old.Value == "/Icons/islam/islam.png")
            {
                Image1.ImageUrl = "/Icons/islam/artado_islam.png";
            }
            else if (old.Value == "/Icons/oldies/old.png")
            {
                Image1.ImageUrl = "/Icons/oldies/old-icon.png";
            }
        }
        else
        {
            Image1.ImageUrl = "/Icons/android-chrome-192x192.png";
        }
        Start();
    }
    protected override void InitializeCulture()
    {
        HttpCookie cookielang = HttpContext.Current.Request.Cookies["Lang"];
        if (cookielang != null && cookielang.Value != null)
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(cookielang.Value);
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cookielang.Value);
        }
        else
        {
            System.Globalization.CultureInfo kultur = System.Threading.Thread.CurrentThread.CurrentUICulture;
            string lang = kultur.TwoLetterISOLanguageName;

            if (lang == "tr".ToLower())
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("tr-TR");
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("tr-TR");
            }
            else if (lang == "en".ToLower())
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            }
            else if (lang == "fr".ToLower())
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("fr-FR");
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("fr-FR");
            }
            else if (lang == "de".ToLower())
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("de-DE");
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("de-DE");
            }
            else if (lang == "az".ToLower())
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-AU");
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-AU");
            }
            else if (lang == "it".ToLower())
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("it-IT");
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("it-IT");
            }
            else if (lang == "ru".ToLower())
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ru-RU");
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("ru-RU");
            }
            else if (lang == "zh".ToLower())
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CHS");
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("zh-CHS");
            }
            else if (lang == "es".ToLower())
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("es-ES");
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("es-ES");
            }
            else if (lang == "pt".ToLower())
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("pt-PT");
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-PT");
            }
            else if (lang == "ko".ToLower())
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ko-KR");
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("ko-KR");
            }
            else if (lang == "jp".ToLower())
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("jp-JP");
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("jp-JP");
            }
            else if (lang == "hu".ToLower())
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("hu-HU");
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("hu-HU");
            }
            else if (lang == "bg".ToLower())
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("bg-BG");
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("bg-BG");
            }
            else if (lang == "bs".ToLower())
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-BZ");
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-BZ");
            }
            else
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            }
        }

        base.InitializeCulture();
    }

    protected void Page_PreInit(object sender, EventArgs e)
    {
        System.Web.HttpCookie cookie = HttpContext.Current.Request.Cookies["Theme"];
        if (cookie != null && cookie.Value != null)
        {
            Page.Theme = cookie.Value;
        }
        else
        {
            Page.Theme = "Night";
        }

        System.Web.HttpCookie cookie2 = HttpContext.Current.Request.Cookies["Results"];
        if (cookie2 != null && cookie2.Value != null)
        {
            DropDownList2.SelectedValue = cookie2.Value;
        }
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        int sharp = arama_çubugu2.Text.IndexOf("#");
        int ok1 = arama_çubugu2.Text.IndexOf("<");
        int ok2 = arama_çubugu2.Text.IndexOf(">");

        string schange = arama_çubugu2.Text.Replace("#", "%23");
        string okc1 = arama_çubugu2.Text.Replace("<", "%3C");
        string okc2 = arama_çubugu2.Text.Replace("'", " ");

        if (ok1 >= 0)
        {
            Response.Redirect("/search?i=" + okc1.Trim());
        }
        else if (ok2 >= 0)
        {
            Response.Redirect("/search?i=" + schange.Trim());
        }
        else if (sharp >= 0)
        {
            Response.Redirect("/search?i=" + schange.Trim());
        }
        else
        {
            Response.Redirect("/search?i=" + arama_çubugu2.Text.Trim());
        }
    }



    protected void rptAramaSonuclari_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        Response.Redirect("url.aspx");
    }

    protected void Gönder_Click(object sender, EventArgs e)
    {
        try
        {
            int mailsembol;
            mailsembol = Mail.Text.IndexOf("@");
            if (Mail.Text.EndsWith(".com") || mailsembol >= 0)
            {
                //Geri Bildirimi veritabanına kaydediyor
                SqlConnection baglantiistek = new SqlConnection(con);
                if (baglantiistek.State == ConnectionState.Closed)
                    baglantiistek.Open();
                string istek = "insert into Feedbacks(MailAdress, Feedback) values (@MailAdress,@Feedback)";
                SqlCommand komut = new SqlCommand(istek, baglantiistek);
                komut.Parameters.AddWithValue("@MailAdress", Mail.Text);
                komut.Parameters.AddWithValue("@Feedback", Deneyim.InnerText);
                komut.ExecuteNonQuery();

                Sonuc.Text = "<br/>" + "Geri bildiriminiz için teşekkür ederiz.";
            }
            else
            {
                Sonuc.Text = "E-postanızı kontrol ediniz.";
            }

        }
        catch (Exception hata)
        {
            Sonuc.Text = "<br/>" + "Üzgünüz bir sorun oluştu. Sorunu bize <a href='https://twitter.com/intent/tweet?text=Bir%20sorunum%20var!%20@ArtadoL%20Sorun:" + hata + "'>Twitter<a/> veya <a href='/İletişim'>başka platformlardan<a/> bildirebilirsiniz.";
        }
    }

    protected void Image1_Click1(object sender, ImageClickEventArgs e)
    {
        Response.Redirect("/");
    }

    protected void Web_Click(object sender, EventArgs e)
    {
        Start();
    }

    protected void Görsel_Click(object sender, EventArgs e)
    {
        Image();
    }

    protected void Sözlük_Click(object sender, EventArgs e)
    {
        Results.Visible = false;

        //Sözlük Arama
        SqlConnection baglanti = new SqlConnection(con);
        string aranan = Request.QueryString["i"];
        if (aranan == null)
        {
            aranan = Request.QueryString["q"];
            if (aranan == null)
            {
                aranan = Request.QueryString["p"];
            }
            Response.Redirect("search?i=" + aranan + "&page=1");
        }
        string[] s;
        aranan = aranan.Replace(",", "").Replace(":", "").Replace(".", "").Replace(";", "").Replace("için", "").Replace("ile", "");
        s = aranan.Split(' ');

        foreach (string kelime in s)
        {
            int ek;
            ek = kelime.ToLower().IndexOf("ler");

            if (ek >= 0)
            {
                string yenikelime = kelime.Substring(0, kelime.Length - 3);

                SqlDataAdapter adpsoz = new SqlDataAdapter("select *, Anlam from arda.Sözlük where Kelime Like '%" + yenikelime + "%'", baglanti);
                DataTable dtsoz = new DataTable();
                adpsoz.Fill(dtsoz);
                sözlüksonuc.DataSource = dtsoz;
                sözlüksonuc.DataBind();
            }
            else
            {
                SqlDataAdapter adpsoz = new SqlDataAdapter("select *, Anlam from arda.Sözlük where Kelime Like '%" + kelime + "%'", baglanti);
                DataTable dtsoz = new DataTable();
                adpsoz.Fill(dtsoz);
                sözlüksonuc.DataSource = dtsoz;
                sözlüksonuc.DataBind();
            }
        }

        SözlükArama.Visible = true;
        GörselArama.Visible = false;
        WebArama.Visible = false;
        MüzikArama.Visible = false;
        Panel3.Visible = false;
        Panel4.Visible = false;
        Panel5.Visible = false;
        Panel6.Visible = false;
        Makaleler.Visible = false;
        HavaDurumu.Visible = false;

        Web.ForeColor = System.Drawing.Color.LightBlue;
        Web.Font.Bold = false;

        Görsel.ForeColor = System.Drawing.Color.LightBlue;
        Görsel.Font.Bold = false;

        Sözlük.ForeColor = System.Drawing.Color.BlueViolet;
        Sözlük.Font.Bold = true;

        Müzik.ForeColor = System.Drawing.Color.LightBlue;
        Müzik.Font.Bold = false;

        Akademik.ForeColor = System.Drawing.Color.LightBlue;
        Akademik.Font.Bold = false;

        string lang = Request.ServerVariables["HTTP_ACCEPT_LANGUAGE"].Substring(0, 2);

        int sonuçlar = sözlüksonuc.Items.Count;
        System.Web.HttpCookie cookielang = HttpContext.Current.Request.Cookies["Lang"];
        if (cookielang != null && cookielang.Value != null)
        {
            if (cookielang.Value == "tr-TR")
            {
                Label1.Text = watch.Elapsed.Seconds + " saniyede " + sonuçlar + " sonuç bulundu";
            }
            else if (cookielang.Value == "en-US")
            {
                Label1.Text = sonuçlar + " results found in " + watch.Elapsed.Seconds + " second";
            }
            else if (cookielang.Value == "fr-FR")
            {
                Label1.Text = sonuçlar + " résultats trouvés en " + watch.Elapsed.Seconds + " seconde";
            }
            else if (cookielang.Value == "de-DE")
            {
                Label1.Text = sonuçlar + " Ergebnisse in " + watch.Elapsed.Seconds + " Sekunde gefunden";
            }
            else if (cookielang.Value == "en-AU")
            {
                Label1.Text = watch.Elapsed.Seconds + " saniyədə " + sonuçlar + " nəticə tapıldı";
            }
            else if (cookielang.Value == "it-IT")
            {
                Label1.Text = sonuçlar + " risultati trovati in " + watch.Elapsed.Seconds + " secondi";
            }
            else if (cookielang.Value == "ru-RU")
            {
                Label1.Text = sonuçlar + " результатов найдено за " + watch.Elapsed.Seconds + " секунд";
            }
            else if (cookielang.Value == "zh-CHS")
            {
                Label1.Text = sonuçlar + "秒内找到" + watch.Elapsed.Seconds + "个结果";
            }
            else if (cookielang.Value == "es-ES")
            {
                Label1.Text = sonuçlar + " resultados encontrados en " + watch.Elapsed.Seconds + " segundos";
            }
            else if (cookielang.Value == "pt-PT")
            {
                Label1.Text = sonuçlar + " resultados encontrados em " + watch.Elapsed.Seconds + " segundos";
            }
            else if (cookielang.Value == "ko-KR")
            {
                Label1.Text = watch.Elapsed.Seconds + " 초 안에 " + sonuçlar + " 개의 결과를 찾았습니다";
            }
            else if (cookielang.Value == "ja-JP")
            {
                Label1.Text = watch.Elapsed.Seconds + "秒で" + sonuçlar + "件の結果が見つかりました";
            }
            else if (cookielang.Value == "hu-HU")
            {
                Label1.Text = sonuçlar + " találat " + watch.Elapsed.Seconds + " másodperc alatt";
            }
            else if (cookielang.Value == "bg-BG")
            {
                Label1.Text = sonuçlar + " резултата намерени за " + watch.Elapsed.Seconds + " секунди";
            }
            else if (cookielang.Value == "en-BZ")
            {
                Label1.Text = sonuçlar + " rezultata pronađeno u " + watch.Elapsed.Seconds + " sekundi";
            }
            else
            {
                Label1.Text = sonuçlar + " results found in " + watch.Elapsed.Seconds + " second";
            }
        }
        else
        {
            if (lang == "tr".ToLower())
            {
                Label1.Text = watch.Elapsed.Seconds + " saniyede " + sonuçlar + " bilgi bulundu";
            }
            else if (lang == "en".ToLower())
            {
                Label1.Text = sonuçlar + " results found in " + watch.Elapsed.Seconds + " second";
            }
            else if (lang == "fr".ToLower())
            {
                Label1.Text = sonuçlar + " résultats trouvés en " + watch.Elapsed.Seconds + " seconde";
            }
            else if (lang == "de".ToLower())
            {
                Label1.Text = sonuçlar + " Ergebnisse in " + watch.Elapsed.Seconds + " Sekunde gefunden";
            }
            else if (lang == "az".ToLower())
            {
                Label1.Text = watch.Elapsed.Seconds + " saniyədə " + sonuçlar + " nəticə tapıldı";
            }
            else if (lang == "it".ToLower())
            {
                Label1.Text = sonuçlar + " risultati trovati in " + watch.Elapsed.Seconds + " secondi";
            }
            else if (lang == "ru".ToLower())
            {
                Label1.Text = sonuçlar + " результатов найдено за " + watch.Elapsed.Seconds + " секунд";
            }
            else if (lang == "zh".ToLower())
            {
                Label1.Text = sonuçlar + "秒内找到" + watch.Elapsed.Seconds + "个结果";
            }
            else if (lang == "es".ToLower())
            {
                Label1.Text = sonuçlar + " resultados encontrados en " + watch.Elapsed.Seconds + " segundos";
            }
            else if (lang == "pt".ToLower())
            {
                Label1.Text = sonuçlar + " resultados encontrados em " + watch.Elapsed.Seconds + " segundos";
            }
            else if (lang == "ko".ToLower())
            {
                Label1.Text = watch.Elapsed.Seconds + " 초 안에 " + sonuçlar + " 개의 결과를 찾았습니다";
            }
            else if (lang == "jp".ToLower())
            {
                Label1.Text = watch.Elapsed.Seconds + "秒で" + sonuçlar + "件の結果が見つかりました";
            }
            else if (lang == "hu".ToLower())
            {
                Label1.Text = sonuçlar + " találat " + watch.Elapsed.Seconds + " másodperc alatt";
            }
            else if (lang == "bg".ToLower())
            {
                Label1.Text = sonuçlar + " резултата намерени за " + watch.Elapsed.Seconds + " секунди";
            }
            else if (lang == "bs".ToLower())
            {
                Label1.Text = sonuçlar + " rezultata pronađeno u " + watch.Elapsed.Seconds + " sekundi";
            }
            else
            {
                Label1.Text = sonuçlar + " results found in " + watch.Elapsed.Seconds + " second";
            }
        }
    }

    protected void Müzik_Click(object sender, EventArgs e)
    {
        Results.Visible = false;

        //Film Arama
        SqlConnection baglanti = new SqlConnection(con);
        string aranan = Request.QueryString["i"];
        if (aranan == null)
        {
            aranan = Request.QueryString["q"];
            if (aranan == null)
            {
                aranan = Request.QueryString["p"];
            }
            Response.Redirect("search?i=" + aranan + "&page=1");
        }
        string[] s;
        aranan = aranan.Replace(",", "").Replace(":", "").Replace(".", "").Replace(";", "").Replace("için", "").Replace("ile", "");
        s = aranan.Split(' ');

        foreach (string kelime in s)
        {
            SqlDataAdapter adpınfo = new SqlDataAdapter("select * from dbo.Films where Name Like '%" + kelime + "%' or Actors Like '%" + kelime + "%'", baglanti);
            DataTable dtınfo = new DataTable();
            adpınfo.Fill(dtınfo);
            Filmler.DataSource = dtınfo;
            Filmler.DataBind();
        }

        SözlükArama.Visible = false;
        GörselArama.Visible = false;
        WebArama.Visible = false;
        MüzikArama.Visible = true;
        Panel3.Visible = false;
        Panel4.Visible = false;
        Panel5.Visible = false;
        Panel6.Visible = false;
        Makaleler.Visible = false;
        HavaDurumu.Visible = false;

        Web.ForeColor = System.Drawing.Color.LightBlue;
        Web.Font.Bold = false;

        Görsel.ForeColor = System.Drawing.Color.LightBlue;
        Görsel.Font.Bold = false;

        Sözlük.ForeColor = System.Drawing.Color.LightBlue;
        Sözlük.Font.Bold = false;

        Müzik.ForeColor = System.Drawing.Color.BlueViolet;
        Müzik.Font.Bold = true;

        Akademik.ForeColor = System.Drawing.Color.LightBlue;
        Akademik.Font.Bold = false;

        string lang = Request.ServerVariables["HTTP_ACCEPT_LANGUAGE"].Substring(0, 2);

        int sonuçlar = Filmler.Items.Count;
        System.Web.HttpCookie cookielang = HttpContext.Current.Request.Cookies["Lang"];
        if (cookielang != null && cookielang.Value != null)
        {
            if (cookielang.Value == "tr-TR")
            {
                Label1.Text = watch.Elapsed.Seconds + " saniyede " + sonuçlar + " sonuç bulundu";
            }
            else if (cookielang.Value == "en-US")
            {
                Label1.Text = sonuçlar + " results found in " + watch.Elapsed.Seconds + " second";
            }
            else if (cookielang.Value == "fr-FR")
            {
                Label1.Text = sonuçlar + " résultats trouvés en " + watch.Elapsed.Seconds + " seconde";
            }
            else if (cookielang.Value == "de-DE")
            {
                Label1.Text = sonuçlar + " Ergebnisse in " + watch.Elapsed.Seconds + " Sekunde gefunden";
            }
            else if (cookielang.Value == "en-AU")
            {
                Label1.Text = watch.Elapsed.Seconds + " saniyədə " + sonuçlar + " nəticə tapıldı";
            }
            else if (cookielang.Value == "it-IT")
            {
                Label1.Text = sonuçlar + " risultati trovati in " + watch.Elapsed.Seconds + " secondi";
            }
            else if (cookielang.Value == "ru-RU")
            {
                Label1.Text = sonuçlar + " результатов найдено за " + watch.Elapsed.Seconds + " секунд";
            }
            else if (cookielang.Value == "zh-CHS")
            {
                Label1.Text = sonuçlar + "秒内找到" + watch.Elapsed.Seconds + "个结果";
            }
            else if (cookielang.Value == "es-ES")
            {
                Label1.Text = sonuçlar + " resultados encontrados en " + watch.Elapsed.Seconds + " segundos";
            }
            else if (cookielang.Value == "pt-PT")
            {
                Label1.Text = sonuçlar + " resultados encontrados em " + watch.Elapsed.Seconds + " segundos";
            }
            else if (cookielang.Value == "ko-KR")
            {
                Label1.Text = watch.Elapsed.Seconds + " 초 안에 " + sonuçlar + " 개의 결과를 찾았습니다";
            }
            else if (cookielang.Value == "ja-JP")
            {
                Label1.Text = watch.Elapsed.Seconds + "秒で" + sonuçlar + "件の結果が見つかりました";
            }
            else if (cookielang.Value == "hu-HU")
            {
                Label1.Text = sonuçlar + " találat " + watch.Elapsed.Seconds + " másodperc alatt";
            }
            else if (cookielang.Value == "bg-BG")
            {
                Label1.Text = sonuçlar + " резултата намерени за " + watch.Elapsed.Seconds + " секунди";
            }
            else if (cookielang.Value == "en-BZ")
            {
                Label1.Text = sonuçlar + " rezultata pronađeno u " + watch.Elapsed.Seconds + " sekundi";
            }
            else
            {
                Label1.Text = sonuçlar + " results found in " + watch.Elapsed.Seconds + " second";
            }
        }
        else
        {
            if (lang == "tr".ToLower())
            {
                Label1.Text = watch.Elapsed.Seconds + " saniyede " + sonuçlar + " film bulundu";
            }
            else if (lang == "en".ToLower())
            {
                Label1.Text = sonuçlar + " results found in " + watch.Elapsed.Seconds + " second";
            }
            else if (lang == "fr".ToLower())
            {
                Label1.Text = sonuçlar + " résultats trouvés en " + watch.Elapsed.Seconds + " seconde";
            }
            else if (lang == "de".ToLower())
            {
                Label1.Text = sonuçlar + " Ergebnisse in " + watch.Elapsed.Seconds + " Sekunde gefunden";
            }
            else if (lang == "az".ToLower())
            {
                Label1.Text = watch.Elapsed.Seconds + " saniyədə " + sonuçlar + " nəticə tapıldı";
            }
            else if (lang == "it".ToLower())
            {
                Label1.Text = sonuçlar + " risultati trovati in " + watch.Elapsed.Seconds + " secondi";
            }
            else if (lang == "ru".ToLower())
            {
                Label1.Text = sonuçlar + " результатов найдено за " + watch.Elapsed.Seconds + " секунд";
            }
            else if (lang == "zh".ToLower())
            {
                Label1.Text = sonuçlar + "秒内找到" + watch.Elapsed.Seconds + "个结果";
            }
            else if (lang == "es".ToLower())
            {
                Label1.Text = sonuçlar + " resultados encontrados en " + watch.Elapsed.Seconds + " segundos";
            }
            else if (lang == "pt".ToLower())
            {
                Label1.Text = sonuçlar + " resultados encontrados em " + watch.Elapsed.Seconds + " segundos";
            }
            else if (lang == "ko".ToLower())
            {
                Label1.Text = watch.Elapsed.Seconds + " 초 안에 " + sonuçlar + " 개의 결과를 찾았습니다";
            }
            else if (lang == "jp".ToLower())
            {
                Label1.Text = watch.Elapsed.Seconds + "秒で" + sonuçlar + "件の結果が見つかりました";
            }
            else if (lang == "hu".ToLower())
            {
                Label1.Text = sonuçlar + " találat " + watch.Elapsed.Seconds + " másodperc alatt";
            }
            else if (lang == "bg".ToLower())
            {
                Label1.Text = sonuçlar + " резултата намерени за " + watch.Elapsed.Seconds + " секунди";
            }
            else if (lang == "bs".ToLower())
            {
                Label1.Text = sonuçlar + " rezultata pronađeno u " + watch.Elapsed.Seconds + " sekundi";
            }
            else
            {
                Label1.Text = sonuçlar + " results found in " + watch.Elapsed.Seconds + " second";
            }
        }
    }


    protected void Feedback_Click(object sender, EventArgs e)
    {
        SözlükArama.Visible = false;
        GörselArama.Visible = false;
        WebArama.Visible = false;
        MüzikArama.Visible = false;
        Panel3.Visible = true;
        Panel4.Visible = false;
        Panel5.Visible = false;
        Panel6.Visible = false;
        Makaleler.Visible = false;
        HavaDurumu.Visible = false;

        Web.ForeColor = System.Drawing.Color.Blue;
        Web.Font.Bold = false;

        Görsel.ForeColor = System.Drawing.Color.Blue;
        Görsel.Font.Bold = false;

        Sözlük.ForeColor = System.Drawing.Color.Blue;
        Sözlük.Font.Bold = false;

        Müzik.ForeColor = System.Drawing.Color.Blue;
        Müzik.Font.Bold = false;

        Akademik.ForeColor = System.Drawing.Color.Blue;
        Akademik.Font.Bold = false;
    }

    protected void B1_Click1(object sender, EventArgs e)
    {
        if (TextBox1.Text == "")
        { TextBox1.Text = "1"; }
        else
        { TextBox1.Text = TextBox1.Text + "1"; }
    }

    protected void B2_Click(object sender, EventArgs e)
    {
        if (TextBox1.Text == "")
        { TextBox1.Text = "2"; }
        else
        { TextBox1.Text = TextBox1.Text + "2"; }
    }

    protected void B3_Click(object sender, EventArgs e)
    {
        if (TextBox1.Text == "")
        { TextBox1.Text = "3"; }
        else
        { TextBox1.Text = TextBox1.Text + "3"; }
    }

    protected void B4_Click(object sender, EventArgs e)
    {
        if (TextBox1.Text == "")
        { TextBox1.Text = "4"; }
        else
        { TextBox1.Text = TextBox1.Text + "4"; }
    }

    protected void B5_Click(object sender, EventArgs e)
    {
        if (TextBox1.Text == "")
        { TextBox1.Text = "5"; }
        else
        { TextBox1.Text = TextBox1.Text + "5"; }
    }

    protected void B6_Click(object sender, EventArgs e)
    {
        if (TextBox1.Text == "")
        { TextBox1.Text = "6"; }
        else
        { TextBox1.Text = TextBox1.Text + "6"; }
    }

    protected void B7_Click(object sender, EventArgs e)
    {
        if (TextBox1.Text == "")
        { TextBox1.Text = "7"; }
        else
        { TextBox1.Text = TextBox1.Text + "7"; }
    }

    protected void B8_Click(object sender, EventArgs e)
    {
        if (TextBox1.Text == "")
        { TextBox1.Text = "8"; }
        else
        { TextBox1.Text = TextBox1.Text + "8"; }
    }

    protected void B9_Click(object sender, EventArgs e)
    {
        if (TextBox1.Text == "")
        { TextBox1.Text = "9"; }
        else
        { TextBox1.Text = TextBox1.Text + "9"; }
    }

    protected void B13_Click(object sender, EventArgs e)
    {
        d = "+";
        a = Convert.ToInt16(TextBox1.Text);
        TextBox1.Text = "";
    }

    protected void B12_Click(object sender, EventArgs e)
    {
        d = "-";
        a = Convert.ToInt16(TextBox1.Text);
        TextBox1.Text = "";
    }

    protected void B11_Click(object sender, EventArgs e)
    {
        d = "*";
        a = Convert.ToInt16(TextBox1.Text);
        TextBox1.Text = "";
    }

    protected void B10_Click(object sender, EventArgs e)
    {
        d = "/";
        a = Convert.ToInt16(TextBox1.Text);
        TextBox1.Text = "";
    }

    protected void B14_Click(object sender, EventArgs e)
    {
        b = Convert.ToInt16(TextBox1.Text);

        if (d == "+")
            TextBox1.Text = Convert.ToString(a + b);
        if (d == "-")
            TextBox1.Text = Convert.ToString((sbyte)(a - b));
        if (d == "*")
            TextBox1.Text = Convert.ToString(a * b);
        if (d == "/")
            TextBox1.Text = Convert.ToString(a / b);
    }

    protected void Button16_Click(object sender, EventArgs e)
    {
        TextBox1.Text = "";
    }

    protected void B15_Click(object sender, EventArgs e)
    {
        if (TextBox1.Text == "")
        { TextBox1.Text = "0"; }
        else
        { TextBox1.Text = TextBox1.Text + "0"; }
    }

    protected void Akademik_Click(object sender, EventArgs e)
    {
        Results.Visible = true;

        string lang = Request.ServerVariables["HTTP_ACCEPT_LANGUAGE"].Substring(0, 2);
        System.Web.HttpCookie cookielang = HttpContext.Current.Request.Cookies["Lang"];

        //Akademik Arama
        SqlConnection baglanti = new SqlConnection(con);
        string aranan = Request.QueryString["i"];
        if (aranan == null)
        {
            aranan = Request.QueryString["q"];
            if (aranan == null)
            {
                aranan = Request.QueryString["p"];
            }
            Response.Redirect("search?i=" + aranan + "&page=1");
        }
        string[] s;
        aranan = aranan.Replace(",", "").Replace(":", "").Replace(".", "").Replace(";", "").Replace("için", "").Replace("ile", "");
        s = aranan.Split(' ');
        PagedDataSource pdsakademik = new PagedDataSource();
        foreach (string kelime in s)
        {
            int ek;
            ek = kelime.ToLower().IndexOf("ler");

            if (ek >= 0)
            {
                string yenikelime = kelime.Substring(0, kelime.Length - 3);

                //Makale Arama
                SqlDataAdapter adp = new SqlDataAdapter("select *, Title from dbo.Articles where Title Like '%" + yenikelime + "%' or Description Like '%" + yenikelime + "%' order by Rank desc", baglanti);
                DataTable dt = new DataTable();
                pdsakademik.DataSource = dt.DefaultView;
                pdsakademik.AllowPaging = true;
                pdsakademik.PageSize = 10;
                int currentPage;
                if (Request.QueryString["page"] != null)
                {
                    currentPage = Int32.Parse(Request.QueryString["mpage"]);
                }
                else
                {
                    currentPage = 1;
                }
                pdsakademik.CurrentPageIndex = currentPage - 1;
                Label5.Text = "Sayfa: " + currentPage + " / " + pdsakademik.PageCount;
                if (!pdsakademik.IsFirstPage)
                {
                    HyperLink3.NavigateUrl = "search?i=" + aranan + "&theme=" + Page.Theme + "&mpage=" + (currentPage - 1);
                }
                if (!pdsakademik.IsLastPage)
                {
                    HyperLink4.NavigateUrl = "search?i=" + aranan + "&theme=" + Page.Theme + "&mpage=" + (currentPage + 1);
                }
                AkademikSonuçlar.DataSource = pdsakademik;
                AkademikSonuçlar.DataBind();
            }
            else
            {
                //Makale Arama
                SqlDataAdapter adp = new SqlDataAdapter("select *, Title from dbo.Articles where Title Like '%" + kelime + "%' or Description Like '%" + kelime + "%' order by Rank desc", baglanti);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                pdsakademik.DataSource = dt.DefaultView;
                pdsakademik.AllowPaging = true;
                pdsakademik.PageSize = 10;
                int currentPage;
                if (Request.QueryString["page"] != null)
                {
                    currentPage = Int32.Parse(Request.QueryString["mpage"]);
                }
                else
                {
                    currentPage = 1;
                }
                pdsakademik.CurrentPageIndex = currentPage - 1;
                Label5.Text = "Sayfa: " + currentPage + " / " + pdsakademik.PageCount;
                if (!pdsakademik.IsFirstPage)
                {
                    HyperLink3.NavigateUrl = "search?i=" + aranan + "&theme=" + Page.Theme + "&mpage=" + (currentPage - 1);
                }
                if (!pdsakademik.IsLastPage)
                {
                    HyperLink4.NavigateUrl = "search?i=" + aranan + "&theme=" + Page.Theme + "&mpage=" + (currentPage + 1);
                }
                AkademikSonuçlar.DataSource = pdsakademik;
                AkademikSonuçlar.DataBind();
            }
        }
        try
        {
            DropDownList2.Items.FindByValue("Google").Enabled = false;
            DropDownList2.Items.FindByValue("Bing").Enabled = false;
            DropDownList2.Items.FindByValue("Yahoo").Enabled = false;
            DropDownList2.Items.FindByValue("Github").Enabled = false;
            if (DropDownList2.SelectedValue == "Artado")
            {
                SözlükArama.Visible = false;
                GörselArama.Visible = false;
                WebArama.Visible = false;
                MüzikArama.Visible = false;
                Panel3.Visible = false;
                Panel4.Visible = false;
                Panel5.Visible = false;
                Panel6.Visible = false;
                Makaleler.Visible = true;
                HavaDurumu.Visible = false;
            }
            else if (DropDownList2.SelectedValue == "Scholar")
            {
                if (cookielang != null && cookielang.Value != null)
                {
                    if (cookielang.Value == "en-AU")
                    {
                        ScholarFiltre.SelectedValue = "az";
                    }
                    else if (cookielang.Value == "en-BZ")
                    {
                        ScholarFiltre.SelectedValue = "ba";
                    }
                    else
                    {
                        ScholarFiltre.SelectedValue = cookielang.Value;
                    }
                }
                else
                {
                    try
                    {
                        ScholarFiltre.SelectedValue = lang;
                    }
                    catch
                    {
                        ScholarFiltre.SelectedValue = "en-US";
                    }
                }

                Google.Visible = false;
                rptAramaSonuclari.Visible = false;
                Filtre.Visible = true;
                DropDownList1.Visible = false;
                Sort.Visible = false;
                PageSelect.Visible = false;
                Text.Visible = false;
                GoogleImage.Visible = false;
                OtherResults.Visible = true;
                int currentPage;
                if (Request.QueryString["page"] != null)
                {
                    currentPage = Int32.Parse(Request.QueryString["page"]);
                }
                else
                {
                    currentPage = 0;
                }
                string github = "https://scholar.google.com/scholar?start=" + currentPage + "&q=" + aranan + "&hl=" + ScholarFiltre.SelectedValue;
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(github.Trim());
                WebResponse response = request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string htmlText = reader.ReadToEnd();
                reader.Close();
                response.Close();
                string a = "href=\"/";
                int href = htmlText.IndexOf(a);
                while (href >= 0)
                {
                    htmlText = htmlText.Replace("\"/", "\"https://scholar.google.com/");
                    if (a.Length < htmlText.Length)
                    {
                        href = htmlText.IndexOf(a, a.Length);
                    }
                    else
                    {
                        href = -1;
                    }
                }
                int results1 = htmlText.IndexOf("<div id=\"gs_res_ccl_mid\">") + 25;
                int results2 = htmlText.Substring(results1).IndexOf("</div><div id=\"gs_res_ccl_bot\">");
                string resulttext = htmlText.Substring(results1, results2);
                ResultsTxt.Text = resulttext;

                if (currentPage < 1)
                {
                    HyperLink5.Visible = false;
                }
                else
                {
                    HyperLink5.Visible = true;
                    HyperLink5.NavigateUrl = "search?i=" + aranan + "&page=" + (currentPage - 1);
                }

                HyperLink6.NavigateUrl = "search?i=" + aranan + "&page=" + (currentPage + 1);
            }
            else if (DropDownList2.SelectedValue == "Base")
            {
                Google.Visible = false;
                rptAramaSonuclari.Visible = false;
                Filtre.Visible = true;
                DropDownList1.Visible = false;
                ScholarFiltre.Visible = false;
                Lang.Visible = false;
                Sort.Visible = false;
                PageSelect.Visible = false;
                Text.Visible = false;
                GoogleImage.Visible = false;
                OtherResults.Visible = true;
                int currentPage;
                if (Request.QueryString["page"] != null)
                {
                    currentPage = Int32.Parse(Request.QueryString["page"]);
                }
                else
                {
                    currentPage = 0;
                }
                string github = "https://www.base-search.net/Search/Results?lookfor=" + aranan + "&type=all&page=" + currentPage;
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(github.Trim());
                WebResponse response = request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string htmlText = reader.ReadToEnd();
                reader.Close();
                response.Close();
                string a = "href=\"/";
                int href = htmlText.IndexOf(a);
                while (href >= 0)
                {
                    htmlText = htmlText.Replace("\"/", "\"https://www.base-search.net/");
                    if (a.Length < htmlText.Length)
                    {
                        href = htmlText.IndexOf(a, a.Length);
                    }
                    else
                    {
                        href = -1;
                    }
                }
                int results1 = htmlText.IndexOf("<div id=\"hit-list\">") + 19;
                int results2 = htmlText.Substring(results1).IndexOf("<div class=\"row\" id=\"all-hits-export-row\">");
                string resulttext = htmlText.Substring(results1, results2);
                ResultsTxt.Text = resulttext;

                if (currentPage < 1)
                {
                    HyperLink5.Visible = false;
                }
                else
                {
                    HyperLink5.Visible = true;
                    HyperLink5.NavigateUrl = "search?i=" + aranan + "&page=" + (currentPage - 1);
                }

                HyperLink6.NavigateUrl = "search?i=" + aranan + "&page=" + (currentPage + 1);
            }
            else
            {
                DropDownList2.SelectedValue = "Base";
                Google.Visible = false;
                rptAramaSonuclari.Visible = false;
                Filtre.Visible = true;
                DropDownList1.Visible = false;
                ScholarFiltre.Visible = false;
                Lang.Visible = false;
                Sort.Visible = false;
                PageSelect.Visible = false;
                Text.Visible = false;
                GoogleImage.Visible = false;
                OtherResults.Visible = true;
                int currentPage;
                if (Request.QueryString["page"] != null)
                {
                    currentPage = Int32.Parse(Request.QueryString["page"]);
                }
                else
                {
                    currentPage = 0;
                }
                string github = "https://www.base-search.net/Search/Results?lookfor=" + aranan + "&type=all&page=" + currentPage;
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(github.Trim());
                WebResponse response = request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string htmlText = reader.ReadToEnd();
                reader.Close();
                response.Close();
                string a = "href=\"/";
                int href = htmlText.IndexOf(a);
                while (href >= 0)
                {
                    htmlText = htmlText.Replace("\"/", "\"https://www.base-search.net/");
                    if (a.Length < htmlText.Length)
                    {
                        href = htmlText.IndexOf(a, a.Length);
                    }
                    else
                    {
                        href = -1;
                    }
                }
                int results1 = htmlText.IndexOf("<div id=\"hit-list\">") + 19;
                int results2 = htmlText.Substring(results1).IndexOf("<div class=\"row\" id=\"all-hits-export-row\">");
                string resulttext = htmlText.Substring(results1, results2);
                ResultsTxt.Text = resulttext;

                if (currentPage < 1)
                {
                    HyperLink5.Visible = false;
                }
                else
                {
                    HyperLink5.Visible = true;
                    HyperLink5.NavigateUrl = "search?i=" + aranan + "&page=" + (currentPage - 1);
                }

                HyperLink6.NavigateUrl = "search?i=" + aranan + "&page=" + (currentPage + 1);
            }
        }
        catch
        {
            Google.Visible = false;
            rptAramaSonuclari.Visible = false;
            Filtre.Visible = false;
            PageSelect.Visible = false;
            Text.Visible = false;
            GoogleImage.Visible = false;
            ResultsTxt.Visible = true;
            ResultsTxt.Text = "Upps! Bir hata oluştu.<br/><br/> Upps! Something went wrong.<br/><br/> Opps! Etwas ist schief gelaufen.<br/><br/> Oups! Quelque chose s'est mal passé.<br/><br/> Ой! Что-то пошло не так.<br/><br/> 出问题了。";
        }

        Web.ForeColor = System.Drawing.Color.LightBlue;
        Web.Font.Bold = false;

        Görsel.ForeColor = System.Drawing.Color.LightBlue; 
        Görsel.Font.Bold = false;

        Sözlük.ForeColor = System.Drawing.Color.LightBlue;
        Sözlük.Font.Bold = false;

        Müzik.ForeColor = System.Drawing.Color.LightBlue;
        Müzik.Font.Bold = false;

        Akademik.ForeColor = System.Drawing.Color.BlueViolet;
        Akademik.Font.Bold = true;

        //int sonuçlar = AkademikSonuçlar.Items.Count * pdsakademik.Count;

        int sonuçlar = AkademikSonuçlar.Items.Count;
        if (cookielang != null && cookielang.Value != null)
        {
            if (cookielang.Value == "tr-TR")
            {
                Label1.Text = watch.Elapsed.Seconds + " saniyede " + sonuçlar + " makale bulundu";
            }
            else if (cookielang.Value == "en-US")
            {
                Label1.Text = sonuçlar + " results found in " + watch.Elapsed.Seconds + " second";
            }
            else if (cookielang.Value == "fr-FR")
            {
                Label1.Text = sonuçlar + " résultats trouvés en " + watch.Elapsed.Seconds + " seconde";
            }
            else if (cookielang.Value == "de-DE")
            {
                Label1.Text = sonuçlar + " Ergebnisse in " + watch.Elapsed.Seconds + " Sekunde gefunden";
            }
            else if (cookielang.Value == "en-AU")
            {
                Label1.Text = watch.Elapsed.Seconds + " saniyədə " + sonuçlar + " nəticə tapıldı";
            }
            else if (cookielang.Value == "it-IT")
            {
                Label1.Text = sonuçlar + " risultati trovati in " + watch.Elapsed.Seconds + " secondi";
            }
            else if (cookielang.Value == "ru-RU")
            {
                Label1.Text = sonuçlar + " результатов найдено за " + watch.Elapsed.Seconds + " секунд";
            }
            else if (cookielang.Value == "zh-CHS")
            {
                Label1.Text = sonuçlar + "秒内找到" + watch.Elapsed.Seconds + "个结果";
            }
            else if (cookielang.Value == "es-ES")
            {
                Label1.Text = sonuçlar + " resultados encontrados en " + watch.Elapsed.Seconds + " segundos";
            }
            else if (cookielang.Value == "pt-PT")
            {
                Label1.Text = sonuçlar + " resultados encontrados em " + watch.Elapsed.Seconds + " segundos";
            }
            else if (cookielang.Value == "ko-KR")
            {
                Label1.Text = watch.Elapsed.Seconds + " 초 안에 " + sonuçlar + " 개의 결과를 찾았습니다";
            }
            else if (cookielang.Value == "ja-JP")
            {
                Label1.Text = watch.Elapsed.Seconds + "秒で" + sonuçlar + "件の結果が見つかりました";
            }
            else if (cookielang.Value == "hu-HU")
            {
                Label1.Text = sonuçlar + " találat " + watch.Elapsed.Seconds + " másodperc alatt";
            }
            else if (cookielang.Value == "bg-BG")
            {
                Label1.Text = sonuçlar + " резултата намерени за " + watch.Elapsed.Seconds + " секунди";
            }
            else if (cookielang.Value == "en-BZ")
            {
                Label1.Text = sonuçlar + " rezultata pronađeno u " + watch.Elapsed.Seconds + " sekundi";
            }
            else
            {
                Label1.Text = sonuçlar + " results found in " + watch.Elapsed.Seconds + " second";
            }
        }
        else
        {
            if (lang == "tr".ToLower())
            {
                Label1.Text = watch.Elapsed.Seconds + " saniyede " + sonuçlar + " makale bulundu";
            }
            else if (lang == "en".ToLower())
            {
                Label1.Text = sonuçlar + " results found in " + watch.Elapsed.Seconds + " second";
            }
            else if (lang == "fr".ToLower())
            {
                Label1.Text = sonuçlar + " résultats trouvés en " + watch.Elapsed.Seconds + " seconde";
            }
            else if (lang == "de".ToLower())
            {
                Label1.Text = sonuçlar + " Ergebnisse in " + watch.Elapsed.Seconds + " Sekunde gefunden";
            }
            else if (lang == "az".ToLower())
            {
                Label1.Text = watch.Elapsed.Seconds + " saniyədə " + sonuçlar + " nəticə tapıldı";
            }
            else if (lang == "it".ToLower())
            {
                Label1.Text = sonuçlar + " risultati trovati in " + watch.Elapsed.Seconds + " secondi";
            }
            else if (lang == "ru".ToLower())
            {
                Label1.Text = sonuçlar + " результатов найдено за " + watch.Elapsed.Seconds + " секунд";
            }
            else if (lang == "zh".ToLower())
            {
                Label1.Text = sonuçlar + "秒内找到" + watch.Elapsed.Seconds + "个结果";
            }
            else if (lang == "es".ToLower())
            {
                Label1.Text = sonuçlar + " resultados encontrados en " + watch.Elapsed.Seconds + " segundos";
            }
            else if (lang == "pt".ToLower())
            {
                Label1.Text = sonuçlar + " resultados encontrados em " + watch.Elapsed.Seconds + " segundos";
            }
            else if (lang == "ko".ToLower())
            {
                Label1.Text = watch.Elapsed.Seconds + " 초 안에 " + sonuçlar + " 개의 결과를 찾았습니다";
            }
            else if (lang == "jp".ToLower())
            {
                Label1.Text = watch.Elapsed.Seconds + "秒で" + sonuçlar + "件の結果が見つかりました";
            }
            else if (lang == "hu".ToLower())
            {
                Label1.Text = sonuçlar + " találat " + watch.Elapsed.Seconds + " másodperc alatt";
            }
            else if (lang == "bg".ToLower())
            {
                Label1.Text = sonuçlar + " резултата намерени за " + watch.Elapsed.Seconds + " секунди";
            }
            else if (lang == "bs".ToLower())
            {
                Label1.Text = sonuçlar + " rezultata pronađeno u " + watch.Elapsed.Seconds + " sekundi";
            }
            else
            {
                Label1.Text = sonuçlar + " results found in " + watch.Elapsed.Seconds + " second";
            }
        }
    }

    protected void Button17_Click(object sender, EventArgs e)
    {

    }

    protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        HttpCookie old = HttpContext.Current.Request.Cookies["Results"];
        if (old != null && old.Value != null)
        {
            old.Expires = DateTime.UtcNow.AddDays(-1);
            Response.Cookies.Add(old);
            Session.Abandon();

            HttpCookie cookie = new HttpCookie("Results");
            cookie.Value = DropDownList2.SelectedValue;
            cookie.Expires = DateTime.UtcNow.AddDays(360);
            Response.Cookies.Add(cookie);
            Page.Response.Redirect(Page.Request.Url.ToString());
        }
        else
        {
            HttpCookie cookie = new HttpCookie("Results");
            cookie.Value = DropDownList2.SelectedValue;
            cookie.Expires = DateTime.UtcNow.AddDays(360);
            Response.Cookies.Add(cookie);
            Page.Response.Redirect(Page.Request.Url.ToString());
        }
    }
}

//Test Area

//Yandex Results
//else if (DropDownList2.SelectedValue == "Yandex")
//{
//    Google.Visible = false;
//    rptAramaSonuclari.Visible = false;
//    Filtre.Visible = false;
//    PageSelect.Visible = false;
//    Text.Visible = false;
//    ScholarFiltre.Visible = false;
//    Lang.Visible = false;
//    GoogleImage.Visible = false;
//    OtherResults.Visible = true;
//    int currentPage;
//    if (Request.QueryString["page"] != null)
//    {
//        currentPage = Int32.Parse(Request.QueryString["page"]);
//    }
//    else
//    {
//        currentPage = 0;
//    }
//    string bing = "https://yandex.com.tr/search/?lr=103850&text=" + aranan + "&p=" + currentPage;
//    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(bing.Trim());
//    request.Referer = "https://yandex.com.tr/";
//    request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:91.0) Gecko/20100101 Firefox/91.0";
//    WebResponse response = request.GetResponse();
//    StreamReader reader = new StreamReader(response.GetResponseStream());
//    string htmlText = reader.ReadToEnd();
//    reader.Close();
//    response.Close();
//    int results1 = htmlText.IndexOf("<div class=\"content i-bem\" data-bem=\"{\"content\":{\"sizes\":[{\"width\":0,\"cols\":6},{\"width\":1094,\"cols\":8},{\"width\":1186,\"cols\":10},{\"width\":1278,\"cols\":12}]}}\"><div class=\"content__left\" data-log-node=\"6j1c36\">".ToLower()) + 207;
//    int results2 = htmlText.Substring(results1).IndexOf("sonraki</a></div></div></div>");
//    string resulttext = htmlText.Substring(results1, results2);
//    ResultsTxt.Text = resulttext;

//    if (currentPage <= 1)
//    {
//        HyperLink5.Visible = false;
//    }
//    else
//    {
//        HyperLink5.Visible = true;
//        HyperLink5.NavigateUrl = "search?i=" + aranan + "&page=" + (currentPage - 1);
//    }

//    HyperLink6.NavigateUrl = "search?i=" + aranan + "&page=" + (currentPage + 1);
//}