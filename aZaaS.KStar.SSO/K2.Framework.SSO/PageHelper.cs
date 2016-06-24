using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace K2.Framework.SSO
{
    /// <summary>
    /// PageHelper：页面常用功能
    /// </summary>
    public class PageHelper
    {
        public static void MessageBox(string msg)
        {
            try
            {
                HttpContext.Current.Response.Write("<script>window.alert('" + msg.Replace(@"\", @"\\") + "')</script>");
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }


        public static void MessageBox(Page page, string msg)
        {
            try
            {
                string script = "<script>window.alert('" + msg.Replace(@"\", @"\\") + "')</script>";
                string key = System.Guid.NewGuid().ToString();
                page.ClientScript.RegisterStartupScript(page.GetType(),key, script);
                //page.Response.Write() ;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }



        public static void ConfirmBox(Page page, string msg)
        {
            try
            {
                string script = "<script>return  window.confirm('" + msg.Replace(@"\", @"\\") + "')</script>";
                string key = System.Guid.NewGuid().ToString();
                page.ClientScript.RegisterStartupScript(page.GetType(), key, script);
                //page.Response.Write() ;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }


        public static void InsertSelectTipItem(System.Web.UI.WebControls.DropDownList dropList)
        {
            dropList.Items.Insert(0, new ListItem("--请选择--", "0"));
        }


        public static string GetTAG_A_DocListString_ForWindowOpen(string docListStr, string dirName)
        {
            return GetTAG_A_DocListString_ForWindowOpen(docListStr.Split(','), dirName);
        }


        /// <summary>
        ///  将一个文档名字符串数组 转换成 由逗号分隔的 多个 <a> 标记字符串，点击时 用新窗口打开
        /// </summary>
        /// <param name="arrayDoc"></param>
        /// <param name="sDirName"></param>
        /// <returns></returns>

        public static string GetTAG_A_DocListString_ForWindowOpen(string[] arrayDoc, string dirName)
        {
            string sAllAnchor = ""; string sAnchor = ""; string sURL = "";
            for (int i = 0; i < arrayDoc.Length; i++)
            {
                sURL = dirName + arrayDoc[i];
                sAnchor = "<a href='#' onclick='" + GetWindowOpen_OfOnClick(sURL) + "'>" + arrayDoc[i] + "</a>";
                sAllAnchor += (sAllAnchor == "" ? "" : ",") + sAnchor;
            }
            return sAllAnchor;
        }


        public static string GetTAG_WindowOpenUrl(string url, string text)
        {
            string sAnchor = "<a target ='_blank'  href='#' onclick='" + GetWindowOpen_OfOnClick(url) + "'>" + text + "</a>";
            return sAnchor;
        }


        public static string GetTAG_WindowOpenUrl(string url, string text, string target)
        {
            string sAnchor = "<a target ='" + target + "'  href='#' onclick='" + GetWindowOpen_OfOnClick(url) + "'>" + text + "</a>";
            return sAnchor;
        }



        public static string GetWindowOpen_OfOnClick(string url)
        {
            return "javascript:window.open(\"" + url + "\") ";
        }



        public static void InitOnClick_ConfirmWindow_ForButton(System.Web.UI.WebControls.Button button, string msg)
        {
            if (msg.EndsWith("?") == false && msg.EndsWith("？") == false)
            {
                msg += "?";
            }

            if (button == null) return;

            button.Attributes.Add("OnClick", "jscript:window.confirm('" + msg + "')");
        }


        public static void WindowClose(Page page)
        {
            page.Response.Write("<script>window.close()</script>");
        }


        public static void WindowOpen(Page page, string url)
        {
            page.Response.Write("<script>window.open('" + url + "')</script>");
        }


        public static void WindowShowModelessDialog(string url, string pam, int width, int height)
        {
            string script = "<script>window.showModelessDialog('{0}','{1}','dialogWidth:{2}px;dialogHeight:{3}px;status:no');</script>";
            HttpContext.Current.Response.Write(string.Format(script, url, pam, width, height));
        }


        public static void WindowShowModalDialog(string url, string pam, int width, int height)
        {
            string script = "<script>window.showModalDialog('{0}','{1}','dialogWidth:{2}px;dialogHeight:{3}px;status:no');</script>";
            HttpContext.Current.Response.Write(string.Format(script, url, pam, width, height));
        }


    }
}
