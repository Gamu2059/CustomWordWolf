using Cysharp.Threading.Tasks;

namespace Common {
    /// <summary>
    /// ステート遷移時イベントを受け取るメソッドを定義するインタフェース。
    /// </summary>
    public interface IStateChangeable {
        UniTask StateInAsync(IChangeStateArg arg, bool isBack);
        UniTask StateOutAsync();
    }
}