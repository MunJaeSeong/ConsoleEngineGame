# 그리드 디펜스 시스템

## 1. 작업 개요

`src/GridDefense`에 초기 프로토타입 코드를 추가했습니다. 기존 `ConsoleEngine`의 게임 루프를 사용해 20×20 맵을 그리고, 적 한 마리가 ㄹ자 경로를 따라 이동하도록 구현했습니다.

## 2. 구현 내용

- `DefenseGame`이 `GameBase`를 상속해 `Input`, `Update`, `Render`를 구현합니다.
- 맵의 출발점은 `S`, 도착점은 `G`, 경로는 `#`, 이동 중인 적은 `E`로 표시합니다.
- 적은 `deltaTime`을 누적해 게임 시간 기준 0.3초마다 다음 경로 칸으로 이동합니다.
- 적이 마지막 칸에 도착하면 이동을 멈추고 화면에 `Enemy reached goal`을 표시합니다.
- `Esc`를 누르면 게임을 종료합니다.

## 3. 추가한 파일

| 파일 | 역할 |
|---|---|
| `src/GridDefense/Program.cs` | `DefenseGame`을 생성하고 `Run()`을 호출합니다. |
| `src/GridDefense/DefenseGame.cs` | 종료 입력을 처리하고 매 프레임 적의 상태를 갱신한 뒤 맵과 상태를 그립니다. |
| `src/GridDefense/GridMap.cs` | 맵 크기와 ㄹ자 경로의 좌표를 이동 순서대로 정의합니다. |
| `src/GridDefense/Enemy.cs` | 현재 경로 위치와 이동 시간을 관리하고 도착 여부를 판정합니다. |

## 4. 실행 확인

`GridDefense.csproj`에 `ConsoleEngine` 프로젝트 참조를 설정한 뒤 저장소 루트에서 실행합니다.

```bash
dotnet run --project src/GridDefense/GridDefense.csproj
```

- [ ] 프로젝트가 빌드되고 20×20 맵이 표시되는지 확인
- [ ] 적이 ㄹ자 경로를 따라 한 칸씩 이동하는지 확인
- [ ] 도착 후 `E`가 사라지고 `Enemy reached goal`이 표시되는지 확인
- [ ] `Esc`를 눌렀을 때 게임이 종료되는지 확인