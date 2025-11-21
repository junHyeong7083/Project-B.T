using CardBullet.Core.Card;
using CardBullet.Core.Hand;
using UnityEngine;

namespace CardBullet.Core.Battle
{
    /// <summary>
    /// 데미지 계산 전담 클래스
    /// SRP: 데미지 계산 로직만 담당
    /// </summary>
    public static class DamageCalculator
    {
        /// <summary>
        /// 기본 데미지 계산 (카드 정보만으로)
        /// </summary>
        public static int CalculateBaseDamage(CardBullet.Core.Card.Card card)
        {
            int baseDamage = card.power;

            // 카드 타입별 보정
            switch (card.type)
            {
                case CardType.Sprint:
                    // 스프린트는 낮은 데미지
                    baseDamage = Mathf.Max(1, baseDamage / 2);
                    break;
                case CardType.Fatal:
                    // 페이탈은 높은 데미지
                    baseDamage = baseDamage * 2;
                    break;
                case CardType.Normal:
                default:
                    // 노말은 기본 데미지
                    break;
            }

            return baseDamage;
        }

        /// <summary>
        /// 족보 보너스 적용
        /// </summary>
        public static float ApplyPatternMultiplier(float baseDamage, HandManager.PokerPattern pattern)
        {
            float multiplier = PokerPatternDetector.GetPatternDamageMultiplier(pattern);
            return baseDamage * multiplier;
        }

        /// <summary>
        /// 유물 보너스 적용
        /// </summary>
        public static float ApplyArtifactMultiplier(float damage, float artifactMultiplier)
        {
            return damage * artifactMultiplier;
        }

        /// <summary>
        /// 유물 보너스 추가
        /// </summary>
        public static int ApplyArtifactBonus(int damage, int artifactBonus)
        {
            return damage + artifactBonus;
        }

        /// <summary>
        /// 최종 데미지 계산 (모든 보정 적용)
        /// </summary>
        public static int CalculateFinalDamage(
            CardBullet.Core.Card.Card card,
            HandManager.PokerPattern? pattern = null,
            float artifactMultiplier = 1f,
            int artifactBonus = 0)
        {
            // 기본 데미지
            int baseDamage = CalculateBaseDamage(card);

            // 족보 보너스 적용
            float damage = pattern.HasValue
                ? ApplyPatternMultiplier(baseDamage, pattern.Value)
                : baseDamage;

            // 유물 배율 적용
            damage = ApplyArtifactMultiplier(damage, artifactMultiplier);

            // 유물 보너스 적용
            int finalDamage = ApplyArtifactBonus(Mathf.RoundToInt(damage), artifactBonus);

            return Mathf.Max(1, finalDamage); // 최소 1 데미지
        }
    }
}

