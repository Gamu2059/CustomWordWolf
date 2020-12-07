using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Common;
using Game;
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
        private GameGroupPresenter gamePresenter;

        private void Start() {
            Initialize();
        }

        protected override void OnInitialize() {
            InitializeChild();
            BindState();
            InjectStateMachine();
            StateMachine.RequestChangeState(GroupState.Title);
        }

        private void InitializeChild() {
            titlePresenter.Initialize();
            lobbyPresenter.Initialize();
            gamePresenter.Initialize();
        }

        private void BindState() {
            StateDictionary.Add(GroupState.Title, titlePresenter);
            StateDictionary.Add(GroupState.Lobby, lobbyPresenter);
            StateDictionary.Add(GroupState.Game, gamePresenter);
        }

        private void InjectStateMachine() {
            titlePresenter.InjectStateMachine(StateMachine);
            lobbyPresenter.InjectStateMachine(StateMachine);
            gamePresenter.InjectStateMachine(StateMachine);
        }
    }
}