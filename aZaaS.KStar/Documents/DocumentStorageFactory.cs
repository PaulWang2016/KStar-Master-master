using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.Documents
{
    internal class DocumentStorageFactory : IStorageProvider
    {
        private static readonly List<IStorageProvider> _implInstances = new List<IStorageProvider>();

        public void Register(Type providerImpl)
        {
            // 此处将类型实例化后注册。传入Type而非其实例是为后续可能的扩展做考虑。
            // 需要注意：
            // 1. 判断Type是否实现了 IStorageProvider 接口
            // 2. 每个实例的内部切勿有状态，否则会引起线程冲突的发生。
            throw new NotImplementedException();
        }

        // 此处采用了 Composite Pattern 的变体来实现 管道-过滤器 架构。
        // 以下每个方法的调用，实际上是对所有实例的一次轮询，找到 CanHandle 
        // 返回为 True 的实例，然后调用目标方法。

        #region 管道-过滤器 方法
        public string Save(byte[] documentStream)
        {
            throw new NotImplementedException();
        }

        public void Delete(string storageUri)
        {
            throw new NotImplementedException();
        }

        public bool Exists(string stoargeUri)
        {
            throw new NotImplementedException();
        }

        public bool CanHandle(string storageUri)
        {
            throw new NotImplementedException();
        }

        public byte[] Retrieve(string storageUri)
        {
            throw new NotImplementedException();
        } 
        #endregion
    }
}
