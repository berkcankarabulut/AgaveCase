using Cysharp.Threading.Tasks;

namespace Project.LevelInitialization.Runtime
{
    public interface IInitializable
    {
        UniTask InitializeAsync();
        bool IsInitialized { get; }
        string SystemName { get; }
    }
}