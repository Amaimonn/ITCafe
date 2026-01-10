using System;
using Cysharp.Threading.Tasks;

namespace ITCafe
{
    public interface ILocalizationLoader : IDisposable
    {
        public void Init();
        public UniTaskVoid LoadTables();
    }
}