using System;
using ConnectData;

namespace ManagedData {
    public class RoomData {
        public Guid RoomGuid;
        public DateTime DateTime;
        public uint CurrentHostNetId;
        public string RoomName;
        public string HostName;
        public int MemberNum;

        public void JoinRoom() {
            MemberNum++;
        }

        public void LeaveRoom() {
            MemberNum--;
        }

        public ConnectRoomData CreateConnectRoomData() {
            return new ConnectRoomData {
                RoomGuid = RoomGuid,
                RoomName = RoomName,
                HostName = HostName,
                MemberNum = MemberNum,
            };
        }
    }
}