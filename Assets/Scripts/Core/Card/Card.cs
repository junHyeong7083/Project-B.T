using System;
using UnityEngine;

namespace CardBullet.Core.Card
{
    /// <summary>
    /// 카드 데이터 구조
    /// 기획서 3.1 참조: 테두리, 숫자, 무늬, 각인, 코스트(AP/TC)
    /// </summary>
    [Serializable]
    public class Card
    {
        [Header("카드 기본 정보")]
        public CardType type;          // 테두리 (스프린트/페이탈/노말)
        public int power;              // 숫자 (기본 데미지 계수)
        public CardTheme theme;        // 무늬 (스페이드/클로버/다이아/하트)
        public string markID;          // 각인 (고유 추가 효과 ID)
        
        [Header("코스트")]
        public int apCost;             // AP Cost: 사용 시 소모 AP
        public int tcCost;             // TC Cost: 사용 시 누적 TC

        [Header("낙인")]
        public string prefixMarkID;    // 접두사 낙인 ID
        public string suffixMarkID;    // 접미사 낙인 ID

        public Card(CardType cardType, int cardPower, CardTheme cardTheme, 
                   int apCostValue, int tcCostValue, string cardMarkID = "")
        {
            type = cardType;
            power = cardPower;
            theme = cardTheme;
            apCost = apCostValue;
            tcCost = tcCostValue;
            markID = cardMarkID;
            prefixMarkID = "";
            suffixMarkID = "";
        }

        /// <summary>
        /// 카드 사용 가능 여부 (AP 체크)
        /// </summary>
        public bool CanUse(int currentAP)
        {
            return currentAP >= apCost || apCost == 0; // 스프린트는 AP 0
        }

        /// <summary>
        /// 최종 AP 코스트 계산 (낙인 적용)
        /// </summary>
        public int GetFinalAPCost()
        {
            int finalCost = apCost;
            // TODO: 낙인 효과로 AP 코스트 조정
            return Mathf.Max(0, finalCost);
        }

        /// <summary>
        /// 최종 TC 코스트 계산 (낙인 적용)
        /// </summary>
        public int GetFinalTCCost()
        {
            int finalCost = tcCost;
            // TODO: 낙인 효과로 TC 코스트 조정
            return Mathf.Max(0, finalCost);
        }
    }
}

