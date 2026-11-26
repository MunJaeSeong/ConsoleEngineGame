# Program.cs

`Program.cs`는 실행 프로그램의 시작 지점입니다.

## 코드 구조

```csharp
using GridDefense;

var game = new DefenseGame();
game.Run();
```

실제로는 다음 두 단계로 실행됩니다.

1. `DefenseGame` 인스턴스를 생성한다.
2. 생성된 게임의 `Run()` 메서드로 콘솔 게임 루프를 시작한다.

## `using GridDefense`

`DefenseGame`이 속한 `GridDefense` 네임스페이스를 가져와서 짧은 이름으로 클래스를 사용할 수 있게 합니다.

네임스페이스를 사용하지 않으면 다음과 같이 작성해야 합니다.

```csharp
var game = new GridDefense.DefenseGame();
```

## 실행 흐름

```text
Program 시작
  ↓
DefenseGame 생성
  ↓
ConsoleEngine 게임 루프 시작
  ↓
Escape 입력 또는 게임 루프 종료 조건
  ↓
프로그램 종료
```

이 파일은 화면을 직접 그리거나 Enemy를 직접 갱신하지 않습니다. 생성과 실행을 시작하는 진입점 역할에 집중합니다.