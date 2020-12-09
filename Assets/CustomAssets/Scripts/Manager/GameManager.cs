using Common;
using Game;
using Lobby;
using Title;
using UnityEngine;

namespace Manager {
    /// <summary>
    /// ゲーム全体の上位概念となるコンポーネント。
    /// </summary>
    public class GameManager :
        // GroupStateに関するステートマシンを持つMonoBehavior
        StateMachineMonoBehavior<GroupState> {
        /// <summary>
        /// タイトルグループ全体のPresenter
        /// </summary>
        [SerializeField]
        private TitleGroupPresenter titlePresenter;

        /// <summary>
        /// ロビーグループ全体のPresenter
        /// </summary>
        [SerializeField]
        private LobbyGroupPresenter lobbyPresenter;

        /// <summary>
        /// ゲームグループ全体のPresenter
        /// </summary>
        [SerializeField]
        private GameGroupPresenter gamePresenter;

        private void Start() {
            Initialize();
        }

        /// <summary>
        /// ゲーム全体を初期化する。
        /// Initializeを呼び出された時に自動的に呼び出される。
        /// </summary>
        protected override void OnInitialize() {
            InitializeChild();
            BindState();
            InjectStateMachine();
            StateMachine.RequestChangeState(GroupState.Title);
        }

        /// <summary>
        /// 子要素のPresenterを初期化する。
        /// </summary>
        private void InitializeChild() {
            titlePresenter.Initialize();
            lobbyPresenter.Initialize();
            gamePresenter.Initialize();
        }

        /// <summary>
        /// ステートと子要素のPresenterを紐づける。
        /// あるステートに状態遷移した時、結びつけられた子要素のPresenterが呼び出される。
        /// </summary>
        private void BindState() {
            StateDictionary.Add(GroupState.Title, titlePresenter);
            StateDictionary.Add(GroupState.Lobby, lobbyPresenter);
            StateDictionary.Add(GroupState.Game, gamePresenter);
        }

        /// <summary>
        /// 子要素で状態遷移させる処理を実現するためにステートマシンを子要素にも伝える。
        /// </summary>
        private void InjectStateMachine() {
            titlePresenter.InjectStateMachine(StateMachine);
            lobbyPresenter.InjectStateMachine(StateMachine);
            gamePresenter.InjectStateMachine(StateMachine);
        }
    }
}