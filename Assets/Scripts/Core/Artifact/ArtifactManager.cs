using System.Collections.Generic;
using System.Linq;
using CardBullet.Core.Battle;
using CardBullet.Core.Card;
using Card = CardBullet.Core.Card.Card;
using UnityEngine;

namespace CardBullet.Core.Artifact
{
    /// <summary>
    /// 유물 관리 시스템
    /// 기획서 4.1 참조: 카드 사용 시 자동 발동 버프
    /// </summary>
    public class ArtifactManager : MonoBehaviour
    {
        [Header("참조")]
        [SerializeField] private BattleManager battleManager;

        private List<Artifact> ownedArtifacts = new List<Artifact>();

        // 이벤트
        public System.Action<Artifact> OnArtifactAcquired;
        public System.Action<Artifact> OnArtifactSold;
        public System.Action<Artifact> OnArtifactTriggered;

        public List<Artifact> GetOwnedArtifacts() => ownedArtifacts;

        /// <summary>
    /// 카드 사용 시 유물 효과 체크 및 적용
        /// </summary>
        public ArtifactEffectContext ProcessCardPlay(CardBullet.Core.Card.Card playedCard, CardBullet.Core.Card.Card previousCard, int baseDamage)
        {
            ArtifactEffectContext context = new ArtifactEffectContext
            {
                baseDamage = baseDamage,
                finalDamage = baseDamage,
                playedCard = playedCard,
                multiplier = 1f,
                bonusDamage = 0
            };

            ArtifactTriggerContext triggerContext = new ArtifactTriggerContext
            {
                lastPlayedCard = playedCard,
                previousCard = previousCard,
                patternCompleted = false, // TODO: 족보 완성 체크
                playerHP = battleManager.GetPlayerHP(),
                enemyHP = battleManager.GetEnemyHP()
            };

            // 보유한 유물들 중 발동 조건 충족하는 것들 확인
            foreach (Artifact artifact in ownedArtifacts)
            {
                if (artifact.CheckCondition(triggerContext))
                {
                    ApplyArtifactEffect(artifact, context);
                    OnArtifactTriggered?.Invoke(artifact);
                }
            }

            // 최종 데미지 계산
            context.finalDamage = Mathf.RoundToInt(context.baseDamage * context.multiplier) + context.bonusDamage;

            return context;
        }

        /// <summary>
        /// 유물 효과 적용
        /// </summary>
        private void ApplyArtifactEffect(Artifact artifact, ArtifactEffectContext context)
        {
            switch (artifact.effectType)
            {
                case ArtifactEffectType.DamageMultiplier:
                    context.multiplier *= artifact.effectValue;
                    Debug.Log($"[유물: {artifact.artifactName}] 데미지 {artifact.effectValue}배 증폭");
                    break;

                case ArtifactEffectType.DamageBonus:
                    context.bonusDamage += Mathf.RoundToInt(artifact.effectValue);
                    Debug.Log($"[유물: {artifact.artifactName}] 데미지 +{artifact.effectValue} 추가");
                    break;

                case ArtifactEffectType.ReduceAPCost:
                    // TODO: 카드 코스트에 직접 적용
                    break;

                case ArtifactEffectType.ReduceTCCost:
                    // TODO: 카드 코스트에 직접 적용
                    break;

                case ArtifactEffectType.HealOnPlay:
                    // TODO: 회복 효과 적용
                    break;

                case ArtifactEffectType.DrawCard:
                    // TODO: 카드 드로우
                    break;

                default:
                    break;
            }

            artifact.ApplyEffect(context);
        }

        /// <summary>
        /// 유물 획득
        /// </summary>
        public void AcquireArtifact(Artifact artifact)
        {
            ownedArtifacts.Add(artifact);
            OnArtifactAcquired?.Invoke(artifact);
            Debug.Log($"유물 획득: {artifact.artifactName}");
        }

        /// <summary>
        /// 유물 판매
        /// 기획서: 최소 1개 이상 보유 시에만 판매 가능, 판매가는 구매가의 20%
        /// </summary>
        public bool SellArtifact(Artifact artifact)
        {
            if (ownedArtifacts.Count <= 1)
            {
                Debug.LogWarning("최소 1개 이상 보유해야 합니다!");
                return false;
            }

            if (ownedArtifacts.Remove(artifact))
            {
                OnArtifactSold?.Invoke(artifact);
                // TODO: 판매가 계산 및 골드 획득
                return true;
            }

            return false;
        }
    }
}

