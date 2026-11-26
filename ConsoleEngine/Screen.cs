using System.Text;

namespace ConsoleEngine;

/// <summary>한 프레임을 그려 두는 문자 버퍼. Present 때 한 번에 출력한다.</summary>
public class Screen
{
    public readonly int Width;
    public readonly int Height;
    private readonly char[,] _cells;
    private readonly ConsoleColor?[,] _foregroundColors;
    private readonly StringBuilder _frame = new StringBuilder();
    private int _coloredCellCount;

    public Screen(int width, int height)
    {
        Width = width;
        Height = height;
        _cells = new char[height, width];
        _foregroundColors = new ConsoleColor?[height, width];
    }

    public void Clear()
    {
        _coloredCellCount = 0;
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
            {
                _cells[y, x] = ' ';
                _foregroundColors[y, x] = null;
            }
    }

    public void Draw(int x, int y, char c)
    {
        DrawCell(x, y, c, null);
    }

    /// <summary>
    /// 타일이나 유닛을 주변 문자와 구분해 표시할 때 지정한 전경색으로 글자를 그린다.
    /// 색을 셀과 함께 버퍼에 저장하므로 게임은 기존 Render 흐름을 그대로 사용할 수 있다.
    /// </summary>
    /// <param name="x">왼쪽에서부터의 좌표</param>
    /// <param name="y">위쪽에서부터의 좌표</param>
    /// <param name="c">그릴 문자</param>
    /// <param name="foregroundColor">글자 전경색</param>
    public void Draw(int x, int y, char c, ConsoleColor foregroundColor)
    {
        DrawCell(x, y, c, foregroundColor);
    }

    private void DrawCell(int x, int y, char c, ConsoleColor? foregroundColor)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height) return;  // 화면 밖은 무시
        if (_foregroundColors[y, x].HasValue != foregroundColor.HasValue)
            _coloredCellCount += foregroundColor.HasValue ? 1 : -1;
        _cells[y, x] = c;
        _foregroundColors[y, x] = foregroundColor;
    }

    /// <summary>(x, y)부터 한 글자씩 오른쪽으로 그린다. 화면 밖 글자는 무시. (v0.2.0, #6)</summary>
    /// <param name="text">ASCII 문자만. 한글은 2칸을 차지해 줄이 밀린다 (ADR 0002)</param>
    public void DrawText(int x, int y, string text)
    {
        for (int i = 0; i < text.Length; i++) Draw(x + i, y, text[i]);
    }

    /// <summary>색상으로 구분할 필요가 있는 상태 안내나 타일 이름을 그린다.</summary>
    /// <param name="x">문자열을 그릴 시작 x 좌표</param>
    /// <param name="y">문자열을 그릴 y 좌표</param>
    /// <param name="text">그릴 ASCII 문자열</param>
    /// <param name="foregroundColor">문자열의 전경색</param>
    public void DrawText(int x, int y, string text, ConsoleColor foregroundColor)
    {
        for (int i = 0; i < text.Length; i++)
            Draw(x + i, y, text[i], foregroundColor);
    }

    internal void Present()
    {
        ConsoleColor originalColor = Console.ForegroundColor;
        try
        {
            Console.SetCursorPosition(0, 0);   // 지우지 않고 덮어써서 깜빡임 방지
            if (_coloredCellCount == 0)
            {
                _frame.Clear();
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++) _frame.Append(_cells[y, x]);
                    _frame.Append('\n');
                }
                Console.Write(_frame.ToString());
                return;
            }

            ConsoleColor activeColor = originalColor;
            for (int y = 0; y < Height; y++)
            {
                int x = 0;
                while (x < Width)
                {
                    ConsoleColor? cellColor = _foregroundColors[y, x];
                    ConsoleColor runColor = cellColor ?? originalColor;
                    if (runColor != activeColor)
                    {
                        Console.ForegroundColor = runColor;
                        activeColor = runColor;
                    }

                    _frame.Clear();
                    while (x < Width && _foregroundColors[y, x] == cellColor)
                        _frame.Append(_cells[y, x++]);
                    Console.Write(_frame.ToString());
                }
                Console.Write('\n');
            }
        }
        finally
        {
            // 엔진이 바꾼 터미널 색상이 게임 종료 후에도 남지 않게 복원한다.
            Console.ForegroundColor = originalColor;
        }
    }
}
