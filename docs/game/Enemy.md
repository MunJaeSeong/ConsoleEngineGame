# Enemy.cs

`Enemy`는 맵의 경로를 일정 간격으로 이동하는 개체의 상태를 관리하는 클래스입니다.

## 역할

- 이동 경로에서 현재 위치의 인덱스를 보관합니다.
- 일정 시간마다 다음 경로 칸으로 이동합니다.
- 목표 지점에 도착했는지 기록합니다.

## 상수

| 이름 | 값 | 설명 |
|---|---:|---|
| `MoveInterval` | `0.3f` | Enemy가 한 칸 이동하는 데 필요한 시간(초) |

## 주요 속성

| 속성 | 형식 | 설명 |
|---|---|---|
| `PathIndex` | `int` | 현재 위치에 해당하는 `GridMap.Path`의 인덱스 |
| `ReachedGoal` | `bool` | 목표 지점 도착 여부 |

두 속성은 외부에서 값을 변경할 수 없고, 클래스 내부의 메서드를 통해서만 갱신됩니다.

## `_moveElapsed`

한 칸 이동까지 누적된 시간을 저장하는 비공개 필드입니다.

```text
누적 시간 + deltaTime
누적 시간이 0.3초 이상이면 이동
남은 시간은 다음 이동 계산에 보존
```

이동 간격은 다음 방식으로 처리합니다.

```csharp
while (_moveElapsed >= MoveInterval)
{
    _moveElapsed -= MoveInterval;
    PathIndex++;
}
```

따라서 프레임 지연이 생겨 한 번의 갱신에 경과 시간이 많이 누적되더라도 가능한 만큼 여러 칸을 이동할 수 있습니다.

## 목표 도착 처리

경로 배열의 마지막 인덱스에 도달하면 `ReachedGoal`을 `true`로 바꾸고 이후 이동을 멈춥니다.

```csharp
if (ReachedGoal)
    return;
```

목표 도착 이후에는 `_enemy.Update()`가 호출되어도 상태가 변경되지 않습니다.

## 의존 관계

`Enemy`는 자신의 위치를 계산할 때 `GridMap.Path`를 참조합니다.
실제 좌표 변환은 `Enemy` 내부가 아니라 `DefenseGame.Render()`에서 처리합니다.

```text
Enemy.PathIndex
  → GridMap.Path[PathIndex]
  → 화면 좌표 (X, Y)
```