using System;
using ConnectData;
using Cysharp.Threading.Tasks;
using Manager;
using Mirror;

namespace Api {
    public class UpdateMemberReceiver : ReceiverApiBase<UpdateMember.SendRoom> {
        public UpdateMemberReceiver(Action<UpdateMember.SendRoom> action) : base(action) {
        }

        protected override void Bind(CustomNetworkManager networkManager) {
            networkManager.OnUpdateMemberReceiveEvent += Invoke;
        }

        protected override void Unbind(CustomNetworkManager networkManager) {
            networkManager.OnUpdateMemberReceiveEvent -= Invoke;
        }
    }
}