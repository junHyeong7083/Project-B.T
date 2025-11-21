using System;
using UnityEngine;

namespace CardBullet.Core.Maintenance
{
    /// <summary>
    /// 낙인 데이터 구조
    /// 기획서 5.2 참조: 접두어/접미어 형태의 카드 강화
    /// </summary>
    [Serializable]
    public class Stigma
    {
        [Header("낙인 기본 정보")]
        public string stigmaID;              // 낙인 ID
        public string stigmaName;            // 낙인 이름
        public StigmaType stigmaType;        // 접두사/접미사 판정
        public string keywordName;           // 키워드 이름
        public string effectDescription;     // 효과 설명

        [Header("효과")]
        public StigmaEffectType effectType;
        public float effectValue;            // 효과 수치

        [Header("코스트 변경")]
        public int apCostChange;             // AP 코스트 변화 (-면 감소, +면 증가)
        public int tcCostChange;             // TC 코스트 변화

        public Stigma(string id, string name, StigmaType type)
        {
            stigmaID = id;
            stigmaName = name;
            stigmaType = type;
        }

        /// <summary>
        /// 낙인 효과 적용
        /// </summary>
        public void ApplyEffect(StigmaEffectContext context)
        {
            switch (effectType)
            {
                case StigmaEffectType.DamageBonus:
                    context.damageBonus += Mathf.RoundToInt(effectValue);
                    break;
                case StigmaEffectType.DamageMultiplier:
                    context.damageMultiplier *= effectValue;
                    break;
                case StigmaEffectType.HealOnPlay:
                    context.healAmount += Mathf.RoundToInt(effectValue);
                    break;
                case StigmaEffectType.DrawOnPlay:
                    context.drawCards += Mathf.RoundToInt(effectValue);
                    break;
                default:
                    break;
            }

            // 코스트 변경 적용
            context.apCostChange += apCostChange;
            context.tcCostChange += tcCostChange;
        }
    }

    /// <summary>
    /// 낙인 타입
    /// </summary>
    public enum StigmaType
    {
        Prefix,  // 접두사
        Suffix   // 접미사
    }

    /// <summary>
    /// 낙인 효과 타입
    /// </summary>
    public enum StigmaEffectType
    {
        DamageBonus,      // 데미지 추가
        DamageMultiplier, // 데미지 배율
        HealOnPlay,       // 사용 시 회복
        DrawOnPlay,       // 사용 시 드로우
        ReduceCost        // 코스트 감소
    }

    /// <summary>
    /// 낙인 효과 컨텍스트
    /// </summary>
    public class StigmaEffectContext
    {
        public int damageBonus = 0;
        public float damageMultiplier = 1f;
        public int healAmount = 0;
        public int drawCards = 0;
        public int apCostChange = 0;
        public int tcCostChange = 0;
    }
}

