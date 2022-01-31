using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 해당 컴포넌트를 가지고 있는 UILabel객체는 자동으로 다중언어에 대한 기능이 적용된다.
/// </summary>

public class UILabelLocalize : Label
{
    // bool isLocalized = false;

    // public override void MarkAsChanged()
    // {
    //     if(!isLocalized)
    //     {
    //         isLocalized = true;
    //         text = LocalizeText(text);
    //     }

    //     isLocalized = false;
    //     base.MarkAsChanged();
    // }

    // protected override void OnEnable()
    // {
    //     base.OnEnable();

    //     isLocalized = true;
    //     text = LocalizeText(text);
    //     LanguageController.Inst.EventLanguageChanged += OnLanguageChanged;
    // }

    // protected override void OnDisable() 
    // {
    //     base.OnDisable();
    //     LanguageController.Inst.EventLanguageChanged -= OnLanguageChanged;
    // }

    // private string LocalizeText(string text)
    // {
    //     return LanguageController.Inst.ConvertTextToTextCodeToLanguageTexts(text);
    // }

    // void OnLanguageChanged(LanguageType from, LanguageType to)
    // {
    //     isLocalized = true;
    //     text = LocalizeText(text);
    // }


}
