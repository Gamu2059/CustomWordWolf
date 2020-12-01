using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Common;
using Guide;
using Lobby;
using Title;
using UnityEngine;

namespace Manager {
    public class GameManager : StateMachineMonoBehavior<GroupState> {
        [SerializeField]
        private TitleGroupPresenter titlePresenter;

        [SerializeField]
        private LobbyGroupPresenter lobbyPresenter;

        [SerializeField]
        private GuideGroupPresenter guidePresenter;

        private void Start() {
            Initialize();
        }

        protected override void OnInitialize() {
            InitializeChild();
            BindState();
            SetEvents();
            StateMachine.StateRequestChange(GroupState.Title);
        }

        private void InitializeChild() {
            titlePresenter.Initialize();
            lobbyPresenter.Initialize();
        }

        private void BindState() {
            StateDictionary.Add(GroupState.Title, titlePresenter);
            StateDictionary.Add(GroupState.Lobby, lobbyPresenter);
        }

        private void SetEvents() {
            titlePresenter.OnStateRequestEvent += StateMachine.StateRequestChange;
            lobbyPresenter.OnStateRequestEvent += StateMachine.StateRequestChange;
        }
    }
}