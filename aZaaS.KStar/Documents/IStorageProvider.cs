using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.Documents
{
    // 在短期内，文档存储的扩展是可预见的。
    // 因此此接口暂不对外公布，一切扩展内部完成。
    internal interface IStorageProvider
    {
        // 将传入的字节流保存到目标存储中，并返回对应的StorageUri
        string Save(byte[] documentStream);

        // 从目标系统中删除给定 stoargeUri 所对应的文档
        void Delete(string storageUri);

        // 判断给定的 stoargeUri 在目标系统中是否存在
        bool Exists(string stoargeUri);

        // 告诉系统给定的 stoargeUri 自身是否可以处理
        // 比如以sps:// 开头的，只有SharePointStorageProvider 
        // 可以Handle，其他都会返回False
        bool CanHandle(string storageUri);

        // 根据给定的 storageUri 获取内容的字节流
        byte[] Retrieve(string storageUri);
    }
}
