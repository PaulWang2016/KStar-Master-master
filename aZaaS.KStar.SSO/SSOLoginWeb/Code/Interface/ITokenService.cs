using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SSOLoginWeb.Code.Interface
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“ITokenService”。
    [ServiceContract]
    public interface ITokenService
    {
        /// <summary>
        /// 根据Token判定是否有登录信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [OperationContract]
        string HasLoginedByToken(string token);

          /// <summary>
        /// 根据Token判定是否有登录信息返回UserAccount
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [OperationContract]
        string HasLoginedByUserAccount(string token);

        /// <summary>
        /// 指定用某个系统账号登录
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="userAccount"></param>
        /// <param name="tokenID"></param>
        /// <returns></returns>
        [OperationContract]
        string Login(string systemCode, string userAccount, string tokenID);

        /// <summary>
        /// 指定用某个系统账号注销登录
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="userAccount"></param>
        [OperationContract]
        void LoginOut(string systemCode, string userAccount);

        /// <summary>
        /// 通过Token去注销登录
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="userAccount"></param>
        [OperationContract]
        bool LoginOutByToken(string token);
        
    }
}
