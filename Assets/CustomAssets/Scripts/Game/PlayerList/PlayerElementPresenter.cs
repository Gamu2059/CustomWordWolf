using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using ConnectData;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;

namespace Game.PlayerList {
    public class PlayerElementPresenter : MonoBehaviour {
        [SerializeField]
        private PlayerElementView view;

        public event Action<int> VotePlayerEvent;

        public void Initialize(PlayerSimpleData data) {
            view.Initialize(data.PlayerName);
            view.VoteObservable
                .Subscribe(_ => VotePlayerEvent?.Invoke(data.PlayerConnectionId))
                .AddTo(gameObject);
        }
    }
}