using System;

[Serializable]

public class FarmData : IBuildingData
{
    public int CurrentCoin;
    public int MaxCoin;
    public int CoinPerMinute;
    public DateTime ColectedTime; // thoi gian bat dau thu thap
}


