# DefenseGame.cs

`DefenseGame`는 현재 버전의 타워 디펜스 게임 흐름을 담당하는 메인 게임 클래스입니다.

## 역할

- `ConsoleEngine.GameBase`를 상속해 게임 루프에 연결합니다.
- 맵 크기와 화면 크기를 설정합니다.
- Enemy의 갱신과 렌더링을 담당합니다.
- Escape 키 입력으로 게임을 종료합니다.

## 상속 관계

```text
GameBase
  └─ DefenseGame
```

생성자에서 다음과 같이 엔진의 화면 크기를 지정합니다.

```csharp
defenseGame : base(GridMap.Width, GridMap.Height + 2)
```

가로는 맵의 너비인 20칸이며, 세로는 맵 높이 20칸에 상태 표시 영역 2칸을 합친 22칸입니다.

## 주요 메서드

### `Input(ConsoleKey key)`

키 입력을 처리합니다.

- `Escape`: `Quit()`을 호출해 게임 루프를 종료합니다.

### `Update(float deltaTime)`

매 프레임 Enemy의 상태를 갱신합니다.

```csharp
_enemy.Update(deltaTime);
```

### `Render(Screen screen)`

다음 순서로 화면을 그립니다.

1. 모든 칸을 `.` 문자로 채우는 기본 바닥
2. `GridMap.Path`의 이동 경로를 `#` 문자로 표시
3. 시작 지점을 `S`, 목표 지점을 `G`로 표시
4. 목표에 도착하지 않았다면 Enemy를 `E`로 표시
5. 맵 아래에 상태와 종료 안내 문구 표시

상태 표시 문구는 다음 두 값 중 하나입니다.

- `Enemy moving`
- `Enemy reached goal`

가장 마지막 줄에는 `Esc: Quit`을 표시합니다.

## 필드

| 이름 | 형식 | 설명 |
|---|---|---|
| `_enemy` | `Enemy` | 맵 경로를 따라 이동하는 Enemy 인스턴스 |

## 전체 흐름

```text
키 입력 → Update → Enemy 이동 갱신 → Render → 화면 출력
```