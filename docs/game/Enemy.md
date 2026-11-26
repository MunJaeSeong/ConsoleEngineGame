# Enemy.cs

`Enemy`는 웨이브에 등장하는 몬스터의 종류, 체력, 이동 속도, 보상 및 결승점 도착 여부를 관리합니다.

## 생성 규칙

`new Enemy(wave, random)`으로 생성하며, 랜덤 확률에 따라 10종 중 하나를 선택합니다.

| 구간 | 종류 |
|---|---|
| 0~39% | Normal |
| 40~54% | Fast |
| 55~67% | Tank |
| 68~75% | Heavy |
| 76~82% | Runner |
| 83~87% | Brute |
| 88~91% | Wisp |
| 92~94% | Plague |
| 95~97% | Mage |
| 98~99% | Boss |

## 몬스터 스펙

| 종류 | 기본 HP | 이동 간격 | 처치 보상 |
|---|---:|---:|---:|
| Normal | 45 | 0.30초 | 5 |
| Fast | 30 | 0.20초 | 5 |
| Tank | 110 | 0.30초 | 10 |
| Heavy | 180 | 0.30초 | 15 |
| Runner | 38 | 0.18초 | 5 |
| Brute | 70 | 0.30초 | 5 |
| Wisp | 55 | 0.24초 | 5 |
| Plague | 90 | 0.30초 | 5 |
| Mage | 65 | 0.30초 | 5 |
| Boss | 300 | 0.38초 | 35 |

실제 최대 HP는 웨이브 배율을 기본 HP에 곱해 계산합니다.

```text
MaxHp = 기본 HP × WaveRules.GetEnemyHpMultiplier(wave)
```

## 주요 속성

| 속성 | 형식 | 설명 |
|---|---|---|
| `Type` | `int` | 몬스터 종류 인덱스 |
| `PathIndex` | `int` | 현재 이동 경로 배열 인덱스 |
| `Hp` | `int` | 현재 체력 |
| `MaxHp` | `int` | 웨이브 배율이 적용된 최대 체력 |
| `Reward` | `int` | 처치 시 지급 다이아 |
| `Speed` | `float` | 한 칸 이동에 필요한 시간 |
| `ReachedGoal` | `bool` | 결승점 도착 여부 |

## 이동 처리

`Update(deltaTime)`는 누적 이동 시간을 갱신합니다. 누적 시간이 몬스터의 `Speed` 이상이면 `PathIndex`를 증가시키고 남은 시간을 다음 이동 계산에 이어서 사용합니다.

```text
누적 시간 += deltaTime
누적 시간 >= Speed
  → 한 칸 이동
  → 남은 누적 시간 유지
```

적의 위치는 `GridMap.Path[PathIndex]`로 변환됩니다.

## 사망 및 도착 판정

- 체력이 0 이하가 되면 필드에서 제거되고 처치 보상을 지급합니다.
- 경로의 마지막 위치를 지나면 `ReachedGoal`이 됩니다.
- 결승점에 도착한 적은 플레이어 HP를 1 감소시키고 제거됩니다.

## 화면 문자

| 종류 | 문자 |
|---|---|
| Normal | `e` |
| Fast | `f` |
| Tank | `t` |
| Heavy | `H` |
| Runner | `v` |
| Brute | `b` |
| Wisp | `w` |
| Plague | `p` |
| Mage | `m` |
| Boss | `B` |

모든 적은 화면에서 빨간색으로 렌더링됩니다.