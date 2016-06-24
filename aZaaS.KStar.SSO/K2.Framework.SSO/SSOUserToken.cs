using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace K2.Framework.SSO
{

    public enum CrypterKind { SSO = 0 }  ;


    /// <summary>
    /// SSOUserToken:用户标识
    /// </summary>
    public class SSOUserToken
    {
        //字段数
        public const int ItemCount = 4;

        #region  private field

        private string _userID = "";
        private DateTime _expireTime = System.DateTime.Now.AddHours(SSOConst.DEFAULT_VALID_PERIOD);
        private int _validPeriod = SSOConst.DEFAULT_VALID_PERIOD;
        private string _errorMessage = "";
        private DateTime _createTime = System.DateTime.Now;

        private CrypterKind _crypterKind; 


        #endregion
 

        #region  构造函数
        /// <summary>
        /// 创建一个无效(过期)的SSOUserToken
        /// </summary>
        internal SSOUserToken()
        {
            this._expireTime = System.DateTime.Now;
            this._validPeriod = 0;
            this._errorMessage = "空参数构造，直接返回过期的SSOUserToken.";

            this._crypterKind = CrypterKind.SSO;
        }

        /// <summary>
        /// 设置有效期
        /// </summary>
        /// <param name="Minute"></param>
        private void RefreshValidPeirod()
        {
            this._validPeriod = Config.UserTokenValidPeriod;
            this._expireTime = System.DateTime.Now.AddHours(Config.UserTokenValidPeriod);
            this._createTime = System.DateTime.Now;
        }		
         

        /// <summary>
        /// 根据密文构造SSOUsertoken实例
        /// </summary>
        /// <param name="eData">加密后的User Token</param>
        internal SSOUserToken(string eData, CrypterKind crypterKind)
        {
            this._crypterKind = crypterKind;
            string Content = "";
            try
            {
                Content =UserTokenCryptographer.Decrypt(eData, crypterKind);
 
            }
            catch (Exception ex)
            {                     
                PageHelper.MessageBox("解密不成功：" + ex.Message);
            }
             

            if (Content == "")
            {
                this._errorMessage = "解密不成功，可能密文不正确，加密键改变，需要重新登录。";
                new SSOUserToken();
                return;
            }


            string[] items = Content.Replace("||", ";").Split(';');
            if (items.Length != SSOUserToken.ItemCount)
            {
                this._errorMessage = "密文SSOUserToken无法解析得到正确的SSOUserToken";
                new SSOUserToken();
                return;
            }



            this._userID = items[0];
            this._expireTime = DateTime.Parse(items[1]);
            this._validPeriod = int.Parse(items[2]);
            this._createTime = DateTime.Parse(items[3]);
            this._errorMessage = "";
        }


        /// <summary>
        /// 产生token基本信息
        /// </summary>
        /// <returns></returns>
        private string GeanInfo()
        {
            if (this.UserID == "")
            {
                this._expireTime = System.DateTime.Now.AddMinutes(-1);
            }

            string data = this.UserID
                  + "||" + this.ExpireTime.ToString("yyyy-MM-dd HH:mm:ss")
                  + "||" + Config.UserTokenValidPeriod.ToString()
                  + "||" + this.CreateTime.ToString("yyyy-MM-dd HH:mm:ss");

            return data;
        }
         

        /// <summary>
        /// 对token信息加密
        /// </summary>
        /// <returns></returns>
        private string Format()
        { 
            return UserTokenCryptographer.Encrypt(this.GeanInfo(), this._crypterKind);
        }

        /// <summary>
        /// 对token信息加密
        /// </summary>
        /// <returns></returns>
        private string Format(string userID)
        {
            this._userID = userID;
            return UserTokenCryptographer.Encrypt(this.GeanInfo(), this._crypterKind);
        }        


        #endregion


        #region   internal

        /// <summary>
        /// 根据密文构造新的SSOUserToken Cookie对象
        /// </summary>
        /// <param name="eData"></param>
        /// <returns></returns>
        internal HttpCookie NewCookie(string eData)
        {  
            HttpCookie cookie = new HttpCookie(SSOConst.SSOUserToken);
            cookie.Values.Add("Value", eData);
           // cookie.Expires = this._expireTime;
            if (Config.CookieDomain.Trim() != "")
            {
                cookie.Domain = Config.CookieDomain;
            }
            return cookie;
        }
          
        /// <summary>
        /// 建立Cookie
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>

        internal string Build(HttpApplication application)
        {
            application.Request.Cookies.Remove(SSOConst.SSOUserToken);
            string eData = this.Format();

            application.Response.AppendCookie(this.NewCookie(eData));
            return eData;
        }


        /// <summary>
        /// 建立Cookie
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        internal string Build(Page page)
        {
            page.Request.Cookies.Remove(SSOConst.SSOUserToken);
            string eData = this.Format();

            page.Response.AppendCookie(this.NewCookie(eData));
            return eData;
        }

        /// <summary>
        /// 建立过期的Cookie
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        internal DateTime ExpireBuild(Page page)
        {
            page.Request.Cookies.Remove(SSOConst.SSOUserToken);
            string eData = this.Format(); 
            HttpCookie _cookie = this.NewCookie(eData);
            _cookie.Expires = DateTime.Now.AddHours(-SSOConst.DEFAULT_VALID_PERIOD);
            page.Response.AppendCookie(_cookie);
            return _cookie.Expires;
        }

        /// <summary>
        /// 建立过期的Cookie
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        internal DateTime ExpireBuild(Controller page)
        {
            page.Request.Cookies.Remove(SSOConst.SSOUserToken);
            string eData = this.Format();
            HttpCookie _cookie = this.NewCookie(eData);
            _cookie.Expires = DateTime.Now.AddHours(-SSOConst.DEFAULT_VALID_PERIOD);
            page.Response.AppendCookie(_cookie);
            return _cookie.Expires;
        }


        


        /// <summary>
        /// 建立Token
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        internal string Build(Page page,string userID)
        {
            page.Request.Cookies.Remove(SSOConst.SSOUserToken);
            string eData = this.Format(userID); 
            page.Response.AppendCookie(this.NewCookie(eData));
            return eData;
        }


        /// <summary>
        /// 建立Token
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        internal string Build(HttpContextBase context, string userID)
        {
            context.Request.Cookies.Remove(SSOConst.SSOUserToken);
            string eData = this.Format(userID);
            context.Response.AppendCookie(this.NewCookie(eData));
            return eData;
        }

        #endregion


        #region  只读属性 public

        /// <summary>
        /// 取得最后的错误信息
        /// </summary>
        /// <returns></returns>
        public string GetErrorMessage()
        {
            return _errorMessage;
        }

        /// <summary>
        /// UserID
        /// </summary>
        public string UserID
        {
            get
            {
                return this._userID;
            }
            set {
                this._userID = value;
            }
        }

        /// <summary>
        /// 取得过期时刻
        /// </summary>
        public DateTime ExpireTime
        {
            get
            {
                return this._expireTime;
            }
        }


        /// <summary>
        /// 创建时间
        /// </summary>           
        public DateTime CreateTime
        {
            get
            {
                return this._createTime;
            }
        }



        /// <summary>
        /// 判断是否已过期
        /// </summary>
        public bool IsExpire
        {
            get
            {
                return this.ExpireTime <= System.DateTime.Now;
            }
        }




        public bool IsOverByCreateTime
        {
            //超期：// CreateTime        CreateTime +5min        now
            get
            {
                return (this.CreateTime.AddMinutes(Config.UserTokenCreateTimeValidPeriod) < (System.DateTime.Now));

            }
        }

        #endregion  属性


        #region
 

        /// <summary>
        /// 延长时间
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        public string RefreshBuild(HttpApplication application)
        {
            this.RefreshValidPeirod();      //延长时间                                                 
            return this.Build(application);
        }

         public string RefreshBuild(Page page)
        {
            this.RefreshValidPeirod();      //延长时间                                                 
            return this.Build(page);
        } 

        /// <summary>
        /// 重建User Token
        /// </summary>
        /// <param name="page"></param>
        public string ReBuild(Page page,string userID)
        {
            this.RefreshValidPeirod();      //延长时间                                                 
            return this.Build(page,userID);
        }

        /// <summary>
        /// 重建User Token
        /// </summary>
        /// <param name="page"></param>
        public string ReBuild(HttpContextBase context, string userID)
        {
            this.RefreshValidPeirod();      //延长时间                                                 
            return this.Build(context, userID);
        }
        /// <summary>
        /// 重建User Token
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        public string ReBuild(HttpApplication application)
        {                                        
            return this.Build(application);
        }
         

        public DateTime Expire()
        {
            this._expireTime = System.DateTime.Now.AddHours(-SSOConst.DEFAULT_VALID_PERIOD);
            this.Format(); 
            return this.ExpireTime;
        }

        #endregion


    }	
}
