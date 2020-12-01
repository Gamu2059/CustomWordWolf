using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Common {
    public interface IGroupStateChangeable {
        UniTask GroupInAsync(IChangeGroupArg arg);
        UniTask GroupOutAsync();
    }
}