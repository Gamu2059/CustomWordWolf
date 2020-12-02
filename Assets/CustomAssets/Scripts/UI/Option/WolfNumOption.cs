using Common;
using ConnectData;
using UI.Option;
using UnityEngine;

namespace CustomAssets.Scripts.UI.Option {
    public class WolfNumOption : OptionBase {
        public override void UpdateOption(IOptionMessage option) {
            base.UpdateOption(option);

            if (option is ChangeWolfNum.SendRoom wolfNumOption) {
                optionValueText.text = $"{wolfNumOption.NewWolfNum}人";
            }
        }
    }
}