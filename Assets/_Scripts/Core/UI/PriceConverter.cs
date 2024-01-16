using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PriceConverter
{
    public static string Convert(this float value)
    {
        if (value >= 1000)
        {
            var dividedValue = value / 1000;
            return  "$" + Math.Round(dividedValue, 1, MidpointRounding.ToEven).ToString() + "K";
        }

        return  "$" + value.ToString("0");
    }
}
