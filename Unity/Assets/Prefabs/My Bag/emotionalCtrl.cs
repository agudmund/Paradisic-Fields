using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class emotionalCtrl : MonoBehaviour
{
    public PlayerCtrl player;

    [Header("Volume Markers")]
    public Sprite[] statBars;

    [Header("Emotional Scales")]
    public Image shynessBar;
    public Image magnetismBar;
    public Image aggressionBar;
    public Image empathyBar;
    public Image luckBar;
    public Image uncertaintyBar;

    void FixedUpdate(){
        EnergyUpdate();
    }

    void EnergyUpdate()
    {
        // shynessBar.sprite = statBars[(int)Mathf.RoundToInt(player.shyness)];
        // magnetismBar.sprite = statBars[(int)Mathf.RoundToInt(player.magnetism)];
        // aggressionBar.sprite = statBars[(int)Mathf.RoundToInt(player.aggression)];
        // empathyBar.sprite = statBars[(int)Mathf.RoundToInt(player.empathy)];
        // luckBar.sprite = statBars[(int)Mathf.RoundToInt(player.luck)];
        // uncertaintyBar.sprite = statBars[(int)Mathf.RoundToInt(player.uncertainty)];
    }
}
