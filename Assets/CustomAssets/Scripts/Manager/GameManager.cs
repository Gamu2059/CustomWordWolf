using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Common;
using Guide;
using Title;
using UnityEngine;

namespace Manager {
    public class GameManager : MonoBehaviour {

        [SerializeField]
        private TitleGroupPresenter titlePresenter;

        [SerializeField]
        private GuideGroupPresenter guidePresenter;

        private GroupStateMachine stateMachine;

        private Dictionary<GroupState, IGroupStateChangeable> stateDictionary;
        
        private void Start() {
            Initialize();
            InjectStateMachine();
            SetEvents();
            
            stateMachine.RequestChangeState(GroupState.Title);
        }

        private void Initialize() {
            stateMachine = new GroupStateMachine();
            stateDictionary = new Dictionary<GroupState, IGroupStateChangeable> {
                {GroupState.Title, titlePresenter},
                // {GroupState.Guide, guidePresenter},
            };
            titlePresenter.Initialize();
        }

        private void InjectStateMachine() {
            titlePresenter.InjectStateMachine(stateMachine);
        }

        private void SetEvents() {
            stateMachine.OnChangeState += OnChangeState;
        }

        private async void OnChangeState(GroupState nextState, GroupState oldState, IChangeGroupArg arg) {
            if (stateDictionary.ContainsKey(oldState)) {
                await stateDictionary[oldState].GroupOutAsync();
            }

            if (stateDictionary.ContainsKey(nextState)) {
                await stateDictionary[nextState].GroupInAsync(arg);
            }
        }
    }
}