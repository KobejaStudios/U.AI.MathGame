using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ColorPalettesSO", fileName = "ColorPalettes")]
public class ColorPalettesSO : ScriptableObject
{
    public List<ColorPalette> colorPalettes = new();
}
