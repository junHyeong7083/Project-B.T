using CardBullet.Core.Card;
using CardBullet.Core.Artifact;
using CardBullet.Core.Hand;
using UnityEngine;

namespace CardBullet.Core.Battle
{
    /// <summary>
    /// 카드 효과 처리 전담 클래스
    /// SRP: 카드 효과 적용 로직만 담당
    /// </summary>
    public class CardEffectProcessor : MonoBehaviour
    {
        [Header("시스템 참조")]
        [SerializeField] private ArtifactManager artifactManager;
        [SerializeField] private HandManager handManager;

        /// <summary>
        /// 카드 효과 적용
        /// </summary>
        public CardEffectResult ProcessCardEffect(
            CardBullet.Core.Card.Card card,
            CardBullet.Core.Card.Card previousCard = null)
        {
            CardEffectResult result = new CardEffectResult
            {
                card = card,
                baseDamage = DamageCalculator.CalculateBaseDamage(card)
            };

            // 족보 패턴 확인
            var patterns = handManager?.CheckAvailablePatterns();
            if (patterns != null && patterns.Count > 0)
            {
                result.pattern = patterns[0]; // 가장 높은 우선순위 패턴
                result.baseDamage = Mathf.RoundToInt(
                    DamageCalculator.ApplyPatternMultiplier(result.baseDamage, result.pattern.Value));
            }

            // 유물 효과 적용
            if (artifactManager != null)
            {
                var artifactContext = artifactManager.ProcessCardPlay(card, previousCard, result.baseDamage);
                result.finalDamage = artifactContext.finalDamage;
                result.multiplier = artifactContext.multiplier;
                result.bonusDamage = artifactContext.bonusDamage;
            }
            else
            {
                result.finalDamage = result.baseDamage;
            }

            return result;
        }

        /// <summary>
        /// 카드 무늬에 따른 효과 타입 결정
        /// </summary>
        public CardEffectType GetEffectType(CardBullet.Core.Card.Card card)
        {
            switch (card.theme)
            {
                case CardTheme.Spade:
                    return CardEffectType.Attack;
                case CardTheme.Clover:
                    return CardEffectType.Defense;
                case CardTheme.Diamond:
                    return CardEffectType.Resource;
                case CardTheme.Heart:
                    return CardEffectType.Heal;
                default:
                    return CardEffectType.None;
            }
        }
    }

    /// <summary>
    /// 카드 효과 결과
    /// </summary>
    public class CardEffectResult
    {
        public CardBullet.Core.Card.Card card;
        public int baseDamage;
        public int finalDamage;
        public HandManager.PokerPattern? pattern;
        public float multiplier = 1f;
        public int bonusDamage = 0;
        public CardEffectType effectType = CardEffectType.None;
    }

    /// <summary>
    /// 카드 효과 타입
    /// </summary>
    public enum CardEffectType
    {
        None,
        Attack,      // 공격
        Defense,     // 방어
        Resource,    // 자원
        Heal         // 회복
    }
}

