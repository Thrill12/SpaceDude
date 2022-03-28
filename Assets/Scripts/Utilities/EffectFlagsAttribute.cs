using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class EnumMaskAttribute : PropertyAttribute
{
    public bool alwaysFoldOut;
    public EnumMaskLayout layout = EnumMaskLayout.Vertical;
}

public enum EnumMaskLayout
{
    Vertical,
    Horizontal
}