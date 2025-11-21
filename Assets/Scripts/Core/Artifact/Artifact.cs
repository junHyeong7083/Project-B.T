using System;
using System.Collections.Generic;
using CardBullet.Core.Card;
using Card = CardBullet.Core.Card.Card;
using UnityEngine;

namespace CardBullet.Core.Artifact
{
    /// <summary>
    /// 유물 데이터 구조
    /// 기획서 4.1 참조
    /// </summary>
    [Serializable]
    public class Artifact
    {
        [Header("유물 기본 정보")]
        public string artifactName;         // 유물 이름
        public string[] keywords;           // 유물 키워드
        public ArtifactRarity rarity;       // 유물 등급
        public string conditionText;        // 발동 조건 텍스트
        public string flavorText;          // 플레이버 텍스트
        public string synergyTip;          // 시너지 팁

        [Header("발동 조건")]
        public ArtifactTriggerCondition triggerCondition; // 발동 조건 타입
        public string conditionFormula;     // 발동 조건 수식

        [Header("효과")]
        public ArtifactEffectType effectType;
        public float effectValue;          // 효과 수치

        public Artifact(string name, ArtifactRarity artifactRarity)
        {
            artifactName = name;
            rarity = artifactRarity;
            keywords = new string[0];
        }

        /// <summary>
        /// 발동 조건 체크
        /// </summary>
        public bool CheckCondition(ArtifactTriggerContext context)
        {
            // TODO: 조건 수식 파싱 및 평가
            // 임시: 기본 조건 체크
            switch (triggerCondition)
            {
                case ArtifactTriggerCondition.OnCardPlay:
                    return context.lastPlayedCard != null;
                case ArtifactTriggerCondition.OnFatalCardPlay:
                    return context.lastPlayedCard != null && 
                           context.lastPlayedCard.type == CardType.Fatal;
                case ArtifactTriggerCondition.OnSprintCardPlay:
                    return context.lastPlayedCard != null && 
                           context.lastPlayedCard.type == CardType.Sprint;
                case ArtifactTriggerCondition.OnPatternComplete:
                    return context.patternCompleted;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 효과 적용
        /// </summary>
        public void ApplyEffect(ArtifactEffectContext context)
        {
            // TODO: 효과 타입에 따른 실제 효과 적용
            Debug.Log($"[유물: {artifactName}] 효과 발동!");
        }
    }

    /// <summary>
    /// 유물 등급
    /// </summary>
    public enum ArtifactRarity
    {
        Normal,
        Rare,
        Unique,
        Legendary
    }

    /// <summary>
    /// 유물 발동 조건 타입
    /// </summary>
    public enum ArtifactTriggerCondition
    {
        OnCardPlay,          // 카드 사용 시
        OnFatalCardPlay,     // 페이탈 카드 사용 시
        OnSprintCardPlay,    // 스프린트 카드 사용 시
        OnPatternComplete,   // 족보 완성 시
        OnTurnStart,         // 턴 시작 시
        OnTurnEnd,           // 턴 종료 시
        OnTakeDamage,        // 데미지 받을 시
        OnDealDamage         // 데미지 줄 시
    }

    /// <summary>
    /// 유물 효과 타입
    /// </summary>
    public enum ArtifactEffectType
    {
        DamageMultiplier,    // 데미지 배율
        DamageBonus,         // 데미지 추가
        ReduceAPCost,        // AP 코스트 감소
        ReduceTCCost,        // TC 코스트 감소
        HealOnPlay,          // 카드 사용 시 회복
        DrawCard,            // 카드 드로우
        ReduceDamage,        // 데미지 감소
        IncreaseMaxAP        // 최대 AP 증가
    }

    /// <summary>
    /// 유물 발동 조건 컨텍스트
    /// </summary>
    public class ArtifactTriggerContext
    {
        public CardBullet.Core.Card.Card lastPlayedCard;
        public CardBullet.Core.Card.Card previousCard;
        public bool patternCompleted;
        public int turnNumber;
        public float playerHP;
        public float enemyHP;
    }

    /// <summary>
    /// 유물 효과 적용 컨텍스트
    /// </summary>
    public class ArtifactEffectContext
    {
        public int baseDamage;
        public int finalDamage;
        public CardBullet.Core.Card.Card playedCard;
        public float multiplier = 1f;
        public int bonusDamage = 0;
    }
}

