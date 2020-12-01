using Cysharp.Threading.Tasks;

namespace Common {
    public interface IStateChangeable {
        UniTask StateInAsync(IChangeStateArg arg);
        UniTask StateOutAsync();
    }
}