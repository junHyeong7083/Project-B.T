# Card Bullet - 턴제 덱빌딩 로그라이트 게임

기획서 기반 시스템 구현 완료 상태입니다.

## 프로젝트 구조

### Core Systems

#### 1. 카드 시스템 (`Scripts/Core/Card/`)
- **Card.cs**: 카드 데이터 구조 (타입, 무늬, 숫자, 각인, 코스트)
- **CardType.cs**: 카드 타입 열거형 (Sprint, Fatal, Normal) 및 무늬 (Spade, Clover, Diamond, Heart)

#### 2. 덱 시스템 (`Scripts/Core/Deck/`)
- **DeckManager.cs**: 덱 관리, 드로우, 셔플, 최소 덱 용량(15장) 관리

#### 3. 전투 시스템 (`Scripts/Core/Battle/`)
- **ResourceManager.cs**: AP(행동력) 및 TC(시간 비용) 관리
- **TurnManager.cs**: TC 기반 턴 결정 로직
- **BattleManager.cs**: 전투 메인 관리, 카드 효과 적용
- **RaiseSystem.cs**: 레이즈 시스템 (주사위 기반 역전 기믹)

#### 4. 족보 시스템 (`Scripts/Core/Hand/`)
- **HandManager.cs**: 손패 관리 및 족보 패턴 체크
- **PokerPatternDetector.cs**: 포커 족보 패턴 감지 로직 (원페어 ~ 로얄 플러시)

#### 5. 유물 시스템 (`Scripts/Core/Artifact/`)
- **Artifact.cs**: 유물 데이터 구조 및 발동 조건
- **ArtifactManager.cs**: 유물 관리 및 효과 적용

#### 6. 정비 시스템 (`Scripts/Core/Maintenance/`)
- **MaintenancePhase.cs**: 운명의 수레바퀴 페이즈 (덱 관리, 카드 뭉치 획득)
- **Stigma.cs**: 낙인 시스템 (접두어/접미어 카드 강화)

#### 7. UI 시스템 (`Scripts/UI/`)
- **BattleUI.cs**: 전투 UI 관리 (TC 타임라인, AP/TC 표시, 족보 알림, 레이즈 버튼)

## 핵심 메커니즘

### 1. AP/TC 시스템
- **AP**: 카드 사용을 위한 행동력, 매 턴 시작 시 회복
- **TC**: 행동에 소요되는 시간값, 누적량에 따라 턴 순서 결정
- `PC_TC <= NPC_TC`: 플레이어 턴 유지
- `PC_TC > NPC_TC`: 적 턴으로 전환

### 2. 카드 타입
- **Sprint**: AP 0, 낮은 데미지, 덱 순환용
- **Fatal**: AP 2, 높은 데미지, 마무리용
- **Normal**: 기본 타입

### 3. 레이즈 시스템
발동 조건: `PC HP < 10% AND Current AP == 0`
- **[1~4]**: 체력 평균화
- **[5]**: 완전 회복 + 1턴 스턴
- **[6]**: 풀 드로우 + AP 완전 회복

### 4. 족보 시스템
포커 족보 패턴을 패시브 룰로 활용:
- 원페어 ~ 로얄 플러시까지 9단계 패턴 인식
- 패턴별 데미지 배율 보너스

### 5. 운명의 수레바퀴 페이즈
- 카드 뭉치 획득 (10장 단위)
- 덱 압축 (최대 10장 제거, 최소 15장 유지)
- 낙인 부여 (접두어/접미어)

## 다음 단계

### 구현 필요 사항
1. **ScriptableObject 데이터 구조**
   - CardData ScriptableObject
   - ArtifactData ScriptableObject
   - StigmaData ScriptableObject

2. **전투 시스템 보완**
   - BattleManager에서 유물 효과 통합
   - BattleManager에서 족보 효과 통합
   - 적 AI 시스템

3. **상점 시스템**
   - 유물 구매/판매 (구매가의 20%로 판매)
   - 카드 구매
   - 낙인 구매

4. **UI 완성**
   - 카드 드래그 앤 드롭
   - 카드 시각화 (스프린트/페이탈 아이콘)
   - 레이즈 연출 (화면 붉은 점멸, 시간 정지)

5. **로그라이트 구조**
   - 스테이지 진행 시스템
   - 보상 시스템
   - 덱 저장/로드

## 사용 방법

### 기본 설정
1. Unity 씬에 BattleManager 오브젝트 생성
2. ResourceManager (Player, Enemy) 생성 및 연결
3. TurnManager, DeckManager, HandManager, ArtifactManager 생성 및 연결
4. BattleUI 생성 및 모든 시스템 참조 연결
5. 초기 덱 구성 후 DeckManager.InitializeDeck() 호출

### 전투 시작
```csharp
// BattleManager는 자동으로 전투 초기화
// 카드 사용 시
battleManager.TryPlayCard(selectedCard);
```

### 정비 페이즈
```csharp
maintenancePhase.StartMaintenancePhase();
// 카드 제거
maintenancePhase.RemoveCardFromDeck(cardToRemove);
// 카드 뭉치 획득
maintenancePhase.AcquireCardPack(newCardPack);
maintenancePhase.EndMaintenancePhase();
```

## 참고 문서

기획서: `../신규 기획.pptx`, `../제안서.pdf`

