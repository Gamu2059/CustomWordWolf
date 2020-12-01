using Cysharp.Threading.Tasks;

namespace Common {
    public interface IStateChangeable {
        UniTask StateInAsync(IChangeStateArg arg, bool isBack);
        UniTask StateOutAsync();
    }
}