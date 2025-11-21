namespace CardBullet.Core.Card
{
    /// <summary>
    /// 카드의 테두리 타입 (성질)
    /// </summary>
    public enum CardType
    {
        Sprint,  // 스프린트: AP 0, 낮은 데미지, 덱 순환용
        Fatal,   // 페이탈: AP 2, 높은 데미지, 마무리용
        Normal   // 노말: 기본 타입
    }

    /// <summary>
    /// 카드의 무늬 (테마)
    /// </summary>
    public enum CardTheme
    {
        Spade,   // 스페이드: 공격
        Clover,  // 클로버: 방어
        Diamond, // 다이아: 자원
        Heart    // 하트: 회복
    }
}

