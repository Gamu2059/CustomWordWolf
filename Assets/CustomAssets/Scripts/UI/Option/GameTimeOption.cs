using Common;
using ConnectData;
using UI.Option;
using UnityEngine;

namespace CustomAssets.Scripts.UI.Option {
    public class GameTimeOption : OptionBase {
        public override void UpdateOption(IOptionMessage option) {
            base.UpdateOption(option);

            if (option is ChangeGameTime.SendRoom gameTimeOption) {
                optionValueText.text = $"{gameTimeOption.NewGameTime}秒";
            }
        }
    }
}